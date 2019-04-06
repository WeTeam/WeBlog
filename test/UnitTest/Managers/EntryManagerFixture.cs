using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;
using Moq;
using NUnit.Framework;
#if FEATURE_XCONNECT
using Sitecore.Xdb.Reporting;
#elif FEATURE_XDB
using Sitecore.Analytics.Reporting;
#endif
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
using Sitecore.Modules.WeBlog.Search;
using Sitecore.Modules.WeBlog.UnitTest.Comparers;

namespace Sitecore.Modules.WeBlog.UnitTest
{
    [TestFixture]
    public class EntryManagerFixture
    {
        private const string IndexName = "WeBlog-master";

        [Test]
        public void DeleteEntry_NullID()
        {
            var settings = MockSettings(ID.NewID);
            var manager = CreateManager(settings);

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
            var manager = CreateManager(settings);

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
            var manager = CreateManager(settings);

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
            var manager = CreateManager(settings);

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
            var manager = CreateManager(settings);

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
        public void GetBlogEntries_NullRootItem_ReturnsEmptyCollection()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);

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
                    var entries = manager.GetBlogEntries(null, EntryCriteria.AllEntries);

                    // Assert
                    Assert.That(entries, Is.Empty);
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove(IndexName);
                }
            }
        }

        [Test]
        public void GetBlogEntries_NoEntries()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);

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
                    var entries = manager.GetBlogEntries(blogItem, EntryCriteria.AllEntries);

                    // Assert
                    Assert.That(entries, Is.Empty);
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove(IndexName);
                }
            }
        }

        [Test]
        public void GetBlogEntries_WithEntries()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);

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
                    var entries = manager.GetBlogEntries(blogItem, EntryCriteria.AllEntries);

                    // Assert
                    var ids = from result in entries select result.Uri;
                    Assert.That(ids, Is.EqualTo(new[] { entry1.Uri, entry2.Uri, entry3.Uri}));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove(IndexName);
                }
            }
        }

        [Test]
        public void GetBlogEntries_EntryItemFound_EntryContainsFields()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("Duis ligula massa", ID.NewID, settings.EntryTemplateIds.First())
                        {
                            new DbField("tags")
                            {
                                Value = "lorem, ipsum, dolor"
                            }
                        }
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");
                    var entry = db.GetItem("/sitecore/content/blog/2016/Duis ligula massa");

                    IndexEntries(blogItem, settings);

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, EntryCriteria.AllEntries);

                    // Assert
                    Assert.That(entries.Count, Is.EqualTo(1));

                    var returnedEntry = entries[0];
                    Assert.That(returnedEntry.Title, Is.EqualTo("Duis ligula massa"));
                    Assert.That(returnedEntry.Tags, Is.EquivalentTo(new[]{ "lorem", "ipsum", "dolor" }));
                    Assert.That(returnedEntry.EntryDate, Is.EqualTo(entry.Created));
                    Assert.That(returnedEntry.Uri, Is.EqualTo(entry.Uri));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove(IndexName);
                }
            }
        }

        [Test]
        public void GetBlogEntries_EntryWithExplicitTitle_TitleIsCorrect()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First())
                        {
                            new DbField("title")
                            {
                                Value = "Duis ligula massa"
                            }
                        }
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");
                    var entry = db.GetItem("/sitecore/content/blog/2016/entry1");

                    IndexEntries(blogItem, settings);

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, EntryCriteria.AllEntries);

                    // Assert
                    Assert.That(entries.Count, Is.EqualTo(1));

                    var returnedEntry = entries[0];
                    Assert.That(returnedEntry.Title, Is.EqualTo("Duis ligula massa"));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove(IndexName);
                }
            }
        }

        [Test]
        public void GetBlogEntries_EntryEntryDate_DateIsCorrect()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First())
                        {
                            new DbField("entry date")
                            {
                                Value = "20140317T130029"
                            }
                        }
                    }
                }
            })
            {
                try
                {
                    // Assign
                    var blogItem = db.GetItem("/sitecore/content/blog");
                    var entry = db.GetItem("/sitecore/content/blog/2016/entry1");

                    IndexEntries(blogItem, settings);

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, EntryCriteria.AllEntries);

                    // Assert
                    Assert.That(entries.Count, Is.EqualTo(1));

                    var returnedEntry = entries[0];
                    Assert.That(returnedEntry.EntryDate, Is.EqualTo(new DateTime(2014, 3, 17, 13, 0, 29)));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove(IndexName);
                }
            }
        }

        [Test]
        public void GetBlogEntries_WithEntriesMultipleBlogs()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);

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
                    var entries = manager.GetBlogEntries(blogItem, EntryCriteria.AllEntries);

                    // Assert
                    var ids = from result in entries select result.Uri;
                    Assert.That(ids, Is.EqualTo(new[] { entry1.Uri, entry2.Uri, entry3.Uri }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove(IndexName);
                }
            }
        }

        [Test]
        public void GetBlogEntries_LimitedMultipleBlogs()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);

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

                    var criteria = new EntryCriteria
                    {
                        PageNumber = 1,
                        PageSize = 3
                    };

                    // Act
                    var entries = manager.GetBlogEntries(entry2, criteria);

                    // Assert
                    var ids = from result in entries select result.Uri;
                    Assert.That(ids, Is.EqualTo(new[] { entry1.Uri, entry2.Uri, entry3.Uri }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove(IndexName);
                }
            }
        }

        [Test]
        public void GetBlogEntries_LimitEntries()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);

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

                    var criteria = new EntryCriteria
                    {
                        PageNumber = 1,
                        PageSize = 2
                    };

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, criteria);

                    // Assert
                    var ids = from result in entries select result.Uri;
                    Assert.That(ids, Is.EqualTo(new[] { entry1.Uri, entry2.Uri }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove(IndexName);
                }
            }
        }

        [Test]
        public void GetBlogEntries_LimitEntriesZero()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);

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

                    var criteria = new EntryCriteria
                    {
                        PageNumber = 0,
                        PageSize = 10
                    };

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, criteria);

                    // Assert
                    Assert.That(entries, Is.Empty);
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove(IndexName);
                }
            }
        }

        [Test]
        public void GetBlogEntries_LimitEntriesNegativeLimit()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);

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

                    var criteria = new EntryCriteria
                    {
                        PageNumber = 1,
                        PageSize = -7
                    };

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, criteria);

                    // Assert
                    Assert.That(entries, Is.Empty);
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove(IndexName);
                }
            }
        }

        [Test]
        public void GetBlogEntries_ByTag()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);

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

                    var criteria = new EntryCriteria
                    {
                        PageNumber = 1,
                        PageSize = 10,
                        Tag = "prowl"
                    };

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, criteria);

                    // Assert
                    var ids = from result in entries select result.Uri;
                    Assert.That(ids, Is.EqualTo(new[] { entry1.Uri, entry2.Uri }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove(IndexName);
                }
            }
        }

        [Test]
        public void GetBlogEntries_ByTagWithSpace()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);

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

                    var criteria = new EntryCriteria
                    {
                        PageNumber = 1,
                        PageSize = 10,
                        Tag = "orion pax"
                    };

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, criteria);

                    // Assert
                    var ids = from result in entries select result.Uri;
                    Assert.That(ids, Is.EqualTo(new[] { entry2.Uri, entry3.Uri }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove(IndexName);
                }
            }
        }

        [Test]
        public void GetBlogEntries_ByTagLimited()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);

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

                    var criteria = new EntryCriteria
                    {
                        PageNumber = 1,
                        PageSize = 2,
                        Tag = "wheeljack"
                    };

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, criteria);


                    // Assert
                    var ids = from result in entries select result.Uri;
                    Assert.That(ids, Is.EqualTo(new[] { entry1.Uri, entry3.Uri}));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove(IndexName);
                }
            }
        }

        [Test]
        public void GetBlogEntries_ByInvalidTag()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);

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

                    var criteria = new EntryCriteria
                    {
                        PageNumber = 1,
                        PageSize = 2,
                        Tag = "blurr"
                    };

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, criteria);


                    // Assert
                    Assert.That(entries, Is.Empty);
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove(IndexName);
                }
            }
        }

        [Test]
        public void GetBlogEntries_ByCategory()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);

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

                    var criteria = new EntryCriteria
                    {
                        PageNumber = 1,
                        PageSize = 10,
                        Category = alphaId.ToString()
                    };

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, criteria);


                    // Assert
                    var ids = from result in entries select result.Uri;
                    Assert.That(ids, Is.EqualTo(new[] { entry2.Uri, entry3.Uri, entry4.Uri}));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove(IndexName);
                }
            }
        }

        [Test]
        public void GetBlogEntries_ByInvalidCategory()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);

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

                    var criteria = new EntryCriteria
                    {
                        PageNumber = 1,
                        PageSize = 10,
                        Category = ID.NewID.ToString()
                    };

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, criteria);


                    // Assert
                    Assert.That(entries, Is.Empty);
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove(IndexName);
                }
            }
        }

        [Test]
        public void GetBlogEntries_ByCategoryLimited()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);

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

                    var criteria = new EntryCriteria
                    {
                        PageNumber = 1,
                        PageSize = 1,
                        Category = alphaId.ToString()
                    };

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, criteria);


                    // Assert
                    var ids = from result in entries select result.Uri;
                    Assert.That(ids, Is.EqualTo(new[] { entry2.Uri }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove(IndexName);
                }
            }
        }

        [Test]
        public void GetBlogEntries_InDateRange()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);

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

                    var criteria = new EntryCriteria
                    {
                        PageNumber = 1,
                        PageSize = 10,
                        MinimumDate = new DateTime(2014, 11, 1),
                        MaximumDate = new DateTime(2014, 12, 20)
                    };

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, criteria);

                    // Assert
                    var ids = from result in entries select result.Uri;
                    Assert.That(ids, Is.EqualTo(new[] { entry3.Uri, entry2.Uri }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove(IndexName);
                }
            }
        }

        [Test]
        public void GetBlogEntries_SearchDateBeforeEntries_ReturnsEmpty()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);

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

                    IndexEntries(blogItem, settings);

                    var criteria = new EntryCriteria
                    {
                        PageNumber = 1,
                        PageSize = 10,
                        MaximumDate = new DateTime(2014, 09, 30)
                    };

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, criteria);

                    // Assert
                    Assert.That(entries, Is.Empty);
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove(IndexName);
                }
            }
        }

        [Test]
        public void GetBlogEntries_SearchDateAfterEntries_ReturnsEmpty()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);

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

                    IndexEntries(blogItem, settings);

                    var criteria = new EntryCriteria
                    {
                        PageNumber = 1,
                        PageSize = 10,
                        MinimumDate = new DateTime(2015, 01, 2)
                    };

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, criteria);

                    // Assert
                    Assert.That(entries, Is.Empty);
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove(IndexName);
                }
            }
        }

        [Test]
        public void GetBlogEntries_InDateRangeLimited()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);

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

                    var criteria = new EntryCriteria
                    {
                        PageNumber = 1,
                        PageSize = 2,
                        MinimumDate = new DateTime(2014, 11, 1),
                        MaximumDate = new DateTime(2015, 1, 20)
                    };

                    // Act
                    var entries = manager.GetBlogEntries(blogItem, criteria);

                    // Assert
                    var uris = from result in entries select result.Uri;
                    Assert.That(uris, Is.EqualTo(new[] { entry4.Uri, entry3.Uri }));
                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove(IndexName);
                }
            }
        }

        private Mock<ISearchIndex> MockIndex(string indexName = "WeBlog-master")
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
            srItem.Setup(x => x.Uri).Returns(entryItem.Uri);
            srItem.Setup(x => x.TemplateId).Returns(new BlogHomeItem(blogItem).BlogSettings.EntryTemplateID);
            srItem.Setup(x => x.Paths).Returns(GetPaths(entryItem));
            srItem.Setup(x => x.Language).Returns(blogItem.Language.ToString);
            srItem.Setup(x => x.DatabaseName).Returns(blogItem.Database.Name);
            srItem.Setup(x => x.Name).Returns(entryItem.Name);
            srItem.Setup(x => x.Tags).Returns(GetTags(entryItem));
            srItem.Setup(x => x.Category).Returns(GetCategories(entryItem));
            srItem.Setup(x => x.EntryDate).Returns(GetEntryData(entryItem));

            if(!string.IsNullOrEmpty(entryItem["title"]))
                srItem.Setup(x => x.Title).Returns(entryItem["title"]);

            return srItem;
        }

        private Mock<CommentResultItem> MockCommentItem(Item commentItem, Item blogItem)
        {
            var srItem = new Mock<CommentResultItem>();
            srItem.Setup(x => x.GetItem()).Returns(commentItem);
            srItem.Setup(x => x.TemplateId).Returns(new BlogHomeItem(blogItem).BlogSettings.CommentTemplateID);
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
        public void GetBlogEntryItemByCommentUri_NullUri_ReturnsNull()
        {
            var settings = MockSettings(ID.NewID);
            var manager = CreateManager(settings);
            var foundEntry = manager.GetBlogEntryItemByCommentUri(null);

            Assert.That(foundEntry, Is.Null);
        }

        [Test]
        public void GetBlogEntryItemByCommentUri_WithValidEntryUri_ReturnsEntry()
        {
            var settings = MockSettings(ID.NewID);
            var manager = CreateManager(settings);

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
                var result = manager.GetBlogEntryItemByCommentUri(item.Uri);

                Assert.That(result.ID, Is.EqualTo(item.ID));
            }
        }

        [Test]
        public void GetBlogEntryItemByCommentUri_UnderEntry_ReturnsEntry()
        {
            var settings = MockSettings(ID.NewID);
            var manager = CreateManager(settings);

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
                var result = manager.GetBlogEntryItemByCommentUri(commentItem.Uri);

                Assert.That(result.ID, Is.EqualTo(entryItem.ID));
            }
        }

        [Test]
        public void GetBlogEntryItemByCommentUri_OutsideEntry_ReturnsNull()
        {
            var settings = MockSettings(ID.NewID);
            var manager = CreateManager(settings);

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
                var result = manager.GetBlogEntryItemByCommentUri(item.Uri);

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
            var manager = new TestableEntryManager(settings, 1);

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
                    ContentSearchManager.SearchConfiguration.Indexes.Add(IndexName, index.Object);

                    var srItem = new Mock<EntryResultItem>();
                    srItem.Setup(x => x.GetItem()).Returns(entry1);
                    srItem.Setup(x => x.Uri).Returns(entry1.Uri);
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
                    Assert.That(entryItems.Count, Is.EqualTo(expected));

                }
                finally
                {
                    ContentSearchManager.SearchConfiguration.Indexes.Remove(IndexName);
                }

            }
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

        private EntryManager CreateManager(IWeBlogSettings settings)
        {
#if FEATURE_XDB
            return new EntryManager(null, null, settings);
#else
            return new EntryManager(settings);
#endif
        }
    }

    internal class TestableEntryManager : EntryManager
    {
        private long _viewCount = 0;

        public TestableEntryManager(IWeBlogSettings settings, long viewCount)
#if FEATURE_XDB
            : base(null, null, settings, null)
#else
            : base(settings)
#endif
        {
            _viewCount = viewCount;
        }

        protected override long GetItemViews(ID itemId)
        {
            return _viewCount;
        }
    }
}