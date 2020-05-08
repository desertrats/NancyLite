﻿using System.Collections.Generic;
using System.Reflection;

namespace RazorEngineCore
{
    public class RazorEngineCompilationOptions
    {
        public IList<Assembly> ReferencedAssemblies { get; set; } = new List<Assembly>()
        {
            typeof(object).Assembly,
            Assembly.Load(new AssemblyName("Microsoft.CSharp")),
            typeof(RazorEngineTemplateBase).Assembly,
            Assembly.Load(new AssemblyName("netstandard")),
            Assembly.Load(new AssemblyName("System.Runtime")),
            Assembly.Load(new AssemblyName("System.Collections")),
            Assembly.Load(new AssemblyName("System.Linq")),
            Assembly.Load(new AssemblyName("System.Linq.Expressions")),
        };

        public string TemplateNamespace { get; set; } = "TemplateNamespace";
        public string Inherits { get; set; } = "RazorEngineCore.RazorEngineTemplateBase";

        public IList<string> DefaultUsings { get; set; } = new List<string>()
        {
            "System",
            "System.Linq",
            "NancyLite.Razor",
            "NancyLite"
        };
    }
}