using System.Data;
using System;
using System.Linq;
using System.Web;
using NUnit.Framework;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using System.IO;
using Moq;
using Mod = Sitecore.Modules.WeBlog.Managers;
using Sitecore.Data;
using Sitecore.ContentSearch;
using Sitecore.Data.Events;
#if FEATURE_XDB
using Sitecore.Analytics.Reporting;
using System.Collections.Generic;
#elif FEATURE_DMS
using Sitecore.Analytics.Data.DataAccess;
#endif

namespace Sitecore.Modules.WeBlog.Test
{
    [TestFixture]
    [Category("EntryManager")]
    public class EntryManager : UnitTestBase
    {
        protected Item m_testRoot = null;
        protected Item m_blog1 = null;
        protected Item m_entry11 = null;
        protected Item m_entry12 = null;
        protected Item m_entry13 = null;
        protected Item m_category12 = null;
        protected Item m_category13 = null;
        protected Item m_blog2 = null;
        protected Item m_entry21 = null;
        protected Item m_entry22 = null;
        protected Item m_entry23 = null;
        protected Item m_category21 = null;
        protected Item m_category22 = null;
        protected Item m_comment1 = null;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            // Create test content
            using (new SecurityDisabler())
            {
                using (new EventDisabler())
                {
                    m_testContentRoot.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\entry manager content.xml")), false, PasteMode.Overwrite);
                }
            }

            Initialize();
        }

        protected void Initialize()
        {
            // Retrieve created content items
            m_testRoot = m_testContentRoot.Axes.GetChild("weblog testroot");
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
            var index = ContentSearchManager.GetIndex(Settings.SearchIndexName);
            index.Rebuild();

#if FEATURE_DMS
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

        public void VerifyAnalyticsSetup()
        {
            Assert.True(Sitecore.Configuration.Settings.Analytics.Enabled, "Sitecore.Analytics must be enabled to test");
        }

        [Test]
        public void GetBlogEntries_Blog1()
        {
            var entries = new Mod.EntryManager().GetBlogEntries(m_blog1, int.MaxValue, null, null, (DateTime?)null);

            Assert.That(entries.Select(x => x.ID), Is.EquivalentTo(new[]
            {
            m_entry11.ID,
            m_entry12.ID,
            m_entry13.ID
          }));
        }

        [Test]
        public void GetBlogEntries_Blog2()
        {
            var entries = new Mod.EntryManager().GetBlogEntries(m_blog2, int.MaxValue, null, null, (DateTime?)null);

            Assert.That(entries.Select(x => x.ID), Is.EquivalentTo(new[]
            {
            m_entry21.ID,
            m_entry22.ID,
            m_entry23.ID
          }));
        }

        [Test]
        public void GetBlogEntries_Blog1_WithLimit()
        {
            var entries = new Mod.EntryManager().GetBlogEntries(m_blog1, 2, null, null, (DateTime?)null);

            Assert.That(entries.Select(x => x.ID), Is.EquivalentTo(new[]
            {
            m_entry13.ID,
            m_entry12.ID
          }));
        }

        [Test]
        public void GetBlogEntries_Blog1_WithTag()
        {
            var entries = new Mod.EntryManager().GetBlogEntries(m_blog1, int.MaxValue, "tagb", null, (DateTime?)null);

            Assert.That(entries.Select(x => x.ID), Is.EquivalentTo(new[]
            {
            m_entry11.ID,
            m_entry12.ID
          }));
        }

        [Test]
        public void GetBlogEntries_TagWithSpace()
        {
            var entries = new Mod.EntryManager().GetBlogEntries(m_blog1, int.MaxValue, "tag with space", null, (DateTime?)null);

            Assert.That(entries.Select(x => x.ID), Is.EquivalentTo(new[]
            {
            m_entry13.ID
          }));
        }

        [Test]
        public void GetBlogEntries_Blog1_WithCategory()
        {
            var entries = new Mod.EntryManager().GetBlogEntries(m_blog1, int.MaxValue, null, m_category13.Name, (DateTime?)null);

            Assert.That(entries.Select(x => x.ID), Is.EquivalentTo(new[]
            {
            m_entry13.ID
          }));
        }

        [Test]
        public void GetBlogEntries_Blog1_WithLimitAndCategory()
        {
            var entries = new Mod.EntryManager().GetBlogEntries(m_blog1, 1, null, m_category12.Name, (DateTime?)null);

            Assert.That(entries.Select(x => x.ID), Is.EquivalentTo(new[]
            {
            m_entry13.ID
          }));
        }

        [Test]
        public void GetBlogEntries_Blog1_EntryItem()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetBlogEntries(m_entry11, int.MaxValue, null, null, (DateTime?)null)
                            select entry.ID).ToArray();

            //entry is part of blog with 3 items
            Assert.AreEqual(3, entryIds.Length);
        }

        [Test]
        public void GetBlogEntries_Blog1_WithZeroLimit()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetBlogEntries(m_blog1, 0, null, null, (DateTime?)null)
                            select entry.ID).ToArray();

            Assert.AreEqual(0, entryIds.Length);
        }

        [Test]
        public void GetBlogEntries_Blog1_WithNegativeLimit()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetBlogEntries(m_blog1, -7, null, null, (DateTime?)null)
                            select entry.ID).ToArray();

            Assert.AreEqual(0, entryIds.Length);
        }

        [Test]
        public void GetBlogEntries_Blog1_WithInvalidCategory()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetBlogEntries(m_blog1, int.MaxValue, null, "bler", (DateTime?)null)
                            select entry.ID).ToArray();

            Assert.AreEqual(0, entryIds.Length);
        }

        [Test]
        public void GetBlogEntries_Blog1_WithInvalidTag()
        {
            var entryIds = (from entry in new Mod.EntryManager().GetBlogEntries(m_blog1, int.MaxValue, "bler", null, (DateTime?)null)
                            select entry.ID).ToArray();

            Assert.AreEqual(0, entryIds.Length);
        }

        [Test]
        public void GetBlogEntryByCategorie_Blog2_Category1_ById()
        {
            var entries = new Mod.EntryManager().GetBlogEntryByCategorie(m_blog2.ID, m_category21.ID);

            Assert.That(entries.Select(x => x.ID), Is.EquivalentTo(new[]
            {
            m_entry21.ID,
            m_entry22.ID
          }));
        }

        [Test]
        public void GetBlogEntryByCategorie_Blog2_Category1()
        {
            var entries = new Mod.EntryManager().GetBlogEntryByCategorie(m_blog2.ID, m_category21.Name);

            Assert.That(entries.Select(x => x.ID), Is.EquivalentTo(new[]
            {
            m_entry21.ID,
            m_entry22.ID
          }));
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
            var entries = new Mod.EntryManager().GetBlogEntriesByMonthAndYear(m_blog1, 3, 2011);

            Assert.That(entries.Select(x => x.ID), Is.EquivalentTo(new[]
            {
            m_entry11.ID,
            m_entry12.ID
          }));
        }

        [Test]
        public void GetBlogEntriesByMonthAndYear_Blog1_April2011()
        {
            var entries = new Mod.EntryManager().GetBlogEntriesByMonthAndYear(m_blog1, 4, 2011);

            Assert.That(entries.Select(x => x.ID), Is.EquivalentTo(new[]
            {
            m_entry13.ID
          }));
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
                using (new EventDisabler())
                {
                    m_testRoot.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\entries in order.xml")), false, PasteMode.Overwrite);
                }

            }

            // rebuild the WeBlog search index (or the entry manager won't work)
            var index = ContentSearchManager.GetIndex(Settings.SearchIndexName);
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
                using (new EventDisabler())
                {
                    m_testRoot.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\entries reverse order.xml")), false, PasteMode.Overwrite);
                }
            }

            // rebuild the WeBlog search index (or the entry manager won't work)
            var index = ContentSearchManager.GetIndex(Settings.SearchIndexName);
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
                using (new EventDisabler())
                {
                    m_testRoot.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\entries out of order.xml")), false, PasteMode.Overwrite);
                }
            }

            // rebuild the WeBlog search index (or the entry manager won't work)
            var index = ContentSearchManager.GetIndex(Settings.SearchIndexName);
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
#if FEATURE_XDB
            var reportProvider = CreateMockReportDataProvider(new[] { m_entry13.ID, m_entry11.ID, m_entry12.ID });
            var manager = new Mod.EntryManager(reportProvider);
#else
            var manager = new Mod.EntryManager();
#endif

            var entryIds = (from entry in manager.GetPopularEntriesByComment(m_blog1, int.MaxValue)
                            select entry.ID).ToArray();

            Assert.AreEqual(3, entryIds.Length);
            Assert.AreEqual(m_entry12.ID, entryIds[0]);
            Assert.AreEqual(m_entry13.ID, entryIds[1]);
            Assert.AreEqual(m_entry11.ID, entryIds[2]);
        }

        [Test]
        public void GetPopularEntriesByComment_ValidItem_Limited()
        {
#if FEATURE_XDB
            var reportProvider = CreateMockReportDataProvider(new[] { m_entry13.ID, m_entry11.ID, m_entry12.ID });
            var manager = new Mod.EntryManager(reportProvider);
#else
            var manager = new Mod.EntryManager();
#endif

            var entryIds = (from entry in manager.GetPopularEntriesByComment(m_blog1, 2)
                            select entry.ID).ToArray();

            Assert.AreEqual(2, entryIds.Length);
            Assert.AreEqual(m_entry12.ID, entryIds[0]);
            Assert.AreEqual(m_entry13.ID, entryIds[1]);
        }

        [Test]
        public void GetPopularEntriesByComment_InvalidItem()
        {
#if FEATURE_XDB
            var reportProvider = CreateMockReportDataProvider(new ID[0]);
            var manager = new Mod.EntryManager(reportProvider);
#else
            var manager = new Mod.EntryManager();
#endif

            var entryIds = (from entry in manager.GetPopularEntriesByComment(m_entry12, int.MaxValue)
                            select entry.ID).ToArray();

            Assert.AreEqual(1, entryIds.Length);
            Assert.AreEqual(m_entry12.ID, entryIds[0]);
        }

        [Test]
        public void GetPopularEntriesByComment_NullItem()
        {
#if FEATURE_XDB
            var reportProvider = CreateMockReportDataProvider(new ID[0]);
            var manager = new Mod.EntryManager(reportProvider);
#else
            var manager = new Mod.EntryManager();
#endif

            var entryIds = (from entry in manager.GetPopularEntriesByComment(null, int.MaxValue)
                            select entry.ID).ToArray();

            Assert.AreEqual(0, entryIds.Length);
        }

        [Test]
        public void GetPopularEntriesByView_ValidItem()
        {
            VerifyAnalyticsSetup();

#if FEATURE_XDB
            var reportProvider = CreateMockReportDataProvider(new[] { m_entry13.ID, m_entry11.ID, m_entry12.ID });
            var manager = new Mod.EntryManager(reportProvider);
#else
            var manager = new Mod.EntryManager();
#endif

            var entryIds = (from entry in manager.GetPopularEntriesByView(m_blog1, int.MaxValue)
                            select entry.ID).ToArray();

            Assert.AreEqual(3, entryIds.Length);
            Assert.AreEqual(m_entry13.ID, entryIds[0]);
            Assert.AreEqual(m_entry11.ID, entryIds[1]);
            Assert.AreEqual(m_entry12.ID, entryIds[2]);
        }

        [Test]
        public void GetPopularEntriesByView_ValidItem_Limited()
        {
            VerifyAnalyticsSetup();

#if FEATURE_XDB
            var reportProvider = CreateMockReportDataProvider(new[] { m_entry13.ID, m_entry11.ID, m_entry12.ID });
            var manager = new Mod.EntryManager(reportProvider);
#else
            var manager = new Mod.EntryManager();
#endif

            var entryIds = (from entry in manager.GetPopularEntriesByView(m_blog1, 1)
                            select entry.ID).ToArray();

            Assert.AreEqual(1, entryIds.Length);
            Assert.AreEqual(m_entry13.ID, entryIds[0]);
        }

        [Test]
        public void GetPopularEntriesByView_InvalidItem()
        {
            VerifyAnalyticsSetup();

#if FEATURE_XDB
            var reportProvider = CreateMockReportDataProvider(new ID[0]);
            var manager = new Mod.EntryManager(reportProvider);
#else
            var manager = new Mod.EntryManager();
#endif

            var entryIds = (from entry in manager.GetPopularEntriesByView(m_testRoot, int.MaxValue)
                            select entry.ID).ToArray();

            Assert.AreEqual(0, entryIds.Length);
        }

        [Test]
        public void GetPopularEntriesByView_NullItem()
        {
            VerifyAnalyticsSetup();

#if FEATURE_XDB
            var reportProvider = CreateMockReportDataProvider(new ID[0]);
            var manager = new Mod.EntryManager(reportProvider);
#else
            var manager = new Mod.EntryManager();
#endif

            var entryIds = (from entry in manager.GetPopularEntriesByView(null, 1)
                            select entry.ID).ToArray();

            Assert.AreEqual(0, entryIds.Length);
        }

        // TODO: Write tests for methods accepting language

#if FEATURE_XDB
        protected ReportDataProviderBase CreateMockReportDataProvider(IEnumerable<ID> ids)
        {
            var dataTable = new DataTable();
            dataTable.Columns.AddRange(new[]
            {
          new DataColumn("ItemId", typeof(Guid))
        });

            foreach (var id in ids)
            {
                dataTable.Rows.Add(id.Guid);
            }

            var reportingProvider = Mock.Of<ReportDataProviderBase>(x =>
              x.GetData(It.IsAny<string>(), It.IsAny<ReportDataQuery>(), It.IsAny<CachingPolicy>()) == new ReportDataResponse(() => dataTable));

            return reportingProvider;
        }

        private bool Blah(ReportDataQuery a)
        {
            var b = 0;
            return false;
        }
#endif
    }
}