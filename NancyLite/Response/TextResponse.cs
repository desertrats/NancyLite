using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace NancyLite
{
    public class TextResponse : NancyLiteResponse
    {
        private const string contentType = "text/plain;charset=utf-8";
        private string _content;
        public int StatusCode { get; set; }

        public TextResponse(string content, int code = 200)
        {
            StatusCode = code;
            _content = content;
        }

        public static explicit operator TextResponse(string content)
        {
            return new TextResponse(content);
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
