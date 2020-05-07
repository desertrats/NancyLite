using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace NancyLite
{
    public class JsonResponse : NancyLiteResponse
    {
        private const string ContentType = "application/json;charset=utf-8";

        public string Content { get; set; }
        public int StatusCode { get; set; }

        public JsonResponse(object obj, int code = 200)
        {
            Content = JsonConvert.SerializeObject(obj);
            StatusCode = code;
        }

        public JsonResponse(string content, int code = 200)
        {
            Content = content;
            StatusCode = code;
        }
        public JsonResponse(object obj, HttpStatusCode code)
        {
            Content = JsonConvert.SerializeObject(obj);
            StatusCode = (int)code;
        }

        public JsonResponse(string content, HttpStatusCode code)
        {
            Content = content;
            StatusCode = (int)code;
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
                context.Response.ContentType = ContentType;
                await context.Response.WriteAsync(Content);
            };
        }
    }
}
