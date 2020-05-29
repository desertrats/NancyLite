
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace NancyLite
{
    public class NancyLiteConfig
    {
        public NancyLiteConfig()
        {
            ScopeAssembly = new ScopeAssembly();
            _allRouter = new Dictionary<string, Dictionary<string, List<string>>>();
        }
        public ScopeAssembly ScopeAssembly { get; set; }

        public Action<HttpContext> Before { get; set; }
        public Action<HttpContext> After { get; set; }
        private readonly Dictionary<string, Dictionary<string, List<string>>> _allRouter;

        public void RegisterRouter(string method, string folder, string path)
        {
            if(!_allRouter.ContainsKey(folder))_allRouter.Add(folder, new Dictionary<string, List<string>>());
            if (!_allRouter[folder].ContainsKey(method)) _allRouter[folder].Add(method, new List<string>());
            if(!_allRouter[folder][method].Contains(path))  _allRouter[folder][method].Add(path);
        }

        public string Report()
        {
            var htmlBuilder = new StringBuilder();
            htmlBuilder.AppendLine("<head><title>Report</title></head>");
            htmlBuilder.AppendLine("<body>");
            htmlBuilder.AppendLine("<h1>Routers:</h1>");
            foreach (var (folder, content) in _allRouter)
            {
                htmlBuilder.AppendLine($"<span>{folder}</span>");
                htmlBuilder.AppendLine("<div>");
                foreach (var (method, pathList) in content)
                {

                    htmlBuilder.AppendLine($"<h2>{method}</h2>");
                    htmlBuilder.AppendLine("<ul>");
                    foreach (var path in pathList)
                    {
                        htmlBuilder.AppendLine($"<li>{path}</li>");
                    }
                    htmlBuilder.AppendLine("</ul>");

                }
                htmlBuilder.AppendLine("</div>");
            }
            htmlBuilder.AppendLine($"<h1>Has Before: {Before != null}</h1>");
            htmlBuilder.AppendLine($"<h1>Has After: {After != null}</h1>");
            htmlBuilder.AppendLine("</body>");
            return htmlBuilder.ToString();
        }
    }
}
