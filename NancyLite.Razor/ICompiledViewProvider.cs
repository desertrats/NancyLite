using RazorEngineCore;

namespace NancyLite.Razor
{
    public interface ICompiledViewProvider
    {
        public bool HasCompiledView(string relativePath);
        public RazorEngineCompiledTemplate Get(string relativePath);
        public bool SetView(string relativePath, RazorEngineCompiledTemplate template);
    }
}
