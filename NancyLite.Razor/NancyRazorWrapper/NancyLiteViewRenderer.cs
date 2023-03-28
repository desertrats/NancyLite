namespace NancyLite.Razor
{
    public class NancyLiteViewRenderer
    {
        private readonly RazorEnginePlus _razor;

        public NancyLiteViewRenderer(RazorEnginePlus razor)
        {
            _razor = razor;
        }

        public HtmlResponse this[string viewName, dynamic model = null, dynamic viewBag = null] => _razor.Render(viewName, model, viewBag ?? new RazorExpandoObject());
    }
}
