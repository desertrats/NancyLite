using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace NancyLite.Razor.Test
{
    [TestClass]
    public class QuickTest : RazorEnginePlusModel
    {
        /// <summary>
        /// 简单的view视图试渲染
        /// </summary>
        [TestMethod]
        public void SimpleQuickTest()
        {
            dynamic viewBag = new RazorExpandoObject();
            viewBag.lalalal = "zh_cn";

            string result = razorEnginePlus.RenderRaw("SimpleQuickTest", new { Name = "pick" }, viewBag);
            Console.WriteLine(result);
            Assert.IsNotNull(result);
            Assert.IsTrue(result == @"<div>Hello  world! pick 589666</div>");
        }
        /// <summary>
        /// 简单的view视图试渲染 异步方式
        /// </summary>
        [TestMethod]
        public async Task SimpleQuickTestAsync()
        {
            dynamic viewBag = new RazorExpandoObject();
            viewBag.lalalal = "zh_cn";

            string result = await razorEnginePlus.RenderRawAsync("SimpleQuickTest", new { Name = "pick" }, viewBag);
            Console.WriteLine(result);
            Assert.IsNotNull(result);
            Assert.IsTrue(result == @"<div>Hello  world! pick 589666</div>");
        }


        /// <summary>
        /// 简单的view视图试渲染 类型安全
        /// <para>但是只有view上标注了类型</para>
        /// </summary>
        [TestMethod]
        public async Task SimpleQuickTestT()
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            string result = await razorEnginePlus.RenderRawAsync("SimpleQuickTestTOnlyView", new { Name = "pick", Age = 12, text = "566" });
            sw.Stop();
            Console.WriteLine("渲染耗时:" + sw.ElapsedMilliseconds);

            Console.WriteLine(result);
            Assert.IsNotNull(result);
            Assert.IsTrue(result == $"<div>Hello  world! pick 12</div>");
        }

    }
}
