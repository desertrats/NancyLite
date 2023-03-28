using NancyLite.Razor;
using System.IO;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    /// <summary>
    /// 编译后的模板内容
    /// </summary>
    public interface IRazorEngineCompiledTemplate
    {
        void SaveToStream(Stream stream);
        Task SaveToStreamAsync(Stream stream);
        void SaveToFile(string fileName);
        Task SaveToFileAsync(string fileName);
        /// <summary>
        /// 运行编译后的模板
        /// <para>CustomEdit:添加对ViewBag,Layout,Html的支持</para>
        /// </summary>
        /// <param name="razor"></param>
        /// <param name="model"></param>
        /// <param name="viewBag"></param>
        /// <param name="isRunSub">是否是运行子视图</param>
        /// <returns></returns>
        string Run(RazorEnginePlus razor, object model = null, dynamic viewBag = null, bool isRunSub = false);
        /// <summary>
        /// 运行编译后的模板 异步版本
        /// <para>CustomEdit:添加对ViewBag,Layout,Html的支持</para>
        /// </summary>
        /// <param name="razor"></param>
        /// <param name="model"></param>
        /// <param name="viewBag"></param>
        /// <param name="isRunSub">是否是运行子视图</param>
        /// <returns></returns>
        Task<string> RunAsync(RazorEnginePlus razor, object model = null, dynamic viewBag = null, bool isRunSub = false);
    }
}
