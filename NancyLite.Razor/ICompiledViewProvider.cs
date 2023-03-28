using RazorEngineCore;
using System.Threading.Tasks;

namespace NancyLite.Razor
{
    /// <summary>
    /// 编译后模板提供程序接口约束
    /// </summary>
    public interface ICompiledViewProvider
    {
        /// <summary>
        /// 是否具有编译后的模板
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        bool HasCompiledView(string relativePath);
        /// <summary>
        /// 是否具有编译后的模板 异步版本
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        Task<bool> HasCompiledViewAsync(string relativePath);
        /// <summary>
        /// 获取编译后的模板内容
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        IRazorEngineCompiledTemplate Get(string relativePath);
        /// <summary>
        /// 获取编译后的模板内容 异步版本
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        Task<IRazorEngineCompiledTemplate> GetAsync(string relativePath);
        /// <summary>
        /// 保存编译后的模板内容
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="template"></param>
        /// <returns>是否保存成功</returns>
        bool SetView(string relativePath, IRazorEngineCompiledTemplate template);

        /// <summary>
        /// 保存编译后的模板内容 异步
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="template"></param>
        /// <returns>是否保存成功</returns>
        Task<bool> SetViewAsync(string relativePath, IRazorEngineCompiledTemplate template);
    }
}
