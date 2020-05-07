using RazorEngineCore;
using System;


namespace NancyLite.Razor
{
    public class NancyLiteRazorConfig
    {
        public Action<RazorEngineCompilationOptionsBuilder> DefaultBuildAction { get; set; }

        public ICompiledViewProvider CompiledViewProvider { get; set; }

        public IViewProvider RawViewProvider { get; set; }
    }
}
