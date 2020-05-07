using Microsoft.AspNetCore.Http;
using System;
using System.Text;
using System.Threading.Tasks;

namespace NancyLite
{
    public class RedirectResponse : NancyLiteResponse
    {
        private readonly int _statusCode;
        private readonly bool _forwardQuery;
        public string RedirectTo { get; private set; }
        public RedirectResponse(string redirectTo, bool permanent = true, bool forwardQuery = false)
        {
            _statusCode = permanent ? 301 : 302;
            RedirectTo = redirectTo;
            _forwardQuery = forwardQuery;
        }
        public RedirectResponse(string redirectTo, int code, bool forwardQuery = false)
        {
            _statusCode = code;
            RedirectTo = redirectTo;
            _forwardQuery = forwardQuery;
        }
        public override Func<HttpContext, Task> BuildDelegate()
        {
            return context =>
            {
                if (_forwardQuery)
                {
                    var mRedirectTo = new StringBuilder(RedirectTo);
                    var dict = context.GetAllQuery();
                    var first = true;
                    foreach (var (key, value) in dict)
                    {
                        if (first)
                        {
                            mRedirectTo.Append($"?{key}={value}");
                            first = false;
                        }
                        else
                        {
                            mRedirectTo.Append($"&{key}={value}");
                        }
                        RedirectTo = mRedirectTo.ToString();
                    }
                }
                context.Response.Redirect(RedirectTo);
                context.Response.StatusCode = _statusCode;
                return Task.CompletedTask;
            };
        }
    }
}
