namespace NancyLite.Razor
{
    public static class ViewBagCombine
    {
        public static dynamic Combine(dynamic src, dynamic appendEx)
        {
            if (appendEx == null || appendEx.Count <= 0) return src;
            //if (instance.ViewBag == null) instance.ViewBag = new ExpandoObject();
            //var tempDict = (IDictionary<string, object>)src;

            foreach (var key in appendEx.Keys)
            {
                if (!src.ContainsKey(key)) src.Add(key, appendEx[key]);
            }
            return src;
        }
    }
}
