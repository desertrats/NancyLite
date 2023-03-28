using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace NancyLite.Razor.Test
{
    [TestClass]
    public class NestViewTest : RazorEnginePlusModel
    {
        /// <summary>
        /// 带Layout的视图渲染
        /// </summary>
        [TestMethod]
        public void WithLayout()
        {
            dynamic viewBag = new RazorExpandoObject();
            string result = razorEnginePlus.RenderRaw("NestViewTest/WithLayout", new { Name = "pick" }, viewBag);
            Console.WriteLine(result);
            Assert.IsNotNull(result);
            Assert.IsTrue(result == "<div>pick 欢迎登录 电子邮件地址</div>");
        }

        /// <summary>
        /// 带Layout的视图渲染  但是异步版本
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task WithLayoutAsync()
        {
            dynamic viewBag = new RazorExpandoObject();
            string result = await razorEnginePlus.RenderRawAsync("NestViewTest/WithLayout", new { Name = "pick" }, viewBag);
            Console.WriteLine(result);
            Assert.IsNotNull(result);
            Assert.IsTrue(result == "<div>pick 欢迎登录 电子邮件地址</div>");
        }

        /// <summary>
        /// 带Layout,并且Layout中含有分部视图
        /// 分部视图声明新的ViewBag，并且分部视图还具有Layout
        /// </summary>
        [TestMethod]
        public void WithLayoutAndPartial()
        {
            dynamic viewBag = new RazorExpandoObject();
            QuickTestModel model = new()
            {
                Name = "pick",
                Age = 12,
                ExtraInfo = new QuickTestModel2()
                {
                    Company = "义翘神州"
                }
            };
            string result = razorEnginePlus.RenderRaw("NestViewTest/WithLayoutAndPartial", model, viewBag);
            Console.WriteLine(result);
            Assert.IsNotNull(result);
            Assert.IsTrue(result == @"<div>我是布局页 登录中,请稍后</div>
<div>我是分部视图布局页:我这里声明了一个新的ViewBag-{我是在分部视图中声明的ViewBag}</div>
<div>我的Model 义翘神州</div>
<div>我尝试在布局页中展示分部视图中的ViewBag:我是在分部视图中声明的ViewBag</div>");
        }

        /// <summary>
        /// 带Layout,并且Layout中含有分部视图
        /// 分部视图声明新的ViewBag，并且分部视图还具有Layout
        /// 异步版本
        /// </summary>
        [TestMethod]
        public async Task WithLayoutAndPartialAsync()
        {
            dynamic viewBag = new RazorExpandoObject();
            QuickTestModel model = new()
            {
                Name = "pick",
                Age = 12,
                ExtraInfo = new QuickTestModel2()
                {
                    Company = "义翘神州"
                }
            };
            string result = await razorEnginePlus.RenderRawAsync("NestViewTest/WithLayoutAndPartial", model, viewBag);
            Console.WriteLine(result);
            Assert.IsNotNull(result);
            Assert.IsTrue(result == @"<div>我是布局页 登录中,请稍后</div>
<div>我是分部视图布局页:我这里声明了一个新的ViewBag-{我是在分部视图中声明的ViewBag}</div>
<div>我的Model 义翘神州</div>
<div>我尝试在布局页中展示分部视图中的ViewBag:我是在分部视图中声明的ViewBag</div>");
        }
    }
}
