namespace NancyLite.Razor
{
    public class NancyLiteViewRenderer
    {
        private readonly RazorEnginePlus _razor;
        private readonly NancyLiteRazorModule _module;

        public NancyLiteViewRenderer(NancyLiteRazorModule module, RazorEnginePlus razor)
        {
            _razor = razor;
            _module = module;
        }

        public HtmlResponse this[string viewName] => _razor.Render(viewName, _module.ViewBag.Duplicate(), _module.ViewBag.Duplicate());


        public HtmlResponse this[string viewName, dynamic model] => _razor.Render(viewName, model, _module.ViewBag.Duplicate());
    }
}
