using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RazorEngineCore;
using System;
using System.Net;

namespace NancyLite.Razor
{
    public class RazorEnginePlus : RazorEngine
    {
        private readonly ILogger<RazorEnginePlus> logProvider;
        private readonly NancyLiteRazorConfig config;
        public RazorEnginePlus(ILogger<RazorEnginePlus> iLogProvider, NancyLiteRazorConfig razorConfig)
        {
            logProvider = iLogProvider;
            config = razorConfig;
        }

        public string QuickRenderRaw(string content, dynamic model = null, dynamic viewBag = null)
        {
            try
            {
                var template = Compile(content, config.DefaultBuildAction);
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
        public string RenderRaw(string viewPath, dynamic model = null, dynamic viewBag = null)
        {
            RazorEngineCompiledTemplate template;
            if (config.CompiledViewProvider.HasCompiledView(viewPath))
            {
                #region 已编译的直接运行
                try
                {
                    template = config.CompiledViewProvider.Get(viewPath);
                    return template.Run(this, model, viewBag);
                }
                catch (Exception ex)
                {
                    if (model == null)
                    {
                        logProvider.LogError($"unable to run compiled view {viewPath}, Error: {ex.Message}");
                    }
                    else
                    {
                        logProvider.LogError($"unable to run compiled view {viewPath} with parameter:[{JsonConvert.SerializeObject(model)}], Error: {ex.Message}");
                    }
                    return null;
                }
                #endregion
            }

            if (config.RawViewProvider.HasView(viewPath))
            {
                #region 未编译的现编译
                try
                {
                    var rowContent = config.RawViewProvider.GetContent(viewPath);
                    template = Compile(rowContent, config.DefaultBuildAction);
                    config.CompiledViewProvider.SetView(viewPath, template);
                }
                catch (Exception ex)
                {
                    logProvider.LogError($"unable to compile raw view {viewPath}, Error: {ex.Message}");
                    return null;
                }
                #endregion

                #region 再运行
                try
                {
                    return template.Run(this, model, viewBag);
                }
                catch (Exception ex)
                {
                    if (model == null)
                    {
                        logProvider.LogError($"unable to run compiled view {viewPath}, Error: {ex.Message}");
                    }
                    else
                    {
                        logProvider.LogError($"unable to run compiled view {viewPath} with parameter:[{JsonConvert.SerializeObject(model)}], Error: {ex.Message}");
                    }
                }
                #endregion
            }

            return null;
        }
        public HtmlResponse Render(string viewPath, dynamic model = null, dynamic viewBag = null, HttpStatusCode code = HttpStatusCode.OK, bool throwErrorOnFailed = true)
        {
            var rawContent = RenderRaw(viewPath, model, viewBag);
            if (rawContent == null)
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

        public string RenderRawSub(string viewPath, ref dynamic viewBag, dynamic model = null)
        {
            RazorEngineCompiledTemplate template;
            if (config.CompiledViewProvider.HasCompiledView(viewPath))
            {
                #region 已编译的直接运行
                try
                {
                    template = config.CompiledViewProvider.Get(viewPath);
                    return template.RunSub(this, ref viewBag, model);
                }
                catch (Exception ex)
                {
                    if (model == null)
                    {
                        logProvider.LogError($"unable to run compiled view {viewPath}, Error: {ex.Message}");
                    }
                    else
                    {
                        logProvider.LogError($"unable to run compiled view {viewPath} with parameter:[{JsonConvert.SerializeObject(model)}], Error: {ex.Message}");
                    }
                }
                #endregion
            }

            if (config.RawViewProvider.HasView(viewPath))
            {
                #region 未编译的现编译
                try
                {
                    var rowContent = config.RawViewProvider.GetContent(viewPath);
                    template = Compile(rowContent, config.DefaultBuildAction);
                    config.CompiledViewProvider.SetView(viewPath, template);
                }
                catch (Exception ex)
                {
                    logProvider.LogError($"unable to compile raw view {viewPath}, Error: {ex.Message}");
                    return null;
                }
                #endregion

                #region 再运行
                try
                {
                    return template.RunSub(this, ref viewBag, model);
                }
                catch (Exception ex)
                {
                    if (model == null)
                    {
                        logProvider.LogError($"unable to run compiled view {viewPath}, Error: {ex.Message}");
                    }
                    else
                    {
                        logProvider.LogError($"unable to run compiled view {viewPath} with parameter:[{JsonConvert.SerializeObject(model)}], Error: {ex.Message}");
                    }
                }
                #endregion
            }

            return null;
        }
    }
}
