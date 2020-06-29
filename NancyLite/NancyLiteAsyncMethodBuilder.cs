using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace NancyLite
{
    public class NancyLiteAsyncMethodBuilder
    {
        private readonly NancyLiteModule _module;
        private readonly string _method;
        public NancyLiteAsyncMethodBuilder(NancyLiteModule module, string method)
        {
            _module = module;
            _method = method;
        }

        public Func<HttpContext, Task<NancyLiteResponse>> this[string path]
        {
            set => _module.Add(_method, path, GetRequestAsyncDelegate(value));
        }

        public Func<HttpContext, Task> GetRequestAsyncDelegate(Func<HttpContext, Task<NancyLiteResponse>> incoming)
        {
            return async context =>
            {
                var rawResponse = await incoming(context);
                await rawResponse.BuildDelegate().Invoke(context);
            };
        }

    }
}
