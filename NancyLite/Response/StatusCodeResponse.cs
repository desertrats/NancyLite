using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace NancyLite
{
    public class StatusCodeResponse : NancyLiteResponse
    {
        private readonly int _statusCode;
        public string Content { get; set; }
        public string MimeType { get; set; }
        public StatusCodeResponse(int code)
        {
            _statusCode = code;
        }

        public override Func<HttpContext, Task> BuildDelegate()
        {
            return async context => 
            {
                context.Response.StatusCode = _statusCode;
                if(Content != null)
                {
                    await context.Response.WriteAsync(Content);
                }
                if(MimeType != null)
                {
                    context.Response.ContentType = MimeType;
                }
            };
        }

        public static implicit operator StatusCodeResponse(int statusCode)
        {
            return new StatusCodeResponse(statusCode);
        }

        public static implicit operator StatusCodeResponse(HttpStatusCode statusCode)
        {
            return new StatusCodeResponse((int)statusCode);
        }
    }
}
