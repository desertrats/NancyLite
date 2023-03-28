namespace NancyLite.Razor.Test
{
    /// <summary>
    /// 用于快速测试的实体
    /// </summary>
    public class QuickTestModel
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public QuickTestModel2 ExtraInfo { get; set; }
    }

    public class QuickTestModel2
    {
        public string Company { get; set; }
    }
}
