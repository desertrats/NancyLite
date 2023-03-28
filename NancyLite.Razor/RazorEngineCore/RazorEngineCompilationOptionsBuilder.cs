using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RazorEngineCore
{
    /// <summary>
    /// <see cref="RazorEngineCompilationOptions"/>的Builder实现
    /// </summary>
    public class RazorEngineCompilationOptionsBuilder : IRazorEngineCompilationOptionsBuilder
    {
        /// <summary>
        /// 要进行Builder的模板编译选项信息
        /// </summary>
        public RazorEngineCompilationOptions Options { get; set; }

        public RazorEngineCompilationOptionsBuilder(RazorEngineCompilationOptions options = null)
        {
            Options = options ?? new RazorEngineCompilationOptions();
        }

        public void AddAssemblyReferenceByName(string assemblyName)
        {
            var assembly = Assembly.Load(new AssemblyName(assemblyName));
            AddAssemblyReference(assembly);
        }

        public void AddAssemblyReference(Assembly assembly)
        {
            Options.ReferencedAssemblies.Add(assembly);
        }

        public void AddAssemblyReference(Type type)
        {
            AddAssemblyReference(type.Assembly);

            foreach (var argumentType in type.GenericTypeArguments)
            {
                AddAssemblyReference(argumentType);
            }
        }

        public void AddMetadataReference(MetadataReference reference)
        {
            Options.MetadataReferences.Add(reference);
        }

        public void AddUsing(string namespaceName)
        {
            Options.DefaultUsings.Add(namespaceName);
        }

        public void Inherits(Type type)
        {
            Options.Inherits = RenderTypeName(type);
            AddAssemblyReference(type);
        }

        private string RenderTypeName(Type type)
        {
            IList<string> elements = new List<string>
            {
                type.Namespace,
                RenderDeclaringType(type.DeclaringType),
                type.Name
            };

            var result = string.Join(".", elements.Where(e => !string.IsNullOrWhiteSpace(e)));

            if (result.Contains('`'))
            {
                result = result.Substring(0, result.IndexOf("`", StringComparison.Ordinal));
            }

            if (type.GenericTypeArguments.Length == 0)
            {
                return result;
            }

            return result + "<" + string.Join(",", type.GenericTypeArguments.Select(RenderTypeName)) + ">";
        }

        private static string RenderDeclaringType(Type type)
        {
            if (type == null)
            {
                return null;
            }

            var parent = RenderDeclaringType(type.DeclaringType);

            if (string.IsNullOrWhiteSpace(parent))
            {
                return type.Name;
            }

            return parent + "." + type.Name;
        }
    }
}