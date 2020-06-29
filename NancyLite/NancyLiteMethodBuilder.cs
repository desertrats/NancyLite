using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NancyLite
{
    public class NancyLiteMethodBuilder
    {
        private readonly NancyLiteModule _module;
        private readonly string _method;
        public NancyLiteMethodBuilder(NancyLiteModule module, string method)
        {
            _module = module;
            _method = method;
        }

        public Func<HttpContext, NancyLiteResponse> this[string path]
        {
            set => _module.Add(_method, path, GetRequestDelegate(value));
        }

        public Func<HttpContext, Task> GetRequestDelegate(Func<HttpContext, NancyLiteResponse> incoming)
        {
            return async context =>
            {
                await incoming(context).BuildDelegate().Invoke(context);
            };
        }

    }
}
