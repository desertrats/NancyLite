using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace NancyLite
{
    public class JsonResponse : NancyLiteResponse
    {
        private const string contentType = "application/json;charset=utf-8";

        public string _content { get; set; }
        public int StatusCode { get; set; }

        public JsonResponse(object obj, int code = 200)
        {
            _content = JsonConvert.SerializeObject(obj);
            StatusCode = code;
        }

        public JsonResponse(string content, int code = 200)
        {
            _content = content;
            StatusCode = code;
        }

        public static explicit operator JsonResponse(string content)
        {
            return new JsonResponse(content);
        }

        public override Func<HttpContext, Task> BuildDelegate()
        {
            return async context => 
            {
                context.Response.StatusCode = StatusCode;
                context.Response.ContentType = contentType;
                await context.Response.WriteAsync(_content); 
            };
        }
    }
}
