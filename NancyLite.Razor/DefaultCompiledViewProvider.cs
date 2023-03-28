using RazorEngineCore;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace NancyLite.Razor
{
    /// <summary>
    /// 默认的编译模板提供程序
    /// <para>使用ConcurrentDictionary进行编译模板内容的缓存</para>
    /// </summary>
    public class DefaultCompiledViewProvider : ICompiledViewProvider
    {
        private readonly ConcurrentDictionary<string, IRazorEngineCompiledTemplate> caches = new ConcurrentDictionary<string, IRazorEngineCompiledTemplate>();

        public bool HasCompiledView(string name) => caches.ContainsKey(name);

        public Task<bool> HasCompiledViewAsync(string name) => Task.FromResult(caches.ContainsKey(name));

        public IRazorEngineCompiledTemplate Get(string name)
        {
            return caches.TryGetValue(name, out var content) ? content : null;
        }

        public Task<IRazorEngineCompiledTemplate> GetAsync(string name)
        {
            return Task.FromResult(Get(name));
        }

        public bool SetView(string name, IRazorEngineCompiledTemplate template)
        {
            return caches.AddOrUpdate(name, template, (key, oldTemplate) => template) == template;
        }

        public Task<bool> SetViewAsync(string name, IRazorEngineCompiledTemplate template)
        {
            return Task.FromResult(SetView(name, template));
        }
    }
}
