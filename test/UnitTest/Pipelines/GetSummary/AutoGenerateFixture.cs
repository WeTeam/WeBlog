using NUnit.Framework;
using Sitecore.FakeDb;
using Sitecore.Modules.WeBlog.Pipelines.GetSummary;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.GetSummary
{
    [TestFixture]
    public class AutoGenerateFixture
    {
        [TestCase("", "", 300, true, TestName = "Empty field")]
        [TestCase("Lorem ipsum dolor sit amet", "Lorem ipsum dolor sit amet", 300, true, TestName = "No tags")]
        [TestCase("<div class=\"something\"><strong>Lorem</strong> ipsum <em>dolor sit</em> amet</div>", "Lorem ipsum dolor sit amet", 300, true, TestName = "Tags present")]
        [TestCase("<h1>HTML Ipsum Presents</h1><p><strong>Pellentesque habitant morbi tristique</strong> senectus et netus et malesuada fames ac turpis egestas. Vestibulum tortor quam, feugiat vitae, ultricies eget, tempor sit amet, ante. Donec eu libero sit amet quam egestas semper. <em>Aenean ultricies mi vitae est.</em> Mauris placerat eleifend leo. Quisque sit amet est et sapien ullamcorper pharetra. Vestibulum erat wisi, condimentum sed, <code>commodo vitae</code>, ornare sit amet, wisi. Aenean fermentum, elit eget tincidunt condimentum, eros ipsum rutrum orci, sagittis tempus lacus enim ac dui. <a href=\"#\">Donec non enim</a> in turpis pulvinar facilisis. Ut felis.</p><h2>Header Level 2</h2><ol><li>Lorem ipsum dolor sit amet, consectetuer adipiscing elit.</li><li>Aliquam tincidunt mauris eu risus.</li></ol><blockquote><p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus magna. Cras in mi at felis aliquet congue. Ut a est eget ligula molestie gravida. Curabitur massa. Donec eleifend, libero at sagittis mollis, tellus est malesuada tellus, at luctus turpis elit sit amet quam. Vivamus pretium ornare est.</p></blockquote><h3>Header Level 3</h3><ul><li>Lorem ipsum dolor sit amet, consectetuer adipiscing elit.</li><li>Aliquam tincidunt mauris eu risus.</li></ul><pre><code>#header h1 a { display: block; width: 300px; height: 80px; }</code></pre>", "HTML Ipsum PresentsPellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Vestibulum tortor quam, feugiat vitae, ultricies eget, tempor sit amet, ante. Donec eu ...", 200, true, TestName = "Over limit")]
        [TestCase("<h1>HTML Ipsum Presents</h1><p><strong>Pellentesque habitant morbi tristique</strong> senectus et netus et malesuada fames ac turpis egestas. Vestibulum tortor quam, feugiat vitae, ultricies eget, tempor sit amet, ante. Donec eu libero sit amet quam egestas semper. <em>Aenean ultricies mi vitae est.</em> Mauris placerat eleifend leo. Quisque sit amet est et sapien ullamcorper pharetra. Vestibulum erat wisi, condimentum sed, <code>commodo vitae</code>, ornare sit amet, wisi. Aenean fermentum, elit eget tincidunt condimentum, eros ipsum rutrum orci, sagittis tempus lacus enim ac dui. <a href=\"#\">Donec non enim</a> in turpis pulvinar facilisis. Ut felis.</p><h2>Header Level 2</h2><ol><li>Lorem ipsum dolor sit amet, consectetuer adipiscing elit.</li><li>Aliquam tincidunt mauris eu risus.</li></ol><blockquote><p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus magna. Cras in mi at felis aliquet congue. Ut a est eget ligula molestie gravida. Curabitur massa. Donec eleifend, libero at sagittis mollis, tellus est malesuada tellus, at luctus turpis elit sit amet quam. Vivamus pretium ornare est.</p></blockquote><h3>Header Level 3</h3><ul><li>Lorem ipsum dolor sit amet, consectetuer adipiscing elit.</li><li>Aliquam tincidunt mauris eu risus.</li></ul><pre><code>#header h1 a { display: block; width: 300px; height: 80px; }</code></pre>", "<h1>HTML Ipsum Presents</h1><p><strong>Pellentesque habitant ...</strong>", 41, false, TestName = "Over limit keep tags")]
        public void Test(string input, string expected, int limit, bool stripTags)
        {
            var procesor = new AutoGenerate
            {
                FieldName = "Text",
                StripTags = stripTags,
                MaximumCharacterCount = limit
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
            var procesor = new AutoGenerate
            {
                FieldName = "Text",
                StripTags = true,
                MaximumCharacterCount = 200
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
