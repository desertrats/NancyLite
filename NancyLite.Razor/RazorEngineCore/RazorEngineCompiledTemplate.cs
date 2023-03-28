using NancyLite.Razor;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public class RazorEngineCompiledTemplate : IRazorEngineCompiledTemplate
    {
        #region 字段以及构造方法
        /**
         * CustomEdit-Begin:为了将编译后的模板进行序列化存储，我们在字段以及构造方法上加上了Json.Net的注解
         */
        /// <summary>
        /// 编译后的二进制内容
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(MemoryStreamJsonConverter))]
        private readonly MemoryStream assemblyByteCode;
        /// <summary>
        /// 模板类型
        /// </summary>
        [JsonIgnore]
        private readonly Type templateType;

        /// <summary>
        /// 构造方法
        /// <para>标注JsonConstructor代表在Json反序列化时使用此构造函数创建对象</para>
        /// <para>但是要注意参数名称要与json数据源中的属性名一致，忽略大小写</para>
        /// </summary>
        /// <param name="assemblyByteCode"></param>
        /// <param name="templateNamespace">注意此处如果用户在编译时自定义了templateNamespace的话，需要在Json序列化时将templateNamespace一并进行保存 这样才能正常反序列化</param>
        [JsonConstructor]
        internal RazorEngineCompiledTemplate(MemoryStream assemblyByteCode, string templateNamespace = null)
        {
            this.assemblyByteCode = assemblyByteCode;

            var assembly = Assembly.Load(assemblyByteCode.ToArray());
            templateNamespace = string.IsNullOrEmpty(templateNamespace) ? "TemplateNamespace" : templateNamespace;
            templateType = assembly.GetType(templateNamespace + ".Template");
        }
        /*
        * CustomEdit-End
        */
        #endregion

        #region Load From File Or Stream
        /// <summary>
        /// 从文件中读取编译模板
        /// </summary>
        /// <param name="fileName">文件名称，含路径</param>
        /// <param name="templateNamespace">编译时指定的模板命名空间</param>
        /// <returns></returns>
        public static IRazorEngineCompiledTemplate LoadFromFile(string fileName, string templateNamespace = "TemplateNamespace")
        {
            //CustomEdit:防止阻塞异步代码
            //return LoadFromFileAsync(fileName, templateNamespace).GetAwaiter().GetResult();
            return new RazorEngineCompiledTemplate(new MemoryStream(File.ReadAllBytes(fileName)), templateNamespace);
        }
        /// <summary>
        /// 从文件中读取编译模板 异步版本
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="templateNamespace">编译时指定的模板命名空间</param>
        /// <returns></returns>
        public static async Task<IRazorEngineCompiledTemplate> LoadFromFileAsync(string fileName, string templateNamespace = "TemplateNamespace")
        {
            var memoryStream = new MemoryStream();

            await using (var fileStream = new FileStream(
                             path: fileName,
                             mode: FileMode.Open,
                             access: FileAccess.Read,
                             share: FileShare.None,
                             bufferSize: 4096,
                             useAsync: true))
            {
                await fileStream.CopyToAsync(memoryStream);
            }

            return new RazorEngineCompiledTemplate(memoryStream, templateNamespace);
        }
        /// <summary>
        /// 从流中读取编译模板
        /// </summary>
        /// <param name="stream">对应的流</param>
        /// <param name="templateNamespace">编译时指定的模板命名空间</param>
        /// <returns></returns>
        public static IRazorEngineCompiledTemplate LoadFromStream(Stream stream, string templateNamespace = "TemplateNamespace")
        {
            //CustomEdit:防止阻塞异步代码
            // return LoadFromStreamAsync(stream).GetAwaiter().GetResult();

            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            return new RazorEngineCompiledTemplate(memoryStream, templateNamespace);
        }
        /// <summary>
        /// 从流中读取编译模板 异步版本
        /// </summary>
        /// <param name="stream">对应的流</param>
        /// <param name="templateNamespace">编译时指定的模板命名空间</param>
        /// <returns></returns>
        public static async Task<IRazorEngineCompiledTemplate> LoadFromStreamAsync(Stream stream, string templateNamespace = "TemplateNamespace")
        {
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            return new RazorEngineCompiledTemplate(memoryStream, templateNamespace);
        }
        #endregion

        #region Save To Stream Or File
        /// <summary>
        /// 将编译后的模板内容保存至流
        /// </summary>
        /// <param name="stream"></param>
        public void SaveToStream(Stream stream)
        {
            //CustomEdit:防止阻塞异步代码
            //this.SaveToStreamAsync(stream).GetAwaiter().GetResult();
            assemblyByteCode.CopyTo(stream);
        }
        /// <summary>
        /// 将编译后的模板内容保存至流 异步版本
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public Task SaveToStreamAsync(Stream stream)
        {
            return assemblyByteCode.CopyToAsync(stream);
        }

        /// <summary>
        /// 将编译后的模板内容保存至文件
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveToFile(string fileName)
        {
            //CustomEdit:防止阻塞异步代码
            //this.SaveToFileAsync(fileName).GetAwaiter().GetResult();
            File.WriteAllBytes(fileName, assemblyByteCode.ToArray());
        }
        /// <summary>
        /// 将编译后的模板内容保存至文件 异步版本
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Task SaveToFileAsync(string fileName)
        {
            using var fileStream = new FileStream(
                path: fileName,
                mode: FileMode.OpenOrCreate,
                access: FileAccess.Write,
                share: FileShare.None,
                bufferSize: 4096,
                useAsync: true);
            return assemblyByteCode.CopyToAsync(fileStream);
        }
        #endregion


        public string Run(RazorEnginePlus razor, object model = null, dynamic viewBag = null, bool isRunSub = false)
        {
            if (model != null && model.IsAnonymous())
            {
                model = new AnonymousTypeWrapper(model);
            }

            var instance = (IRazorEngineTemplate)Activator.CreateInstance(templateType);

#if !DEBUG
            if (instance == null) return "";
#endif
            instance.Html.Initialize(razor);
            instance.Model = model;
            instance.ViewBag = ViewBagCombine.Combine(instance.ViewBag, viewBag);

            instance.Execute();
            //渲染子视图的核心  将当前页面新声明的ViewBag属性加到父级的ViewBag上
            //因为ViewBag是个引用类型，所以此处的修改同样可以作用到父级的ViewBag上  而不必采用ref的方式
            if (isRunSub)
            {
                ViewBagCombine.Combine(viewBag, instance.ViewBag);
            }
            //获取instance.Layout必须在 instance.Execute();之后否则无法取值
            if (!string.IsNullOrEmpty(instance.Layout))
            {
                //递归调用RenderRaw方法渲染Layout视图
                //注意将渲染子视图参数设置为true
                return razor.RenderRaw(instance.Layout, instance.Model, instance.ViewBag, true);
            }
            return instance.Result();
        }

        public async Task<string> RunAsync(RazorEnginePlus razor, object model = null, dynamic viewBag = null, bool isRunSub = false)
        {
            if (model != null && model.IsAnonymous())
            {
                model = new AnonymousTypeWrapper(model);
            }

            var instance = (IRazorEngineTemplate)Activator.CreateInstance(templateType);
#if !DEBUG
            if (instance == null) return "";
#endif

            instance.Html.Initialize(razor);
            instance.Model = model;
            instance.ViewBag = ViewBagCombine.Combine(instance.ViewBag, viewBag);

            await instance.ExecuteAsync();
            //渲染子视图的核心  将当前页面新声明的ViewBag属性加到父级的ViewBag上
            //因为ViewBag是个引用类型，所以此处的修改同样可以作用到父级的ViewBag上  而不必采用ref的方式
            if (isRunSub)
            {
                ViewBagCombine.Combine(viewBag, instance.ViewBag);
            }
            //获取instance.Layout必须在 instance.ExecuteAsync();之后否则无法取值
            if (!string.IsNullOrEmpty(instance.Layout))
            {
                //递归调用RenderRawAsync方法渲染Layout视图
                //注意将渲染子视图参数设置为true
                return await razor.RenderRawAsync(instance.Layout, instance.Model, instance.ViewBag, true);
            }

            return await instance.ResultAsync();
        }
    }
}