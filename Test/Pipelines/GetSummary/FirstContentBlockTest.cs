using NUnit.Framework;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Pipelines.GetSummary;
using Sitecore.SecurityModel;

namespace Sitecore.Modules.WeBlog.Test.Pipelines.GetSummary
{
    [TestFixture]
    [Category("GetSummary FirstContentBlock")]
    public class FirstContentBlockTest : UnitTestBase
    {
        Item m_testRoot = null;
        Item m_contentContainsHr = null;
        Item m_contentContainsSpan = null;
        Item m_contentNoTags = null;
        Item m_contentSurroundingTag = null;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var folderTemplate = Sitecore.Context.Database.GetTemplate(Constants.FolderTemplate);
            var template = Sitecore.Context.Database.GetTemplate(Constants.EntryTemplate);

            using (new SecurityDisabler())
            {
                m_testRoot = m_testContentRoot.Add("test root", folderTemplate);
                m_contentContainsHr = CreateTestContentItem(m_testRoot, template, "contains hr", "Lorem ipsum<hr/> dolor sit amet");
                m_contentContainsSpan = CreateTestContentItem(m_testRoot, template, "contains span", "Nullam et arcu dui, in pharetra diam. In vitae ante ac orci mollis egestas a <span>vitae nunc</span>.");
                m_contentNoTags = CreateTestContentItem(m_testRoot, template, "no tags", "Lorem ipsum dolor sit amet");
                m_contentSurroundingTag = CreateTestContentItem(m_testRoot, template, "surrounding tag", "<div>Lorem ipsum<hr/> dolor sit amet</div>");
            }
        }

        [Test]
        public void ContainsSelfClosingTag()
        {
            var procesor = new FirstContentBlock();
            procesor.XPath = "//hr";

            var args = new GetSummaryArgs();
            args.Entry = m_contentContainsHr;

            procesor.Process(args);

            Assert.AreEqual("Lorem ipsum", args.Summary);
        }

        [Test]
        public void ContainsSurroundingTag()
        {
            var procesor = new FirstContentBlock();
            procesor.XPath = "//span";

            var args = new GetSummaryArgs();
            args.Entry = m_contentContainsSpan;

            procesor.Process(args);

            Assert.AreEqual("Nullam et arcu dui, in pharetra diam. In vitae ante ac orci mollis egestas a ", args.Summary);
        }

        [Test]
        public void ContainsNoTag()
        {
            var procesor = new FirstContentBlock();
            procesor.XPath = "//hr";

            var args = new GetSummaryArgs();
            args.Entry = m_contentNoTags;

            procesor.Process(args);

            Assert.IsNullOrEmpty(args.Summary);
        }

        [Test]
        public void ContainsCutTag()
        {
            var procesor = new FirstContentBlock();
            procesor.XPath = "//hr";

            var args = new GetSummaryArgs();
            args.Entry = m_contentSurroundingTag;

            procesor.Process(args);

            Assert.AreEqual("<div>Lorem ipsum</div>", args.Summary);
        }

        private Item CreateTestContentItem(Item item, TemplateItem template, string name, string content)
        {
            var child = item.Add(name, template);
            child.Editing.BeginEdit();
            child["content"] = content;
            child.Editing.EndEdit();

            return child;
        }
    }
}