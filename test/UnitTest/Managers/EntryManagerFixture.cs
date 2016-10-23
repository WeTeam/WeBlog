using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;
using Moq;
using NUnit.Framework;
using Sitecore.Analytics.Reporting;
using Sitecore.Buckets.Util;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.LuceneProvider.Analyzers;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.ContentSearch.Security;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Search.SearchTypes;
using Sitecore.Modules.WeBlog.UnitTest.Extensions;

namespace Sitecore.Modules.WeBlog.UnitTest
{
    [TestFixture]
    public class EntryManagerFixture
    {
        [Test]
        public void GetCurrentBlogEntry_Null()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new EntryManager(null, settings);
            var entry = manager.GetCurrentBlogEntry(null);
            Assert.That(entry, Is.Null);
        }

        [Test]
        public void GetCurrentBlogEntry_ItemFromTemplate()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new EntryManager(null, settings);

            using (var db = new Db
            {
                new DbItem("entry", ID.NewID, settings.EntryTemplateIds.First())
            })
            {
                var item = db.GetItem("/sitecore/content/entry");

                var entryItem = manager.GetCurrentBlogEntry(item);
                Assert.That(entryItem.ID, Is.EqualTo(item.ID));
            }
        }

        [Test]
        public void GetCurrentBlogEntry_ItemDerivedTemplate()
        {
            var baseBaseTemplateId = ID.NewID;
            var baseTemplateId = ID.NewID;
            var entryTemplateId = ID.NewID;

            var settings = MockSettings(baseBaseTemplateId);
            var manager = new EntryManager(null, settings);

            using (var db = new Db
            {
                new DbTemplate(baseBaseTemplateId),
                new DbTemplate(baseTemplateId)
                {
                    BaseIDs = new [] { baseBaseTemplateId }
                },
                new DbTemplate(entryTemplateId)
                {
                    BaseIDs = new [] { baseTemplateId }
                },
                new DbItem("entry", ID.NewID, entryTemplateId)
            })
            {
                var item = db.GetItem("/sitecore/content/entry");

                var entryItem = manager.GetCurrentBlogEntry(item);
                Assert.That(entryItem.ID, Is.EqualTo(item.ID));
            }
        }

        [Test]
        public void GetCurrentBlogEntry_ItemNotCorrectTemplate()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new EntryManager(null, settings);

            using (var db = new Db
            {
                new DbItem("entry", ID.NewID, ID.NewID)
            })
            {
                var item = db.GetItem("/sitecore/content/entry");

                var entryItem = manager.GetCurrentBlogEntry(item);
                Assert.That(entryItem, Is.Null);
            }
        }

        [Test]
        public void DeleteEntry_NullID()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new EntryManager(null, settings);

            using (var db = new Db())
            {
                var item = db.GetItem("/sitecore/content");
                var result = manager.DeleteEntry(null, item.Database);

                Assert.That(result, Is.False);
            }
        }

        [Test]
        public void DeleteEntry_NullDatabase()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new EntryManager(null, settings);

            using (var db = new Db
            {
                new DbItem("item")
            })
            {
                Assert.That(() => manager.DeleteEntry("/sitecore/content/item", null), Throws.InvalidOperationException);
            }
        }

        [Test]
        public void DeleteEntry_InvalidID()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new EntryManager(null, settings);

            using (var db = new Db
            {
                new DbItem("item")
            })
            {
                var item = db.GetItem("/sitecore/content/item");
                var result = manager.DeleteEntry(ID.NewID.ToString(), item.Database);

                Assert.That(result, Is.False);
            }
        }

        [Test]
        public void DeleteEntry_ValidID()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new EntryManager(null, settings);

            using (var db = new Db
            {
                new DbItem("item")
            })
            {
                var item = db.GetItem("/sitecore/content/item");
                var result = manager.DeleteEntry("/sitecore/content/item", item.Database);

                Assert.That(result, Is.True);
            }
        }

        [Test]
        public void DeleteEntry_UnauthorizedUser()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new EntryManager(null, settings);

            using (var db = new Db
            {
                new DbItem("item")
                {
                    Access =
                    {
                        CanDelete = false
                    }
                }
            })
            {
                var item = db.GetItem("/sitecore/content/item");
                var result = manager.DeleteEntry("/sitecore/content/item", item.Database);

                Assert.That(result, Is.False);
            }
        }

        // GetBlogEntries tests use Content Search, so test those methods with integration tests

        [Test]
        public void GetBlogEntries_NullItem()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");
                    var entry1 = db.GetItem("/sitecore/content/blog/2016/entry1");

                    MockIndex().IndexItems(new[] { MockEntryItem(entry1, blogItem).Object });

                    // Act
                    var entries = manager.GetBlogEntries((Item)null);

                    // Assert
                    Assert.That(entries, Is.Empty);
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetBlogEntries_NoEntries()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First())
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");

                    MockIndex().IndexItems(new List<EntryResultItem>(0));

                    // Act
                    blogItem.DeleteChildren();
                    var entries = manager.GetBlogEntries(blogItem);

                    // Assert
                    Assert.That(entries, Is.Empty);
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetBlogEntries_WithEntries()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");
                    var entry1 = db.GetItem("/sitecore/content/blog/2016/entry1");
                    var entry2 = db.GetItem("/sitecore/content/blog/2016/entry2");
                    var entry3 = db.GetItem("/sitecore/content/blog/2016/entry3");

                    IndexEntries(blogItem, settings);

                    // Act
                    var entries = manager.GetBlogEntries(blogItem);

                    // Assert
                    var ids = from result in entries select result.ID;
                    Assert.That(ids, Is.EqualTo(new[] { entry1.ID, entry2.ID, entry3.ID }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetBlogEntries_WithEntriesMultipleBlogs()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First())
                    }
                },
                new DbItem("blog-second", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");
                    var entry1 = db.GetItem("/sitecore/content/blog/2016/entry1");
                    var entry2 = db.GetItem("/sitecore/content/blog/2016/entry2");
                    var entry3 = db.GetItem("/sitecore/content/blog/2016/entry3");

                    IndexEntries(blogItem, settings);

                    // Act
                    var entries = manager.GetBlogEntries(entry2);

                    // Assert
                    var ids = from result in entries select result.ID;
                    Assert.That(ids, Is.EqualTo(new[] { entry1.ID, entry2.ID, entry3.ID }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetBlogEntries_LimitedMultipleBlogs()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First())
                    }
                },
                new DbItem("blog-second", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry4", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog-second");
                    var entry1 = db.GetItem("/sitecore/content/blog-second/2016/entry1");
                    var entry2 = db.GetItem("/sitecore/content/blog-second/2016/entry2");
                    var entry3 = db.GetItem("/sitecore/content/blog-second/2016/entry3");

                    IndexEntries(blogItem, settings);

                    // Act
                    var entries = manager.GetBlogEntries(entry2, 3, null, null, null, null);

                    // Assert
                    var ids = from result in entries select result.ID;
                    Assert.That(ids, Is.EqualTo(new[] { entry1.ID, entry2.ID, entry3.ID }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetBlogEntries_LimitEntries()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");
                    var entry1 = db.GetItem("/sitecore/content/blog/2016/entry1");
                    var entry2 = db.GetItem("/sitecore/content/blog/2016/entry2");

                    IndexEntries(blogItem, settings);

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, 2, null, null, null, null);

                    // Assert
                    var ids = from result in entries select result.ID;
                    Assert.That(ids, Is.EqualTo(new[] { entry1.ID, entry2.ID }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetBlogEntries_LimitEntriesZero()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");

                    IndexEntries(blogItem, settings);

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, 0, null, null, null, null);

                    // Assert
                    Assert.That(entries, Is.Empty);
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetBlogEntries_LimitEntriesNegativeLimit()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");

                    IndexEntries(blogItem, settings);

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, -7, null, null, null, null);

                    // Assert
                    Assert.That(entries, Is.Empty);
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetBlogEntries_ByTag()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()) { { "Tags", "wheeljack, prowl" } },
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()){ { "Tags", "prowl, cliffjumper" } },
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First()){ { "Tags", "wheeljack, cliffjumper" } }
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");
                    var entry1 = db.GetItem("/sitecore/content/blog/2016/entry1");
                    var entry2 = db.GetItem("/sitecore/content/blog/2016/entry2");

                    IndexEntries(blogItem, settings);

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, 10, "prowl", null, null, null);

                    // Assert
                    var ids = from result in entries select result.ID;
                    Assert.That(ids, Is.EqualTo(new[] { entry1.ID, entry2.ID }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetBlogEntries_ByTagWithSpace()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()) { { "Tags", "wheeljack, prowl" } },
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()){ { "Tags", "prowl, orion pax, cliffjumper" } },
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First()){ { "Tags", "wheeljack, hot rod, orion pax" } }
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");
                    var entry2 = db.GetItem("/sitecore/content/blog/2016/entry2");
                    var entry3 = db.GetItem("/sitecore/content/blog/2016/entry3");

                    IndexEntries(blogItem, settings);

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, 10, "orion pax", null, null, null);

                    // Assert
                    var ids = from result in entries select result.ID;
                    Assert.That(ids, Is.EqualTo(new[] { entry2.ID, entry3.ID }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetBlogEntries_ByTagLimited()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()) { { "Tags", "wheeljack, prowl" } },
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()){ { "Tags", "prowl, orion pax, cliffjumper" } },
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First()){ { "Tags", "wheeljack, hot rod, orion pax" } },
                        new DbItem("entry4", ID.NewID, settings.EntryTemplateIds.First()){ { "Tags", "wheeljack, hot rod, orion pax, prowl" } }
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");
                    var entry1 = db.GetItem("/sitecore/content/blog/2016/entry1");
                    var entry3 = db.GetItem("/sitecore/content/blog/2016/entry3");

                    IndexEntries(blogItem, settings);

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, 2, "wheeljack", null, null, null);


                    // Assert
                    var ids = from result in entries select result.ID;
                    Assert.That(ids, Is.EqualTo(new[] { entry1.ID, entry3.ID }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetBlogEntries_ByInvalidTag()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()) { { "Tags", "wheeljack, prowl" } },
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()){ { "Tags", "prowl, orion pax, cliffjumper" } },
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First()){ { "Tags", "wheeljack, hot rod, orion pax" } },
                        new DbItem("entry4", ID.NewID, settings.EntryTemplateIds.First()){ { "Tags", "wheeljack, hot rod, orion pax, prowl" } }
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");

                    IndexEntries(blogItem, settings);

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, 10, "blurr", null, null, null);


                    // Assert
                    Assert.That(entries, Is.Empty);
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetBlogEntries_ByCategory()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            ID alphaId = ID.NewID;
            ID betaId = ID.NewID;
            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    {"Defined Category Template",settings.CategoryTemplateIds.First().ToString()},
                    {"Defined Entry Template",settings.EntryTemplateIds.First().ToString()},
                    {"Defined Comment Template",settings.CommentTemplateIds.First().ToString()},
                    new DbItem("Categories",ID.NewID)
                    {
                        new DbItem("CategoryAlpha", alphaId) ,new DbItem("CategoryBeta",betaId)
                    },
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()) { { "Category", string.Join<ID>("|", new[] { betaId })} },
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()){ { "Category", string.Join<ID>("|", new[] {alphaId, betaId })} },
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First()){ { "Category", string.Join<ID>("|", new[] { betaId,alphaId })} },
                        new DbItem("entry4", ID.NewID, settings.EntryTemplateIds.First()){ { "Category", string.Join<ID>("|", new[] { betaId,alphaId })} }
                    }
                }
            })
            {
                try
                {
                    // Assign
                    db.Configuration.Settings["WeBlog.BlogTemplateID"] = settings.BlogTemplateIds.First().ToString();

                    var blogItem = db.GetItem("/sitecore/content/blog");
                    var entry2 = db.GetItem("/sitecore/content/blog/2016/entry2");
                    var entry3 = db.GetItem("/sitecore/content/blog/2016/entry3");
                    var entry4 = db.GetItem("/sitecore/content/blog/2016/entry4");

                    IndexEntries(blogItem, settings);

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, 10, null, alphaId.ToString(), null, null);


                    // Assert
                    var ids = from result in entries select result.ID;
                    Assert.That(ids, Is.EqualTo(new[] { entry2.ID, entry3.ID, entry4.ID }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetBlogEntries_ByInvalidCategory()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            ID alphaId = ID.NewID;
            ID betaId = ID.NewID;
            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    {"Defined Category Template",settings.CategoryTemplateIds.First().ToString()},
                    {"Defined Entry Template",settings.EntryTemplateIds.First().ToString()},
                    {"Defined Comment Template",settings.CommentTemplateIds.First().ToString()},
                    new DbItem("Categories",ID.NewID)
                    {
                        new DbItem("CategoryAlpha", alphaId) ,new DbItem("CategoryBeta",betaId)
                    },
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()) { { "Category", string.Join<ID>("|", new[] { betaId })} },
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()){ { "Category", string.Join<ID>("|", new[] {alphaId, betaId })} },
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First()){ { "Category", string.Join<ID>("|", new[] { betaId,alphaId })} },
                        new DbItem("entry4", ID.NewID, settings.EntryTemplateIds.First()){ { "Category", string.Join<ID>("|", new[] { betaId,alphaId })} }
                    }
                }
            })
            {
                try
                {
                    // Assign
                    db.Configuration.Settings["WeBlog.BlogTemplateID"] = settings.BlogTemplateIds.First().ToString();

                    var blogItem = db.GetItem("/sitecore/content/blog");

                    IndexEntries(blogItem, settings);

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, 10, null, ID.NewID.ToString(), null, null);


                    // Assert
                    Assert.That(entries, Is.Empty);
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetBlogEntries_ByCategoryLimited()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            ID alphaId = ID.NewID;
            ID betaId = ID.NewID;
            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    {"Defined Category Template",settings.CategoryTemplateIds.First().ToString()},
                    {"Defined Entry Template",settings.EntryTemplateIds.First().ToString()},
                    {"Defined Comment Template",settings.CommentTemplateIds.First().ToString()},
                    new DbItem("Categories",ID.NewID)
                    {
                        new DbItem("CategoryAlpha", alphaId) ,new DbItem("CategoryBeta",betaId)
                    },
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()) { { "Category", string.Join<ID>("|", new[] { betaId })} },
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()){ { "Category", string.Join<ID>("|", new[] {alphaId, betaId })} },
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First()){ { "Category", string.Join<ID>("|", new[] { betaId,alphaId })} },
                        new DbItem("entry4", ID.NewID, settings.EntryTemplateIds.First()){ { "Category", string.Join<ID>("|", new[] { betaId,alphaId })} }
                    }
                }
            })
            {
                try
                {
                    // Assign
                    db.Configuration.Settings["WeBlog.BlogTemplateID"] = settings.BlogTemplateIds.First().ToString();

                    var blogItem = db.GetItem("/sitecore/content/blog");
                    var entry2 = db.GetItem("/sitecore/content/blog/2016/entry2");

                    IndexEntries(blogItem, settings);

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, 1, null, alphaId.ToString(), null, null);


                    // Assert
                    var ids = from result in entries select result.ID;
                    Assert.That(ids, Is.EqualTo(new[] { entry2.ID }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetBlogEntries_InDateRange()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2014, 10, 1)) } },
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2014, 11, 1)) } },
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2014, 12, 1)) } },
                        new DbItem("entry4", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2015, 1, 1)) } }
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");
                    var entry2 = db.GetItem("/sitecore/content/blog/2016/entry2");
                    var entry3 = db.GetItem("/sitecore/content/blog/2016/entry3");

                    IndexEntries(blogItem, settings);

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, 10, null, null, new DateTime(2014, 11, 1), new DateTime(2014, 12, 20));

                    // Assert
                    var ids = from result in entries select result.ID;
                    Assert.That(ids, Is.EqualTo(new[] { entry3.ID, entry2.ID }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetBlogEntries_InDateRangeLimited()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2014, 10, 1)) } },
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2014, 11, 1)) } },
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2014, 12, 1)) } },
                        new DbItem("entry4", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2015, 1, 1)) } }
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");
                    var entry3 = db.GetItem("/sitecore/content/blog/2016/entry3");
                    var entry4 = db.GetItem("/sitecore/content/blog/2016/entry4");

                    IndexEntries(blogItem, settings);

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, 2, null, null, new DateTime(2014, 11, 1), new DateTime(2015, 1, 20));

                    // Assert
                    var ids = from result in entries select result.ID;
                    Assert.That(ids, Is.EqualTo(new[] { entry4.ID, entry3.ID }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetBlogEntriesByMonthAndYear_BeforeEntries()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2012, 3, 1)) } },
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2012, 3, 2)) } },
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2012, 4, 3)) } },
                        new DbItem("entry4", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2012, 5, 4)) } }
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");

                    IndexEntries(blogItem, settings);

                    // Act
                    var entries = manager.GetBlogEntriesByMonthAndYear(blogItem, 1, 2012);

                    // Assert
                    Assert.That(entries, Is.Empty);
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetBlogEntriesByMonthAndYear_WithinEntries()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2012, 3, 1)) } },
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2012, 3, 2)) } },
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2012, 4, 3)) } },
                        new DbItem("entry4", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2012, 5, 4)) } }
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");
                    var entry1 = db.GetItem("/sitecore/content/blog/2016/entry1");
                    var entry2 = db.GetItem("/sitecore/content/blog/2016/entry2");

                    IndexEntries(blogItem, settings);

                    // Act
                    var entries = manager.GetBlogEntriesByMonthAndYear(blogItem, 3, 2012);

                    // Assert
                    var ids = from result in entries select result.ID;
                    Assert.That(ids, Is.EqualTo(new[] { entry2.ID, entry1.ID }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetBlogEntriesByMonthAndYear_LastMonth()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2012, 12, 30)) } },
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2012, 12, 31)) } },
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2012, 1, 1)) } },
                        new DbItem("entry4", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2013, 1, 1)) } }
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");
                    var entry1 = db.GetItem("/sitecore/content/blog/2016/entry1");
                    var entry2 = db.GetItem("/sitecore/content/blog/2016/entry2");

                    IndexEntries(blogItem, settings);

                    // Act
                    var entries = manager.GetBlogEntriesByMonthAndYear(blogItem, 12, 2012);

                    // Assert
                    var ids = from result in entries select result.ID;
                    Assert.That(ids, Is.EqualTo(new[] { entry2.ID, entry1.ID }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetBlogEntriesByMonthAndYear_FirstMonth()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2012, 12, 30)) } },
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2012, 12, 31)) } },
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2013, 1, 1)) } },
                        new DbItem("entry4", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2013, 1, 1)) } }
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");
                    var entry3 = db.GetItem("/sitecore/content/blog/2016/entry3");
                    var entry4 = db.GetItem("/sitecore/content/blog/2016/entry4");

                    IndexEntries(blogItem, settings);

                    // Act
                    var entries = manager.GetBlogEntriesByMonthAndYear(blogItem, 1, 2013);

                    // Assert
                    var ids = from result in entries select result.ID;
                    Assert.That(ids, Is.EqualTo(new[] { entry3.ID, entry4.ID }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetBlogEntriesByMonthAndYear_AfterEntries()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2012, 3, 1)) } },
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2012, 3, 2)) } },
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2012, 4, 3)) } },
                        new DbItem("entry4", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2012, 5, 4)) } }
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");

                    IndexEntries(blogItem, settings);

                    // Act
                    var entries = manager.GetBlogEntriesByMonthAndYear(blogItem, 6, 2012);

                    // Assert
                    Assert.That(entries, Is.Empty);
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetBlogEntriesByMonthAndYear_InvalidDate()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2012, 3, 1)) } },
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2012, 3, 2)) } },
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2012, 3, 3)) } },
                        new DbItem("entry4", ID.NewID, settings.EntryTemplateIds.First()) { { "Entry Date", DateUtil.ToIsoDate(new DateTime(2012, 3, 4)) } }
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");

                    IndexEntries(blogItem, settings);

                    // Act
                    var entries = manager.GetBlogEntriesByMonthAndYear(blogItem, 17, 209992);

                    // Assert
                    Assert.That(entries, Is.Empty);
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetPopularEntriesByComment_ValidItem()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    {"Defined Category Template",settings.CategoryTemplateIds.First().ToString()},
                    {"Defined Entry Template",settings.EntryTemplateIds.First().ToString()},
                    {"Defined Comment Template",settings.CommentTemplateIds.First().ToString()},
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First())
                        {
                            new DbItem("Comment1", ID.NewID,settings.CommentTemplateIds.First())
                        },
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First())
                        {
                            new DbItem("Comment1", ID.NewID,settings.CommentTemplateIds.First()),
                            new DbItem("Comment2", ID.NewID,settings.CommentTemplateIds.First())
                        },
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First())
                        {
                            new DbItem("Comment1", ID.NewID,settings.CommentTemplateIds.First()),
                            new DbItem("Comment2", ID.NewID,settings.CommentTemplateIds.First()),
                            new DbItem("Comment3", ID.NewID,settings.CommentTemplateIds.First())
                        },
                        new DbItem("entry4", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                try
                {
                    // Assign
                    db.Configuration.Settings["WeBlog.BlogTemplateID"] = settings.BlogTemplateIds.First().ToString();

                    var blogItem = db.GetItem("/sitecore/content/blog");
                    var entry1 = db.GetItem("/sitecore/content/blog/2016/entry1");
                    var entry2 = db.GetItem("/sitecore/content/blog/2016/entry2");
                    var entry3 = db.GetItem("/sitecore/content/blog/2016/entry3");

                    var mockIndex = MockIndex();

                    IndexEntries(blogItem, settings, mockIndex);
                    IndexComments(blogItem, settings, mockIndex);

                    // Act
                    var entries = manager.GetPopularEntriesByComment(blogItem, 10);

                    // Assert
                    var ids = from result in entries select result.ID;
                    Assert.That(ids, Is.EqualTo(new[] { entry3.ID, entry2.ID, entry1.ID }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetPopularEntriesByComment_ValidItem_Limited()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    {"Defined Category Template",settings.CategoryTemplateIds.First().ToString()},
                    {"Defined Entry Template",settings.EntryTemplateIds.First().ToString()},
                    {"Defined Comment Template",settings.CommentTemplateIds.First().ToString()},
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First())
                        {
                            new DbItem("Comment1", ID.NewID,settings.CommentTemplateIds.First())
                        },
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First())
                        {
                            new DbItem("Comment1", ID.NewID,settings.CommentTemplateIds.First()),
                            new DbItem("Comment2", ID.NewID,settings.CommentTemplateIds.First())
                        },
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First())
                        {
                            new DbItem("Comment1", ID.NewID,settings.CommentTemplateIds.First()),
                            new DbItem("Comment2", ID.NewID,settings.CommentTemplateIds.First()),
                            new DbItem("Comment3", ID.NewID,settings.CommentTemplateIds.First())
                        },
                        new DbItem("entry4", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                try
                {
                    // Assign
                    db.Configuration.Settings["WeBlog.BlogTemplateID"] = settings.BlogTemplateIds.First().ToString();

                    var blogItem = db.GetItem("/sitecore/content/blog");
                    var entry2 = db.GetItem("/sitecore/content/blog/2016/entry2");
                    var entry3 = db.GetItem("/sitecore/content/blog/2016/entry3");

                    var mockIndex = MockIndex();

                    IndexEntries(blogItem, settings, mockIndex);
                    IndexComments(blogItem, settings, mockIndex);

                    // Act
                    var entries = manager.GetPopularEntriesByComment(blogItem, 2);

                    // Assert
                    var ids = from result in entries select result.ID;
                    Assert.That(ids, Is.EqualTo(new[] { entry3.ID, entry2.ID }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetPopularEntriesByComment_InvalidItem()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    {"Defined Category Template",settings.CategoryTemplateIds.First().ToString()},
                    {"Defined Entry Template",settings.EntryTemplateIds.First().ToString()},
                    {"Defined Comment Template",settings.CommentTemplateIds.First().ToString()},
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First())
                        {
                            new DbItem("Comment1", ID.NewID,settings.CommentTemplateIds.First())
                        },
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First())
                        {
                            new DbItem("Comment1", ID.NewID,settings.CommentTemplateIds.First()),
                            new DbItem("Comment2", ID.NewID,settings.CommentTemplateIds.First())
                        },
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First())
                        {
                            new DbItem("Comment1", ID.NewID,settings.CommentTemplateIds.First()),
                            new DbItem("Comment2", ID.NewID,settings.CommentTemplateIds.First()),
                            new DbItem("Comment3", ID.NewID,settings.CommentTemplateIds.First())
                        },
                        new DbItem("entry4", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                try
                {
                    // Assign
                    db.Configuration.Settings["WeBlog.BlogTemplateID"] = settings.BlogTemplateIds.First().ToString();

                    var blogItem = db.GetItem("/sitecore/content/blog");
                    var entry2 = db.GetItem("/sitecore/content/blog/2016/entry2");

                    var mockIndex = MockIndex();

                    IndexEntries(blogItem, settings, mockIndex);
                    IndexComments(blogItem, settings, mockIndex);

                    // Act
                    var entries = manager.GetPopularEntriesByComment(entry2, 10);

                    // Assert
                    var ids = from result in entries select result.ID;
                    Assert.That(ids, Is.EqualTo(new[] { entry2.ID }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetPopularEntriesByComment_NullItem()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    {"Defined Category Template",settings.CategoryTemplateIds.First().ToString()},
                    {"Defined Entry Template",settings.EntryTemplateIds.First().ToString()},
                    {"Defined Comment Template",settings.CommentTemplateIds.First().ToString()},
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                try
                {
                    // Assign
                    db.Configuration.Settings["WeBlog.BlogTemplateID"] = settings.BlogTemplateIds.First().ToString();

                    var blogItem = db.GetItem("/sitecore/content/blog");

                    var mockIndex = MockIndex();

                    IndexEntries(blogItem, settings, mockIndex);
                    IndexComments(blogItem, settings, mockIndex);

                    // Act
                    var entries = manager.GetPopularEntriesByComment(blogItem, 10);

                    // Assert
                    var ids = from result in entries select result.ID;
                    Assert.That(ids, Is.Empty);
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetPopularEntriesByView_ValidItem()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, ID.NewID)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");
                    var entry1 = db.GetItem("/sitecore/content/blog/2016/entry1");
                    var entry2 = db.GetItem("/sitecore/content/blog/2016/entry2");
                    var entry3 = db.GetItem("/sitecore/content/blog/2016/entry3");

                    IndexEntries(blogItem, settings);

                    db.Configuration.Settings["Xdb.Enabled"] = true.ToString();

                    // Act
                    var entries = manager.GetPopularEntriesByView(blogItem, int.MaxValue);

                    // Assert
                    var ids = from result in entries select result.ID;
                    Assert.That(ids, Is.EqualTo(new[] { entry1.ID, entry2.ID, entry3.ID }));

                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetPopularEntriesByView_ValidItem_Limited()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, ID.NewID)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");
                    var entry1 = db.GetItem("/sitecore/content/blog/2016/entry1");

                    IndexEntries(blogItem, settings);

                    db.Configuration.Settings["Xdb.Enabled"] = true.ToString();

                    // Act
                    var entries = manager.GetPopularEntriesByView(blogItem, 1);

                    // Assert
                    var ids = from result in entries select result.ID;
                    Assert.That(ids, Is.EqualTo(new[] { entry1.ID }));

                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetPopularEntriesByView_InvalidItem()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, ID.NewID)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First())
                    }
                },
                new DbItem("folder")
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");
                    var folder = db.GetItem("/sitecore/content/folder");

                    IndexEntries(blogItem, settings);

                    db.Configuration.Settings["Xdb.Enabled"] = true.ToString();

                    // Act
                    var entries = manager.GetPopularEntriesByView(folder, int.MaxValue);

                    // Assert
                    Assert.That(entries, Is.Empty);
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        [Test]
        public void GetPopularEntriesByView_NullItem()
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, ID.NewID)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry2", ID.NewID, settings.EntryTemplateIds.First()),
                        new DbItem("entry3", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");

                    IndexEntries(blogItem, settings);

                    db.Configuration.Settings["Xdb.Enabled"] = true.ToString();

                    // Act
                    var entries = manager.GetPopularEntriesByView(null, int.MaxValue);

                    // Assert
                    Assert.That(entries, Is.Empty);
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }
            }
        }

        private Mock<ISearchIndex> MockIndex(string indexName = "WeBlog")
        {
            var index = new Mock<ISearchIndex>();
            ContentSearchManager.SearchConfiguration.Indexes.Add(indexName, index.Object);
            return index;
        }

        private void IndexComments(Item blogItem, IWeBlogSettings settings, Mock<ISearchIndex> index = null)
        {
            IndexItems(blogItem, settings.CommentTemplateIds.First(), MockCommentItem, index);
        }

        private void IndexEntries(Item blogItem, IWeBlogSettings settings, Mock<ISearchIndex> index = null)
        {
            IndexItems(blogItem, settings.EntryTemplateIds.First(), MockEntryItem, index);
        }

        private void IndexItems<T>(Item blogItem, ID template, Func<Item, Item, Mock<T>> mockFunc, Mock<ISearchIndex> index = null)
            where T : SearchResultItem
        {
            index = index ?? MockIndex();
            blogItem.Axes.GetDescendants()
                .Where(item => item.TemplateID.Equals(template))
                .Select(item => mockFunc(item, blogItem).Object)
                .IndexItems(index);
        }

        private Mock<EntryResultItem> MockEntryItem(Item entryItem, Item blogItem)
        {
            var srItem = new Mock<EntryResultItem>();
            srItem.Setup(x => x.GetItem()).Returns(entryItem);
            srItem.Setup(x => x.TemplateId).Returns((new BlogHomeItem(blogItem)).BlogSettings.EntryTemplateID);
            srItem.Setup(x => x.Paths).Returns(GetPaths(entryItem));
            srItem.Setup(x => x.Language).Returns(blogItem.Language.ToString);
            srItem.Setup(x => x.DatabaseName).Returns(blogItem.Database.Name);
            srItem.Setup(x => x.Tags).Returns(GetTags(entryItem));
            srItem.Setup(x => x.Category).Returns(GetCategories(entryItem));
            srItem.Setup(x => x.EntryDate).Returns(GetEntryData(entryItem));
            return srItem;
        }

        private Mock<CommentResultItem> MockCommentItem(Item commentItem, Item blogItem)
        {
            var srItem = new Mock<CommentResultItem>();
            srItem.Setup(x => x.GetItem()).Returns(commentItem);
            srItem.Setup(x => x.TemplateId).Returns((new BlogHomeItem(blogItem)).BlogSettings.CommentTemplateID);
            srItem.Setup(x => x.Paths).Returns(GetPaths(commentItem));
            srItem.Setup(x => x.Language).Returns(blogItem.Language.ToString);
            srItem.Setup(x => x.DatabaseName).Returns(blogItem.Database.Name);
            return srItem;
        }

        private static IEnumerable<ID> GetPaths(Item commentItem)
        {
            return commentItem.Paths.LongID.Split('/')
                .Where(idStr => !String.IsNullOrEmpty(idStr))
                .Select(idStr => new ID(idStr));
        }

        private DateTime GetEntryData(Item item)
        {
            return DateUtil.IsoDateToDateTime(item.Fields["Entry Date"]?.Value, DateTime.MinValue);
        }

        private ID[] GetCategories(Item item)
        {
            var value = item.Fields["Category"]?.Value;
            if (String.IsNullOrEmpty(value))
            {
                return new ID[0];
            }
            return value.Split('|').Select(s => new ID(s)).ToArray();
        }

        private string GetTags(Item item)
        {
            // Based on tags field configuration for Content Search
            var analyzer = new LowerCaseKeywordAnalyzer();

            var value = item.Fields["Tags"]?.Value;
            if (String.IsNullOrEmpty(value))
            {
                return String.Empty;
            }
            using (TokenStream tokenStream = analyzer.TokenStream("tags", new StringReader(value)))
            {
                ITermAttribute termAttribute = tokenStream.AddAttribute<ITermAttribute>();
                List<string> stringList = new List<string>();
                while (tokenStream.IncrementToken())
                {
                    string term = termAttribute.Term;
                    stringList.Add(term);
                }
                tokenStream.End();
                return stringList.FirstOrDefault();
            }
        }

        [Test]
        public void GetBlogEntryByComment_NullItem()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new EntryManager(null, settings);
            var foundEntry = manager.GetBlogEntryByComment(null);

            Assert.That(foundEntry, Is.Null);
        }

        [Test]
        public void GetBlogEntryByComment_OnEntry()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new EntryManager(null, settings);

            using (var db = new Db
            {
                new DbItem("entry", ID.NewID, settings.EntryTemplateIds.First())
                {
                    new DbItem("2013", ID.NewID, ID.NewID)
                    {
                        new DbItem("comment", ID.NewID, settings.CommentTemplateIds.First())
                    }
                }
            })
            {
                var item = db.GetItem("/sitecore/content/entry");
                var result = manager.GetBlogEntryByComment(item);

                Assert.That(result.ID, Is.EqualTo(item.ID));
            }
        }

        [Test]
        public void GetBlogEntryByComment_UnderEntry()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new EntryManager(null, settings);

            using (var db = new Db
            {
                new DbItem("entry", ID.NewID, settings.EntryTemplateIds.First())
                {
                    new DbItem("2013", ID.NewID, ID.NewID)
                    {
                        new DbItem("comment", ID.NewID, settings.CommentTemplateIds.First())
                    }
                }
            })
            {
                var commentItem = db.GetItem("/sitecore/content/entry/2013/comment");
                var entryItem = db.GetItem("/sitecore/content/entry");
                var result = manager.GetBlogEntryByComment(commentItem);

                Assert.That(result.ID, Is.EqualTo(entryItem.ID));
            }
        }

        [Test]
        public void GetBlogEntryByComment_OutsideEntry()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new EntryManager(null, settings);

            using (var db = new Db
            {
                new DbItem("entry", ID.NewID, settings.EntryTemplateIds.First())
                {
                    new DbItem("2013", ID.NewID, ID.NewID)
                    {
                        new DbItem("comment", ID.NewID, settings.CommentTemplateIds.First())
                    }
                }
            })
            {
                var item = db.GetItem("/sitecore/content");
                var result = manager.GetBlogEntryByComment(item);

                Assert.That(result, Is.Null);
            }
        }

        [Test]
        [TestCase("Analytics.Enabled", "false", 0, TestName = "Analytics disabled")]
        [TestCase("Analytics.Enabled", "true", 1, TestName = "Analytics enabled")]
        [TestCase("Xdb.Enabled", "false", 0, TestName = "Xdb disabled")]
        [TestCase("Xdb.Enabled", "true", 1)]
        public void GetPopularEntriesByView_DifferentAnalyticsState(string settingName, string settingValue, int expected)
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2013", ID.NewID, ID.NewID)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");
                    var entry1 = db.GetItem("/sitecore/content/blog/2013/entry1");
                    var index = new Mock<ISearchIndex>();
                    ContentSearchManager.SearchConfiguration.Indexes.Add("WeBlog", index.Object);

                    var srItem = new Mock<EntryResultItem>();
                    srItem.Setup(x => x.GetItem()).Returns(entry1);
                    srItem.Setup(x => x.TemplateId).Returns((new BlogHomeItem(blogItem)).BlogSettings.EntryTemplateID);
                    srItem.Setup(x => x.Paths).Returns(new[] { blogItem.ID });
                    srItem.Setup(x => x.Language).Returns(blogItem.Language.ToString);
                    srItem.Setup(x => x.DatabaseName).Returns(blogItem.Database.Name);

                    index.Setup(i => i.CreateSearchContext(It.IsAny<SearchSecurityOptions>()).GetQueryable<EntryResultItem>())
                        .Returns(new EnumerableQuery<EntryResultItem>(new[] { srItem.Object }));

                    db.Configuration.Settings[settingName] = settingValue;

                    // Act
                    var entryItems = manager.GetPopularEntriesByView(blogItem, 10);

                    // Assert
                    Assert.That(entryItems.Length, Is.EqualTo(expected));

                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove("WeBlog");
                }

            }
        }

        private Mock<ReportDataProviderBase> MockDataProvider()
        {
            var dataProvider = new Mock<ReportDataProviderBase>();
            DataTable dt = new DataTable();
            dt.Clear();
            dt.Columns.Add("Visits", typeof(long));
            DataRow row = dt.NewRow();
            row["Visits"] = 1;
            dt.Rows.Add(row);
            dataProvider.Setup(b => b.GetData(It.IsAny<string>(), It.IsAny<ReportDataQuery>(), It.IsAny<CachingPolicy>()))
                .Returns(new ReportDataResponse(() => dt));
            return dataProvider;
        }

        private IWeBlogSettings MockSettings(params ID[] entryTemplateIds)
        {
            return Mock.Of<IWeBlogSettings>(x =>
                x.BlogTemplateIds == new[] { ID.NewID, ID.NewID } &&
                x.CategoryTemplateIds == new[] { ID.NewID, ID.NewID } &&
                x.EntryTemplateIds == entryTemplateIds &&
                x.CommentTemplateIds == new[] { ID.NewID } &&
                x.SearchIndexName == "WeBlog"
            );
        }
    }
}