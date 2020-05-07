using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NancyLite
{
    public class NancyLiteModule
    {
        internal readonly Dictionary<(string method, string path), Func<HttpContext, Task>> Routes;
        protected NancyLiteModule()
            : this(string.Empty)
        {

        }
        protected NancyLiteModule(string path)
        {
            basePath = path;
            Routes = new Dictionary<(string method, string path), Func<HttpContext, Task>>();
            Get = new NancyLiteMethodBuilder(this, HttpMethods.Get);
            Post = new NancyLiteMethodBuilder(this, HttpMethods.Post);
            Delete = new NancyLiteMethodBuilder(this, HttpMethods.Delete);
            Put = new NancyLiteMethodBuilder(this, HttpMethods.Put);
            Head = new NancyLiteMethodBuilder(this, HttpMethods.Head);
        }
        private readonly string basePath;

        public readonly NancyLiteMethodBuilder Get;
        public readonly NancyLiteMethodBuilder Post;
        public readonly NancyLiteMethodBuilder Delete;
        public readonly NancyLiteMethodBuilder Put;
        public readonly NancyLiteMethodBuilder Head;

        internal bool Add(string method, string path, Func<HttpContext, Task> requestDelegate)
        {
            var fullPath = basePath + path;
            if (Routes.ContainsKey((method, fullPath))) return false;
            Routes.Add((method, fullPath), requestDelegate);
            return true;
        }
    }
}
