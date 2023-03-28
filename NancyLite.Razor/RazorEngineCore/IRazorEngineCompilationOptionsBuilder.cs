using Microsoft.CodeAnalysis;
using System;
using System.Reflection;

namespace RazorEngineCore
{
    /// <summary>
    /// <see cref="RazorEngineCompilationOptions"/>所对应的Builder接口约束
    /// </summary>
    public interface IRazorEngineCompilationOptionsBuilder
    {
        RazorEngineCompilationOptions Options { get; set; }
        /// <summary>
        /// 根据程序集名称添加程序集
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        void AddAssemblyReferenceByName(string assemblyName);
        /// <summary>
        /// 添加程序集引用
        /// </summary>
        /// <param name="assembly">对应的程序集</param>
        void AddAssemblyReference(Assembly assembly);
        /// <summary>
        /// 根据type添加程序集
        /// </summary>
        /// <param name="type"></param>
        void AddAssemblyReference(Type type);
        /// <summary>
        /// 添加<see cref="MetadataReference"/>引用
        /// </summary>
        /// <param name="reference"></param>
        void AddMetadataReference(MetadataReference reference);
        /// <summary>
        /// 添加命名空间引用
        /// </summary>
        /// <param name="namespaceName">命名空间名称</param>
        void AddUsing(string namespaceName);
        /// <summary>
        /// 设置对应的继承类
        /// </summary>
        /// <param name="type"></param>
        void Inherits(Type type);
    }
}
