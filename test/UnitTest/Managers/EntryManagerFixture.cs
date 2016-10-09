using System.Data;
using System.Linq;
using Moq;
using NUnit.Framework;
using Sitecore.Analytics.Reporting;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Security;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Search.SearchTypes;

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
        public void GetPopularEntriesByView(string settingName, string settingValue, int expected)
        {
            var settings = MockSettings(ID.NewID);
            var dataProvider = MockDataProvider();
            var manager = new EntryManager(dataProvider.Object, settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2013", ID.NewID, ID.NewID)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First()),
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
