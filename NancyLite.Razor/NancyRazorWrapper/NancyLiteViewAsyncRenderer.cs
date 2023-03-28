using System.Threading.Tasks;

namespace NancyLite.Razor
{
    /// <summary>
    /// 异步渲染指定的View
    /// </summary>
    public class NancyLiteViewAsyncRenderer
    {
        private readonly RazorEnginePlus _razor;

        public NancyLiteViewAsyncRenderer(RazorEnginePlus razor)
        {
            _razor = razor;
        }

        public Task<HtmlResponse> this[string viewName, dynamic model = null, dynamic viewBag = null] => _razor.RenderAsync(viewName, model, viewBag ?? new RazorExpandoObject());
    }
}
