using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NancyLite.Razor.Test
{
    /// <summary>
    /// 分部视图相关的测试
    /// <para>主要测试Html.Partial是否可以使用异步以及其他方法  因为分部视图渲染已经在<see cref="NestViewTest"/>中测试了</para>
    /// </summary>
    [TestClass]
    public class PartialViewTest : RazorEnginePlusModel
    {
        /// <summary>
        /// 测试异步加载分部视图是否正常
        /// 经测试不支持@Html.PartialAsync方法
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        [Obsolete("不支持Html.PartialAsync")]
        public async Task TestAsyncPartial()
        {
            dynamic viewBag = new RazorExpandoObject();
            viewBag.Test = "我是分部视图测试";
            QuickTestModel model = null;
            string result = await razorEnginePlus.RenderRawAsync("PartialViewTest/TestAsyncPartial", model, viewBag);
            Console.WriteLine(result);
            //经测试不支持@Html.PartialAsync方法
            Assert.IsNull(result);
        }

        /// <summary>
        /// 测试@Html.Raw方法使用
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestPartialRaw()
        {
            dynamic viewBag = new RazorExpandoObject();
            viewBag.rawList = new List<string>()
            {
                "<div>rawDiv</div>",
                "<script scr='https://www.baidu.com'></script>",
            };
            viewBag.jsonRaw = JsonConvert.SerializeObject(new QuickTestModel()
            {
                Name = "pick",
                Age = 5
            });
            string result = await razorEnginePlus.RenderRawAsync("PartialViewTest/TestPartialRaw", null, viewBag);
            Console.WriteLine(result);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Trim() == @"<div>rawDiv</div><script scr='https://www.baidu.com'></script><script>var jsonObj = {""Name"":""pick"",""Age"":5,""ExtraInfo"":null};</script>");
        }
    }
}
