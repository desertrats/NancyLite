using Microsoft.AspNetCore.Http;
using System;
using System.Text;
using System.Threading.Tasks;

namespace NancyLite
{
    public class RedirectResponse : NancyLiteResponse
    {
        private int _statucCode;
        private bool _forwardQuery;
        public string RedirTo { get; private set; }
        public RedirectResponse(string redirTo, bool permanent = true, bool forwardQuery=false)
        {
            _statucCode = permanent ? 301 : 302;
            RedirTo = redirTo;
        }
        public RedirectResponse(string redirTo, int code, bool forwardQuery = false)
        {
            _statucCode = code;
            RedirTo = redirTo;
            _forwardQuery = forwardQuery;
        }
        public override Func<HttpContext, Task> BuildDelegate()
        {
            return context =>
            {
                if (_forwardQuery)
                {
                    var mRedirTo = new StringBuilder(RedirTo);
                    var dict = context.GetAllQuery();
                    var first = true;
                    foreach(var kvp in dict)
                    {
                        if (first)
                        {
                            mRedirTo.Append($"?{kvp.Key}={kvp.Value}");
                            first = false;
                        }
                        else
                        {
                            mRedirTo.Append($"&{kvp.Key}={kvp.Value}");
                        }
                        RedirTo = mRedirTo.ToString();
                    }
                }
                context.Response.Redirect(RedirTo);
                context.Response.StatusCode = _statucCode;
                return Task.CompletedTask;
            };
        }
    }
}
