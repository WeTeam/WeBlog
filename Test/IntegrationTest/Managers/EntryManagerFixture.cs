using Moq;
using NUnit.Framework;
using Sitecore.Data;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Search;
using Sitecore.Xdb.Reporting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Sitecore.Modules.WeBlog.IntegrationTest.Managers
{
    [TestFixture]
    [Category("EntryManagerFixture")]
    public class EntryManagerFixture : UnitTestBase
    {
        [Test]
        public void GetBlogEntries_NullItem()
        {
            var manager = new EntryManager();
            var entries = manager.GetBlogEntries(null, EntryCriteria.AllEntries, ListOrder.Descending);
            Assert.That(entries.Results, Is.Empty);
        }

        [Test]
        public void GetBlogEntries_NoEntries()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);

            var manager = new EntryManager();
            var entries = manager.GetBlogEntries(blog, EntryCriteria.AllEntries, ListOrder.Descending);
            Assert.That(entries.Results, Is.Empty);
        }

        [Test]
        public void GetBlogEntries_EntriesExist_ReturnsEntries()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2012, 3, 3));
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, EntryCriteria.AllEntries, ListOrder.Descending);
            var ids = from result in results.Results select result.Uri.ItemID;

            Assert.That(ids, Is.EqualTo(new[] { entryPhobos.ID, entryDeimos.ID, entryLuna.ID }));
        }

        [Test]
        public void GetBlogEntries_EntriesExist_FieldsRehydratedProperly()
        {
            // arrange
            var entryDate = new DateTime(2012, 3, 1, 13, 42, 10, DateTimeKind.Utc);
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(
                blog,
                name: "Luna",
                tags: "tag",
                entryDate: entryDate);
            
            TestUtil.UpdateIndex();

            var manager = new EntryManager();

            // act
            var results = manager.GetBlogEntries(blog, EntryCriteria.AllEntries, ListOrder.Descending);

            // assert
            var ids = from result in results.Results select result.Uri.ItemID;

            Assert.That(results.Results[0].Title, Is.EqualTo("Luna"));
            Assert.That(results.Results[0].Tags, Is.EquivalentTo(new[] { "tag" }));
            Assert.That(results.Results[0].EntryDate, Is.EqualTo(entryDate));
        }

        [Test]
        public void GetBlogEntries_WithEntriesAscending()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryLuna = TestUtil.CreateNewEntry(blog, "Luna", entryDate: new DateTime(2012, 3, 1));
            var entryDeimos = TestUtil.CreateNewEntry(blog, "Deimos", entryDate: new DateTime(2012, 3, 2));
            var entryPhobos = TestUtil.CreateNewEntry(blog, "Phobos", entryDate: new DateTime(2012, 3, 3));
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var results = manager.GetBlogEntries(blog, EntryCriteria.AllEntries, ListOrder.Ascending);
            var ids = from result in results.Results select result.Uri.ItemID;

            Assert.That(ids, Is.EqualTo(new[] { entryLuna.ID, entryDeimos.ID, entryPhobos.ID  }));
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
            var results = manager.GetBlogEntries(entryDeimos, EntryCriteria.AllEntries, ListOrder.Descending);
            var ids = from result in results.Results select result.Uri.ItemID;

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
            var criteria = new EntryCriteria
            {
                PageNumber = 1,
                PageSize = 3
            };

            var results = manager.GetBlogEntries(entryAmalthea, criteria, ListOrder.Descending);
            var ids = from result in results.Results select result.Uri.ItemID;

            Assert.That(ids, Is.EqualTo(new[] { entryAnanke.ID, entryAmalthea.ID, entryAitne.ID }));
            Assert.That(results.HasMoreResults, Is.True);
        }

        [Test]
        public void GetBlogEntries_LimitedAscending()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var entryAdrastea = TestUtil.CreateNewEntry(blog, "Adrastea", entryDate: new DateTime(2012, 3, 1));
            var entryAitne = TestUtil.CreateNewEntry(blog, "Aitne", entryDate: new DateTime(2012, 3, 2));
            var entryAmalthea = TestUtil.CreateNewEntry(blog, "Amalthea", entryDate: new DateTime(2012, 3, 3));
            var entryAnanke = TestUtil.CreateNewEntry(blog, "Ananke", entryDate: new DateTime(2012, 3, 4));
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var criteria = new EntryCriteria
            {
                PageNumber = 1,
                PageSize = 3
            };

            var results = manager.GetBlogEntries(entryAmalthea, criteria, ListOrder.Ascending);
            var ids = from result in results.Results select result.Uri.ItemID;

            Assert.That(ids, Is.EqualTo(new[] { entryAdrastea.ID, entryAitne.ID, entryAmalthea.ID }));
            Assert.That(results.HasMoreResults, Is.True);
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
            var criteria = new EntryCriteria
            {
                PageNumber = 1,
                PageSize = 2
            };

            var results = manager.GetBlogEntries(blog, criteria, ListOrder.Descending);
            var ids = from result in results.Results select result.Uri.ItemID;

            Assert.That(ids, Is.EqualTo(new[] { entryAdrastea.ID, entryPhobos.ID }));
            Assert.That(results.HasMoreResults, Is.True);
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
            var criteria = new EntryCriteria
            {
                PageNumber = 1,
                PageSize = 0
            };
            var results = manager.GetBlogEntries(blog, criteria, ListOrder.Descending);

            Assert.That(results.Results, Is.Empty);
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
            var criteria = new EntryCriteria
            {
                PageNumber = 1,
                PageSize = -7
            };
            var results = manager.GetBlogEntries(blog, criteria, ListOrder.Descending);

            Assert.That(results.Results, Is.Empty);
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
            var criteria = new EntryCriteria
            {
                PageNumber = 1,
                PageSize = 10,
                Tag = "prowl"
            };

            var results = manager.GetBlogEntries(blog, criteria, ListOrder.Descending);
            var ids = from result in results.Results select result.Uri.ItemID;

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
            var criteria = new EntryCriteria
            {
                PageNumber = 1,
                PageSize = 10,
                Tag = "orion pax"
            };

            var results = manager.GetBlogEntries(blog, criteria, ListOrder.Descending);
            var ids = from result in results.Results select result.Uri.ItemID;

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
            var criteria = new EntryCriteria
            {
                PageNumber = 1,
                PageSize = 2,
                Tag = "wheeljack"
            };

            var results = manager.GetBlogEntries(blog, criteria, ListOrder.Descending);
            var ids = from result in results.Results select result.Uri.ItemID;

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
            var criteria = new EntryCriteria
            {
                PageNumber = 1,
                PageSize = 10,
                Tag = "blurr"
            };

            var results = manager.GetBlogEntries(blog, criteria, ListOrder.Descending);

            Assert.That(results.Results, Is.Empty);
        }

        [Test]
        public void GetBlogEntries_ByCategory()
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
            var criteria = new EntryCriteria
            {
                PageNumber = 1,
                PageSize = 10,
                Category= categoryAlpha.ID.ToString()
            };

            var results = manager.GetBlogEntries(blog, criteria, ListOrder.Descending);
            var ids = from result in results.Results select result.Uri.ItemID;

            Assert.That(ids, Is.EqualTo(new[] { entryAdrastea.ID, entryPhobos.ID, entryDeimos.ID }));
        }

        [Test]
        public void GetBlogEntries_ByInvalidCategory()
        {
            var blog = TestUtil.CreateNewBlog(TestContentRoot);
            var categoryAlpha = TestUtil.CreateNewCategory(blog, "Alpha");
            var categoryBeta = TestUtil.CreateNewCategory(blog, "Beta");
            TestUtil.CreateNewEntry(blog, "Luna", categories: new[] { categoryBeta.ID }, entryDate: new DateTime(2012, 3, 1));
            TestUtil.CreateNewEntry(blog, "Deimos", categories: new[] { categoryAlpha.ID, categoryBeta.ID }, entryDate: new DateTime(2012, 3, 2));
            TestUtil.CreateNewEntry(blog, "Phobos", categories: new[] { categoryBeta.ID, categoryAlpha.ID }, entryDate: new DateTime(2012, 3, 3));
            TestUtil.CreateNewEntry(blog, "Adrastea", categories: new[] { categoryBeta.ID, categoryAlpha.ID }, entryDate: new DateTime(2012, 3, 4));
            TestUtil.UpdateIndex();

            var manager = new EntryManager();
            var criteria = new EntryCriteria
            {
                PageNumber = 1,
                PageSize = 10,
                Category = ID.NewID.ToString()
            };

            var results = manager.GetBlogEntries(blog, criteria, ListOrder.Descending);

            Assert.That(results.Results, Is.Empty);
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
            var criteria = new EntryCriteria
            {
                PageNumber = 1,
                PageSize = 1,
                Category = categoryAlpha.ID.ToString()
            };

            var results = manager.GetBlogEntries(blog, criteria, ListOrder.Descending);
            var ids = from result in results.Results select result.Uri.ItemID;

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
            var criteria = new EntryCriteria
            {
                PageNumber = 1,
                PageSize = 10,
                MinimumDate = new DateTime(2014, 11, 1),
                MaximumDate = new DateTime(2014, 12, 20)
            };

            var results = manager.GetBlogEntries(blog, criteria, ListOrder.Descending);
            var ids = from result in results.Results select result.Uri.ItemID;

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
            var criteria = new EntryCriteria
            {
                PageNumber = 1,
                PageSize = 2,
                MinimumDate = new DateTime(2014, 11, 1),
                MaximumDate = new DateTime(2015, 1, 20)
            };

            var results = manager.GetBlogEntries(blog, criteria, ListOrder.Descending);
            var ids = from result in results.Results select result.Uri.ItemID;

            Assert.That(ids, Is.EqualTo(new[] { entryAdrastea.ID, entryPhobos.ID }));
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
            var entryIds = from entry in entries select entry.ItemID;

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
            var entryIds = from entry in entries select entry.ItemID;

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
            var entryIds = from entry in entries select entry.ItemID;

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
            var entryIds = from entry in entries select entry.ItemID;

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
            var entryIds = from entry in entries select entry.ItemID;

            Assert.That(entryIds, Is.EqualTo(new[] { entryLuna.ID }));
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
            bool enabled = Sitecore.Configuration.Settings.GetBoolSetting("Analytics.Enabled", false)
                || Sitecore.Configuration.Settings.GetBoolSetting("Xdb.Enabled", false);

            Assert.True(enabled, "Sitecore.Analytics must be enabled to test");
        }

        private EntryManager CreateEntryManagerForAnalyticsTest(params ID[] popularEntryIdsInOrder)
        {
            var reportProvider = CreateMockReportDataProvider(popularEntryIdsInOrder);
            return new EntryManager(reportProvider, null, null, null, null);
        }

        private ReportDataProviderBase CreateMockReportDataProvider(IEnumerable<ID> ids)
        {
            var reportingProviderMock = new Mock<ReportDataProviderBase>();

            var visitCount = ids.Count();
            foreach (var id in ids)
            {
                var dataTable = new System.Data.DataTable();
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
    }
}