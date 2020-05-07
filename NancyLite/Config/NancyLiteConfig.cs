
using Microsoft.AspNetCore.Http;
using System;

namespace NancyLite
{
    public class NancyLiteConfig
    {
        public NancyLiteConfig()
        {
            ScopeAssembly = new ScopeAssembly();
        }
        public ScopeAssembly ScopeAssembly { get; set; }

        public Action<HttpContext> Before { get; set; }
        public Action<HttpContext> After { get; set; }
    }
}
