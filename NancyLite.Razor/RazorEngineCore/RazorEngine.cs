using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;

namespace RazorEngineCore
{
    public class RazorEngine
    {

        public RazorEngineCompiledTemplate Compile(string content, Action<RazorEngineCompilationOptionsBuilder> builderAction = null)
        {
            var compilationOptionsBuilder = new RazorEngineCompilationOptionsBuilder();
            compilationOptionsBuilder.Inherits(typeof(RazorEngineTemplateBase));
             
            builderAction?.Invoke(compilationOptionsBuilder);

            var memoryStream = CreateAndCompileToStream(content, compilationOptionsBuilder.Options);

            return new RazorEngineCompiledTemplate(memoryStream);
        }

        private MemoryStream CreateAndCompileToStream(string templateSource, RazorEngineCompilationOptions options)
        {
            templateSource = WriteDirectives(templateSource, options);

            var engine = RazorProjectEngine.Create(
                RazorConfiguration.Default,
                RazorProjectFileSystem.Create(@"."),
                (builder) =>
                {
                    builder.SetNamespace(options.TemplateNamespace);
                });

            var fileName = Path.GetRandomFileName();

            var document = RazorSourceDocument.Create(templateSource, fileName);

            var codeDocument = engine.Process(
                document,
                null,
                new List<RazorSourceDocument>(),
                new List<TagHelperDescriptor>());

            var razorCSharpDocument = codeDocument.GetCSharpDocument();

            var syntaxTree = CSharpSyntaxTree.ParseText(razorCSharpDocument.GeneratedCode);

            var compilation = CSharpCompilation.Create
            (
                fileName,
                new[]{syntaxTree},
                options.ReferencedAssemblies.Select(ass => MetadataReference.CreateFromFile(ass.Location)).ToList(),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
               );

            var memoryStream = new MemoryStream();

            var emitResult = compilation.Emit(memoryStream);

            if (!emitResult.Success)
            {
                var errors = emitResult.Diagnostics.ToList();
                var errorMsg = JsonConvert.SerializeObject(emitResult.Diagnostics.Select(x => x.GetMessage()).ToList(), Formatting.Indented);
                var exception = new RazorEngineCompilationException($"Unable to compile template: {errorMsg}" )
                {
                    Errors = errors
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