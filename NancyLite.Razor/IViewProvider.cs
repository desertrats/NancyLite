namespace NancyLite.Razor
{
    public interface IViewProvider
    {
        public bool HasView(string relativePath);
        public string GetContent(string relativePath);
    }
}
