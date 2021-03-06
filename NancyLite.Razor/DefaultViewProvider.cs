﻿using System.Collections.Concurrent;
using System.IO;

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
        public string GetContent(string relativePath)
        {
            if (caches.TryGetValue(relativePath, out var cacheContent))
            {
                return cacheContent;
            }
            var path = Path.Combine(_rootPath, relativePath + ".cshtml");

            if (!File.Exists(path)) return null;

            var content = File.ReadAllText(path);
            return caches.AddOrUpdate(relativePath, content, (key, oldContent) => content);
        }

        public bool HasView(string relativePath)
        {
            if (caches.ContainsKey(relativePath)) return true;
            var path = Path.Combine(_rootPath, relativePath + ".cshtml");
            return File.Exists(path);
        }

    }
}
