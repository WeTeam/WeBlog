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

namespace Sitecore.Modules.WeBlog.Test
{
    [TestFixture]
    [Category("TagManager")]
    public class TagManager
    {
        Item m_testRoot = null;
        Item m_blog1 = null;
        Item m_entry1 = null;
        Item m_entry2 = null;
        Item m_entry3 = null;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            // Create test content
            var home = Sitecore.Context.Database.GetItem("/sitecore/content/home");
            using (new SecurityDisabler())
            {
                home.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\tag manager content.xml")), false, PasteMode.Overwrite);
            }

            // Retrieve created content items
            m_testRoot = home.Axes.GetChild("blog testroot");
            m_blog1 = m_testRoot.Axes.GetChild("myblog");

            m_entry1 = m_blog1.Axes.GetDescendant("Entry1");
            m_entry2 = m_blog1.Axes.GetDescendant("Entry2");
            m_entry3 = m_blog1.Axes.GetDescendant("Entry3");
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
            var tags = Mod.TagManager.GetAllTags(new Items.WeBlog.BlogHomeItem(m_blog1));
            Assert.AreEqual(1, tags["tagb"]);
            Assert.AreEqual(2, tags["tagc"]);
            Assert.AreEqual(3, tags["taga"]);
        }

        [Test]
        public void GetAllTags_NonBlogItem()
        {
            var tags = Mod.TagManager.GetAllTags(new Items.WeBlog.BlogHomeItem(m_entry1));
            Assert.AreEqual(0, tags.Count);
        }

        [Test]
        public void GetAllTags_Null()
        {
            var tags = Mod.TagManager.GetAllTags(new Items.WeBlog.BlogHomeItem(m_entry1));
            Assert.AreEqual(0, tags.Count);
        }

        [Test]
        public void GetTagsByBlog_Blog1()
        {
            var tags = Mod.TagManager.GetTagsByBlog(m_blog1.ID);
            Assert.Contains("taga", tags);
            Assert.Contains("tagb", tags);
            Assert.Contains("tagc", tags);
        }

        [Test]
        public void GetTagsByBlog_Null()
        {
            var tags = Mod.TagManager.GetTagsByBlog((ID)null);
            Assert.AreEqual(0, tags.Length);
        }

        [Test]
        public void GetTagsByBlog_NonBlogID()
        {
            var tags = Mod.TagManager.GetTagsByBlog(m_entry1.ID);
            Assert.AreEqual(0, tags.Length);
        }

        [Test]
        public void GetTagsByBlog_InvalidId()
        {
            var tags = Mod.TagManager.GetTagsByBlog(ID.NewID);
            Assert.AreEqual(0, tags.Length);
        }

        [Test]
        public void SortByWeight_Normal()
        {
            var weightedTags = Mod.TagManager.SortByWeight(new string[] { "a", "a", "b", "c", "c", "c", "c", "a", "b" });
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
            var weightedTags = Mod.TagManager.SortByWeight(new string[] { "a", "b", "c", "A", "B", "C", "D", "d" });
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
            var weightedTags = Mod.TagManager.SortByWeight(new string[0]);
            Assert.AreEqual(0, weightedTags.Count);
        }

        [Test]
        public void SortByWeight_CaseInsensitive()
        {
            var weightedTags = Mod.TagManager.SortByWeight(new string[] { "mytag", "MyTag", "MYTAG", "myTaG"});
            Assert.AreEqual(1, weightedTags.Count);
        }
    }
}