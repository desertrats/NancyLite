using System.Threading.Tasks;

namespace NancyLite.Razor
{
    /// <summary>
    /// 源视图view提供程序接口约束
    /// </summary>
    public interface IViewProvider
    {
        /// <summary>
        /// 是否具有指定的视图
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        bool HasView(string relativePath);
        /// <summary>
        /// 是否具有指定的视图 异步版本
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        Task<bool> HasViewAsync(string relativePath);
        /// <summary>
        /// 获取源视图内容
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        string GetContent(string relativePath);
        /// <summary>
        /// 获取源视图内容 异步版本
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        Task<string> GetContentAsync(string relativePath);
    }
}
