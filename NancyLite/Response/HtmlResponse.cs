using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace NancyLite
{
    public class HtmlResponse : NancyLiteResponse
    {
        private const string ContentType = "text/html;charset=utf-8";
        private readonly string _content;
        public int StatusCode { get; set; }
        public HtmlResponse(string content, int code = 200)
        {
            _content = content;
            StatusCode = code;
        }


        public static implicit operator HtmlResponse(string content)
        {
            return new HtmlResponse(content);
        }

        public override Func<HttpContext, Task> BuildDelegate()
        {
            return async context =>
            {
                context.Response.StatusCode = StatusCode; 
                context.Response.ContentType = ContentType;
                await context.Response.WriteAsync(_content); 
            };
        }

    }
}
