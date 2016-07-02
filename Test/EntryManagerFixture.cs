using System.Data;
using System;
using System.Linq;
using NUnit.Framework;
using Sitecore.Data.Items;
using Moq;
using Sitecore.Data;
using Sitecore.ContentSearch;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Security.Accounts;

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
        private readonly BranchId BlogBranchId = new BranchId(ID.Parse("{8F890A99-5AD0-48B9-B930-B44BEC524840}")); // MVC blog branch
        private readonly TemplateID EntryTemplateId = new TemplateID(ID.Parse("{BE9675B1-4951-4E11-B935-A698227B6A97}")); // MVC entry
        private readonly TemplateID CategoryTemplateId = new TemplateID(ID.Parse("{6C455315-01BF-4E2E-9BA3-31B5695E9C77}")); // MVC category
        private readonly TemplateID CommentTemplateId = new TemplateID(ID.Parse("{70949D4E-35D8-4581-A7A2-52928AA119D5}")); // Comment

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
            var blog = CreateNewBlog();

            var manager = new EntryManager();
            var entries = manager.GetBlogEntries(blog);
            Assert.That(entries, Is.Empty);
        }

        [Test]
        public void GetBlogEntries_WithEntries()
        {
            var blog = CreateNewBlog();
            var entryLuna = CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2012, 3, 3));
            UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog);
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryPhobos.ID, entryDeimos.ID, entryLuna.ID }));
        }

        [Test]
        public void GetBlogEntries_WithEntriesMultipleBlogs()
        {
            var blog1 = CreateNewBlog();
            var entryLuna = CreateNewEntry(blog1, "Luna", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = CreateNewEntry(blog1, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = CreateNewEntry(blog1, "Phobos", entryDate: new DateTime(2012, 3, 3));

            var blog2 = CreateNewBlog();
            CreateNewEntry(blog2, "Adrastea");
            CreateNewEntry(blog2, "Aitne");
            CreateNewEntry(blog2, "Amalthea");
            CreateNewEntry(blog2, "Ananke");
            UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(entryDeimos);
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryPhobos.ID, entryDeimos.ID, entryLuna.ID }));
        }

        [Test]
        public void GetBlogEntries_LimitedMultipleBlogs()
        {
            var blog1 = CreateNewBlog();
            var entryLuna = CreateNewEntry(blog1, "Luna", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = CreateNewEntry(blog1, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = CreateNewEntry(blog1, "Phobos", entryDate: new DateTime(2012, 3, 3));

            var blog2 = CreateNewBlog();
            var entryAdrastea = CreateNewEntry(blog2, "Adrastea", entryDate: new DateTime(2012, 3, 1));
            var entryAitne = CreateNewEntry(blog2, "Aitne", entryDate: new DateTime(2012, 3, 2));
            var entryAmalthea = CreateNewEntry(blog2, "Amalthea", entryDate: new DateTime(2012, 3, 3));
            var entryAnanke = CreateNewEntry(blog2, "Ananke", entryDate: new DateTime(2012, 3, 4));
            UpdateIndex();

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
            var blog = CreateNewBlog();
            var entryLuna = CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2012, 3, 3));
            var entryAdrastea = CreateNewEntry(blog, "Adrastea", entryDate: new DateTime(2012, 3, 4));
            UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, 2, null, null, null, null);
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryAdrastea.ID, entryPhobos.ID }));
        }

        [Test]
        public void GetBlogEntries_LimitEntriesZero()
        {
            var blog = CreateNewBlog();
            var entryLuna = CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2012, 3, 3));
            var entryAdrastea = CreateNewEntry(blog, "Adrastea", entryDate: new DateTime(2012, 3, 4));
            UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, 0, null, null, null, null);

            Assert.That(results, Is.Empty);
        }

        [Test]
        public void GetBlogEntries_LimitEntriesNegativeLimit()
        {
            var blog = CreateNewBlog();
            var entryLuna = CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2012, 3, 3));
            var entryAdrastea = CreateNewEntry(blog, "Adrastea", entryDate: new DateTime(2012, 3, 4));
            UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, -7, null, null, null, null);

            Assert.That(results, Is.Empty);
        }

        [Test]
        public void GetBlogEntries_ByTag()
        {
            var blog = CreateNewBlog();
            var entryLuna = CreateNewEntry(blog, "Luna", tags: "wheeljack, prowl", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = CreateNewEntry(blog, "Deimos", tags: "prowl, cliffjumper", entryDate: new DateTime(2012, 3, 2));
            CreateNewEntry(blog, "Phobos", tags: "wheeljack, cliffjumper", entryDate: new DateTime(2012, 3, 3));
            UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, 10, "prowl", null, null, null);
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryDeimos.ID, entryLuna.ID }));
        }

        [Test]
        public void GetBlogEntries_ByTagWithSpace()
        {
            var blog = CreateNewBlog();
            var entryLuna = CreateNewEntry(blog, "Luna", tags: "wheeljack, prowl", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = CreateNewEntry(blog, "Deimos", tags: "prowl, orion pax, cliffjumper", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = CreateNewEntry(blog, "Phobos", tags: "wheeljack, hot rod, orion pax", entryDate: new DateTime(2012, 3, 3));
            UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, 10, "orion pax", null, null, null);
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryPhobos.ID, entryDeimos.ID }));
        }

        [Test]
        public void GetBlogEntries_ByTagLimited()
        {
            var blog = CreateNewBlog();
            var entryLuna = CreateNewEntry(blog, "Luna", tags: "wheeljack, prowl", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = CreateNewEntry(blog, "Deimos", tags: "prowl, orion pax, cliffjumper", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = CreateNewEntry(blog, "Phobos", tags: "wheeljack, hot rod, orion pax", entryDate: new DateTime(2012, 3, 3));
            var entryAdrastea = CreateNewEntry(blog, "Adrastea", tags: "wheeljack, hot rod, orion pax, prowl", entryDate: new DateTime(2012, 3, 4));
            UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, 2, "wheeljack", null, null, null);
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryAdrastea.ID, entryPhobos.ID }));
        }

        [Test]
        public void GetBlogEntries_ByInvalidTag()
        {
            var blog = CreateNewBlog();
            var entryLuna = CreateNewEntry(blog, "Luna", tags: "wheeljack, prowl", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = CreateNewEntry(blog, "Deimos", tags: "prowl, orion pax, cliffjumper", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = CreateNewEntry(blog, "Phobos", tags: "wheeljack, hot rod, orion pax", entryDate: new DateTime(2012, 3, 3));
            var entryAdrastea = CreateNewEntry(blog, "Adrastea", tags: "wheeljack, hot rod, orion pax, prowl", entryDate: new DateTime(2012, 3, 4));
            UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, 10, "blurr", null, null, null);            

            Assert.That(results, Is.Empty);
        }

        [Test]
        public void GetBlogEntries_ByCategory()
        {
            var blog = CreateNewBlog();
            var categoryAlpha = CreateNewCategory(blog, "Alpha");
            var categoryBeta = CreateNewCategory(blog, "Beta");
            var entryLuna = CreateNewEntry(blog, "Luna", categories: new [] { categoryBeta.ID }, entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = CreateNewEntry(blog, "Deimos", categories: new[] { categoryAlpha.ID, categoryBeta.ID }, entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = CreateNewEntry(blog, "Phobos", categories: new[] { categoryBeta.ID, categoryAlpha.ID }, entryDate: new DateTime(2012, 3, 3));
            var entryAdrastea = CreateNewEntry(blog, "Adrastea", categories: new[] { categoryBeta.ID, categoryAlpha.ID }, entryDate: new DateTime(2012, 3, 4));
            UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, 10, null, categoryAlpha.ID.ToString(), null, null);
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryAdrastea.ID, entryPhobos.ID, entryDeimos.ID }));
        }

        [Test]
        public void GetBlogEntries_ByInvalidCategory()
        {
            var blog = CreateNewBlog();
            var categoryAlpha = CreateNewCategory(blog, "Alpha");
            var categoryBeta = CreateNewCategory(blog, "Beta");
            var entryLuna = CreateNewEntry(blog, "Luna", categories: new[] { categoryBeta.ID }, entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = CreateNewEntry(blog, "Deimos", categories: new[] { categoryAlpha.ID, categoryBeta.ID }, entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = CreateNewEntry(blog, "Phobos", categories: new[] { categoryBeta.ID, categoryAlpha.ID }, entryDate: new DateTime(2012, 3, 3));
            var entryAdrastea = CreateNewEntry(blog, "Adrastea", categories: new[] { categoryBeta.ID, categoryAlpha.ID }, entryDate: new DateTime(2012, 3, 4));
            UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, 10, null, ID.NewID.ToString(), null, null);

            Assert.That(results, Is.Empty);
        }

        [Test]
        public void GetBlogEntries_ByCategoryLimited()
        {
            var blog = CreateNewBlog();
            var categoryAlpha = CreateNewCategory(blog, "Alpha");
            var categoryBeta = CreateNewCategory(blog, "Beta");
            var entryLuna = CreateNewEntry(blog, "Luna", categories: new[] { categoryBeta.ID }, entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = CreateNewEntry(blog, "Deimos", categories: new[] { categoryAlpha.ID, categoryBeta.ID }, entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = CreateNewEntry(blog, "Phobos", categories: new[] { categoryBeta.ID, categoryAlpha.ID }, entryDate: new DateTime(2012, 3, 3));
            var entryAdrastea = CreateNewEntry(blog, "Adrastea", categories: new[] { categoryBeta.ID, categoryAlpha.ID }, entryDate: new DateTime(2012, 3, 4));
            UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, 1, null, categoryAlpha.ID.ToString(), null, null);
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryAdrastea.ID }));
        }

        [Test]
        public void GetBlogEntries_InDateRange()
        {
            var blog = CreateNewBlog();
            var entryLuna = CreateNewEntry(blog, "Luna", entryDate: new DateTime(2014, 10, 1));
            var entryDeimos = CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2014, 11, 1));
            var entryPhobos = CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2014, 12, 1));
            var entryAdrastea = CreateNewEntry(blog, "Adrastea", entryDate: new DateTime(2015, 1, 1));
            UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, 10, null, null, new DateTime(2014, 11, 1), new DateTime(2014, 12, 20));
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryPhobos.ID, entryDeimos.ID }));

        }

        [Test]
        public void GetBlogEntries_InDateRangeLimited()
        {
            var blog = CreateNewBlog();
            var entryLuna = CreateNewEntry(blog, "Luna", entryDate: new DateTime(2014, 10, 1));
            var entryDeimos = CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2014, 11, 1));
            var entryPhobos = CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2014, 12, 1));
            var entryAdrastea = CreateNewEntry(blog, "Adrastea", entryDate: new DateTime(2015, 1, 1));
            UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, 2, null, null, new DateTime(2014, 11, 1), new DateTime(2015, 1, 20));
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryAdrastea.ID, entryPhobos.ID }));
        }

        [Test]
        public void GetBlogEntriesByMonthAndYear_BeforeEntries()
        {
            var blog = CreateNewBlog();
            var entryLuna = CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2012, 4, 3));
            var entryAdrastea = CreateNewEntry(blog, "Adrastea", entryDate: new DateTime(2012, 5, 4));
            UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntriesByMonthAndYear(blog, 1, 2012);

            Assert.That(results, Is.Empty);
        }

        [Test]
        public void GetBlogEntriesByMonthAndYear_WithinEntries()
        {
            var blog = CreateNewBlog();
            var entryLuna = CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2012, 4, 3));
            var entryAdrastea = CreateNewEntry(blog, "Adrastea", entryDate: new DateTime(2012, 5, 4));
            UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntriesByMonthAndYear(blog, 3, 2012);
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryDeimos.ID, entryLuna.ID }));
        }

        [Test]
        public void GetBlogEntriesByMonthAndYear_LastMonth()
        {
            var blog = CreateNewBlog();
            var entryLuna = CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 12, 30));
            var entryDeimos = CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 12, 31));
            var entryPhobos = CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2013, 1, 1));
            var entryAdrastea = CreateNewEntry(blog, "Adrastea", entryDate: new DateTime(2013, 1, 1));
            UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntriesByMonthAndYear(blog, 12, 2012);
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryDeimos.ID, entryLuna.ID }));
        }

        [Test]
        public void GetBlogEntriesByMonthAndYear_FirstMonth()
        {
            var blog = CreateNewBlog();
            var entryLuna = CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 12, 30));
            var entryDeimos = CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 12, 31));
            var entryPhobos = CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2013, 1, 1));
            var entryAdrastea = CreateNewEntry(blog, "Adrastea", entryDate: new DateTime(2013, 1, 1));
            UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntriesByMonthAndYear(blog, 1, 2013);
            var ids = from result in results select result.ID;

            Assert.That(ids, Is.EqualTo(new[] { entryPhobos.ID, entryAdrastea.ID }));
        }

        [Test]
        public void GetBlogEntriesByMonthAndYear_AfterEntries()
        {
            var blog = CreateNewBlog();
            var entryLuna = CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2012, 4, 3));
            var entryAdrastea = CreateNewEntry(blog, "Adrastea", entryDate: new DateTime(2012, 5, 4));
            UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntriesByMonthAndYear(blog, 6, 2012);

            Assert.That(results, Is.Empty);
        }

        [Test]
        public void GetBlogEntriesByMonthAndYear_InvalidDate()
        {
            var blog = CreateNewBlog();
            var entryLuna = CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2012, 3, 3));
            var entryAdrastea = CreateNewEntry(blog, "Adrastea", entryDate: new DateTime(2012, 3, 4));
            UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntriesByMonthAndYear(blog, 17, 20992);

            Assert.That(results, Is.Empty);
        }

        [Test]
        public void GetBlogEntryByComment_NullItem()
        {
            // todo: move to unit test
            var blog = CreateNewBlog();
            var entryFirst = CreateNewEntry(blog, "first");
            var comment = CreateNewComment(entryFirst);

            var manager = new EntryManager();
            var foundEntry = manager.GetBlogEntryByComment(null);

            Assert.That(foundEntry, Is.Null);
        }

        [Test]
        public void GetBlogEntryByComment_UnderEntry()
        {
            var blog = CreateNewBlog();
            var entryFirst = CreateNewEntry(blog, "first");
            var comment = CreateNewComment(entryFirst);

            var manager = new EntryManager();
            var foundEntry = manager.GetBlogEntryByComment(comment);

            Assert.That(foundEntry.ID, Is.EqualTo(entryFirst.ID));
        }

        [Test]
        public void GetBlogEntryByComment_OnEntry()
        {
            // todo: move to unit test
            var blog = CreateNewBlog();
            var entryFirst = CreateNewEntry(blog, "first");
            var comment = CreateNewComment(entryFirst);

            var manager = new EntryManager();
            var foundEntry = manager.GetBlogEntryByComment(entryFirst.InnerItem);

            Assert.That(foundEntry.ID, Is.EqualTo(entryFirst.ID));
        }

        [Test]
        public void GetBlogEntryByComment_OutsideEntry()
        {
            // todo: move to unit test
            var blog = CreateNewBlog();
            var entryFirst = CreateNewEntry(blog, "first");
            var comment = CreateNewComment(entryFirst);

            var manager = new EntryManager();
            var foundEntry = manager.GetBlogEntryByComment(blog.InnerItem);

            Assert.That(foundEntry, Is.Null);
        }

        [Test]
        public void GetPopularEntriesByComment_ValidItem()
        {
            // todo: move to unit test
            var blog = CreateNewBlog();

            var entryLuna = CreateNewEntry(blog, "Luna");
            CreateNewComment(entryLuna);

            var entryDeimos = CreateNewEntry(blog, "Deimos");
            CreateNewComment(entryDeimos);
            CreateNewComment(entryDeimos);

            var entryPhobos = CreateNewEntry(blog, "Phobos");
            CreateNewComment(entryPhobos);
            CreateNewComment(entryPhobos);
            CreateNewComment(entryPhobos);

            var entryAdrastea = CreateNewEntry(blog, "Adrastea");

            UpdateIndex();

            var manager = new EntryManager();
            var entries = manager.GetPopularEntriesByComment(blog, 10);
            var entryIds = from entry in entries select entry.ID;

            Assert.That(entryIds, Is.EqualTo(new[] { entryPhobos.ID, entryDeimos.ID, entryLuna.ID }));
        }

        [Test]
        public void GetPopularEntriesByComment_ValidItem_Limited()
        {
            // todo: move to unit test
            var blog = CreateNewBlog();

            var entryLuna = CreateNewEntry(blog, "Luna");
            CreateNewComment(entryLuna);

            var entryDeimos = CreateNewEntry(blog, "Deimos");
            CreateNewComment(entryDeimos);
            CreateNewComment(entryDeimos);

            var entryPhobos = CreateNewEntry(blog, "Phobos");
            CreateNewComment(entryPhobos);
            CreateNewComment(entryPhobos);
            CreateNewComment(entryPhobos);

            var entryAdrastea = CreateNewEntry(blog, "Adrastea");

            UpdateIndex();

            var manager = new EntryManager();
            var entries = manager.GetPopularEntriesByComment(blog, 2);
            var entryIds = from entry in entries select entry.ID;

            Assert.That(entryIds, Is.EqualTo(new[] { entryPhobos.ID, entryDeimos.ID }));
        }

        [Test]
        public void GetPopularEntriesByComment_InvalidItem()
        {
            // todo: move to unit test
            var blog = CreateNewBlog();

            var entryLuna = CreateNewEntry(blog, "Luna");
            CreateNewComment(entryLuna);

            var entryDeimos = CreateNewEntry(blog, "Deimos");
            CreateNewComment(entryDeimos);
            CreateNewComment(entryDeimos);

            var entryPhobos = CreateNewEntry(blog, "Phobos");
            CreateNewComment(entryDeimos);
            CreateNewComment(entryDeimos);
            CreateNewComment(entryDeimos);

            var entryAdrastea = CreateNewEntry(blog, "Adrastea");

            UpdateIndex();

            var manager = new EntryManager();
            var entries = manager.GetPopularEntriesByComment(entryDeimos, 10);
            var entryIds = from entry in entries select entry.ID;

            Assert.That(entryIds, Is.EqualTo(new[] { entryDeimos.ID }));
        }

        [Test]
        public void GetPopularEntriesByComment_NullItem()
        {
            // todo: move to unit test
            var blog = CreateNewBlog();
            var entryLuna = CreateNewEntry(blog, "Luna");
            CreateNewComment(entryLuna);

            var manager = new EntryManager();
            var entries = manager.GetPopularEntriesByComment(blog, 10);

            Assert.That(entries, Is.Empty);
        }

        [Test]
        public void GetPopularEntriesByView_ValidItem()
        {
            VerifyAnalyticsSetup();

            var blog = CreateNewBlog();
            var entryLuna = CreateNewEntry(blog, "Luna");
            var entryDeimos = CreateNewEntry(blog, "Deimos");
            var entryPhobos = CreateNewEntry(blog, "Phobos");
            UpdateIndex();

            var manager = CreateEntryManagerForAnalyticsTest(entryLuna.ID, entryPhobos.ID, entryDeimos.ID);

            var entries = manager.GetPopularEntriesByView(blog, int.MaxValue);
            var entryIds = from entry in entries select entry.ID;

            Assert.That(entryIds, Is.EqualTo(new[] { entryLuna.ID, entryPhobos.ID, entryDeimos.ID }));
        }

        [Test]
        public void GetPopularEntriesByView_ValidItem_Limited()
        {
            VerifyAnalyticsSetup();

            var blog = CreateNewBlog();
            var entryLuna = CreateNewEntry(blog, "Luna");
            var entryDeimos = CreateNewEntry(blog, "Deimos");
            var entryPhobos = CreateNewEntry(blog, "Phobos");
            UpdateIndex();

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

        private BlogHomeItem CreateNewBlog()
        {
            using (new UserSwitcher("sitecore\\admin", true))
            {
                var name = "blog " + ID.NewID.ToShortID();
                return TestContentRoot.Add(name, BlogBranchId);
            }
        }

        private EntryItem CreateNewEntry(BlogHomeItem blogItem, string name, string tags = null, ID[] categories = null, DateTime? entryDate = null)
        {
            using (new UserSwitcher("sitecore\\admin", true))
            {
                var entry = blogItem.InnerItem.Add(name, EntryTemplateId);

                if (tags != null)
                {
                    using (new EditContext(entry))
                    {
                        entry["Tags"] = tags;
                    }
                }

                if (categories != null)
                {
                    using (new EditContext(entry))
                    {
                        entry["Category"] = string.Join<ID>("|", categories);
                    }
                }

                if (entryDate != null)
                {
                    using (new EditContext(entry))
                    {
                        entry["Entry Date"] = DateUtil.ToIsoDate(entryDate.Value);
                    }
                }

                return entry;
            }
        }

        private CategoryItem CreateNewCategory(BlogHomeItem blogItem, string name)
        {
            using (new UserSwitcher("sitecore\\admin", true))
            {
                var categoryRoot = blogItem.InnerItem.Children["Categories"];
                return categoryRoot.Add(name, CategoryTemplateId);
            }
        }

        private CommentItem CreateNewComment(EntryItem entryItem)
        {
            using (new UserSwitcher("sitecore\\admin", true))
            {
                var name = "comment " + ID.NewID.ToShortID();
                return entryItem.InnerItem.Add(name, CommentTemplateId);
            }
        }

        private void UpdateIndex()
        {
            var settings = new WeBlogSettings();
            var index = ContentSearchManager.GetIndex(settings.SearchIndexName);
            index.Rebuild();
        }

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