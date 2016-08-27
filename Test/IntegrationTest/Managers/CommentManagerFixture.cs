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
        public void GetCommentsFor_NullItem()
        {
            var manager = new Mod.CommentManager();
            var entries = manager.GetCommentsFor(null, 10);
            Assert.That(entries, Is.Empty);
        }

        [Test]
        public void GetCommentsFor_BlogItem()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);

            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var commetLuna1 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 1));
            var commetLuna2 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 3));

            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var commentDeimos1 = TestUtil.CreateNewComment(entryDeimos, new DateTime(2012, 3, 5));
            var commentDeimos2 = TestUtil.CreateNewComment(entryDeimos, new DateTime(2012, 3, 10));

            TestUtil.UpdateIndex();

            var manager = new Mod.CommentManager();
            var comments = manager.GetCommentsFor(blog, 10);
            var ids = from comment in comments select comment.ID;

            Assert.That(ids, Is.EquivalentTo(new[] { commetLuna1.ID, commetLuna2.ID, commentDeimos1.ID, commentDeimos2.ID }));
        }

        [Test]
        public void GetCommentsFor_EntryWithComments()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);

            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var commetLuna1 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 2));
            var commetLuna2 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 5));

            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var commentDeimos1 = TestUtil.CreateNewComment(entryDeimos, new DateTime(2012, 3, 6));
            var commentDeimos2 = TestUtil.CreateNewComment(entryDeimos, new DateTime(2012, 3, 8));

            TestUtil.UpdateIndex();

            var manager = new Mod.CommentManager();
            var comments = manager.GetCommentsFor(entryLuna, 10);
            var ids = from comment in comments select comment.ID;

            Assert.That(ids, Is.EquivalentTo(new[] { commetLuna1.ID, commetLuna2.ID }));
        }

        [Test]
        public void GetCommentsFor_EntryWithoutComments()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);

            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var commetLuna1 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 1));
            var commetLuna2 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 3));

            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var commentDeimos1 = TestUtil.CreateNewComment(entryDeimos, new DateTime(2012, 3, 5));
            var commentDeimos2 = TestUtil.CreateNewComment(entryDeimos, new DateTime(2012, 3, 6));

            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2012, 3, 3));

            TestUtil.UpdateIndex();

            var manager = new Mod.CommentManager();
            var comments = manager.GetCommentsFor(entryPhobos, 10);

            Assert.That(comments, Is.Empty);
        }

        [Test]
        public void GetCommentsFor_EntryWithComments_Sorted()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);

            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var commetLuna1 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 4));
            var commetLuna2 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 6));

            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var commentDeimos1 = TestUtil.CreateNewComment(entryDeimos, new DateTime(2012, 3, 7));
            var commentDeimos2 = TestUtil.CreateNewComment(entryDeimos, new DateTime(2012, 3, 8));

            TestUtil.UpdateIndex();

            var manager = new Mod.CommentManager();
            var comments = manager.GetCommentsFor(blog, 10, true);
            var ids = from comment in comments select comment.ID;

            Assert.That(ids, Is.EqualTo(new[] { commetLuna1.ID, commetLuna2.ID, commentDeimos1.ID, commentDeimos2.ID }));
        }

        [Test]
        public void GetCommentsFor_EntryWithComments_Sorted_Reverse()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);

            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var commetLuna1 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 2));
            var commetLuna2 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 6));

            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var commentDeimos1 = TestUtil.CreateNewComment(entryDeimos, new DateTime(2012, 3, 7));
            var commentDeimos2 = TestUtil.CreateNewComment(entryDeimos, new DateTime(2012, 3, 8));

            TestUtil.UpdateIndex();

            var manager = new Mod.CommentManager();
            var comments = manager.GetCommentsFor(blog, 10, true, true);
            var ids = from comment in comments select comment.ID;

            Assert.That(ids, Is.EqualTo(new[] { commentDeimos2.ID, commentDeimos1.ID, commetLuna2.ID, commetLuna1.ID }));
        }

        [Test]
        public void GetCommentsFor_EntryWithComments_Limited()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);

            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var commetLuna1 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 10));
            var commetLuna2 = TestUtil.CreateNewComment(entryLuna, new DateTime(2012, 3, 11));

            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var commentDeimos1 = TestUtil.CreateNewComment(entryDeimos, new DateTime(2012, 3, 12));
            var commentDeimos2 = TestUtil.CreateNewComment(entryDeimos, new DateTime(2012, 3, 13));

            TestUtil.UpdateIndex();

            var manager = new Mod.CommentManager();
            var comments = manager.GetCommentsFor(blog, 3, true, true);
            var ids = from comment in comments select comment.ID;

            Assert.That(ids, Is.EqualTo(new[] { commentDeimos2.ID, commentDeimos1.ID, commetLuna2.ID }));
        }

        [Test]
        public void GetEntryComments_NullItem()
        {
            var manager = new Mod.CommentManager();
            var comments = manager.GetEntryComments((Item)null);

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
            var comments = manager.GetEntryComments(blog);

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
            var comments = manager.GetEntryComments(entryLuna);
            var ids = from comment in comments select comment.ID;

            Assert.That(ids, Is.EqualTo(new[] { commetLuna1.ID, commetLuna2.ID }));
        }

        [Test]
        public void GetEntryComments_EntryWithoutComments()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));

            TestUtil.UpdateIndex();

            var manager = new Mod.CommentManager();
            var comments = manager.GetEntryComments(entryLuna);

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
            var ids = from comment in comments select comment.ID;

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
        public void GetCommentsByBlog_NullItem()
        {
            var manager = new Mod.CommentManager();
            var comments = manager.GetCommentsByBlog(null, 10);
            
            Assert.That(comments, Is.Empty);
        }

        [Test]
        public void GetCommentsByBlog_NoComments()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);

            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));

            TestUtil.UpdateIndex();

            var manager = new Mod.CommentManager();
            var comments = manager.GetCommentsByBlog(blog, 10);

            Assert.That(comments, Is.Empty);
        }

        [Test]
        public void GetCommentsByBlog_CommentsInBlog()
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
            var comments = manager.GetCommentsByBlog(blog, 10);
            var ids = from comment in comments select comment.ID;

            Assert.That(ids, Is.EqualTo(new[] { deimosComment2.ID, deimosComment1.ID, lunaComment2.ID, lunaComment1.ID }));
        }

        [Test]
        public void GetCommentsByBlog_CommentsInBlog_Limited()
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
            var comments = manager.GetCommentsByBlog(blog, 3);
            var ids = from comment in comments select comment.ID;

            Assert.That(ids, Is.EqualTo(new[] { deimosComment2.ID, deimosComment1.ID, lunaComment2.ID }));
        }

        [Test]
        public void GetCommentsByBlog_CommentsInOtherBlog()
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
            var comments = manager.GetCommentsByBlog(blog2, 3);

            Assert.That(comments, Is.Empty);
        }

        [Test]
        public void GetCommentsByBlog_CommentsInManyBlogs()
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
            var comments = manager.GetCommentsByBlog(blog1, 3);
            var ids = from comment in comments select comment.ID;

            Assert.That(ids, Is.EqualTo(new[] { lunaComment1.ID }));
        }
    }
}