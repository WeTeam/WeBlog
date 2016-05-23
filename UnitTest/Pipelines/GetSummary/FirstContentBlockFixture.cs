using NUnit.Framework;
using Sitecore.FakeDb;
using Sitecore.Modules.WeBlog.Pipelines.GetSummary;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.GetSummary
{
    [TestFixture]
    public class FirstContentBlockFixture
    {
        [TestCase("Lorem ipsum<hr/> dolor sit amet", "Lorem ipsum", "//hr", TestName = "Self closing tag")]
        [TestCase("Nullam et arcu dui, in pharetra diam. In vitae ante ac orci mollis egestas a <span>vitae nunc</span>.", "Nullam et arcu dui, in pharetra diam. In vitae ante ac orci mollis egestas a ", "//span", TestName = "Surrounding tag")]
        [TestCase("Lorem ipsum dolor sit amet", "", "//hr", TestName = "No tags")]
        [TestCase("<div>Lorem ipsum<hr/> dolor sit amet</div>", "<div>Lorem ipsum</div>", "//hr", TestName = "Lost closing tag")]
        public void Test(string input, string expected, string xpath)
        {
            var procesor = new FirstContentBlock
            {
                FieldName = "Text",
                XPath = xpath
            };

            using (var db = new Db
            {
                new DbItem("item")
                {
                    new DbField("text")
                    {
                        Value = input
                    }
                }
            })
            {
                var args = new GetSummaryArgs
                {
                    Entry = db.GetItem("/sitecore/content/item")
                };

                procesor.Process(args);

                Assert.That(args.Summary, Is.EqualTo(expected));
            }
        }

        [Test]
        public void NullItem()
        {
            var procesor = new FirstContentBlock
            {
                FieldName = "Text",
                XPath = "//hr"
            };

            var args = new GetSummaryArgs
            {
                Entry = null
            };

            procesor.Process(args);

            Assert.That(args.Summary, Is.Empty);
        }
    }
}
