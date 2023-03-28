using NancyLite.Razor;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    /// <summary>
    /// Razor引擎基础模板接口
    /// </summary>
    public interface IRazorEngineTemplate
    {
        /// <summary>
        /// 模板对应的Model对象
        /// </summary>
        dynamic Model { get; set; }

        /**
         * CustomEdit-Begin:添加对Layout,以及Html方法的支持
         */
        /// <summary>
        ///  add Html.Partial以及Html.Raw支持
        /// </summary>
        HtmlExtensionBase Html { get; set; }
        /// <summary>
        /// add Layout支持
        /// </summary>
        string Layout { get; set; }
        /// <summary>
        /// add ViewBag支持
        /// </summary>
        dynamic ViewBag { get; set; }
        /**
         * CustomEdit-End
         */

        void WriteLiteral(string literal = null);

        Task WriteLiteralAsync(string literal = null);

        void Write(object obj = null);

        Task WriteAsync(object obj = null);

        void BeginWriteAttribute(string name, string prefix, int prefixOffset, string suffix, int suffixOffset, int attributeValuesCount);

        Task BeginWriteAttributeAsync(string name, string prefix, int prefixOffset, string suffix, int suffixOffset, int attributeValuesCount);

        void WriteAttributeValue(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral);

        Task WriteAttributeValueAsync(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral);

        void EndWriteAttribute();

        Task EndWriteAttributeAsync();

        void Execute();

        Task ExecuteAsync();

        string Result();

        Task<string> ResultAsync();
    }
}
