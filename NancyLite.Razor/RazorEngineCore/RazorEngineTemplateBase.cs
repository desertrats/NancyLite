using NancyLite.Razor;
using System.Text;
using System.Threading.Tasks;


namespace RazorEngineCore
{
    public abstract class RazorEngineTemplateBase
    {
        public RazorEngineTemplateBase()
        {
            Html = new HtmlExtensionBase(ViewBag);
        }
        //XW add viewbag support
        public dynamic ViewBag = new RazorExpandoObject();

        public HtmlExtensionBase Html { get; set; }

        public string Layout { get; set; }

        private readonly StringBuilder stringBuilder = new StringBuilder();

        private string attributePrefix = null;
        private string attributeSuffix = null;

        public virtual dynamic Model { get; set; }

        public void WriteLiteral(string literal = null)
        {
            stringBuilder.Append(literal);
        }

        public void Write(object obj = null)
        {
            stringBuilder.Append(obj);
        }

        public void BeginWriteAttribute(string name, string prefix, int prefixOffset, string suffix, int suffixOffset, int attributeValuesCount)
        {
            attributeSuffix = suffix;
            stringBuilder.Append(prefix);
            stringBuilder.Append(attributePrefix);
        }

        public void WriteAttributeValue(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral)
        {
            stringBuilder.Append(prefix);
            stringBuilder.Append(value);
        }

        public void EndWriteAttribute()
        {
            stringBuilder.Append(attributeSuffix);
            attributeSuffix = null;
        }

        public virtual Task ExecuteAsync()
        {
            return Task.CompletedTask;
        }

        public string Result()
        {
            return stringBuilder.ToString();
        }
    }
}