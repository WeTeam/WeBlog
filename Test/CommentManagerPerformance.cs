using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NUnit.Framework;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.Search;
using Sitecore.Jobs;
using Sitecore.Publishing;
using Sitecore.SecurityModel;
using Mod = Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Test
{
    [TestFixture]
    [Category("CommentManagerPerformance")]
    public class CommentManagerPerformance
    {
        private const int SMALL_COUNT = 50;
        private const int MEDIUM_COUNT = 300;
        private const int LARGE_COUNT = 2000;

        private const int SMALL_SPECIFIC_ENTRY = 27;
        private const int MEDIUM_SPECIFIC_ENTRY = 143;
        private const int LARGE_SPECIFIC_ENTRY = 1786;
        
        // values controlling the number of comments to create per entry
        private const int MOD = 6;
        private const int MULTIPLIER = 3;

        private Item m_testRoot = null;
        private Item m_smallTree = null;
        private Item m_mediumTree = null;
        private Item m_largeTree = null;

        // Counts of items created to test against
        private int m_smallTreeCommentCount = 0;
        private int m_mediumTreeCommentCount = 0;
        private int m_largeTreeCommentCount = 0;

        private int m_smallTreeCommentCountForEntry = 0;
        private int m_mediumTreeCommentCountForEntry = 0;
        private int m_largeTreeCommentCountForEntry = 0;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var db = GetDatabase();
            var home = db.GetItem("/sitecore/content/home");

            var folderTemplate = db.GetTemplate(Constants.FolderTemplate);

            using (new SecurityDisabler())
            {
                m_testRoot = home.Add("test root " + DateUtil.ToIsoDate(DateTime.Now), folderTemplate);

               // m_smallTree = CreateTree(m_testRoot, "small", SMALL_COUNT, out m_smallTreeCommentCount, SMALL_SPECIFIC_ENTRY, out m_smallTreeCommentCountForEntry);
                m_mediumTree = CreateTree(m_testRoot, "medium", MEDIUM_COUNT, out m_mediumTreeCommentCount, MEDIUM_SPECIFIC_ENTRY, out m_mediumTreeCommentCountForEntry);
               // m_largeTree = CreateTree(m_testRoot, "large", LARGE_COUNT, out m_largeTreeCommentCount, LARGE_SPECIFIC_ENTRY, out m_largeTreeCommentCountForEntry);
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

            // rebuild the WeBlog search index (or the comment manager won't work)
            // publishing will update the index, but perhaps not as quickly as we need here
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
        public void GetCommentForEntry_Small()
        {
            var entry = m_smallTree.Axes.GetDescendant("Entry" + SMALL_SPECIFIC_ENTRY.ToString());
            var comments = new Mod.CommentManager().GetEntryComments(entry);
            Assert.AreEqual(m_smallTreeCommentCountForEntry, comments.Length);
        }

        [Test]
        public void GetCommentForEntry_Medium()
        {
            var entry = m_mediumTree.Axes.GetDescendant("Entry" + MEDIUM_SPECIFIC_ENTRY.ToString());
            var comments = new Mod.CommentManager().GetEntryComments(entry);
            Assert.AreEqual(m_mediumTreeCommentCountForEntry, comments.Length);
        }

        [Test]
        public void GetCommentForEntry_Large()
        {
            var entry = m_largeTree.Axes.GetDescendant("Entry" + LARGE_SPECIFIC_ENTRY.ToString());
            var comments = new Mod.CommentManager().GetEntryComments(entry);
            Assert.AreEqual(m_largeTreeCommentCountForEntry, comments.Length);
        }

        [Test]
        public void GetCommentCount_Small()
        {
            var entry = m_smallTree.Axes.GetDescendant("Entry" + SMALL_SPECIFIC_ENTRY.ToString());
            var commentCount = new Mod.CommentManager().GetCommentsCount(entry);
            Assert.AreEqual(m_smallTreeCommentCountForEntry, commentCount);
        }

        [Test]
        public void GetCommentCount_Medium()
        {
            var entry = m_mediumTree.Axes.GetDescendant("Entry" + MEDIUM_SPECIFIC_ENTRY.ToString());
            var commentCount = new Mod.CommentManager().GetCommentsCount(entry);
            Assert.AreEqual(m_mediumTreeCommentCountForEntry, commentCount);
        }

        [Test]
        public void GetCommentCount_Large()
        {
            var entry = m_largeTree.Axes.GetDescendant("Entry" + LARGE_SPECIFIC_ENTRY.ToString());
            var commentCount = new Mod.CommentManager().GetCommentsCount(entry);
            Assert.AreEqual(m_largeTreeCommentCountForEntry, commentCount);
        }

        [Test]
        public void GetCommentsByBlog_Small()
        {
            var comments = new Mod.CommentManager().GetCommentsByBlog(m_smallTree, int.MaxValue);
            Assert.AreEqual(m_smallTreeCommentCount, comments.Length);
        }

        [Test]
        public void GetCommentsByBlog_Medium()
        {
            //System.Threading.Thread.Sleep(5000);
            var comments = new Mod.CommentManager().GetCommentsByBlog(m_mediumTree, int.MaxValue);
            Assert.AreEqual(m_mediumTreeCommentCount, comments.Length);
        }

        [Test]
        public void GetCommentsByBlog_Large()
        {
            var comments = new Mod.CommentManager().GetCommentsByBlog(m_largeTree, int.MaxValue);
            Assert.AreEqual(m_largeTreeCommentCount, comments.Length);
        }

        private int CalculateRound(int stop)
        {
            return CalculateRound(0, stop);
        }

        private int CalculateRound(int num, int stop)
        {
            if (num <= stop)
                return (num * MULTIPLIER) + CalculateRound(num + 1, stop);

            return 0;
        }

        private Item CreateTree(Item parent, string name, int entryCount, out int commentCount, int specificEntry, out int commentCountForEntry)
        {
            commentCount = 0;
            commentCountForEntry = 0;

            var db = parent.Database;
            var branchName = Sitecore.Configuration.Settings.GetSetting("Blog.BlogBranchTemplateID");
            var branchTemplate = (BranchItem)db.GetItem(branchName);

            var blog = parent.Add(name, branchTemplate);
            var entryStartDate = DateTime.Now;

            for (var i = 0; i < entryCount; i++)
            {
                entryStartDate = entryStartDate.AddDays(-2);
                var dateString = Sitecore.DateUtil.ToIsoDate(entryStartDate);
                var entryXml = string.Format(Constants.EntryItemXmlTemplate, i, dateString, dateString);

                var entry = blog.PasteItem(entryXml, true, PasteMode.Undefined);

                var commentStartDate = entryStartDate;
                var entryCommentCount = (i % MOD) * MULTIPLIER;
                for (var j = 0; j < entryCommentCount; j++)
                {
                    commentStartDate = commentStartDate.AddDays(0.4);
                    var commentDateString = Sitecore.DateUtil.ToIsoDate(commentStartDate);
                    var commentXml = string.Format(Constants.CommentItemXmlTemplate, j, commentDateString, commentDateString);
                    entry.PasteItem(commentXml, true, PasteMode.Undefined);
                    commentCount++;

                    if (i == specificEntry)
                        commentCountForEntry++;
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