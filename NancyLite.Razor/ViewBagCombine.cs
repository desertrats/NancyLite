using System.Collections.Generic;

namespace NancyLite.Razor
{
    public static class ViewBagCombine
    {
        public static dynamic Combine(dynamic src, dynamic appendex)
        {
            if (appendex != null && appendex.Count > 0)
            {
                //if (instance.ViewBag == null) instance.ViewBag = new ExpandoObject();
                //var tempDict = (IDictionary<string, object>)src;

                foreach (var key in appendex.Keys)
                {
                    if (!src.ContainsKey(key)) src.Add(key, appendex[key]);
                }
            }
            return src;
        }
    }
}
