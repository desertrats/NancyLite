using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NancyLite
{
    public class ScopeAssembly
    {
        private readonly DependencyContext dependencyContext;
        private static readonly string assemblyName;

       static ScopeAssembly()
        {
            assemblyName = typeof(BuilderWarpper).Assembly.GetName().Name;
        }
        public ScopeAssembly()
            : this(Assembly.GetEntryAssembly())
        {
        }
        public ScopeAssembly(Assembly entryAssembly)
        {
            dependencyContext = DependencyContext.Load(entryAssembly);
        }

        public virtual IReadOnlyCollection<Assembly> GetAssemblies()
        {
            var results = new HashSet<Assembly>
            {
                typeof(ScopeAssembly).Assembly
            };

            foreach (var library in dependencyContext.RuntimeLibraries)
            {
                if (ReferencedMe(library))
                {
                    foreach (var assemblyName in library.GetDefaultAssemblyNames(dependencyContext))
                    {
                        results.Add(SafeLoadAssembly(assemblyName));
                    }
                }
            }

            return results;
        }
        private static Assembly SafeLoadAssembly(AssemblyName assemblyName)
        {
            try
            {
                return Assembly.Load(assemblyName);
            }
            catch (Exception)
            {
                return null;
            }
        }
        private static bool ReferencedMe(Library library)
        {
            return library.Dependencies.Any(dependency => dependency.Name.Equals(assemblyName));
        }

    }
}
