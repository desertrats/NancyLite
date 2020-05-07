using RazorEngineCore;
using System.Collections.Concurrent;

namespace NancyLite.Razor
{
    public class DefaultCompiledViewProvider : ICompiledViewProvider
    {
        private readonly ConcurrentDictionary<string, RazorEngineCompiledTemplate> caches = new ConcurrentDictionary<string, RazorEngineCompiledTemplate>();

        public RazorEngineCompiledTemplate Get(string name)
        {
            return caches.TryGetValue(name, out var content) ? content : null;
        }

        public bool HasCompiledView(string name)
        {
            return caches.ContainsKey(name);
        }

        public bool SetView(string name, RazorEngineCompiledTemplate template)
        {
            return caches.AddOrUpdate(name, template, (key, oldTemplate) => template) == template;
        }
    }
}
