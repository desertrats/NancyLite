namespace RazorEngineCore
{
    /// <summary>
    /// 基础模板  但是是类型安全的
    /// <para>CustomEdit:后台关于指定类型的方法被砍掉了，此类只用于前端view进行提示所用</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RazorEngineTemplateBase<T> : RazorEngineTemplateBase
    {
        public new T Model { get; set; }
    }
}
