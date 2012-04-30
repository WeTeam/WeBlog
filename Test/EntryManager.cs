using System;
using System.Linq;
using System.Web;
using NUnit.Framework;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using System.IO;
using Mod = Sitecore.Modules.WeBlog.Managers;
using Sitecore.Data;
using Sitecore.Search;
using Sitecore.Analytics;
#if !PRE_65
using Sitecore.Analytics.Data.DataAccess;
#endif

namespace Sitecore.Modules.WeBlog.Test
{
    [TestFixture]
    [Category("EntryManager")]
    public class EntryManager : UnitTestBase
    {
        private Item m_home = null;
        private Item m_testRoot = null;
        private Item m_blog1 = null;
        private Item m_entry11 = null;
        private Item m_entry12 = null;
        private Item m_entry13 = null;
        private Item m_category12 = null;
        private Item m_category13 = null;
        private Item m_blog2 = null;
        private Item m_entry21 = null;
        private Item m_entry22 = null;
        private Item m_entry23 = null;
        private Item m_category21 = null;
        private Item m_category22 = null;
        private Item m_comment1 = null;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            // Create test content
            m_home = Sitecore.Context.Database.GetItem("/sitecore/content/home");
            using (new SecurityDisabler())
            {
                m_home.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\entry manager content.xml")), false, PasteMode.Overwrite);
            }
            Initialize();
        }

        protected void Initialize()
        {
            // Retrieve created content items
            m_testRoot = m_home.Axes.GetChild("weblog testroot");
            m_blog1 = m_testRoot.Axes.GetChild("blog1");
            m_blog2 = m_testRoot.Axes.GetChild("blog2");

            m_entry11 = m_blog1.Axes.GetDescendant("Entry1");
            m_entry12 = m_blog1.Axes.GetDescendant("Entry2");
            m_entry13 = m_blog1.Axes.GetDescendant("Entry3");
            m_entry21 = m_blog2.Axes.GetDescendant("Entry1");
            m_entry22 = m_blog2.Axes.GetDescendant("Entry2");
            m_entry23 = m_blog2.Axes.GetDescendant("Entry3");

            var blog1Categories = m_blog1.Axes.GetChild("categories");
            m_category12 = blog1Categories.Axes.GetChild("category2");
            m_category13 = blog1Categories.Axes.GetChild("category3");

            var blog2Categories = m_blog2.Axes.GetChild("categories");
            m_category21 = blog2Categories.Axes.GetChild("category1");
            m_category22 = blog2Categories.Axes.GetChild("category2");

            m_comment1 = m_entry21.Axes.GetDescendant("comment1");

            // rebuild the WeBlog search index (or the entry manager won't work)
            var index = SearchManager.GetIndex(Settings.SearchIndexName);
            index.Rebuild();

#if PRE_65
            //Sitecore.Analytics.AnalyticsTracker.Current.
#else
            if (Sitecore.Configuration.Settings.Analytics.Enabled)
            {
                // Register DMS page views for popular items
                var visitor = new Visitor(Guid.NewGuid());
                visitor.CreateVisit(Guid.NewGuid());
                visitor.CurrentVisit.CreatePage().ItemId = m_entry13.ID.ToGuid();
                visitor.CurrentVisit.CreatePage().ItemId = m_entry13.ID.ToGuid();
                visitor.CurrentVisit.CreatePage().ItemId = m_entry13.ID.ToGuid();

                visitor.CurrentVisit.CreatePage().ItemId = m_entry11.ID.ToGuid();
                visitor.CurrentVisit.CreatePage().ItemId = m_entry11.ID.ToGuid();

                visitor.CurrentVisit.CreatePage().ItemId = m_entry12.ID.ToGuid();

                visitor.Submit();
            }
#endif
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            using (new SecurityDisabler())
            {
                if (m_testRoot != null)
                    m_testRoot.Delete();
            }
        }

        [Test]
        public void GetBlogEntries_Blog1()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetBlogEntries(m_blog1, int.MaxValue, null, null)
                            select entry.ID).ToArray();

            Assert.AreEqual(3, entryIds.Length);
            Assert.Contains(m_entry11.ID, entryIds);
            Assert.Contains(m_entry12.ID, entryIds);
            Assert.Contains(m_entry13.ID, entryIds);
        }

        [Test]
        public void GetBlogEntries_Blog2()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetBlogEntries(m_blog2, int.MaxValue, null, null)
                            select entry.ID).ToArray();

            Assert.AreEqual(3, entryIds.Length);
            Assert.Contains(m_entry21.ID, entryIds);
            Assert.Contains(m_entry22.ID, entryIds);
            Assert.Contains(m_entry23.ID, entryIds);
        }

        [Test]
        public void GetBlogEntries_Blog1_WithLimit()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetBlogEntries(m_blog1, 2, null, null)
                            select entry.ID).ToArray();

            Assert.AreEqual(2, entryIds.Length);
            Assert.Contains(m_entry12.ID, entryIds);
            Assert.Contains(m_entry13.ID, entryIds);
        }

        [Test]
        public void GetBlogEntries_Blog1_WithTag()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetBlogEntries(m_blog1, int.MaxValue, "tagb", null)
                            select entry.ID).ToArray();

            Assert.AreEqual(2, entryIds.Length);
            Assert.Contains(m_entry11.ID, entryIds);
            Assert.Contains(m_entry12.ID, entryIds);
        }

        [Test]
        public void GetBlogEntries_TagWithSpace()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetBlogEntries(m_blog1, int.MaxValue, "tag with space", null)
                            select entry.ID).ToArray();

            Assert.AreEqual(1, entryIds.Length);
            Assert.Contains(m_entry13.ID, entryIds);
        }

        [Test]
        public void GetBlogEntries_Blog1_WithCategory()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetBlogEntries(m_blog1, int.MaxValue, null, m_category13.Name)
                            select entry.ID).ToArray();

            Assert.AreEqual(1, entryIds.Length);
            Assert.Contains(m_entry13.ID, entryIds);
        }

        [Test]
        public void GetBlogEntries_Blog1_WithLimitAndCategory()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetBlogEntries(m_blog1, 1, null, m_category12.Name)
                            select entry.ID).ToArray();

            Assert.AreEqual(1, entryIds.Length);
            Assert.Contains(m_entry13.ID, entryIds);
        }

        [Test]
        public void GetBlogEntries_Blog1_EntryItem()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetBlogEntries(m_entry11, int.MaxValue, null, null)
                            select entry.ID).ToArray();

            //entry is part of blog with 3 items
            Assert.AreEqual(3, entryIds.Length);
        }

        [Test]
        public void GetBlogEntries_Blog1_WithZeroLimit()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetBlogEntries(m_blog1, 0, null, null)
                            select entry.ID).ToArray();

            Assert.AreEqual(0, entryIds.Length);
        }

        [Test]
        public void GetBlogEntries_Blog1_WithNegativeLimit()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetBlogEntries(m_blog1, -7, null, null)
                            select entry.ID).ToArray();

            Assert.AreEqual(0, entryIds.Length);
        }

        [Test]
        public void GetBlogEntries_Blog1_WithInvalidCategory()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetBlogEntries(m_blog1, int.MaxValue, null, "bler")
                            select entry.ID).ToArray();

            Assert.AreEqual(0, entryIds.Length);
        }

        [Test]
        public void GetBlogEntries_Blog1_WithInvalidTag()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetBlogEntries(m_blog1, int.MaxValue, "bler", null)
                            select entry.ID).ToArray();

            Assert.AreEqual(0, entryIds.Length);
        }

        [Test]
        public void GetBlogEntryByCategorie_Blog2_Category1_ById()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetBlogEntryByCategorie(m_blog2.ID, m_category21.ID)
                            select entry.ID).ToArray();

            Assert.AreEqual(2, entryIds.Length);
            Assert.Contains(m_entry21.ID, entryIds);
            Assert.Contains(m_entry22.ID, entryIds);
        }

        [Test]
        public void GetBlogEntryByCategorie_Blog2_Category1()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetBlogEntryByCategorie(m_blog2.ID, m_category21.Name)
                            select entry.ID).ToArray();

            Assert.AreEqual(2, entryIds.Length);
            Assert.Contains(m_entry21.ID, entryIds);
            Assert.Contains(m_entry22.ID, entryIds);
        }

        [Test]
        public void GetBlogEntryByCategorie_Blog2_InvalidCategory()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetBlogEntryByCategorie(m_blog2.ID, m_category12.ID)
                            select entry.ID).ToArray();

            Assert.AreEqual(0, entryIds.Length);
        }

        [Test]
        public void GetBlogEntriesByMonthAndYear_Blog1_March2011()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetBlogEntriesByMonthAndYear(m_blog1, 3, 2011)
                            select entry.ID).ToArray();

            Assert.AreEqual(2, entryIds.Length);
            Assert.Contains(m_entry11.ID, entryIds);
            Assert.Contains(m_entry12.ID, entryIds);
        }

        [Test]
        public void GetBlogEntriesByMonthAndYear_Blog1_April2011()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetBlogEntriesByMonthAndYear(m_blog1, 4, 2011)
                            select entry.ID).ToArray();

            Assert.AreEqual(1, entryIds.Length);
            Assert.Contains(m_entry13.ID, entryIds);
        }

        [Test]
        public void GetBlogEntriesByMonthAndYear_Blog1_InvalidMonth()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetBlogEntriesByMonthAndYear(m_blog1, 17, 2011)
                            select entry.ID).ToArray();

            Assert.AreEqual(0, entryIds.Length);
        }

        [Test]
        public void GetBlogEntryByComment()
        {
            var entry = new Mod.EntryManager().GetBlogEntryByComment(m_comment1);

            Assert.AreEqual(m_entry21.ID, entry.ID);
        }

        [Ignore]
        [Test]
        public virtual void MakeSortedEntriesList_InOrder()
        {
            using (new SecurityDisabler())
            {
                m_testRoot.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\entries in order.xml")), false, PasteMode.Overwrite);
            }

            // rebuild the WeBlog search index (or the entry manager won't work)
            var index = SearchManager.GetIndex(Settings.SearchIndexName);
            index.Rebuild();

            var blog = m_testRoot.Axes.GetChild("MyBlog");

            try
            {
                var entries = from entry in new Mod.EntryManager().GetBlogEntries(blog)
                              select entry.InnerItem;

                var sorted = entries.ToArray();
                Assert.AreEqual(3, sorted.Length);
                Assert.AreEqual("Entry3", sorted[0].Name);
                Assert.AreEqual("Entry2", sorted[1].Name);
                Assert.AreEqual("Entry1", sorted[2].Name);
            }
            finally
            {
                if (blog != null)
                {
                    using (new SecurityDisabler())
                    {
                        blog.Delete();
                    }
                }
            }
        }

        [Ignore]
        [Test]
        public virtual void MakeSortedEntriesList_ReverseOrder()
        {
            using (new SecurityDisabler())
            {
                m_testRoot.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\entries reverse order.xml")), false, PasteMode.Overwrite);
            }

            // rebuild the WeBlog search index (or the entry manager won't work)
            var index = SearchManager.GetIndex(Settings.SearchIndexName);
            index.Rebuild();

            var blog = m_testRoot.Axes.GetChild("MyBlog");

            try
            {
                var entries = from entry in new Mod.EntryManager().GetBlogEntries(blog)
                              select entry.InnerItem;

                var sorted = entries.ToArray();
                Assert.AreEqual(3, sorted.Length);
                Assert.AreEqual("Entry1", sorted[0].Name);
                Assert.AreEqual("Entry2", sorted[1].Name);
                Assert.AreEqual("Entry3", sorted[2].Name);
            }
            finally
            {
                if (blog != null)
                {
                    using (new SecurityDisabler())
                    {
                        blog.Delete();
                    }
                }
            }
        }

        [Ignore]
        [Test]
        public virtual void MakeSortedEntriesList_OutOfOrder()
        {
            using (new SecurityDisabler())
            {
                m_testRoot.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\entries out of order.xml")), false, PasteMode.Overwrite);
            }

            // rebuild the WeBlog search index (or the entry manager won't work)
            var index = SearchManager.GetIndex(Settings.SearchIndexName);
            index.Rebuild();

            var blog = m_testRoot.Axes.GetChild("MyBlog");

            try
            {
                var entries = from entry in new Mod.EntryManager().GetBlogEntries(blog)
                              select entry.InnerItem;

                var sorted = entries.ToArray();
                Assert.AreEqual(3, sorted.Length);
                Assert.AreEqual("Yet another entry", sorted[0].Name);
                Assert.AreEqual("Another Entry", sorted[1].Name);
                Assert.AreEqual("First Entry", sorted[2].Name);
            }
            finally
            {
                if (blog != null)
                {
                    using (new SecurityDisabler())
                    {
                        blog.Delete();
                    }
                }
            }
        }

        [Test]
        public void DeleteEntry_Null()
        {
            Assert.IsFalse(new Mod.EntryManager().DeleteEntry(null));
        }

        [Test]
        public void DeleteEntry_InValidID()
        {
            Assert.IsFalse(new Mod.EntryManager().DeleteEntry(ID.NewID.ToString()));
        }

        [Test]
        public void DeleteEntry_ValidItem()
        {
            Item toDel = null;
            var template = Sitecore.Context.Database.GetTemplate(Sitecore.Configuration.Settings.GetSetting("WeBlog.EntryTemplateID"));

            try
            {
                using (new SecurityDisabler())
                {
                    toDel = m_testRoot.Add("todel", template);
                    Assert.IsNotNull(toDel);

                    Assert.IsTrue(new Mod.EntryManager().DeleteEntry(toDel.ID.ToString()));
                }
            }
            finally
            {
                if (toDel != null)
                {
                    using (new SecurityDisabler())
                    {
                        toDel.Delete();
                    }
                }
            }
        }

        [Test]
        public void GetPopularEntriesByComment_ValidItem()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetPopularEntriesByComment(m_blog1, int.MaxValue)
                            select entry.ID).ToArray();

            Assert.AreEqual(3, entryIds.Length);
            Assert.AreEqual(m_entry12.ID, entryIds[0]);
            Assert.AreEqual(m_entry13.ID, entryIds[1]);
            Assert.AreEqual(m_entry11.ID, entryIds[2]);
        }

        [Test]
        public void GetPopularEntriesByComment_ValidItem_Limited()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetPopularEntriesByComment(m_blog1, 2)
                            select entry.ID).ToArray();

            Assert.AreEqual(2, entryIds.Length);
            Assert.AreEqual(m_entry12.ID, entryIds[0]);
            Assert.AreEqual(m_entry13.ID, entryIds[1]);
        }

        [Test]
        public void GetPopularEntriesByComment_InvalidItem()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetPopularEntriesByComment(m_entry12, int.MaxValue)
                            select entry.ID).ToArray();

            Assert.AreEqual(1, entryIds.Length);
            Assert.AreEqual(m_entry12.ID, entryIds[0]);
        }

        [Test]
        public void GetPopularEntriesByComment_NullItem()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetPopularEntriesByComment(null, int.MaxValue)
                            select entry.ID).ToArray();

            Assert.AreEqual(0, entryIds.Length);
        }

        [Test]
        public void GetPopularEntriesByView_ValidItem()
        {
#if PRE_65
            Assert.True(AnalyticsTracker.IsActive, "Sitecore.Analytics must be enabled to test");
#else
            Assert.True(Sitecore.Configuration.Settings.Analytics.Enabled, "Sitecore.Analytics must be enabled to test");
#endif
            var entryIds = (from entry in new Mod.EntryManager().GetPopularEntriesByView(m_blog1, int.MaxValue)
                            select entry.ID).ToArray();

            Assert.AreEqual(3, entryIds.Length);
            Assert.AreEqual(m_entry13.ID, entryIds[0]);
            Assert.AreEqual(m_entry11.ID, entryIds[1]);
            Assert.AreEqual(m_entry12.ID, entryIds[2]);
        }

        [Test]
        public void GetPopularEntriesByView_ValidItem_Limited()
        {
#if PRE_65
            Assert.True(AnalyticsTracker.IsActive, "Sitecore.Analytics must be enabled to test");
#else
            Assert.True(Sitecore.Configuration.Settings.Analytics.Enabled, "Sitecore.Analytics must be enabled to test");
#endif
            var entryIds = (from entry in new Mod.EntryManager().GetPopularEntriesByView(m_blog1, 1)
                            select entry.ID).ToArray();

            Assert.AreEqual(1, entryIds.Length);
            Assert.AreEqual(m_entry13.ID, entryIds[0]);
        }

        [Test]
        public void GetPopularEntriesByView_InvalidItem()
        {
#if PRE_65
            Assert.True(AnalyticsTracker.IsActive, "Sitecore.Analytics must be enabled to test");
#else
            Assert.True(Sitecore.Configuration.Settings.Analytics.Enabled, "Sitecore.Analytics must be enabled to test");
#endif
            var entryIds = (from entry in new Mod.EntryManager().GetPopularEntriesByView(m_entry12, int.MaxValue)
                            select entry.ID).ToArray();

            Assert.AreEqual(0, entryIds.Length);
        }

        [Test]
        public void GetPopularEntriesByView_NullItem()
        {
#if PRE_65
            Assert.True(AnalyticsTracker.IsActive, "Sitecore.Analytics must be enabled to test");
#else
            Assert.True(Sitecore.Configuration.Settings.Analytics.Enabled, "Sitecore.Analytics must be enabled to test");
#endif
            var entryIds = (from entry in new Mod.EntryManager().GetPopularEntriesByView(null, 1)
                            select entry.ID).ToArray();

            Assert.AreEqual(0, entryIds.Length);            
        }
    }
}