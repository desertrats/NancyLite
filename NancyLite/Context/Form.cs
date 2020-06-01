using Microsoft.AspNetCore.Http;
using System;

namespace NancyLite
{
    public static class FormExtension
    {

        public static TValue GetForm<TValue>(this HttpContext context, string queryName, TValue defaultValue = default, Func<string, TValue, TValue> convertor = null)
        {
            if (convertor == null) convertor = StringValueConvertor.ChangeTypeConvertor;
            if (context.HasForm(queryName))
            {
                var strValue = context.Request.Form[queryName].ToString();
                return convertor(strValue, defaultValue);
            }
            return defaultValue;
        }

        public static bool HasForm(this HttpContext context, string queryName)
        {
            return context.Request.HasFormContentType && context.Request.Form.ContainsKey(queryName);
        }
    }
}
