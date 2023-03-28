using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace RazorEngineCore
{
    /// <summary>
    /// Razor模板编译选项类  内含所引用的程序集以及模板命名空间等信息
    /// <para>请与<see cref="RazorEngineCompilationOptionsBuilder"/>配合使用</para>
    /// </summary>
    public class RazorEngineCompilationOptions
    {
        /// <summary>
        /// 所引用的程序集
        /// </summary>
        public HashSet<Assembly> ReferencedAssemblies { get; set; }
        /// <summary>
        /// Metadata引用  
        /// </summary>
        public HashSet<MetadataReference> MetadataReferences { get; set; } = new HashSet<MetadataReference>();
        /// <summary>
        /// 模板命名空间 默认为TemplateNamespace
        /// </summary>
        public string TemplateNamespace { get; set; } = "TemplateNamespace";
        /// <summary>
        /// 模板文件名称  默认为""
        /// </summary>
        public string TemplateFilename { get; set; } = "";
        /// <summary>
        /// 模板文件所继承的类名
        /// <para>默认为<see cref="RazorEngineTemplateBase"/></para>
        /// </summary>
        public string Inherits { get; set; } = "RazorEngineCore.RazorEngineTemplateBase";
        /// <summary>
        /// 默认的命名空间引用
        /// </summary>
        public HashSet<string> DefaultUsings { get; set; } = new HashSet<string>
        {
            "System.Linq",
            "System.Collections",
            "System.Collections.Generic",
            //CustomEdit: 这两个命名空间是我们自己的库 同时新版源码遗漏了System 在此一并加上
            "NancyLite.Razor",
            "NancyLite",
            "System"
        };

        public RazorEngineCompilationOptions()
        {
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            var isFullFramework = RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase);

            if (isWindows)
            {
                if (isFullFramework)
                {
                    ReferencedAssemblies = new HashSet<Assembly>
                    {
                        typeof(object).Assembly,
                        Assembly.Load(new AssemblyName("Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        typeof(RazorEngineTemplateBase).Assembly,
                        typeof(System.Runtime.GCSettings).Assembly,
                        //CustomEdit:System.Collections无法通过typeof方式正常注册  所以改为使用程序集名称
                        Assembly.Load(new AssemblyName("System.Collections")),
                        typeof(System.Linq.Enumerable).Assembly,
                        typeof(System.Linq.Expressions.Expression).Assembly,
                        Assembly.Load(new AssemblyName("netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51"))
                    };
                }
                else  // i.e. NETCore
                {
                    ReferencedAssemblies = new HashSet<Assembly>
                    {
                        typeof(object).Assembly,
                        Assembly.Load(new AssemblyName("Microsoft.CSharp")),
                        typeof(RazorEngineTemplateBase).Assembly,
                        Assembly.Load(new AssemblyName("System.Runtime")),
                        //CustomEdit:System.Collections无法通过typeof方式正常注册  所以改为使用程序集名称
                        Assembly.Load(new AssemblyName("System.Collections")),
                        Assembly.Load(new AssemblyName("System.Linq")),
                        Assembly.Load(new AssemblyName("System.Linq.Expressions")),
                        Assembly.Load(new AssemblyName("netstandard"))
                    };
                }

            }
            else
            {
                ReferencedAssemblies = new HashSet<Assembly>
                {
                    typeof(object).Assembly,
                    Assembly.Load(new AssemblyName("Microsoft.CSharp")),
                    typeof(RazorEngineTemplateBase).Assembly,
                    Assembly.Load(new AssemblyName("System.Runtime")),
                    //CustomEdit:System.Collections无法通过typeof方式正常注册  所以改为使用程序集名称
                    Assembly.Load(new AssemblyName("System.Collections")),
                    Assembly.Load(new AssemblyName("System.Linq")),
                    Assembly.Load(new AssemblyName("System.Linq.Expressions")),
                    Assembly.Load(new AssemblyName("netstandard"))
                };
            }
        }
    }
}