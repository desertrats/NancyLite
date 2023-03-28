using Microsoft.Extensions.DependencyInjection;
using RazorEngineCore;
using System;
using System.IO;
using System.Reflection;

namespace NancyLite.Razor
{
    /// <summary>
    /// 声明了将NancyLiteRazor注册到IOC容器中的扩展方法
    /// </summary>
    public static class BuilderWrapper
    {
        /// <summary>
        /// 默认的注册配置
        /// </summary>
        private static readonly NancyLiteRazorConfig DefaultConfig = new NancyLiteRazorConfig
        {
            DefaultBuildAction = x => x.AddAssemblyReference(Assembly.GetEntryAssembly()),
            CompiledViewProvider = new DefaultCompiledViewProvider(),
            RawViewProvider = new DefaultViewProvider(Path.Combine(Directory.GetCurrentDirectory(), "View"))
        };
        public static void RegisterNancyLiteRazor(this IServiceCollection services, NancyLiteRazorConfig config = null)
        {
            if (config == null)
            {
                config = DefaultConfig;
            }

            services.AddSingleton(_ => config);
            services.AddSingleton<RazorEnginePlus>();
        }

        public static void RegisterNancyLiteRazor(this IServiceCollection services, Action<IRazorEngineCompilationOptionsBuilder> buildAction, IViewProvider viewProvider, ICompiledViewProvider compiledViewProvider)
        {
            var config = new NancyLiteRazorConfig
            {
                DefaultBuildAction = buildAction,
                CompiledViewProvider = compiledViewProvider,
                RawViewProvider = viewProvider
            };
            services.AddSingleton(_ => config);
            services.AddSingleton<RazorEnginePlus>();
        }

        public static void RegisterNancyLiteRazor(this IServiceCollection services, Action<IRazorEngineCompilationOptionsBuilder> buildAction)
        {
            services.AddSingleton(serviceProvider =>
            {
                var vp = serviceProvider.GetRequiredService<IViewProvider>();
                var cvp = serviceProvider.GetRequiredService<ICompiledViewProvider>();
                return new NancyLiteRazorConfig
                {
                    DefaultBuildAction = buildAction,
                    CompiledViewProvider = cvp,
                    RawViewProvider = vp
                };
            });
            services.AddSingleton<RazorEnginePlus>();
        }
    }
}
