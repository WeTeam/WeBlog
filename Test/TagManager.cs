using System;
using System.Linq;
using NUnit.Framework;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using System.Web;
using System.IO;
using Mod = Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Items;
using Sitecore.Data;
using Sitecore.Search;

namespace Sitecore.Modules.WeBlog.Test
{
    [TestFixture]
    [Category("TagManager")]
    public class TagManager : UnitTestBase
    {
        Item m_home = null;
        Item m_testRoot = null;
        Item m_blog1 = null;
        Item m_entry1 = null;
        Item m_entry2 = null;
        Item m_entry3 = null;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            // Create test content
            m_home = Sitecore.Context.Database.GetItem("/sitecore/content/home");
            using (new SecurityDisabler())
            {
                m_home.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\tag manager content.xml")), false, PasteMode.Overwrite);
            }
            Initialize();
        }

        protected void Initialize()
        {
            // Retrieve created content items
            m_testRoot = m_home.Axes.GetChild("blog testroot");
            m_blog1 = m_testRoot.Axes.GetChild("myblog");

            m_entry1 = m_blog1.Axes.GetDescendant("Entry1");
            m_entry2 = m_blog1.Axes.GetDescendant("Entry2");
            m_entry3 = m_blog1.Axes.GetDescendant("Entry3");

            // rebuild the WeBlog search index (or some aspects of TagManager won't work)
            var index = SearchManager.GetIndex(Settings.SearchIndexName);
            index.Rebuild();
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
        public void GetAllTags_Blog1()
        {
            var tags = new Mod.TagManager().GetAllTags(new Items.WeBlog.BlogHomeItem(m_blog1));
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
            var tags = new Mod.TagManager().GetAllTags(new Items.WeBlog.BlogHomeItem(m_entry1));
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