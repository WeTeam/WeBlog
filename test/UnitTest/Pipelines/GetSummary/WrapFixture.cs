using NUnit.Framework;
using Sitecore.FakeDb.Sites;
using Sitecore.Modules.WeBlog.Pipelines.GetSummary;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.GetSummary
{
    [TestFixture]
    public class WrapFixture
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Sitecore.Context.IsUnitTesting = true;
            Sitecore.Context.Site = new FakeSiteContext("fake");
        }

        [TestCase(true, "span", "lorem ipsum", "<span>lorem ipsum</span>", TestName = "Unwrapped, only when required")]
        [TestCase(true, "span", "<span>lorem ipsum</span>", "<span>lorem ipsum</span>", TestName = "Wrapped, only when required")]
        [TestCase(false, "span", "lorem ipsum", "<span>lorem ipsum</span>", TestName = "Unwrapped, wrap always")]
        [TestCase(false, "span", "<span>lorem ipsum</span>", "<span><span>lorem ipsum</span></span>", TestName = "Wrapped, wrap always")]
        [TestCase(true, "p", "", "<p></p>", TestName = "Empty summary")]
        public void Test(bool onlyWhenRequired, string tag, string input, string expected)
        {
            var processor = new Wrap
            {
                OnlyWhenRequired = onlyWhenRequired,
                WrappingTag = tag
            };

            var args = new GetSummaryArgs
            {
                Summary = input
            };

            processor.Process(args);

            Assert.That(args.Summary, Is.EqualTo(expected));
        }
    }
}
