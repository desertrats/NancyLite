using NancyLite.Razor;
using System.Text;
using System.Threading.Tasks;


namespace RazorEngineCore
{
    /// <summary>
    /// 基础模板声明
    /// </summary>
    public abstract class RazorEngineTemplateBase : IRazorEngineTemplate
    {
        /**
         * CustomEdit-Begin:添加对ViewBag,Layout,以及Html方法的支持
         */
        protected RazorEngineTemplateBase()
        {
            Html = new HtmlExtensionBase(ViewBag);
        }
        /// <summary>
        /// add ViewBag support
        /// </summary>
        public dynamic ViewBag { get; set; } = new RazorExpandoObject();
        /// <summary>
        /// add Html.Partial以及Html.Raw支持
        /// </summary>
        public HtmlExtensionBase Html { get; set; }
        /// <summary>
        /// add Layout支持
        /// </summary>
        public string Layout { get; set; }
        /**
         * CustomEdit-End
         */


        private readonly StringBuilder stringBuilder = new StringBuilder();

        private string attributeSuffix;

        public virtual dynamic Model { get; set; }

        #region Write
        public void WriteLiteral(string literal = null)
        {
            //CustomEdit:请不要阻塞异步执行，容易造成死锁以及线程池饥饿
            //WriteLiteralAsync(literal).GetAwaiter().GetResult();
            stringBuilder.Append(literal);
        }

        public virtual Task WriteLiteralAsync(string literal = null)
        {
            stringBuilder.Append(literal);
            return Task.CompletedTask;
        }

        public void Write(object obj = null)
        {
            //CustomEdit:同上不阻塞异步代码
            //WriteAsync(obj).GetAwaiter().GetResult();
            stringBuilder.Append(obj);
        }

        public virtual Task WriteAsync(object obj = null)
        {
            stringBuilder.Append(obj);
            return Task.CompletedTask;
        }

        public void BeginWriteAttribute(string name, string prefix, int prefixOffset, string suffix, int suffixOffset, int attributeValuesCount)
        {
            //CustomEdit:同上不阻塞异步代码
            //BeginWriteAttributeAsync(name, prefix, prefixOffset, suffix, suffixOffset, attributeValuesCount).GetAwaiter().GetResult();
            attributeSuffix = suffix;
            stringBuilder.Append(prefix);
        }

        public virtual Task BeginWriteAttributeAsync(string name, string prefix, int prefixOffset, string suffix, int suffixOffset, int attributeValuesCount)
        {
            attributeSuffix = suffix;
            stringBuilder.Append(prefix);
            return Task.CompletedTask;
        }

        public void WriteAttributeValue(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral)
        {
            //CustomEdit:同上不阻塞异步代码
            //WriteAttributeValueAsync(prefix, prefixOffset, value, valueOffset, valueLength, isLiteral).GetAwaiter().GetResult();
            stringBuilder.Append(prefix);
            stringBuilder.Append(value);
        }

        public virtual Task WriteAttributeValueAsync(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral)
        {
            stringBuilder.Append(prefix);
            stringBuilder.Append(value);
            return Task.CompletedTask;
        }

        public void EndWriteAttribute()
        {
            //CustomEdit:同上不阻塞异步代码
            //  EndWriteAttributeAsync().GetAwaiter().GetResult();
            stringBuilder.Append(attributeSuffix);
            attributeSuffix = null;
        }
        public virtual Task EndWriteAttributeAsync()
        {
            EndWriteAttribute();
            return Task.CompletedTask;
        }
        #endregion

        public void Execute()
        {
            //此处只有使用这种写法才能正常执行
            ExecuteAsync().GetAwaiter().GetResult();
        }

        public virtual Task ExecuteAsync()
        {
            return Task.CompletedTask;
        }


        public virtual string Result()
        {
            //CustomEdit:同上不阻塞异步代码
            // return ResultAsync().GetAwaiter().GetResult();
            return stringBuilder.ToString();
        }

        public virtual Task<string> ResultAsync()
        {
            return Task.FromResult(stringBuilder.ToString());
        }
    }
}