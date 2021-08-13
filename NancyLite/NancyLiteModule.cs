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
            BasePath = path;
            Routes = new Dictionary<(string method, string path), Func<HttpContext, Task>>();
            Get = new NancyLiteMethodBuilder(this, HttpMethods.Get);
            Post = new NancyLiteMethodBuilder(this, HttpMethods.Post);
            Delete = new NancyLiteMethodBuilder(this, HttpMethods.Delete);
            Put = new NancyLiteMethodBuilder(this, HttpMethods.Put);
            Head = new NancyLiteMethodBuilder(this, HttpMethods.Head);

            GetAsync = new NancyLiteAsyncMethodBuilder(this, HttpMethods.Get);
            PostAsync = new NancyLiteAsyncMethodBuilder(this, HttpMethods.Post);
            DeleteAsync = new NancyLiteAsyncMethodBuilder(this, HttpMethods.Delete);
            PutAsync = new NancyLiteAsyncMethodBuilder(this, HttpMethods.Put);
            HeadAsync = new NancyLiteAsyncMethodBuilder(this, HttpMethods.Head);
        }

        public string BasePath { get; }

        public readonly NancyLiteMethodBuilder Get;
        public readonly NancyLiteMethodBuilder Post;
        public readonly NancyLiteMethodBuilder Delete;
        public readonly NancyLiteMethodBuilder Put;
        public readonly NancyLiteMethodBuilder Head;

        
        public readonly NancyLiteAsyncMethodBuilder GetAsync;
        public readonly NancyLiteAsyncMethodBuilder PostAsync;
        public readonly NancyLiteAsyncMethodBuilder DeleteAsync;
        public readonly NancyLiteAsyncMethodBuilder PutAsync;
        public readonly NancyLiteAsyncMethodBuilder HeadAsync;

        protected internal bool Add(string method, string path, Func<HttpContext, Task> requestDelegate)
        {
            var fullPath = BasePath + path;
            if (Routes.ContainsKey((method, fullPath))) return false;
            Routes.Add((method, fullPath), requestDelegate);
            return true;
        }
    }
}
