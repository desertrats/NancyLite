using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace NancyLite
{
    public class HtmlReponse : NancyLiteResponse
    {
        private const string contentType = "text/html;charset=utf-8";
        private string _content;
        public int StatusCode { get; set; }
        public HtmlReponse(string content, int code = 200)
        {
            _content = content;
            StatusCode = code;
        }


        public static implicit operator HtmlReponse(string content)
        {
            return new HtmlReponse(content);
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
