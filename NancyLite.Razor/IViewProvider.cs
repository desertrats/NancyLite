namespace NancyLite.Razor
{
    public interface IViewProvider
    {
        bool HasView(string relativePath);
        string GetContent(string relativePath);
    }
}
