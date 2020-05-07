using Microsoft.Extensions.DependencyInjection;
using RazorEngineCore;
using System;
using System.IO;
using System.Reflection;

namespace NancyLite.Razor
{
    public static class BuilderWarpper
    {
        private static readonly NancyLiteRazorConfig defaultConfig = new NancyLiteRazorConfig
        {
            DefaultBuildAction = x => x.AddAssemblyReference(Assembly.GetEntryAssembly()),
            CompiledViewProvider = new DefaultCompiledViewProvider(),
            RawViewProvider = new DefaultViewProvider(Path.Combine(Directory.GetCurrentDirectory(), "View"))
        };
        public static void RegisterNancyLiteRazor(this IServiceCollection services, NancyLiteRazorConfig config = null)            
        {            
            if(config == null)
            {
                services.AddSingleton(defaultConfig);
            }
            else
            {
                services.AddSingleton(config);
            }
            services.AddSingleton<RazorEnginePlus>();
        }

        public static void RegisterNancyLiteRazor(this IServiceCollection services, Action<RazorEngineCompilationOptionsBuilder> buildAction,  IViewProvider viewProvider,  ICompiledViewProvider compiledViewProvider)
        {
            var config = new NancyLiteRazorConfig
            {
                DefaultBuildAction = buildAction,
                CompiledViewProvider = compiledViewProvider,
                RawViewProvider = viewProvider
            };
            services.AddSingleton(config);
            services.AddSingleton<RazorEnginePlus>();
        }
    }
}
