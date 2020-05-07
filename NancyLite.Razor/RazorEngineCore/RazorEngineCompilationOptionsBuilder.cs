using System;
using System.Linq;
using System.Reflection;

namespace RazorEngineCore
{
    public class RazorEngineCompilationOptionsBuilder
    {
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
            if (this.Options.ReferencedAssemblies.Contains(assembly))
            {
                return;
            }

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

        public void AddUsing(string namespaceName)
        {
            if(!Options.DefaultUsings.Contains(namespaceName)) Options.DefaultUsings.Add(namespaceName);
        }

        public void Inherits(Type type)
        {
            Options.Inherits = RenderTypeName(type);
            AddAssemblyReference(type);
        }

        private string RenderTypeName(Type type)
        {
            var result = type.Namespace + "." + type.Name;

            if (result.Contains('`'))
            {
                result = result.Substring(0, result.IndexOf("`"));
            }

            if (type.GenericTypeArguments.Length == 0)
            {
                return result;
            }

            return result + "<" + string.Join(",", type.GenericTypeArguments.Select(RenderTypeName)) + ">";
        }
    }
}