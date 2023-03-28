using RazorEngineCore;
using System;


namespace NancyLite.Razor
{
    /// <summary>
    /// NancyLiteRazor相关的配置
    /// </summary>
    public class NancyLiteRazorConfig
    {
        /// <summary>
        /// 针对编译选项进行设置的委托
        /// </summary>
        public Action<IRazorEngineCompilationOptionsBuilder> DefaultBuildAction { get; set; }
        /// <summary>
        /// 提供编译后内容的程序
        /// </summary>
        public ICompiledViewProvider CompiledViewProvider { get; set; }
        /// <summary>
        /// 提供原始视图内容的程序
        /// </summary>
        public IViewProvider RawViewProvider { get; set; }
    }
}
