namespace NancyLite.Razor
{
    /// <summary>
    /// 针对@Html方法支持的扩展
    /// </summary>
    public class HtmlExtensionBase
    {
        public HtmlExtensionBase(dynamic viewBag)
        {
            _viewBag = viewBag;
        }
        public void Initialize(RazorEnginePlus compiler)
        {
            _compiler = compiler;
        }

        private RazorEnginePlus _compiler;
        private readonly dynamic _viewBag;

        /// <summary>
        /// 对@Html.Partial的支持
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual string Partial(string viewName, dynamic model = null)
        {
            if (_compiler == null)
            {
                return null;
            }
            return _compiler.RenderRaw(viewName, model, _viewBag, true);
        }

        /// <summary>
        /// 对@Html.Raw的支持
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public virtual string Raw(string viewName)
        {
            return viewName;
        }
    }
}
