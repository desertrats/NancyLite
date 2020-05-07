namespace NancyLite.Razor
{
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
        private dynamic _viewBag;
        public virtual string Partial(string viewName, dynamic model = null)
        {
            if (_compiler == null)
            {
                return null;
            }
            return _compiler.RenderRawSub(viewName, ref _viewBag, model);
        }

        public virtual string Raw(string viewName)
        {
            return viewName;
        }
    }
}
