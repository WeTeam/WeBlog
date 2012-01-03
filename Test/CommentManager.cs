using System;
using System.Linq;
using NUnit.Framework;
using Sitecore.SecurityModel;
using Sitecore.Data.Items;
using System.IO;
using System.Web;
using Sitecore;
using Mod = Sitecore.Modules.WeBlog.Managers;
using Sitecore.Data;
using Sitecore.Search;

namespace Sitecore.Modules.WeBlog.Test
{
    [TestFixture]
    [Category("CommentManager")]
    public class CommentManager
    {
        private string FOLDER_TEMPLATE = "common/folder";
        private readonly string m_commentTemplateId;

        private Item m_testRoot = null;
        private Item m_blog1 = null;
        private Item m_entry11 = null;
        private Item m_comment111 = null;
        private Item m_comment112 = null;
        private Item m_comment113 = null;
        private Item m_entry12 = null;
        private Item m_comment121 = null;
        private Item m_comment122 = null;
        private Item m_blog2 = null;
        private Item m_entry21 = null;
        private Item m_comment211 = null;
        private Item m_comment212 = null;

        public CommentManager()
        {
            m_commentTemplateId = Sitecore.Configuration.Settings.GetSetting("WeBlog.CommentTemplateID");
        }

        [SetUp]
        public void SetUp()
        {
            // Create test content
            var home = Sitecore.Context.Database.GetItem("/sitecore/content/home");
            using (new SecurityDisabler())
            {
                home.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\comment manager content.xml")), false, PasteMode.Overwrite);
            }

            // Retrieve created content items
            m_testRoot = home.Axes.GetChild("blog test root");
            m_blog1 = m_testRoot.Axes.GetChild("blog1");
            m_blog2 = m_testRoot.Axes.GetChild("blog2");

            m_entry11 = m_blog1.Axes.GetDescendant("Entry1");
            m_comment111 = m_entry11.Axes.GetDescendant("Comment1");
            m_comment112 = m_entry11.Axes.GetDescendant("Comment2");
            m_comment113 = m_entry11.Axes.GetDescendant("Comment3");

            m_entry12 = m_blog1.Axes.GetDescendant("Entry2");
            m_comment121 = m_entry12.Axes.GetDescendant("Comment4");
            m_comment122 = m_entry12.Axes.GetDescendant("Comment5");

            m_entry21 = m_blog2.Axes.GetDescendant("Entry1");
            m_comment211 = m_entry21.Axes.GetDescendant("Comment1");
            m_comment212 = m_entry21.Axes.GetDescendant("Comment2");

            // rebuild the WeBlog search index (or the comment manager won't work)
            var index = SearchManager.GetIndex(Settings.SearchIndexName);
            index.Rebuild();
        }

        [TearDown]
        public void TearDown()
        {
            using (new SecurityDisabler())
            {
                if (m_testRoot != null)
                    m_testRoot.Delete();
            }
        }

        [Test]
        public void MakeSortedCommentsList_InOrder()
        {
            using (new SecurityDisabler())
            {
                m_testRoot.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\comments in order.xml")), false, PasteMode.Overwrite);
            }

            var commentFolder = m_testRoot.Axes.GetChild("comments in order");

            try
            {
                var sorted = new Mod.CommentManager().MakeSortedCommentsList(commentFolder.Axes.GetDescendants().Where(i => i.TemplateID.ToString() == m_commentTemplateId).ToArray());
                Assert.AreEqual(3, sorted.Length);
                Assert.AreEqual("Comment1", sorted[0].InnerItem.Name);
                Assert.AreEqual("Comment2", sorted[1].InnerItem.Name);
                Assert.AreEqual("Comment3", sorted[2].InnerItem.Name);
            }
            finally
            {
                if (commentFolder != null)
                {
                    using (new SecurityDisabler())
                    {
                        commentFolder.Delete();
                    }
                }
            }
        }

        [Test]
        public void MakeSortedCommentsList_ReverseOrder()
        {
            using (new SecurityDisabler())
            {
                m_testRoot.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\comments reverse order.xml")), false, PasteMode.Overwrite);
            }

            var commentFolder = m_testRoot.Axes.GetChild("comments reverse order");

            try
            {
                var sorted = new Mod.CommentManager().MakeSortedCommentsList(commentFolder.Axes.GetDescendants().Where(i => i.TemplateID.ToString() == m_commentTemplateId).ToArray());
                Assert.AreEqual(3, sorted.Length);
                Assert.AreEqual("Comment3", sorted[0].InnerItem.Name);
                Assert.AreEqual("Comment2", sorted[1].InnerItem.Name);
                Assert.AreEqual("Comment1", sorted[2].InnerItem.Name);
            }
            finally
            {
                if (commentFolder != null)
                {
                    using (new SecurityDisabler())
                    {
                        commentFolder.Delete();
                    }
                }
            }
        }

        [Test]
        public void MakeSortedCommentsList_OutOfOrder()
        {
            using (new SecurityDisabler())
            {
                m_testRoot.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\comments out of order.xml")), false, PasteMode.Overwrite);
            }

            var commentFolder = m_testRoot.Axes.GetChild("comments out of order");

            try
            {
                var sorted = new Mod.CommentManager().MakeSortedCommentsList(commentFolder.Axes.GetDescendants().Where(i => i.TemplateID.ToString() == m_commentTemplateId).ToArray());
                Assert.AreEqual(4, sorted.Length);
                Assert.AreEqual("Comment3", sorted[0].InnerItem.Name);
                Assert.AreEqual("Comment1", sorted[1].InnerItem.Name);
                Assert.AreEqual("Comment4", sorted[2].InnerItem.Name);
                Assert.AreEqual("Comment2", sorted[3].InnerItem.Name);
            }
            finally
            {
                if (commentFolder != null)
                {
                    using (new SecurityDisabler())
                    {
                        commentFolder.Delete();
                    }
                }
            }
        }

        [Test]
        public void MakeSortedCommentsList_WithNonComment()
        {
            Item commentFolder = null;

            using (new SecurityDisabler())
            {
                m_testRoot.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\comments in order.xml")), false, PasteMode.Overwrite);
                commentFolder = m_testRoot.Axes.GetChild("comments in order");

                var folderTemplate = Sitecore.Context.Database.GetTemplate(FOLDER_TEMPLATE);
                commentFolder.Add("non comment", folderTemplate);
            }

            try
            {
                var sorted = new Mod.CommentManager().MakeSortedCommentsList(commentFolder.Axes.GetDescendants().Where(i => i.TemplateID.ToString() == m_commentTemplateId).ToArray());
                Assert.AreEqual(3, sorted.Length);
                Assert.AreEqual("Comment1", sorted[0].InnerItem.Name);
                Assert.AreEqual("Comment2", sorted[1].InnerItem.Name);
                Assert.AreEqual("Comment3", sorted[2].InnerItem.Name);
            }
            finally
            {
                if (commentFolder != null)
                {
                    using (new SecurityDisabler())
                    {
                        commentFolder.Delete();
                    }
                }
            }
        }

        [Test]
        public void GetCommentsCount_Null()
        {
            Assert.AreEqual(0, new Mod.CommentManager().GetCommentsCount((Item)null));
        }

        [Test]
        public void GetCommentsCount_Entry11()
        {
            Assert.AreEqual(3, new Mod.CommentManager().GetCommentsCount(m_entry11));
        }

        [Test]
        public void GetCommentsCount_Entry11_ById()
        {
            Assert.AreEqual(3, new Mod.CommentManager().GetCommentsCount(m_entry11.ID));
        }

        [Test]
        public void GetCommentsCount_InvalidId()
        {
            Assert.AreEqual(0, new Mod.CommentManager().GetCommentsCount(ID.NewID));
        }

        [Test]
        public void GetCommentsCount_Entry12()
        {
            Assert.AreEqual(2, new Mod.CommentManager().GetCommentsCount(m_entry12));
        }

        [Test]
        public void GetCommentsCount_Entry12_ById()
        {
            Assert.AreEqual(2, new Mod.CommentManager().GetCommentsCount(m_entry12.ID));
        }

        [Test]
        public void GetCommentsCount_Entry21()
        {
            Assert.AreEqual(2, new Mod.CommentManager().GetCommentsCount(m_entry21));
        }

        [Test]
        public void GetCommentsCount_Entry21_ById()
        {
            Assert.AreEqual(2, new Mod.CommentManager().GetCommentsCount(m_entry21.ID));
        }

        [Test]
        public void GetCommentsByBlog_NullId()
        {
            Assert.AreEqual(0, new Mod.CommentManager().GetCommentsByBlog((ID)null, int.MaxValue).Length);
        }

        [Test]
        public void GetCommentsByBlog_InvalidId()
        {
            Assert.AreEqual(0, new Mod.CommentManager().GetCommentsByBlog(ID.NewID, int.MaxValue).Length);
        }

        [Test]
        public void GetCommentsByBlog_Null()
        {
            Assert.AreEqual(0, new Mod.CommentManager().GetCommentsByBlog((Item)null, int.MaxValue).Length);
        }

        [Test]
        public void GetCommentsByBlog_Blog1_NoLimit()
        {
            var comments = new Mod.CommentManager().GetCommentsByBlog(m_blog1, int.MaxValue);
            Assert.AreEqual(5, comments.Length);

            var ids = (from comment in comments
                       select comment.ID).ToArray();

            Assert.Contains(m_comment111.ID, ids);
            Assert.Contains(m_comment112.ID, ids);
            Assert.Contains(m_comment113.ID, ids);
            Assert.Contains(m_comment121.ID, ids);
            Assert.Contains(m_comment122.ID, ids);
        }

        [Test]
        public void GetCommentsByBlog_Blog1_NoLimit_ByID()
        {
            var comments = new Mod.CommentManager().GetCommentsByBlog(m_blog1.ID, int.MaxValue);
            Assert.AreEqual(5, comments.Length);

            var ids = (from comment in comments
                       select comment.ID).ToArray();

            Assert.Contains(m_comment111.ID, ids);
            Assert.Contains(m_comment112.ID, ids);
            Assert.Contains(m_comment113.ID, ids);
            Assert.Contains(m_comment121.ID, ids);
            Assert.Contains(m_comment122.ID, ids);
        }

        [Test]
        public void GetCommentsByBlog_Blog2_NoLimit()
        {
            var comments = new Mod.CommentManager().GetCommentsByBlog(m_blog2, int.MaxValue);
            Assert.AreEqual(2, comments.Length);

            var ids = (from comment in comments
                       select comment.ID).ToArray();

            Assert.Contains(m_comment211.ID, ids);
            Assert.Contains(m_comment212.ID, ids);
        }

        [Test]
        public void GetCommentsByBlog_Blog1_Limited()
        {
            var comments = new Mod.CommentManager().GetCommentsByBlog(m_blog1, 4);
            Assert.AreEqual(4, comments.Length);

            var ids = (from comment in comments
                       select comment.ID).ToArray();

            Assert.Contains(m_comment122.ID, ids);
            Assert.Contains(m_comment121.ID, ids);
            Assert.Contains(m_comment113.ID, ids);
            Assert.Contains(m_comment112.ID, ids);
        }

        [Test]
        public void GetCommentsByBlog_Blog1_Limited_ById()
        {
            var comments = new Mod.CommentManager().GetCommentsByBlog(m_blog1.ID, 4);
            Assert.AreEqual(4, comments.Length);

            var ids = (from comment in comments
                       select comment.ID).ToArray();

            Assert.Contains(m_comment122.ID, ids);
            Assert.Contains(m_comment121.ID, ids);
            Assert.Contains(m_comment113.ID, ids);
            Assert.Contains(m_comment112.ID, ids);
        }

        [Test]
        public void GetCommentsByBlog_Blog1_LimitZero()
        {
            var comments = new Mod.CommentManager().GetCommentsByBlog(m_blog1.ID, 0);
            Assert.AreEqual(0, comments.Length);
        }

        [Test]
        public void GetCommentsByBlog_Blog1_NegativeLimit()
        {
            var comments = new Mod.CommentManager().GetCommentsByBlog(m_blog1.ID, -7);
            Assert.AreEqual(0, comments.Length);
        }

        [Test]
        public void GetEntryComments_Entry11()
        {
            var comments = new Mod.CommentManager().GetEntryComments(m_entry11);
            Assert.AreEqual(3, comments.Length);

            var ids = (from comment in comments
                       select comment.ID).ToArray();

            Assert.Contains(m_comment111.ID, ids);
            Assert.Contains(m_comment112.ID, ids);
            Assert.Contains(m_comment113.ID, ids);
        }

        [Test]
        public void GetEntryComments_Entry21()
        {
            var comments = new Mod.CommentManager().GetEntryComments(m_entry21);
            Assert.AreEqual(2, comments.Length);

            var ids = (from comment in comments
                       select comment.ID).ToArray();

            Assert.Contains(m_comment211.ID, ids);
            Assert.Contains(m_comment212.ID, ids);
        }

        [Test]
        public void GetEntryComments_Null()
        {
            var comments = new Mod.CommentManager().GetEntryComments(null);
            Assert.AreEqual(0, comments.Length);
        }

        [Test]
        public void GetEntryComments_BlogItem()
        {
            var comments = new Mod.CommentManager().GetEntryComments(m_blog1);
            Assert.AreEqual(0, comments.Length);
        }

        [Test]
        public void GetEntryComments_Entry11_WithLimit()
        {
            var comments = new Mod.CommentManager().GetEntryComments(m_entry11, 2);
            Assert.AreEqual(2, comments.Length);

            var ids = (from comment in comments
                       select comment.ID).ToArray();

            Assert.Contains(m_comment113.ID, ids);
            Assert.Contains(m_comment112.ID, ids);
        }

        [Test]
        public void GetEntryComments_Entry11_LimitZero()
        {
            var comments = new Mod.CommentManager().GetEntryComments(m_entry11, 0);
            Assert.AreEqual(0, comments.Length);
        }

        [Test]
        public void GetEntryComments_Entry11_NegativeLimit()
        {
            var comments = new Mod.CommentManager().GetEntryComments(m_entry11, -4);
            Assert.AreEqual(0, comments.Length);
        }

        [Test]
        public void AddCommentToEntry()
        {
            // Perform this test in master DB as comments get created there
            var db = Sitecore.Configuration.Factory.GetDatabase("master");
            var home = db.GetItem("/sitecore/content/home");
            using (new SecurityDisabler())
            {
                home.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\comment manager content.xml")), false, PasteMode.Overwrite);
            }

            // Retrieve created content items
            var testRoot = home.Axes.GetChild("blog test root");
            var blog1 = m_testRoot.Axes.GetChild("blog1");
            var entry12 = m_blog1.Axes.GetDescendant("Entry2");

            var originalCount = entry12.Axes.GetDescendants().Count(i => i.TemplateID.ToString() == m_commentTemplateId);
            ID commentId = null;

            try
            {
                var comment = new Sitecore.Modules.WeBlog.Model.Comment()
                {
                    AuthorEmail = "a@b.com",
                    AuthorName = "commentor",
                    Text = "My Comment"
                };

                comment.Fields[Sitecore.Modules.WeBlog.Constants.Fields.IpAddress] = "127.0.0.1";
                comment.Fields[Sitecore.Modules.WeBlog.Constants.Fields.Website] = "website";

                commentId = new Mod.CommentManager().AddCommentToEntry(m_entry12.ID, comment);
                var childCount = m_entry12.Axes.GetDescendants().Count(i => i.TemplateID.ToString() == m_commentTemplateId);

                Assert.IsTrue(commentId != ID.Null);
                Assert.AreEqual(originalCount + 1, childCount);

                var commentItem = db.GetItem(commentId);
                Assert.IsNotNull(commentItem);

                var commentAsComment = new Sitecore.Modules.WeBlog.Items.WeBlog.CommentItem(commentItem);
                Assert.AreEqual("a@b.com", commentAsComment.Email.Text);
                Assert.AreEqual("commentor", commentAsComment.Name.Text);
                Assert.AreEqual("127.0.0.1", commentAsComment.IPAddress.Text);
                Assert.AreEqual("website", commentAsComment.Website.Text);
                Assert.AreEqual("My Comment", StringUtil.RemoveTags(commentAsComment.Comment.Text));
            }
            finally
            {
                var webDb = Sitecore.Configuration.Factory.GetDatabase("web");

                if (testRoot != null)
                {
                    using (new SecurityDisabler())
                    {
                        testRoot.Delete();
                    }
                }

                if (commentId != (ID)null)
                {
                    var commentItem = webDb.GetItem(commentId);
                    if (commentItem != null)
                    {
                        using (new SecurityDisabler())
                        {
                            commentItem.Delete();
                        }
                    }
                }
            }
        }
    }
}