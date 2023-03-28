using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace NancyLite.Razor
{
    public class DefaultViewProvider : IViewProvider
    {
        private readonly string _rootPath;
        public DefaultViewProvider(string root)
        {
            _rootPath = root;
        }
        private readonly ConcurrentDictionary<string, string> caches = new ConcurrentDictionary<string, string>();

        public bool HasView(string relativePath)
        {
            if (caches.ContainsKey(relativePath)) return true;
            var path = GetFileFullPath(relativePath);
            return File.Exists(path);
        }

        public Task<bool> HasViewAsync(string relativePath)
        {
            return Task.FromResult(HasView(relativePath));
        }

        public string GetContent(string relativePath)
        {
            if (caches.TryGetValue(relativePath, out var cacheContent))
            {
                return cacheContent;
            }
            var path = GetFileFullPath(relativePath);

            if (!File.Exists(path)) return null;

            var content = File.ReadAllText(path);
            return caches.AddOrUpdate(relativePath, content, (key, oldContent) => content);
        }

        public async Task<string> GetContentAsync(string relativePath)
        {
            if (caches.TryGetValue(relativePath, out var cacheContent))
            {
                return cacheContent;
            }
            var path = GetFileFullPath(relativePath);

            if (!File.Exists(path)) return null;

            var content = await File.ReadAllTextAsync(path);
            return caches.AddOrUpdate(relativePath, content, (key, oldContent) => content);
        }

        /// <summary>
        /// 获取文件对应的路径
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        private string GetFileFullPath(string relativePath) => Path.Combine(_rootPath, relativePath + ".cshtml");

    }
}
