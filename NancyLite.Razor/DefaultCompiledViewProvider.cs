using RazorEngineCore;
using System.Collections.Concurrent;

namespace NancyLite.Razor
{
    public class DefaultCompiledViewProvider : ICompiledViewProvider
    {
        private readonly ConcurrentDictionary<string, RazorEngineCompiledTemplate> caches = new ConcurrentDictionary<string, RazorEngineCompiledTemplate>();

        public RazorEngineCompiledTemplate Get(string name)
        {
            if (caches.TryGetValue(name, out var content))
            {
                return content;
            }
            return null;
        }

        public bool HasCompiledView(string name)
        {
            return caches.ContainsKey(name);
        }

        public bool SetView(string name, RazorEngineCompiledTemplate template)
        {
            caches.AddOrUpdate(name, template, (name, odlTemplate) => template);
            return true;
        }
    }
}
