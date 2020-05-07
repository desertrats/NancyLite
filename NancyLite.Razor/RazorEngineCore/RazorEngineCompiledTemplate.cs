using NancyLite.Razor;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace RazorEngineCore
{
    public class RazorEngineCompiledTemplate
    {
        [JsonProperty]
        [JsonConverter(typeof(MemoryStreamJsonConverter))]
        private readonly MemoryStream assemblyByteCode;
        [JsonIgnore]
        private readonly Type templateType;
        [JsonConstructor]
        internal RazorEngineCompiledTemplate(MemoryStream assemblyByteCode)
        {
            this.assemblyByteCode = assemblyByteCode;

            var assembly = Assembly.Load(assemblyByteCode.ToArray());
            templateType = assembly.GetType("TemplateNamespace.Template");
        }

        public static RazorEngineCompiledTemplate LoadFromFile(string fileName)
        {
            return new RazorEngineCompiledTemplate(new MemoryStream(File.ReadAllBytes(fileName)));
        }

        public static RazorEngineCompiledTemplate LoadFromStream(Stream stream)
        {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            return new RazorEngineCompiledTemplate(memoryStream);
        }

        public void SaveToStream(Stream stream)
        {
            assemblyByteCode.CopyTo(stream);
        }

        public void SaveToFile(string fileName)
        {
            File.WriteAllBytes(fileName, assemblyByteCode.ToArray());
        }

        public string Run(RazorEnginePlus razor, object model = null, dynamic viewBag = null)
        {
            if (model != null && model.IsAnonymous())
            {
                model = new AnonymousTypeWrapper(model);
            }

            var instance = (RazorEngineTemplateBase)Activator.CreateInstance(templateType);
#if !DEBUG
            if (instance == null) return "";
#endif
            instance.Html.Initialize(razor);
            instance.Model = model;
            instance.ViewBag = ViewBagCombine.Combine(instance.ViewBag, viewBag);
            instance.ExecuteAsync().Wait();
            //获取instance.Layout必须在 instance.ExecuteAsync().Wait();之后否则无法取值
            if (!string.IsNullOrEmpty(instance.Layout))
            {
                return razor.RenderRawSub(instance.Layout, ref instance.ViewBag, instance.Model);
            }
            var result = instance.Result();
            return result;
        }

        public string RunSub(RazorEnginePlus razor, ref dynamic viewBag, object model = null)
        {
            if (model != null && model.IsAnonymous())
            {
                model = new AnonymousTypeWrapper(model);
            }

            var instance = (RazorEngineTemplateBase)Activator.CreateInstance(templateType);
#if !DEBUG
            if (instance == null) return "";
#endif
            instance.Html.Initialize(razor);
            instance.Model = model;
            instance.ViewBag = ViewBagCombine.Combine(instance.ViewBag, viewBag);
            instance.ExecuteAsync().Wait();
            viewBag = ViewBagCombine.Combine(viewBag, instance.ViewBag);
            //获取instance.Layout必须在 instance.ExecuteAsync().Wait();之后否则无法取值
            if (!string.IsNullOrEmpty(instance.Layout))
            {
                return razor.RenderRawSub(instance.Layout, ref instance.ViewBag, instance.Model);
            }
            var result = instance.Result();
            return result;
        }
    }
}