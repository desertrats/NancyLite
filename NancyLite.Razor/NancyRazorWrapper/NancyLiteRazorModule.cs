using System;

namespace NancyLite.Razor
{
    public class NancyLiteRazorModule : NancyLiteModule
    {
        protected readonly NancyLiteViewRenderer View;
        protected readonly NancyLiteViewAsyncRenderer ViewAsync;
        protected readonly RazorEnginePlus Razor;

        public NancyLiteRazorModule(RazorEnginePlus razor, string root = "")
            : base(root)
        {
            Razor = razor;
            View = new NancyLiteViewRenderer(razor);
            ViewAsync = new NancyLiteViewAsyncRenderer(razor);
        }

        /// <summary>
        /// 获取Razor页面所用的ViewBag和Model对象
        /// </summary>
        /// <remarks>一定要返回dynamic的  这样才能在业务代码中实现View.XXX=""的效果</remarks>
        /// <example>
        /// 1.var (viewBag, model) = GetViewBagAndModel();
        /// 2.var (viewBag, _) = GetViewBagAndModel();
        /// </example>
        /// <returns></returns>
        protected Tuple<dynamic, dynamic> GetViewBagAndModel()
        {
            return Tuple.Create<dynamic, dynamic>(new RazorExpandoObject(), new RazorExpandoObject());
        }
    }
}
