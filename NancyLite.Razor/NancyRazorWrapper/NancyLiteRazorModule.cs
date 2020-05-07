namespace NancyLite.Razor
{
    public class NancyLiteRazorModule : NancyLiteModule
    {
        public dynamic Model { get; }
        public dynamic ViewBag { get; }
        protected readonly NancyLiteViewRenderer View;
        protected readonly RazorEnginePlus Razor;

        public NancyLiteRazorModule(RazorEnginePlus razor, string root = "")
            : base(root)
        {
            Razor = razor;
            View = new NancyLiteViewRenderer(this, razor);
            ViewBag = new RazorExpandoObject();
            Model = new RazorExpandoObject();
        }
    }
}
