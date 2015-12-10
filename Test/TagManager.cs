using System.IO;
using System.Linq;
using System.Web;
using NUnit.Framework;
using Sitecore.ContentSearch;
using Sitecore.Data;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.SecurityModel;
using Mod = Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Test
{
    [TestFixture]
    [Category("TagManager")]
    public class TagManager : UnitTestBase
    {
        protected Item m_testRoot = null;
        protected Item m_blog1 = null;
        protected Item m_entry1 = null;
        protected Item m_entry2 = null;
        protected Item m_entry3 = null;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            // Create test content
            using (new SecurityDisabler())
            {
                using (new EventDisabler())
                {
                    m_testContentRoot.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\tag manager content.xml")), false, PasteMode.Overwrite);
                }
            }

            Initialize();
        }

        protected void Initialize()
        {
            // Retrieve created content items
            m_testRoot = m_testContentRoot.Axes.GetChild("blog testroot");
            m_blog1 = m_testRoot.Axes.GetChild("myblog");

            m_entry1 = m_blog1.Axes.GetDescendant("Entry1");
            m_entry2 = m_blog1.Axes.GetDescendant("Entry2");
            m_entry3 = m_blog1.Axes.GetDescendant("Entry3");

            // rebuild the WeBlog search index (or some aspects of TagManager won't work)
            var index = ContentSearchManager.GetIndex(Settings.SearchIndexName);
            index.Rebuild();
        }

        [Test]
        public void GetAllTags_Blog1()
        {
            var tags = new Mod.TagManager().GetAllTags(new BlogHomeItem(m_blog1));
            Assert.IsTrue(tags.ContainsKey("taga"), "taga not found");
            Assert.IsTrue(tags.ContainsKey("tagb"), "tagb not found");
            Assert.IsTrue(tags.ContainsKey("tagc"), "tagc not found");
            Assert.AreEqual(1, tags["tagb"]);
            Assert.AreEqual(2, tags["tagc"]);
            Assert.AreEqual(3, tags["taga"]);
        }

        [Test]
        public void GetAllTags_EntryItem()
        {
            var tags = new Mod.TagManager().GetAllTags(new BlogHomeItem(m_entry1));
            //entry is part of blog w/ 3 tags
            Assert.AreEqual(3, tags.Count);
        }

        [Test]
        public void GetAllTags_Null()
        {
            var tags = new Mod.TagManager().GetAllTags(null);
            Assert.AreEqual(0, tags.Count);
        }

        [Test]
        public void GetTagsByBlog_Blog1()
        {
            var tags = new Mod.TagManager().GetTagsByBlog(m_blog1.ID);
            Assert.Contains("taga", tags);
            Assert.Contains("tagb", tags);
            Assert.Contains("tagc", tags);
        }

        [Test]
        public void GetTagsByBlog_Null()
        {
            var tags = new Mod.TagManager().GetTagsByBlog((ID)null);
            Assert.AreEqual(0, tags.Length);
        }

        [Test]
        public void GetTagsByBlog_EntryID()
        {
            var tags = new Mod.TagManager().GetTagsByBlog(m_entry1.ID);
            //entry is part of blog w/ 3 tags
            Assert.AreEqual(3, tags.Length);
        }

        [Test]
        public void GetTagsByBlog_InvalidId()
        {
            var tags = new Mod.TagManager().GetTagsByBlog(ID.NewID);
            Assert.AreEqual(0, tags.Length);
        }

        [Test]
        public void SortByWeight_Normal()
        {
            var weightedTags = new Mod.TagManager().SortByWeight(new string[] { "a", "a", "b", "c", "c", "c", "c", "a", "b" });
            Assert.AreEqual(3, weightedTags.Count);
            Assert.AreEqual("a", weightedTags.ElementAt(0).Key);
            Assert.AreEqual(3, weightedTags["a"]);
            Assert.AreEqual("b", weightedTags.ElementAt(1).Key);
            Assert.AreEqual(2, weightedTags["b"]);
            Assert.AreEqual("c", weightedTags.ElementAt(2).Key);
            Assert.AreEqual(4, weightedTags["c"]);
        }

        [Test]
        public void SortByWeight_SameWeight()
        {
            var weightedTags = new Mod.TagManager().SortByWeight(new string[] { "a", "b", "c", "A", "B", "C", "D", "d" });
            Assert.AreEqual(4, weightedTags.Count);
            Assert.AreEqual("a", weightedTags.ElementAt(0).Key);
            Assert.AreEqual(2, weightedTags["a"]);
            Assert.AreEqual("b", weightedTags.ElementAt(1).Key);
            Assert.AreEqual(2, weightedTags["b"]);
            Assert.AreEqual("c", weightedTags.ElementAt(2).Key);
            Assert.AreEqual(2, weightedTags["c"]);
            Assert.AreEqual("D", weightedTags.ElementAt(3).Key);
            Assert.AreEqual(2, weightedTags["d"]);
        }

        [Test]
        public void SortByWeight_Empty()
        {
            var weightedTags = new Mod.TagManager().SortByWeight(new string[0]);
            Assert.AreEqual(0, weightedTags.Count);
        }

        [Test]
        public void SortByWeight_CaseInsensitive()
        {
            var weightedTags = new Mod.TagManager().SortByWeight(new string[] { "mytag", "MyTag", "MYTAG", "myTaG" });
            Assert.AreEqual(1, weightedTags.Count);
        }
    }
}