using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NancyLite
{
    public static class BuilderWrapper
    {
        public static void RegisterNancyLite(this IServiceCollection services)
        {
            RegisterNancyLite(services, new NancyLiteConfig());
        }
        public static void RegisterNancyLite(this IServiceCollection services, NancyLiteConfig config)
        {
            var assemblies = config.ScopeAssembly.GetAssemblies();
            var modules = GetModules(assemblies);
            services.AddSingleton(config);
            foreach (var module in modules)
            {
                services.AddScoped(module);
                services.AddScoped(typeof(NancyLiteModule), module);
            }
        }
        public static IEndpointConventionBuilder UseNancyLite(this IEndpointRouteBuilder builder)
        {
            var builders = new List<IEndpointConventionBuilder>();

            using (var scope = builder.ServiceProvider.CreateScope())
            {
                var config = scope.ServiceProvider.GetService<NancyLiteConfig>();
                foreach (var module in scope.ServiceProvider.GetServices<NancyLiteModule>())
                {
                    foreach (var ((method, path), value) in module.Routes)
                    {
                        config.RegisterRouter(method, module.BasePath, path);
                        var conventionBuilder = builder.MapMethods
                        (
                            path,
                            new[] { method },
                            async context =>
                            {
#if DEBUG
                                var sw = new System.Diagnostics.Stopwatch();
                                sw.Start();
#endif
                                config.Before?.Invoke(context);
                                context.Response.OnStarting
                                (
                                    x =>
                                    {
                                        config.After?.Invoke(context);
                                        return Task.CompletedTask;
                                    }, null
                                );

                                await value.Invoke(context);
#if DEBUG
                                sw.Stop();
                                Console.WriteLine($" {context.Request.Path.Value} takes {sw.ElapsedMilliseconds} ms");
#endif
                            }
                        );
                        builders.Add(conventionBuilder);
                    }
                }
            }

            return new CompositeConventionBuilder(builders);
        }


        private static IEnumerable<Type> GetModules(IEnumerable<Assembly> assemblies)
        {
            var modules = assemblies.SelectMany(x => x.GetTypes()
                .Where(t =>
                    !t.IsAbstract &&
                    typeof(NancyLiteModule).IsAssignableFrom(t) &&
                    t != typeof(NancyLiteModule) &&
                    t.IsPublic
                ));

            return modules;
        }

        private class CompositeConventionBuilder : IEndpointConventionBuilder
        {
            private readonly List<IEndpointConventionBuilder> _builders;

            public CompositeConventionBuilder(List<IEndpointConventionBuilder> builders)
            {
                _builders = builders;
            }


            public void Add(Action<EndpointBuilder> convention)
            {
                foreach (var builder in _builders)
                {
                    builder.Add(convention);
                }
            }
        }
    }
}
