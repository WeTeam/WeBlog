using Moq;
using NUnit.Framework;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Sitecore.Globalization;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Modules.WeBlog.Search;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.Modules.WeBlog.UnitTest.Components
{
    [TestFixture]
    public class EntryNavigationCoreFixture
    {
        [Test]
        public void Ctor_NullBlogManager_ThrowsError()
        {
            // arrange
            TestDelegate sutAction = () => new EntryNavigationCore(null, Mock.Of<IEntryManager>());

            // act, assert
            Assert.That(sutAction, Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_NullEntryManager_ThrowsError()
        {
            // arrange
            TestDelegate sutAction = () => new EntryNavigationCore(Mock.Of<IBlogManager>(), null);

            // act, assert
            Assert.That(sutAction, Throws.ArgumentNullException);
        }

        [TestCase]
        public void GetPreviousEntry_NullEntryItem_ReturnsNull()
        {
            // arrange
            (Db db, IBlogManager blogManager, IEntryManager entryManager) = SetupManagerMocks(true,
                new[] {
                    CreateEntry("entry1"),
                    CreateEntry("entry2")
                }
            );

            using (db)
            {
                var sut = new EntryNavigationCore(blogManager, entryManager);

                // act
                var results = sut.GetPreviousEntry(null);

                // assert
                Assert.That(results, Is.Null);
            }
        }

        [TestCase]
        public void GetNextEntry_NullEntryItem_ReturnsNull()
        {
            // arrange
            (Db db, IBlogManager blogManager, IEntryManager entryManager) = SetupManagerMocks(true,
                new[] {
                    CreateEntry("entry1"),
                    CreateEntry("entry2")
                }
            );

            using (db)
            {
                var sut = new EntryNavigationCore(blogManager, entryManager);

                // act
                var results = sut.GetNextEntry(null);

                // assert
                Assert.That(results, Is.Null);
            }
        }

        [Test]
        public void GetPreviousEntry_NoBlog_ReturnsNull()
        {
            // arrange
            (Db db, IBlogManager blogManager, IEntryManager entryManager) = SetupManagerMocks(false,
                new[] {
                    CreateEntry("entry1"),
                    CreateEntry("entry2")
                }
            );

            using (db)
            {
                var entryItem = db.GetItem("/sitecore/content/entry1");
                var sut = new EntryNavigationCore(blogManager, entryManager);

                // act
                var results = sut.GetPreviousEntry(entryItem);

                // assert
                Assert.That(results, Is.Null);
            }
        }

        [Test]
        public void GetNextEntry_NoBlog_ReturnsNull()
        {
            // arrange
            (Db db, IBlogManager blogManager, IEntryManager entryManager) = SetupManagerMocks(false,
                new[] {
                    CreateEntry("entry1"),
                    CreateEntry("entry2")
                }
            );

            using (db)
            {
                var entryItem = db.GetItem("/sitecore/content/entry1");
                var sut = new EntryNavigationCore(blogManager, entryManager);

                // act
                var results = sut.GetNextEntry(entryItem);

                // assert
                Assert.That(results, Is.Null);
            }
        }

        // not enough entries

        [Test]
        public void GetPreviousEntry_TwoEntries_ReturnsCorrectItem()
        {
            // arrange
            (Db db, IBlogManager blogManager, IEntryManager entryManager) = SetupManagerMocks(true,
                new[] {
                    CreateEntry("entry3"),
                    CreateEntry("entry2")
                }
            );

            using (db)
            {
                var entryItem = db.GetItem("/sitecore/content/entry3");
                var previousEntryItem = db.GetItem("/sitecore/content/entry2");
                var sut = new EntryNavigationCore(blogManager, entryManager);

                // act
                var results = sut.GetPreviousEntry(entryItem);

                // assert
                Assert.That(results.ID, Is.EqualTo(previousEntryItem.ID));
            }
        }

        [Test]
        public void GetNextEntry_TwoEntries_ReturnsCorrectItem()
        {
            // arrange
            (Db db, IBlogManager blogManager, IEntryManager entryManager) = SetupManagerMocks(true,
                new[] {
                    CreateEntry("entry1"),
                    CreateEntry("entry2")
                }
            );

            using (db)
            {
                var entryItem = db.GetItem("/sitecore/content/entry1");
                var nextEntryItem = db.GetItem("/sitecore/content/entry2");
                var sut = new EntryNavigationCore(blogManager, entryManager);

                // act
                var results = sut.GetNextEntry(entryItem);

                // assert
                Assert.That(results.ID, Is.EqualTo(nextEntryItem.ID));
            }
        }

        [Test]
        public void GetPreviousEntry_SeveralEntriesOnSameDay_ReturnsCorrectItem()
        {
            // arrange
            (Db db, IBlogManager blogManager, IEntryManager entryManager) = SetupManagerMocks(true,
                new[] {
                    CreateEntry("entry4"),
                    CreateEntry("entry3")
                    
                },
                new[] {
                    CreateEntry("entry2"),
                    CreateEntry("entry1")
                }
            );

            using (db)
            {
                var entryItem = db.GetItem("/sitecore/content/entry4");
                var previousEntryItem = db.GetItem("/sitecore/content/entry3");
                var sut = new EntryNavigationCore(blogManager, entryManager);

                // act
                var results = sut.GetPreviousEntry(entryItem);

                // assert
                Assert.That(results.ID, Is.EqualTo(previousEntryItem.ID));
            }
        }

        [Test]
        public void GetNextEntry_SeveralEntriesOnSameDay_ReturnsCorrectItem()
        {
            // arrange
            (Db db, IBlogManager blogManager, IEntryManager entryManager) = SetupManagerMocks(true,
                new[] {
                    CreateEntry("entry1"),
                    CreateEntry("entry2")
                },
                new[] {
                    CreateEntry("entry3"),
                    CreateEntry("entry4")
                }
            );

            using (db)
            {
                var entryItem = db.GetItem("/sitecore/content/entry3");
                var nextEntryItem = db.GetItem("/sitecore/content/entry4");
                var sut = new EntryNavigationCore(blogManager, entryManager);

                // act
                var results = sut.GetNextEntry(entryItem);

                // assert
                Assert.That(results.ID, Is.EqualTo(nextEntryItem.ID));
            }
        }

        [Test]
        public void GetNextEntry_NoNextEntry_ReturnsNull()
        {
            // arrange
            (Db db, IBlogManager blogManager, IEntryManager entryManager) = SetupManagerMocks(true,
                new[] {
                    CreateEntry("entry1"),
                    CreateEntry("entry2")
                }
            );

            using (db)
            {
                var entryItem = db.GetItem("/sitecore/content/entry2");
                var sut = new EntryNavigationCore(blogManager, entryManager);

                // act
                var results = sut.GetNextEntry(entryItem);

                // assert
                Assert.That(results, Is.Null);
            }
        }

        [Test]
        public void GetPreviousEntry_NoPreviousEntry_ReturnsNull()
        {
            // arrange
            (Db db, IBlogManager blogManager, IEntryManager entryManager) = SetupManagerMocks(true,
                new[] {
                    CreateEntry("entry3"),
                    CreateEntry("entry2")
                }
            );

            using (db)
            {
                var entryItem = db.GetItem("/sitecore/content/entry2");
                var sut = new EntryNavigationCore(blogManager, entryManager);

                // act
                var results = sut.GetPreviousEntry(entryItem);

                // assert
                Assert.That(results, Is.Null);
            }
        }

        private Entry CreateEntry(string name)
        {
            return new Entry
            {
                Uri = new ItemUri(ID.NewID, Language.Parse("en"), Sitecore.Data.Version.Latest, "master"),
                Title = name,
                EntryDate = new DateTime(2016, 04, 06)
            };
        }

        private (Db Database, IBlogManager BlogManager, IEntryManager EntryManager) SetupManagerMocks(
            bool returnBlogItem,
            Entry[] page1Entries,
            Entry[] page2Entries = null,
            Entry[] page3Entries = null
        )
        {
            var db = new Db("master")
            {
                new DbItem("blog", ID.NewID, WeBlogSettings.Instance.BlogTemplateIds.First())
            };

            var entries = page1Entries.AsEnumerable();

            if (page2Entries != null)
                entries = entries.Concat(page2Entries);

            if (page3Entries != null)
                entries = entries.Concat(page3Entries);

            foreach (var entry in entries)
            {
                db.Add(
                    new DbItem(entry.Title, entry.Uri.ItemID)
                    {
                        new DbField("Entry Date")
                        {
                            Value = DateUtil.ToIsoDate(entry.EntryDate)
                        }
                    }
                );
            }
            
            var blogItem = db.GetItem("/sitecore/content/blog");
            var blogHomeItem = new BlogHomeItem(blogItem, null);

            var blogManager = Mock.Of<IBlogManager>(x =>
                x.GetCurrentBlog(It.IsAny<Item>()) == (returnBlogItem ? blogHomeItem : null)
            );

            var entryManager = Mock.Of<IEntryManager>(x =>
                x.GetBlogEntries(blogHomeItem, It.IsAny<EntryCriteria>(), It.IsAny<ListOrder>()) == new SearchResults<Model.Entry>(entries.ToList(), false)
            );

            return (db, blogManager, entryManager);
        }
    }
}
