using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.IO;
using System.Reflection;

namespace NancyLite.Razor.Test
{
    /// <summary>
    /// 供各单元测试继承的对象  提供RazorEnginePlus实体
    /// </summary>
    public class RazorEnginePlusModel
    {
        private static ILogger<RazorEnginePlus> logger = new Logger<RazorEnginePlus>(new NullLoggerFactory());
        /// <summary>
        /// 默认的注册配置
        /// </summary>
        private static readonly NancyLiteRazorConfig DefaultConfig = new NancyLiteRazorConfig
        {
            DefaultBuildAction = x =>
            {
                x.AddAssemblyReference(Assembly.GetEntryAssembly());
                x.AddAssemblyReference(typeof(QuickTestModel));
            },
            CompiledViewProvider = new DefaultCompiledViewProvider(),
            RawViewProvider = new DefaultViewProvider(Path.Combine(Directory.GetCurrentDirectory(), "View"))
        };
        protected RazorEnginePlus razorEnginePlus = new(logger, DefaultConfig);
    }
}
