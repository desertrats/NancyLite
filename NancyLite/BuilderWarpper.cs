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
    public static class BuilderWarpper
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
                    foreach (var route in module.Routes)
                    {
                        var conventionBuilder = builder.MapMethods
                        (
                            route.Key.path, 
                            new[] { route.Key.method },
                            async context =>
                            {
                                var sw = new System.Diagnostics.Stopwatch();
                                sw.Start();
                                config.Before?.Invoke(context);                                
                                if (config != null)
                                {                                   
                                    context.Response.OnStarting
                                    (
                                        x =>
                                        {
                                            config.After?.Invoke(context);
                                            return Task.CompletedTask;
                                        }, null
                                    );
                                }
                                
                                await route.Value.Invoke(context);
                                sw.Stop();
                                Console.WriteLine($" {context.Request.Path.Value} takes {sw.ElapsedMilliseconds} ms");
                            }
                        );
                        builders.Add(conventionBuilder);
                    }
                }
            }

            return new CompositeConventionBuilder(builders);
        }


        private static IEnumerable<Type> GetModules(IReadOnlyCollection<Assembly> assemblies)
        {
            IEnumerable<Type> modules;

                modules = assemblies.SelectMany(x => x.GetTypes()
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
