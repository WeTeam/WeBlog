using System.Data;
using System;
using System.Linq;
using NUnit.Framework;
using Sitecore.Data.Items;
using Moq;
using Sitecore.Data;
using Sitecore.Modules.WeBlog.Managers;

#if FEATURE_XDB
using Sitecore.Analytics.Reporting;
using System.Collections.Generic;
#elif FEATURE_DMS
using Sitecore.Analytics.Data.DataAccess;
#endif

namespace Sitecore.Modules.WeBlog.Test
{
    [TestFixture]
    [Category("EntryManagerFixture")]
    public class EntryManagerFixture : UnitTestBase
    {
        [Test]
        public void GetBlogEntries_NullItem()
        {
            var manager = new EntryManager();
            var entries = manager.GetBlogEntries((Item)null);
            Assert.That(entries, Is.Empty);
        }

        [Test]
        public void GetBlogEntries_NoEntries()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);

            var manager = new EntryManager();
            var entries = manager.GetBlogEntries(blog);
            Assert.That(entries, Is.Empty);
        }

        [Test]
        public void GetBlogEntries_WithEntries()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2012, 3, 3));
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog);
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryPhobos.ID, entryDeimos.ID, entryLuna.ID }));
        }

        [Test]
        public void GetBlogEntries_WithEntriesMultipleBlogs()
        {
            var blog1 = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog1, "Luna", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = TestUtil.CreateNewEntry(blog1, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = TestUtil.CreateNewEntry(blog1, "Phobos", entryDate: new DateTime(2012, 3, 3));

            var blog2 = TestUtil.CreateNewBlog(TestContentRoot);
            TestUtil.CreateNewEntry(blog2, "Adrastea");
            TestUtil.CreateNewEntry(blog2, "Aitne");
            TestUtil.CreateNewEntry(blog2, "Amalthea");
            TestUtil.CreateNewEntry(blog2, "Ananke");
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(entryDeimos);
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryPhobos.ID, entryDeimos.ID, entryLuna.ID }));
        }

        [Test]
        public void GetBlogEntries_LimitedMultipleBlogs()
        {
            var blog1 = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog1, "Luna", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = TestUtil.CreateNewEntry(blog1, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = TestUtil.CreateNewEntry(blog1, "Phobos", entryDate: new DateTime(2012, 3, 3));

            var blog2 = TestUtil.CreateNewBlog(TestContentRoot);
            var entryAdrastea = TestUtil.CreateNewEntry(blog2, "Adrastea", entryDate: new DateTime(2012, 3, 1));
            var entryAitne = TestUtil.CreateNewEntry(blog2, "Aitne", entryDate: new DateTime(2012, 3, 2));
            var entryAmalthea = TestUtil.CreateNewEntry(blog2, "Amalthea", entryDate: new DateTime(2012, 3, 3));
            var entryAnanke = TestUtil.CreateNewEntry(blog2, "Ananke", entryDate: new DateTime(2012, 3, 4));
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(entryAmalthea, 3, null, null, null, null);
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryAnanke.ID, entryAmalthea.ID, entryAitne.ID }));
        }

        [Test]
        public void GetBlogEntries_DerivedTemplatesWithEntries()
        {
            Assert.Ignore("Test not written");
        }

        [Test]
        public void GetBlogEntries_LimitEntries()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2012, 3, 3));
            var entryAdrastea = TestUtil.CreateNewEntry(blog, "Adrastea", entryDate: new DateTime(2012, 3, 4));
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, 2, null, null, null, null);
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryAdrastea.ID, entryPhobos.ID }));
        }

        [Test]
        public void GetBlogEntries_LimitEntriesZero()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2012, 3, 3));
            var entryAdrastea = TestUtil.CreateNewEntry(blog, "Adrastea", entryDate: new DateTime(2012, 3, 4));
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, 0, null, null, null, null);

            Assert.That(results, Is.Empty);
        }

        [Test]
        public void GetBlogEntries_LimitEntriesNegativeLimit()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2012, 3, 3));
            var entryAdrastea = TestUtil.CreateNewEntry(blog, "Adrastea", entryDate: new DateTime(2012, 3, 4));
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, -7, null, null, null, null);

            Assert.That(results, Is.Empty);
        }

        [Test]
        public void GetBlogEntries_ByTag()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", tags: "wheeljack, prowl", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", tags: "prowl, cliffjumper", entryDate: new DateTime(2012, 3, 2));
            TestUtil.CreateNewEntry(blog, "Phobos", tags: "wheeljack, cliffjumper", entryDate: new DateTime(2012, 3, 3));
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, 10, "prowl", null, null, null);
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryDeimos.ID, entryLuna.ID }));
        }

        [Test]
        public void GetBlogEntries_ByTagWithSpace()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", tags: "wheeljack, prowl", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", tags: "prowl, orion pax, cliffjumper", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos", tags: "wheeljack, hot rod, orion pax", entryDate: new DateTime(2012, 3, 3));
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, 10, "orion pax", null, null, null);
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryPhobos.ID, entryDeimos.ID }));
        }

        [Test]
        public void GetBlogEntries_ByTagLimited()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", tags: "wheeljack, prowl", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", tags: "prowl, orion pax, cliffjumper", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos", tags: "wheeljack, hot rod, orion pax", entryDate: new DateTime(2012, 3, 3));
            var entryAdrastea = TestUtil.CreateNewEntry(blog, "Adrastea", tags: "wheeljack, hot rod, orion pax, prowl", entryDate: new DateTime(2012, 3, 4));
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, 2, "wheeljack", null, null, null);
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryAdrastea.ID, entryPhobos.ID }));
        }

        [Test]
        public void GetBlogEntries_ByInvalidTag()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", tags: "wheeljack, prowl", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", tags: "prowl, orion pax, cliffjumper", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos", tags: "wheeljack, hot rod, orion pax", entryDate: new DateTime(2012, 3, 3));
            var entryAdrastea = TestUtil.CreateNewEntry(blog, "Adrastea", tags: "wheeljack, hot rod, orion pax, prowl", entryDate: new DateTime(2012, 3, 4));
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, 10, "blurr", null, null, null);            

            Assert.That(results, Is.Empty);
        }

        [Test]
        public void GetBlogEntries_ByCategory()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var categoryAlpha = TestUtil.CreateNewCategory(blog, "Alpha");
            var categoryBeta = TestUtil.CreateNewCategory(blog, "Beta");
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", categories: new [] { categoryBeta.ID }, entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", categories: new[] { categoryAlpha.ID, categoryBeta.ID }, entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos", categories: new[] { categoryBeta.ID, categoryAlpha.ID }, entryDate: new DateTime(2012, 3, 3));
            var entryAdrastea = TestUtil.CreateNewEntry(blog, "Adrastea", categories: new[] { categoryBeta.ID, categoryAlpha.ID }, entryDate: new DateTime(2012, 3, 4));
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, 10, null, categoryAlpha.ID.ToString(), null, null);
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryAdrastea.ID, entryPhobos.ID, entryDeimos.ID }));
        }

        [Test]
        public void GetBlogEntries_ByInvalidCategory()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var categoryAlpha = TestUtil.CreateNewCategory(blog, "Alpha");
            var categoryBeta = TestUtil.CreateNewCategory(blog, "Beta");
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", categories: new[] { categoryBeta.ID }, entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", categories: new[] { categoryAlpha.ID, categoryBeta.ID }, entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos", categories: new[] { categoryBeta.ID, categoryAlpha.ID }, entryDate: new DateTime(2012, 3, 3));
            var entryAdrastea = TestUtil.CreateNewEntry(blog, "Adrastea", categories: new[] { categoryBeta.ID, categoryAlpha.ID }, entryDate: new DateTime(2012, 3, 4));
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, 10, null, ID.NewID.ToString(), null, null);

            Assert.That(results, Is.Empty);
        }

        [Test]
        public void GetBlogEntries_ByCategoryLimited()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var categoryAlpha = TestUtil.CreateNewCategory(blog, "Alpha");
            var categoryBeta = TestUtil.CreateNewCategory(blog, "Beta");
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", categories: new[] { categoryBeta.ID }, entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", categories: new[] { categoryAlpha.ID, categoryBeta.ID }, entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos", categories: new[] { categoryBeta.ID, categoryAlpha.ID }, entryDate: new DateTime(2012, 3, 3));
            var entryAdrastea = TestUtil.CreateNewEntry(blog, "Adrastea", categories: new[] { categoryBeta.ID, categoryAlpha.ID }, entryDate: new DateTime(2012, 3, 4));
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, 1, null, categoryAlpha.ID.ToString(), null, null);
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryAdrastea.ID }));
        }

        [Test]
        public void GetBlogEntries_InDateRange()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2014, 10, 1));
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2014, 11, 1));
            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2014, 12, 1));
            var entryAdrastea = TestUtil.CreateNewEntry(blog, "Adrastea", entryDate: new DateTime(2015, 1, 1));
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, 10, null, null, new DateTime(2014, 11, 1), new DateTime(2014, 12, 20));
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryPhobos.ID, entryDeimos.ID }));

        }

        [Test]
        public void GetBlogEntries_InDateRangeLimited()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2014, 10, 1));
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2014, 11, 1));
            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2014, 12, 1));
            var entryAdrastea = TestUtil.CreateNewEntry(blog, "Adrastea", entryDate: new DateTime(2015, 1, 1));
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, 2, null, null, new DateTime(2014, 11, 1), new DateTime(2015, 1, 20));
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryAdrastea.ID, entryPhobos.ID }));
        }

        [Test]
        public void GetBlogEntriesByMonthAndYear_BeforeEntries()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2012, 4, 3));
            var entryAdrastea = TestUtil.CreateNewEntry(blog, "Adrastea", entryDate: new DateTime(2012, 5, 4));
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntriesByMonthAndYear(blog, 1, 2012);

            Assert.That(results, Is.Empty);
        }

        [Test]
        public void GetBlogEntriesByMonthAndYear_WithinEntries()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2012, 4, 3));
            var entryAdrastea = TestUtil.CreateNewEntry(blog, "Adrastea", entryDate: new DateTime(2012, 5, 4));
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntriesByMonthAndYear(blog, 3, 2012);
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryDeimos.ID, entryLuna.ID }));
        }

        [Test]
        public void GetBlogEntriesByMonthAndYear_LastMonth()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 12, 30));
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 12, 31));
            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2013, 1, 1));
            var entryAdrastea = TestUtil.CreateNewEntry(blog, "Adrastea", entryDate: new DateTime(2013, 1, 1));
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntriesByMonthAndYear(blog, 12, 2012);
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryDeimos.ID, entryLuna.ID }));
        }

        [Test]
        public void GetBlogEntriesByMonthAndYear_FirstMonth()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 12, 30));
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 12, 31));
            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2013, 1, 1));
            var entryAdrastea = TestUtil.CreateNewEntry(blog, "Adrastea", entryDate: new DateTime(2013, 1, 1));
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntriesByMonthAndYear(blog, 1, 2013);
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryPhobos.ID, entryAdrastea.ID }));
        }

        [Test]
        public void GetBlogEntriesByMonthAndYear_AfterEntries()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2012, 4, 3));
            var entryAdrastea = TestUtil.CreateNewEntry(blog, "Adrastea", entryDate: new DateTime(2012, 5, 4));
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntriesByMonthAndYear(blog, 6, 2012);

            Assert.That(results, Is.Empty);
        }

        [Test]
        public void GetBlogEntriesByMonthAndYear_InvalidDate()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2012, 3, 3));
            var entryAdrastea = TestUtil.CreateNewEntry(blog, "Adrastea", entryDate: new DateTime(2012, 3, 4));
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntriesByMonthAndYear(blog, 17, 20992);

            Assert.That(results, Is.Empty);
        }

        [Test]
        public void GetPopularEntriesByComment_ValidItem()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);

            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna");
            TestUtil.CreateNewComment(entryLuna);

            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos");
            TestUtil.CreateNewComment(entryDeimos);
            TestUtil.CreateNewComment(entryDeimos);

            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos");
            TestUtil.CreateNewComment(entryPhobos);
            TestUtil.CreateNewComment(entryPhobos);
            TestUtil.CreateNewComment(entryPhobos);

            var entryAdrastea = TestUtil.CreateNewEntry(blog, "Adrastea");

            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var entries = manager.GetPopularEntriesByComment(blog, 10);
            var entryIds = from entry in entries select entry.ID;

            Assert.That(entryIds, Is.EqualTo(new[] { entryPhobos.ID, entryDeimos.ID, entryLuna.ID }));
        }

        [Test]
        public void GetPopularEntriesByComment_ValidItem_Limited()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);

            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna");
            TestUtil.CreateNewComment(entryLuna);

            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos");
            TestUtil.CreateNewComment(entryDeimos);
            TestUtil.CreateNewComment(entryDeimos);

            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos");
            TestUtil.CreateNewComment(entryPhobos);
            TestUtil.CreateNewComment(entryPhobos);
            TestUtil.CreateNewComment(entryPhobos);

            var entryAdrastea = TestUtil.CreateNewEntry(blog, "Adrastea");

            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var entries = manager.GetPopularEntriesByComment(blog, 2);
            var entryIds = from entry in entries select entry.ID;

            Assert.That(entryIds, Is.EqualTo(new[] { entryPhobos.ID, entryDeimos.ID }));
        }

        [Test]
        public void GetPopularEntriesByComment_InvalidItem()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);

            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna");
            TestUtil.CreateNewComment(entryLuna);

            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos");
            TestUtil.CreateNewComment(entryDeimos);
            TestUtil.CreateNewComment(entryDeimos);

            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos");
            TestUtil.CreateNewComment(entryDeimos);
            TestUtil.CreateNewComment(entryDeimos);
            TestUtil.CreateNewComment(entryDeimos);

            var entryAdrastea = TestUtil.CreateNewEntry(blog, "Adrastea");

            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var entries = manager.GetPopularEntriesByComment(entryDeimos, 10);
            var entryIds = from entry in entries select entry.ID;

            Assert.That(entryIds, Is.EqualTo(new[] { entryDeimos.ID }));
        }

        [Test]
        public void GetPopularEntriesByComment_NullItem()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna");
            TestUtil.CreateNewComment(entryLuna);

            var manager = new EntryManager();
            var entries = manager.GetPopularEntriesByComment(blog, 10);

            Assert.That(entries, Is.Empty);
        }

        [Test]
        public void GetPopularEntriesByView_ValidItem()
        {
            VerifyAnalyticsSetup();

            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna");
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos");
            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos");
            TestUtil.UpdateIndex();

            var manager = CreateEntryManagerForAnalyticsTest(entryLuna.ID, entryPhobos.ID, entryDeimos.ID);

            var entries = manager.GetPopularEntriesByView(blog, int.MaxValue);
            var entryIds = from entry in entries select entry.ID;

            Assert.That(entryIds, Is.EqualTo(new[] { entryLuna.ID, entryPhobos.ID, entryDeimos.ID }));
        }

        [Test]
        public void GetPopularEntriesByView_ValidItem_Limited()
        {
            VerifyAnalyticsSetup();

            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna");
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos");
            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos");
            TestUtil.UpdateIndex();

            var manager = CreateEntryManagerForAnalyticsTest(entryLuna.ID, entryPhobos.ID, entryDeimos.ID);

            var entries = manager.GetPopularEntriesByView(blog, 1);
            var entryIds = from entry in entries select entry.ID;

            Assert.That(entryIds, Is.EqualTo(new[] { entryLuna.ID } ));
        }

        [Test]
        public void GetPopularEntriesByView_InvalidItem()
        {
            VerifyAnalyticsSetup();

            var manager = CreateEntryManagerForAnalyticsTest();
            var entries = manager.GetPopularEntriesByView(TestContentRoot, int.MaxValue);

            Assert.That(entries, Is.Empty);
        }

        [Test]
        public void GetPopularEntriesByView_NullItem()
        {
            VerifyAnalyticsSetup();

            var manager = CreateEntryManagerForAnalyticsTest();
            var entries = manager.GetPopularEntriesByView(null, int.MaxValue);

            Assert.That(entries, Is.Empty);
        }

        // TODO: Write tests for methods accepting language

        private void VerifyAnalyticsSetup()
        {
            Assert.True(Sitecore.Configuration.Settings.Analytics.Enabled, "Sitecore.Analytics must be enabled to test");
        }

        private EntryManager CreateEntryManagerForAnalyticsTest(params ID[] popularEntryIdsInOrder)
        {
#if FEATURE_XDB
            var reportProvider = CreateMockReportDataProvider(popularEntryIdsInOrder);
            return new EntryManager(reportProvider);
#else
            // Register DMS page views for popular items

            var visitor = new Visitor(Guid.NewGuid());
            visitor.CreateVisit(Guid.NewGuid());

            var hitCount = 1;
            for(var i = popularEntryIdsInOrder.Length - 1; i >= 0; i--)
            {
                for(var j = 0; j < hitCount; j++)
                {
                    visitor.CurrentVisit.CreatePage().ItemId = popularEntryIdsInOrder[i].ID.ToGuid();
                }
                hitCount++;
            }

            visitor.Submit();

            return new EntryManager();
#endif
        }

#if FEATURE_XDB
        private ReportDataProviderBase CreateMockReportDataProvider(IEnumerable<ID> ids)
        {
            var reportingProviderMock = new Mock<ReportDataProviderBase>();

            var visitCount = ids.Count();
            foreach (var id in ids)
            {
                var dataTable = new DataTable();
                dataTable.Columns.AddRange(new[]
                {
                    new DataColumn("Visits", typeof(long))
                });
            
                dataTable.Rows.Add(visitCount);
                visitCount--;

                reportingProviderMock.Setup(x =>
                    x.GetData(It.IsAny<string>(),
                        It.Is<ReportDataQuery>(y => y.Parameters["@ItemId"].Equals(id)), It.IsAny<CachingPolicy>())).Returns(new ReportDataResponse(() => dataTable));
            }

            return reportingProviderMock.Object;
        }
#endif

    }
}