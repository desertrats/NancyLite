using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RazorEngineCore;
using System;
using System.Net;
using System.Threading.Tasks;

namespace NancyLite.Razor
{
    public class RazorEnginePlus
    {
        private readonly ILogger<RazorEnginePlus> logProvider;
        private readonly NancyLiteRazorConfig config;
        //组合优于继承
        private readonly IRazorEngine razorEngine;
        public RazorEnginePlus(ILogger<RazorEnginePlus> iLogProvider, NancyLiteRazorConfig razorConfig)
        {
            logProvider = iLogProvider;
            config = razorConfig;
            razorEngine = new RazorEngine();
        }

        public string QuickRenderRaw(string content, dynamic model = null, dynamic viewBag = null)
        {
            try
            {
                var template = razorEngine.Compile(content, config.DefaultBuildAction);
                return template.Run(this, model, viewBag);
            }
            catch (Exception ex)
            {
                if (model == null)
                {
                    logProvider.LogError($"unable to run raw string [{content}], Error: {ex.Message}");
                }
                else
                {
                    logProvider.LogError($"unable to run raw string [{content}] with parameter:[{JsonConvert.SerializeObject(model)}], Error: {ex.Message}");
                }

            }
            return null;
        }

        #region Render View
        /// <summary>
        /// 编译并运行指定View
        /// </summary>
        /// <param name="viewPath"></param>
        /// <param name="model"></param>
        /// <param name="viewBag"></param>
        /// <param name="isRenderSub">是否是渲染子视图</param>
        /// <returns></returns>
        public string RenderRaw(string viewPath, dynamic model = null, dynamic viewBag = null, bool isRenderSub = false)
        {
            //获取编译后的模板内容
            IRazorEngineCompiledTemplate template;

            #region 获取编译后的模板内容
            try
            {
                if (config.CompiledViewProvider.HasCompiledView(viewPath))
                {
                    template = config.CompiledViewProvider.Get(viewPath);
                }
                else
                {
                    //如果不具有源视图  那么直接返回null
                    if (!config.RawViewProvider.HasView(viewPath)) return null;
                    //现编译并且存储编译后的内容
                    var rowContent = config.RawViewProvider.GetContent(viewPath);
                    template = razorEngine.Compile(rowContent, config.DefaultBuildAction);
                    config.CompiledViewProvider.SetView(viewPath, template);
                }
            }
            catch (Exception ex)
            {
                LogCompileRawViewError(viewPath, ex);
                return null;
            }
            if (template == null) return null;
            #endregion

            #region 再运行
            try
            {
                return template.Run(this, model, viewBag, isRenderSub);
            }
            catch (Exception ex)
            {
                LogRunCompiledViewError(viewPath, ex, model);
            }
            #endregion

            return null;
        }
        /// <summary>
        /// 编译并运行指定View
        /// </summary>
        /// <param name="viewPath"></param>
        /// <param name="model"></param>
        /// <param name="viewBag"></param>
        /// <param name="code"></param>
        /// <param name="throwErrorOnFailed">当失败时是否抛出异常</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public HtmlResponse Render(string viewPath, dynamic model = null, dynamic viewBag = null, HttpStatusCode code = HttpStatusCode.OK, bool throwErrorOnFailed = true)
        {
            string rawContent = RenderRaw(viewPath, model, viewBag);
            return CreateHtmlResponse(rawContent, code, throwErrorOnFailed);
        }
        #endregion


        #region Render View Async
        /// <summary>
        /// 编译并运行指定View  异步版本
        /// </summary>
        /// <param name="viewPath"></param>
        /// <param name="model"></param>
        /// <param name="viewBag"></param>
        /// <param name="code"></param>
        /// <param name="throwErrorOnFailed">失败时是否抛出异常</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<HtmlResponse> RenderAsync(string viewPath, dynamic model = null, dynamic viewBag = null, HttpStatusCode code = HttpStatusCode.OK, bool throwErrorOnFailed = true)
        {
            string rawContent = await RenderRawAsync(viewPath, model, viewBag);
            return CreateHtmlResponse(rawContent, code, throwErrorOnFailed);
        }
        /// <summary>
        /// 编译并运行指定View  异步版本
        /// </summary>
        /// <param name="viewPath"></param>
        /// <param name="model"></param>
        /// <param name="viewBag"></param>
        /// <param name="isRenderSub">是否是渲染子视图</param>
        /// <returns></returns>
        public async Task<string> RenderRawAsync(string viewPath, dynamic model = null, dynamic viewBag = null, bool isRenderSub = false)
        {
            //获取编译后的模板内容
            IRazorEngineCompiledTemplate template;

            #region 获取编译后的模板内容
            try
            {
                if (await config.CompiledViewProvider.HasCompiledViewAsync(viewPath))
                {
                    template = await config.CompiledViewProvider.GetAsync(viewPath);
                }
                else
                {
                    //如果不具有源视图  那么直接返回null
                    if (!await config.RawViewProvider.HasViewAsync(viewPath)) return null;
                    //现编译并且存储编译后的内容
                    var rowContent = await config.RawViewProvider.GetContentAsync(viewPath);
                    template = await razorEngine.CompileAsync(rowContent, config.DefaultBuildAction);
                    await config.CompiledViewProvider.SetViewAsync(viewPath, template);
                }
            }
            catch (Exception ex)
            {
                LogCompileRawViewError(viewPath, ex);
                return null;
            }
            if (template == null) return null;
            #endregion

            #region 再运行
            try
            {
                return await template.RunAsync(this, model, viewBag, isRenderSub);
            }
            catch (Exception ex)
            {
                LogRunCompiledViewError(viewPath, ex, model);
            }
            #endregion

            return null;
        }
        #endregion


        #region 私有方法
        /// <summary>
        /// 根据字符内容创建HtmlResponse对象
        /// </summary>
        /// <param name="rawContent">源字符内容</param>
        /// <param name="code">http 编码</param>
        /// <param name="throwErrorOnFailed">当内容为空时是否抛出异常</param>
        /// <returns></returns>
        private HtmlResponse CreateHtmlResponse(string rawContent, HttpStatusCode code, bool throwErrorOnFailed)
        {
            if (string.IsNullOrEmpty(rawContent))
            {
                if (throwErrorOnFailed)
                {
                    throw new InvalidOperationException("Unable to render view");
                }
                return null;
            }

            return new HtmlResponse(rawContent)
            {
                StatusCode = (int)code
            };
        }
        /// <summary>
        /// 记录编译源视图的错误信息
        /// </summary>
        /// <param name="viewPath">视图路径</param>
        /// <param name="exception">异常信息</param>
        private void LogCompileRawViewError(string viewPath, Exception exception)
        {
            logProvider.LogError($"unable to compile raw view {viewPath}, Error: {exception.Message}");
        }
        /// <summary>
        /// 记录运行编译视图时的错误信息
        /// </summary>
        /// <param name="viewPath">视图路径</param>
        /// <param name="exception">异常信息</param>
        /// <param name="model">视图附带的model信息</param>
        private void LogRunCompiledViewError(string viewPath, Exception exception, dynamic model = null)
        {
            var modelStr = model == null ? string.Empty : $" with parameter:[{JsonConvert.SerializeObject(model)}]";
            logProvider.LogError($"unable to run compiled view {viewPath}{modelStr}, Error: {exception.Message}");
        }
        #endregion
    }
}
