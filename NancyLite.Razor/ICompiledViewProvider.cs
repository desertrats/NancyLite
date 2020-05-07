using RazorEngineCore;

namespace NancyLite.Razor
{
    public interface ICompiledViewProvider
    {
        bool HasCompiledView(string relativePath);
        RazorEngineCompiledTemplate Get(string relativePath); 
        bool SetView(string relativePath, RazorEngineCompiledTemplate template);
    }
}
