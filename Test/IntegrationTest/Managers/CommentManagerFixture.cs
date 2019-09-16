using System;
using System.Linq;
using NUnit.Framework;
using Sitecore.Data.Items;
using Mod = Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.IntegrationTest.Managers
{
    [TestFixture]
    [Category("CommentManagerFixture")]
    public class CommentManagerFixture : UnitTestBase
    {
        [Test]
        public void GetEntryComments_NullItem()
        {
            var manager = new Mod.CommentManager();
            var comments = manager.GetEntryComments((Item)null, 10);

            Assert.That(comments, Is.Empty);
        }

        [Test]
        public void GetEntryComments_NonEntry()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);

            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var commetLuna1 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 3));
            var commetLuna2 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 4));

            TestUtil.UpdateIndex();

            var manager = new Mod.CommentManager();
            var comments = manager.GetEntryComments(blog, 10);

            Assert.That(comments, Is.Empty);
        }

        [Test]
        public void GetEntryComments_EntryWithComments()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);

            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var commetLuna1 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 2));
            var commetLuna2 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 4));

            TestUtil.UpdateIndex();

            var manager = new Mod.CommentManager();
            var comments = manager.GetEntryComments(entryLuna, 10);
            var ids = from comment in comments select comment.Uri.ItemID;

            Assert.That(ids, Is.EqualTo(new[] { commetLuna1.ID, commetLuna2.ID }));
        }

        [Test]
        public void GetEntryComments_EntryWithoutComments()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));

            TestUtil.UpdateIndex();

            var manager = new Mod.CommentManager();
            var comments = manager.GetEntryComments(entryLuna, 10);

            Assert.That(comments, Is.Empty);
        }

        [Test]
        public void GetEntryComments_EntryWithComments_Limited()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);

            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var commetLuna1 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 1));
            var commetLuna2 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 2));
            var commetLuna3 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 3));

            TestUtil.UpdateIndex();

            var manager = new Mod.CommentManager();
            var comments = manager.GetEntryComments(entryLuna, 2);
            var ids = from comment in comments select comment.Uri.ItemID;

            Assert.That(ids, Is.EqualTo(new[] { commetLuna1.ID, commetLuna2.ID }));
        }

        [Test]
        public void GetCommentsCount_NullItem()
        {
            var manager = new Mod.CommentManager();
            var count = manager.GetCommentsCount((Item)null);
            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public void GetCommentsCount_NonEntry()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);

            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var commetLuna1 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 2));
            var commetLuna2 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 3));

            TestUtil.UpdateIndex();

            var manager = new Mod.CommentManager();
            var count = manager.GetCommentsCount(blog);

            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public void GetCommentsCount_EntryWithComments()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);

            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var commetLuna1 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 4));
            var commetLuna2 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 5));

            TestUtil.UpdateIndex();

            var manager = new Mod.CommentManager();
            var count = manager.GetCommentsCount(entryLuna);

            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public void GetCommentsCount_EntryWithoutComments()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));

            TestUtil.UpdateIndex();

            var manager = new Mod.CommentManager();
            var count = manager.GetCommentsCount(entryLuna);

            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public void GetBlogComments_NullItem_ReturnsEmptyList()
        {
            var manager = new Mod.CommentManager();
            var comments = manager.GetBlogComments(null, 10);
            
            Assert.That(comments, Is.Empty);
        }

        [Test]
        public void GetBlogComments_NoComments_ReturnsEmptyList()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);

            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));

            TestUtil.UpdateIndex();

            var manager = new Mod.CommentManager();
            var comments = manager.GetBlogComments(blog, 10);

            Assert.That(comments, Is.Empty);
        }

        [Test]
        public void GetBlogComments_CommentsInBlog_ReturnsComments()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);

            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));

            var lunaComment1 = TestUtil.CreateNewComment(entryLuna, new DateTime(2013, 4, 5));
            var lunaComment2 = TestUtil.CreateNewComment(entryLuna, new DateTime(2013, 4, 8));

            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var deimosComment1 = TestUtil.CreateNewComment(entryDeimos, new DateTime(2013, 4, 9));
            var deimosComment2 = TestUtil.CreateNewComment(entryDeimos, new DateTime(2013, 4, 10));

            TestUtil.UpdateIndex();

            var manager = new Mod.CommentManager();
            var comments = manager.GetBlogComments(blog, 10);
            var ids = from comment in comments select comment.Uri.ItemID;

            Assert.That(ids, Is.EqualTo(new[] { deimosComment2.ID, deimosComment1.ID, lunaComment2.ID, lunaComment1.ID }));
        }

        [Test]
        public void GetBlogComments_Limited_ReturnsStartOfList()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);

            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var lunaComment1 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 1));
            var lunaComment2 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 2));

            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var deimosComment1 = TestUtil.CreateNewComment(entryDeimos, new DateTime(2012, 3, 3));
            var deimosComment2 = TestUtil.CreateNewComment(entryDeimos, new DateTime(2012, 3, 4));

            TestUtil.UpdateIndex();

            var manager = new Mod.CommentManager();
            var comments = manager.GetBlogComments(blog, 3);
            var ids = from comment in comments select comment.Uri.ItemID;

            Assert.That(ids, Is.EqualTo(new[] { deimosComment2.ID, deimosComment1.ID, lunaComment2.ID }));
        }

        [Test]
        public void GetBlogComments_CommentsInOtherBlog_ReturnsEmptyList()
        {
            var blog1 = TestUtil.CreateNewBlog(TestContentRoot);

            var entryLuna = TestUtil.CreateNewEntry(blog1, "Luna", entryDate: new DateTime(2012, 3, 1));
            var lunaComment1 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 1));
            var lunaComment2 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 2));

            var entryDeimos = TestUtil.CreateNewEntry(blog1, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var deimosComment1 = TestUtil.CreateNewComment(entryDeimos, new DateTime(2012, 3, 3));
            var deimosComment2 = TestUtil.CreateNewComment(entryDeimos, new DateTime(2012, 3, 10));

            var blog2 = TestUtil.CreateNewBlog(TestContentRoot);

            TestUtil.UpdateIndex();

            var manager = new Mod.CommentManager();
            var comments = manager.GetBlogComments(blog2, 3);

            Assert.That(comments, Is.Empty);
        }

        [Test]
        public void GetBlogComments_CommentsInManyBlogs_ReturnsCommentsInTargetBlog()
        {
            var blog1 = TestUtil.CreateNewBlog(TestContentRoot);

            var entryLuna = TestUtil.CreateNewEntry(blog1, "Luna", entryDate: new DateTime(2012, 3, 1));
            var lunaComment1 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 1));

            var entryDeimos = TestUtil.CreateNewEntry(blog1, "Deimos", entryDate: new DateTime(2012, 3, 2));

            var blog2 = TestUtil.CreateNewBlog(TestContentRoot);

            var entryAlpha = TestUtil.CreateNewEntry(blog2, "alpha", entryDate: new DateTime(2012, 3, 1));
            TestUtil.CreateNewComment(entryAlpha, new DateTime(2012, 3, 2));
            TestUtil.CreateNewComment(entryAlpha, new DateTime(2012, 3, 3));

            TestUtil.UpdateIndex();

            var manager = new Mod.CommentManager();
            var comments = manager.GetBlogComments(blog1, 3);
            var ids = from comment in comments select comment.Uri.ItemID;

            Assert.That(ids, Is.EqualTo(new[] { lunaComment1.ID }));
        }
    }
}