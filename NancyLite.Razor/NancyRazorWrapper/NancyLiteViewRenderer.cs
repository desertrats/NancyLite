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

        public HtmlReponse this[string viewName]
        {
            get => _razor.Render(viewName, _module.ViewBag.Duplicate(), _module.ViewBag.Duplicate());
        }


        public HtmlReponse this[string viewName, dynamic model]
        {
            get => _razor.Render(viewName, model, _module.ViewBag.Duplicate());
        }
    }
}
