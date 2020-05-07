using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NancyLite
{
    public class StreamResponse : NancyLiteResponse
    {
        private readonly Stream stream;
        public string MimeType { get; set; }
        public int StatusCode { get; set; }
        public StreamResponse(Stream content, string contentType, int code = 200)
        {
            content.Seek(0, SeekOrigin.Begin);
            stream = content;
            MimeType = contentType;
            StatusCode = code;
        }

        public override Func<HttpContext, Task> BuildDelegate()
        {
            return async context =>
            {
                context.Response.StatusCode = StatusCode;
                context.Response.ContentType = MimeType;
                await stream.CopyToAsync(context.Response.Body);
            };
        }
    }
}
