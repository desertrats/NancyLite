﻿using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace NancyLite
{
    public abstract class NancyLiteResponse
    {

        public abstract Func<HttpContext, Task> BuildDelegate();

        public static implicit operator NancyLiteResponse(HttpStatusCode statusCode)
        {
            return new StatusCodeResponse((int)statusCode);
        }

        public static implicit operator NancyLiteResponse(string content)
        {
            return new HtmlResponse(content);
        }

        public static implicit operator NancyLiteResponse(Stream content)
        {
            return new StreamResponse(content, "application/octet-stream");
        }

        public static NancyLiteResponse AsRedirect(string location, bool permanent = true, bool forwardQuery = false)
        {
            return new RedirectResponse(location, permanent, forwardQuery);
        }
        public static NancyLiteResponse AsRedirect(string location, int code, bool forwardQuery = false)
        {
            return new RedirectResponse(location, code, forwardQuery);
        }

        public static NancyLiteResponse AsJson(object jsObj, int code = 200)
        {
            return new JsonResponse(jsObj, code);
        }


        public static NancyLiteResponse AsText(string msg, int code = 200)
        {
            return new TextResponse(msg, code);
        }

        public static NancyLiteResponse AsJson(object jsObj, HttpStatusCode code)
        {
            return new JsonResponse(jsObj, code);
        }


        public static NancyLiteResponse AsText(string msg, HttpStatusCode code)
        {
            return new TextResponse(msg, code);
        }
    }
}
