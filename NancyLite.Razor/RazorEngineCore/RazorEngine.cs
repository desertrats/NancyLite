using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    /// <summary>
    /// Razor编译引擎实现
    /// </summary>
    public class RazorEngine : IRazorEngine
    {
        public IRazorEngineCompiledTemplate Compile(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null)
        {
            IRazorEngineCompilationOptionsBuilder compilationOptionsBuilder = new RazorEngineCompilationOptionsBuilder();
            compilationOptionsBuilder.Inherits(typeof(RazorEngineTemplateBase));

            builderAction?.Invoke(compilationOptionsBuilder);

            var memoryStream = CreateAndCompileToStream(content, compilationOptionsBuilder.Options);

            return new RazorEngineCompiledTemplate(memoryStream, compilationOptionsBuilder.Options.TemplateNamespace);
        }

        public Task<IRazorEngineCompiledTemplate> CompileAsync(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null)
        {
            //CustomEdit:Task.Run() /Task.Factory.StartNew()  vs  Task.FromResult 详见SOURCE.md中的说明
            //return Task.Factory.StartNew(() => this.Compile(content: content, builderAction: builderAction));
            return Task.FromResult(Compile(content: content, builderAction: builderAction));
        }

        /// <summary>
        /// 创建并且编译指定内容的核心方法
        /// <para>CustomEdit:修改为静态方法</para>
        /// </summary>
        /// <param name="templateSource"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private static MemoryStream CreateAndCompileToStream(string templateSource, RazorEngineCompilationOptions options)
        {
            templateSource = WriteDirectives(templateSource, options);

            var engine = RazorProjectEngine.Create(
                RazorConfiguration.Default,
                RazorProjectFileSystem.Create(@"."),
                builder =>
                {
                    builder.SetNamespace(options.TemplateNamespace);
                });

            var fileName = string.IsNullOrWhiteSpace(options.TemplateFilename) ? Path.GetRandomFileName() : options.TemplateFilename;

            var document = RazorSourceDocument.Create(templateSource, fileName);

            var codeDocument = engine.Process(
                document,
                null,
                new List<RazorSourceDocument>(),
                new List<TagHelperDescriptor>());

            var razorCSharpDocument = codeDocument.GetCSharpDocument();

            var syntaxTree = CSharpSyntaxTree.ParseText(razorCSharpDocument.GeneratedCode);

            var compilation = CSharpCompilation.Create(
              fileName,
              new[]
              {
                    syntaxTree
              },
              options.ReferencedAssemblies
                 .Select(ass =>
                 {
#if NETSTANDARD2_0
                            return  MetadataReference.CreateFromFile(ass.Location); 
#else
                     unsafe
                     {
                         ass.TryGetRawMetadata(out var blob, out var length);
                         var moduleMetadata = ModuleMetadata.CreateFromMetadata((IntPtr)blob, length);
                         var assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);
                         var metadataReference = assemblyMetadata.GetReference();

                         return metadataReference;
                     }
#endif
                 })
                  .Concat(options.MetadataReferences)
                  .ToList(),
              new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var memoryStream = new MemoryStream();

            var emitResult = compilation.Emit(memoryStream);

            if (!emitResult.Success)
            {
                var exception = new RazorEngineCompilationException
                {
                    Errors = emitResult.Diagnostics.ToList(),
                    GeneratedCode = razorCSharpDocument.GeneratedCode
                };

                throw exception;
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        private static string WriteDirectives(string content, RazorEngineCompilationOptions options)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("@inherits " + options.Inherits);

            foreach (var entry in options.DefaultUsings)
            {
                stringBuilder.AppendLine("@using " + entry);
            }

            stringBuilder.Append(content);

            return stringBuilder.ToString();
        }
    }
}