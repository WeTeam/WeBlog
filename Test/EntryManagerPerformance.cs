using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NUnit.Framework;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using Sitecore.Data;
using Mod = Sitecore.Modules.WeBlog.Managers;
using Sitecore.Data.Fields;
using Sitecore.Search;
using Sitecore.Publishing;
using System.Threading;
using Sitecore.Jobs;

namespace Sitecore.Modules.WeBlog.Test
{
    [TestFixture]
    [Category("EntryManagerPerformance")]
    public class EntryManagerPerformance
    {
        private const int SMALL_COUNT = 50;
        private const int MEDIUM_COUNT = 300;
        private const int LARGE_COUNT = 2000;
        private const string TAG_A = "tag a";
        private const string TAG_B = "tag b";

        private Item m_testRoot = null;
        private Item m_smallTree = null;
        private Item m_mediumTree = null;
        private Item m_largeTree = null;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var db = GetDatabase();
            var home = db.GetItem("/sitecore/content/home");

            var folderTemplate = db.GetTemplate(Constants.FolderTemplate);

            using (new SecurityDisabler())
            {
                m_testRoot = home.Add("test root " + DateUtil.ToIsoDate(DateTime.Now), folderTemplate);

                m_smallTree = CreateTree(m_testRoot, "small", SMALL_COUNT);
                m_mediumTree = CreateTree(m_testRoot, "medium", MEDIUM_COUNT);
                m_largeTree = CreateTree(m_testRoot, "large", LARGE_COUNT);
            }

            // publish content so it gets indexed
            var targetDb = GetLiveDatabase();
            var handle = PublishManager.PublishItem(m_testRoot, new Database[] { targetDb }, targetDb.Languages, true, false);
            var job = JobManager.GetJob(handle);

            var start = DateTime.Now;
            while (!job.IsDone)
            {
                if (DateTime.Now - start > new TimeSpan(0, 2, 0))
                    break;
            }

            // rebuild the WeBlog search index (or the entry manager won't work)
            // publishing will update the index, but perhaps not as quickly as we need here
            var index = SearchManager.GetIndex(Sitecore.Modules.WeBlog.Constants.Index.Name);
            index.Rebuild();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            if (m_testRoot != null)
            {
                using (new SecurityDisabler())
                {
                    var parent = m_testRoot.Parent;
                    m_testRoot.Delete();

                    var targetDb = GetLiveDatabase();
                    var handle = PublishManager.PublishItem(parent, new Database[] { targetDb }, targetDb.Languages, true, false);
                    var job = JobManager.GetJob(handle);

                    var start = DateTime.Now;
                    while (!job.IsDone)
                    {
                        if (DateTime.Now - start > new TimeSpan(0, 2, 0))
                            break;
                    }
                }
            }
        }

        [Test]
        public void AllEntries_Small()
        {
            var entries = Mod.EntryManager.GetBlogEntries(m_smallTree);
            Assert.AreEqual(SMALL_COUNT, entries.Length);
        }

        [Test]
        public void AllEntries_Medium()
        {
            var entries = Mod.EntryManager.GetBlogEntries(m_mediumTree);
            Assert.AreEqual(MEDIUM_COUNT, entries.Length);
        }

        [Test]
        public void AllEntries_Large()
        {
            var entries = Mod.EntryManager.GetBlogEntries(m_largeTree);
            Assert.AreEqual(LARGE_COUNT, entries.Length);
        }

        [Test]
        public void LimitedEntries_Small()
        {
            var entries = Mod.EntryManager.GetBlogEntries(m_smallTree.ID, GetDatabase(), 40);
            Assert.AreEqual(40, entries.Length);
        }

        [Test]
        public void LimitedEntries_Medium()
        {
            var entries = Mod.EntryManager.GetBlogEntries(m_mediumTree.ID, GetDatabase(), 40);
            Assert.AreEqual(40, entries.Length);
        }

        [Test]
        public void LimitedEntries_Large()
        {
            var entries = Mod.EntryManager.GetBlogEntries(m_largeTree.ID, GetDatabase(), 40);
            Assert.AreEqual(40, entries.Length);
        }

        [Test]
        public void EntriesByTagA_Small()
        {
            var entries = Mod.EntryManager.GetBlogEntries(m_smallTree.ID, GetDatabase(), int.MaxValue, TAG_A);
            Assert.AreEqual(17, entries.Length);
        }

        [Test]
        public void EntriesByTagA_Medium()
        {
            var entries = Mod.EntryManager.GetBlogEntries(m_mediumTree.ID, GetDatabase(), int.MaxValue, TAG_A);
            Assert.AreEqual(100, entries.Length);
        }

        [Test]
        public void EntriesByTagA_Large()
        {
            var entries = Mod.EntryManager.GetBlogEntries(m_largeTree.ID, GetDatabase(), int.MaxValue, TAG_A);
            Assert.AreEqual(667, entries.Length);
        }

        [Test]
        public void EntriesByTagB_Small()
        {
            var entries = Mod.EntryManager.GetBlogEntries(m_smallTree.ID, GetDatabase(), int.MaxValue, TAG_B);
            Assert.AreEqual(8, entries.Length);
        }

        [Test]
        public void EntriesByTagB_Medium()
        {
            var entries = Mod.EntryManager.GetBlogEntries(m_mediumTree.ID, GetDatabase(), int.MaxValue, TAG_B);
            Assert.AreEqual(43, entries.Length);
        }

        [Test]
        public void EntriesByTagB_Large()
        {
            var entries = Mod.EntryManager.GetBlogEntries(m_largeTree.ID, GetDatabase(), int.MaxValue, TAG_B);
            Assert.AreEqual(286, entries.Length);
        }

        [Test]
        public void EntriesByCategory1_Small()
        {
            var entries = Mod.EntryManager.GetBlogEntries(m_smallTree, int.MaxValue, string.Empty, "category 1");
            Assert.AreEqual(25, entries.Length);
        }

        [Test]
        public void EntriesByCategory1_Medium()
        {
            var entries = Mod.EntryManager.GetBlogEntries(m_mediumTree, int.MaxValue, string.Empty, "category 1");
            Assert.AreEqual(150, entries.Length);
        }

        [Test]
        public void EntriesByCategory1_Large()
        {
            var entries = Mod.EntryManager.GetBlogEntries(m_largeTree, int.MaxValue, string.Empty, "category 1");
            Assert.AreEqual(1000, entries.Length);
        }

        [Test]
        public void EntriesByCategory2_Small()
        {
            var entries = Mod.EntryManager.GetBlogEntries(m_smallTree, int.MaxValue, string.Empty, "category 2");
            Assert.AreEqual(13, entries.Length);
        }

        [Test]
        public void EntriesByCategory2_Medium()
        {
            var entries = Mod.EntryManager.GetBlogEntries(m_mediumTree, int.MaxValue, string.Empty, "category 2");
            Assert.AreEqual(75, entries.Length);
        }

        [Test]
        public void EntriesByCategory2_Large()
        {
            var entries = Mod.EntryManager.GetBlogEntries(m_largeTree, int.MaxValue, string.Empty, "category 2");
            Assert.AreEqual(500, entries.Length);
        }

        private Item CreateTree(Item parent, string name, int entryCount)
        {
            var db = parent.Database;
            var branchName = Sitecore.Configuration.Settings.GetSetting("Blog.BlogBranchTemplateID");
            var branchTemplate = (BranchItem)db.GetItem(branchName);

            var categoryTemplate = db.GetTemplate(Constants.CategoryTemplate);

            var blog = parent.Add(name, branchTemplate);
            var startDate = DateTime.Now;

            var categoriesRoot = blog.Axes.GetChild("categories");
            var category1 = categoriesRoot.Add("category 1", categoryTemplate);
            var category2 = categoriesRoot.Add("category 2", categoryTemplate);

            for (int i = 0; i < entryCount; i++)
            {
                startDate = startDate.AddDays(-2);
                var dateString = Sitecore.DateUtil.ToIsoDate(startDate);
                var xml = string.Format(Constants.ItemXmlTemplate, i, dateString, dateString);

                var entry = blog.PasteItem(xml, true, PasteMode.Undefined);

                if (i % 3 == 0)
                {
                    entry.Editing.BeginEdit();
                    entry["tags"] += "," + TAG_A;
                    entry.Editing.EndEdit();
                }

                if (i % 7 == 0)
                {
                    entry.Editing.BeginEdit();
                    entry["tags"] += "," + TAG_B;
                    entry.Editing.EndEdit();
                }

                if (i % 2 == 0)
                {
                    entry.Editing.BeginEdit();
                    ((MultilistField)entry.Fields["category"]).Add(category1.ID.ToString());
                    entry.Editing.EndEdit();
                }

                if (i % 4 == 0)
                {
                    entry.Editing.BeginEdit();
                    ((MultilistField)entry.Fields["category"]).Add(category2.ID.ToString());
                    entry.Editing.EndEdit();
                }
            }

            return blog;
        }

        private Database GetDatabase()
        {
            // Data needs to be created in master so news mover event handler will run
            return Sitecore.Configuration.Factory.GetDatabase("master");
        }

        private Database GetLiveDatabase()
        {
            return Sitecore.Configuration.Factory.GetDatabase("web");
        }
    }
}