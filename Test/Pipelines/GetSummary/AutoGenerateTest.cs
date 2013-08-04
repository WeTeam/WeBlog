using NUnit.Framework;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Pipelines.GetSummary;
using Sitecore.SecurityModel;

namespace Sitecore.Modules.WeBlog.Test.Pipelines.GetSummary
{
    [TestFixture]
    [Category("GetSummary AutoGenerate")]
    public class AutoGenerateTest
    {
        Item m_testRoot = null;
        Item m_contentNoTagsSmall = null;
        Item m_contentNoTagsLarge = null;
        Item m_contentTagsSmall = null;
        Item m_contentTagsLarge = null;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var home = Sitecore.Context.Database.GetItem("/sitecore/content/home");
            var folderTemplate = Sitecore.Context.Database.GetTemplate(Constants.FolderTemplate);
            var template = Sitecore.Context.Database.GetTemplate(Constants.EntryTemplate);

            using (new SecurityDisabler())
            {
                m_testRoot = home.Add("test root", folderTemplate);
                m_contentNoTagsSmall = CreateTestContentItem(m_testRoot, template, "no tags small", "Lorem ipsum dolor sit amet");
                m_contentNoTagsLarge = CreateTestContentItem(m_testRoot, template, "no tags large", "Nullam et arcu dui, in pharetra diam. In vitae ante ac orci mollis egestas a vitae nunc. Proin sollicitudin eleifend ipsum vitae dictum. Vestibulum ut eros malesuada erat congue pellentesque. Aliquam vulputate urna a arcu rutrum iaculis. In bibendum, augue nec ultricies sollicitudin, sem ante sodales lacus, euismod ultrices ipsum risus et dui. Phasellus rutrum quam elementum dolor interdum ut elementum tortor varius. Nunc dapibus euismod pellentesque. Donec placerat purus et diam ultrices euismod. Proin ac justo leo, non rhoncus est. Quisque iaculis commodo augue non vulputate. Proin accumsan tempor erat eget molestie. Etiam nec erat nec urna tincidunt porta vitae id augue. Integer ullamcorper nunc at leo malesuada sit amet dignissim justo ullamcorper.");
                m_contentTagsSmall = CreateTestContentItem(m_testRoot, template, "tags small", "<div class=\"something\"><strong>Lorem</strong> ipsum <em>dolor sit</em> amet</div>");
                m_contentTagsLarge = CreateTestContentItem(m_testRoot, template, "tags large", "<h1>HTML Ipsum Presents</h1><p><strong>Pellentesque habitant morbi tristique</strong> senectus et netus et malesuada fames ac turpis egestas. Vestibulum tortor quam, feugiat vitae, ultricies eget, tempor sit amet, ante. Donec eu libero sit amet quam egestas semper. <em>Aenean ultricies mi vitae est.</em> Mauris placerat eleifend leo. Quisque sit amet est et sapien ullamcorper pharetra. Vestibulum erat wisi, condimentum sed, <code>commodo vitae</code>, ornare sit amet, wisi. Aenean fermentum, elit eget tincidunt condimentum, eros ipsum rutrum orci, sagittis tempus lacus enim ac dui. <a href=\"#\">Donec non enim</a> in turpis pulvinar facilisis. Ut felis.</p><h2>Header Level 2</h2><ol><li>Lorem ipsum dolor sit amet, consectetuer adipiscing elit.</li><li>Aliquam tincidunt mauris eu risus.</li></ol><blockquote><p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus magna. Cras in mi at felis aliquet congue. Ut a est eget ligula molestie gravida. Curabitur massa. Donec eleifend, libero at sagittis mollis, tellus est malesuada tellus, at luctus turpis elit sit amet quam. Vivamus pretium ornare est.</p></blockquote><h3>Header Level 3</h3><ul><li>Lorem ipsum dolor sit amet, consectetuer adipiscing elit.</li><li>Aliquam tincidunt mauris eu risus.</li></ul><pre><code>#header h1 a { display: block; width: 300px; height: 80px; }</code></pre>");
            }
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            if (m_testRoot != null)
            {
                using (new SecurityDisabler())
                {
                    m_testRoot.Delete();
                }
            }
        }

        [Test]
        public void StripTags_NoTags()
        {
            var procesor = new AutoGenerate();
            procesor.StripTags = true;
            procesor.MaximumCharacterCount = m_contentNoTagsSmall["content"].Length;

            var args = new GetSummaryArgs();
            args.Entry = m_contentNoTagsSmall;

            procesor.Process(args);

            Assert.AreEqual(m_contentNoTagsSmall["content"], args.Summary);
        }

        [Test]
        public void StripTags_TagsPresent()
        {
            var procesor = new AutoGenerate();
            procesor.StripTags = true;
            procesor.MaximumCharacterCount = 300;

            var args = new GetSummaryArgs();
            args.Entry = m_contentTagsSmall;

            procesor.Process(args);

            Assert.AreEqual("Lorem ipsum dolor sit amet", args.Summary);
        }

        [Test]
        public void EmptyField()
        {
            var procesor = new AutoGenerate();
            procesor.StripTags = true;
            procesor.MaximumCharacterCount = 300;

            var args = new GetSummaryArgs();
            args.Entry = m_testRoot;

            procesor.Process(args);

            Assert.IsNullOrEmpty(args.Summary);
        }

        [Test]
        public void NullItem()
        {
            var procesor = new AutoGenerate();
            procesor.StripTags = true;
            procesor.MaximumCharacterCount = 300;

            var args = new GetSummaryArgs();
            args.Entry = null;

            procesor.Process(args);

            Assert.IsNullOrEmpty(args.Summary);
        }

        [Test]
        public void StripTagsLimit_OverLimit()
        {
            var procesor = new AutoGenerate();
            procesor.StripTags = true;
            procesor.MaximumCharacterCount = 200;

            var args = new GetSummaryArgs();
            args.Entry = m_contentTagsLarge;

            procesor.Process(args);

            Assert.AreEqual("HTML Ipsum PresentsPellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Vestibulum tortor quam, feugiat vitae, ultricies eget, tempor sit amet, ante. Donec eu ...", args.Summary);
        }

        [Test]
        public void KeepTagsLimit_OverLimit()
        {
            var procesor = new AutoGenerate();
            procesor.StripTags = false;
            procesor.MaximumCharacterCount = 41;

            var args = new GetSummaryArgs();
            args.Entry = m_contentTagsLarge;

            procesor.Process(args);

            // It doesn't appear HAP is closing the P tag.
            Assert.AreEqual("<h1>HTML Ipsum Presents</h1><p><strong>Pellentesque habitant ...</strong>", args.Summary);
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