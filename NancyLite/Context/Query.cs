using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace NancyLite
{
    public static class QueryExtension
    {
        public static TValue GetQuery<TValue>(this HttpContext context, string queryName, TValue defaultValue = default, Func<string, TValue, TValue> convertor = null)
        {
            if (convertor == null) convertor = StringValueConvertor.ChangeTypeConvertor;
            if (context.Request.Query.ContainsKey(queryName))
            {
                var strValue = context.Request.Query[queryName].ToString();
                return convertor(strValue, defaultValue);
            }
            return defaultValue;
        }

        public static bool HasQuery(this HttpContext context, string queryName)
        {
            return context.Request.Query.ContainsKey(queryName);
        }


        public static Dictionary<string, string> GetAllQuery(this HttpContext context)
        {
            var result = new Dictionary<string, string>();
            foreach (var (key, value) in context.Request.Query)
            {
                if (!result.ContainsKey(key)) result.Add(key, value.ToString());
            }
            return result;
        }
    }
}
