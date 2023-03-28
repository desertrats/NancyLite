using System;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    /// <summary>
    /// Razor编译引擎接口
    /// </summary>
    public interface IRazorEngine
    {
        /// <summary>
        /// 编译指定的内容
        /// </summary>
        /// <param name="content">要编译的内容</param>
        /// <param name="builderAction"></param>
        /// <returns></returns>
        IRazorEngineCompiledTemplate Compile(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null);
        /// <summary>
        /// 编译指定的内容 异步版本
        /// <para>如果可能的话，尽可能使用同步版本，因为内部仅仅是套在了Task.FromResult中使其变成了异步</para>
        /// </summary>
        /// <param name="content"></param>
        /// <param name="builderAction"></param>
        /// <returns></returns>
        Task<IRazorEngineCompiledTemplate> CompileAsync(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null);
    }
}
