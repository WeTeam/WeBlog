using Moq;
using NUnit.Framework;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Modules.WeBlog.Components;
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
            (Dictionary<string, Item> items, IBlogManager blogManager, IEntryManager entryManager) = SetupManagerMocks(true,
                new[] {
                    CreateEntry("entry1"),
                    CreateEntry("entry2")
                }
            );

            var sut = new EntryNavigationCore(blogManager, entryManager);

            // act
            var results = sut.GetPreviousEntry(null);

            // assert
            Assert.That(results, Is.Null);
        }

        [TestCase]
        public void GetNextEntry_NullEntryItem_ReturnsNull()
        {
            // arrange
            (Dictionary<string, Item> items, IBlogManager blogManager, IEntryManager entryManager) = SetupManagerMocks(true,
                new[] {
                    CreateEntry("entry1"),
                    CreateEntry("entry2")
                }
            );

            var sut = new EntryNavigationCore(blogManager, entryManager);

            // act
            var results = sut.GetNextEntry(null);

            // assert
            Assert.That(results, Is.Null);
        }

        [Test]
        public void GetPreviousEntry_NoBlog_ReturnsNull()
        {
            // arrange
            (Dictionary<string, Item> items, IBlogManager blogManager, IEntryManager entryManager) = SetupManagerMocks(false,
                new[] {
                    CreateEntry("entry1"),
                    CreateEntry("entry2")
                }
            );

            var entryItem = items["entry1"];
            var sut = new EntryNavigationCore(blogManager, entryManager);

            // act
            var results = sut.GetPreviousEntry(entryItem);

            // assert
            Assert.That(results, Is.Null);
        }

        [Test]
        public void GetNextEntry_NoBlog_ReturnsNull()
        {
            // arrange
            (Dictionary<string, Item> items, IBlogManager blogManager, IEntryManager entryManager) = SetupManagerMocks(false,
                new[] {
                    CreateEntry("entry1"),
                    CreateEntry("entry2")
                }
            );

            var entryItem = items["entry1"];
            var sut = new EntryNavigationCore(blogManager, entryManager);

            // act
            var results = sut.GetNextEntry(entryItem);

            // assert
            Assert.That(results, Is.Null);
        }

        // not enough entries

        [Test]
        public void GetPreviousEntry_TwoEntries_ReturnsCorrectItem()
        {
            // arrange
            (Dictionary<string, Item> items, IBlogManager blogManager, IEntryManager entryManager) = SetupManagerMocks(true,
                new[] {
                    CreateEntry("entry3"),
                    CreateEntry("entry2")
                }
            );

            var entryItem = items["entry3"];
            var previousEntryItem = items["entry2"];
            var itemsByUri = items.ToDictionary(entry => entry.Value.Uri, entry => entry.Value);
            var sut = new TestableEntryNavigationCore(blogManager, entryManager, itemsByUri);

            // act
            var results = sut.GetPreviousEntry(entryItem);

            // assert
            Assert.That(results.ID, Is.EqualTo(previousEntryItem.ID));
        }

        [Test]
        public void GetNextEntry_TwoEntries_ReturnsCorrectItem()
        {
            // arrange
            (Dictionary<string, Item> items, IBlogManager blogManager, IEntryManager entryManager) = SetupManagerMocks(true,
                new[] {
                    CreateEntry("entry1"),
                    CreateEntry("entry2")
                }
            );

            var entryItem = items["entry1"];
            var nextEntryItem = items["entry2"];
            var itemsByUri = items.ToDictionary(entry => entry.Value.Uri, entry => entry.Value);
            var sut = new TestableEntryNavigationCore(blogManager, entryManager, itemsByUri);

            // act
            var results = sut.GetNextEntry(entryItem);

            // assert
            Assert.That(results.ID, Is.EqualTo(nextEntryItem.ID));
        }

        [Test]
        public void GetPreviousEntry_SeveralEntriesOnSameDay_ReturnsCorrectItem()
        {
            // arrange
            (Dictionary<string, Item> items, IBlogManager blogManager, IEntryManager entryManager) = SetupManagerMocks(true,
                new[] {
                    CreateEntry("entry4"),
                    CreateEntry("entry3")
                    
                },
                new[] {
                    CreateEntry("entry2"),
                    CreateEntry("entry1")
                }
            );

            var entryItem = items["entry4"];
            var previousEntryItem = items["entry3"];
            var itemsByUri = items.ToDictionary(entry => entry.Value.Uri, entry => entry.Value);
            var sut = new TestableEntryNavigationCore(blogManager, entryManager, itemsByUri);

            // act
            var results = sut.GetPreviousEntry(entryItem);

            // assert
            Assert.That(results.ID, Is.EqualTo(previousEntryItem.ID));
        }

        [Test]
        public void GetNextEntry_SeveralEntriesOnSameDay_ReturnsCorrectItem()
        {
            // arrange
            (Dictionary<string, Item> items, IBlogManager blogManager, IEntryManager entryManager) = SetupManagerMocks(true,
                new[] {
                    CreateEntry("entry1"),
                    CreateEntry("entry2")
                },
                new[] {
                    CreateEntry("entry3"),
                    CreateEntry("entry4")
                }
            );

            var entryItem = items["entry3"];
            var nextEntryItem = items["entry4"];
                
            var itemsByUri = items.ToDictionary(entry => entry.Value.Uri, entry => entry.Value);
            var sut = new TestableEntryNavigationCore(blogManager, entryManager, itemsByUri);

            // act
            var results = sut.GetNextEntry(entryItem);

            // assert
            Assert.That(results.ID, Is.EqualTo(nextEntryItem.ID));
        }

        [Test]
        public void GetNextEntry_NoNextEntry_ReturnsNull()
        {
            // arrange
            (Dictionary<string, Item> items, IBlogManager blogManager, IEntryManager entryManager) = SetupManagerMocks(true,
                new[] {
                    CreateEntry("entry1"),
                    CreateEntry("entry2")
                }
            );

            var entryItem = items["entry2"];
            var sut = new EntryNavigationCore(blogManager, entryManager);

            // act
            var results = sut.GetNextEntry(entryItem);

            // assert
            Assert.That(results, Is.Null);
        }

        [Test]
        public void GetPreviousEntry_NoPreviousEntry_ReturnsNull()
        {
            // arrange
            (Dictionary<string, Item> items, IBlogManager blogManager, IEntryManager entryManager) = SetupManagerMocks(true,
                new[] {
                    CreateEntry("entry3"),
                    CreateEntry("entry2")
                }
            );

            var entryItem = items["entry2"];
            var sut = new EntryNavigationCore(blogManager, entryManager);

            // act
            var results = sut.GetPreviousEntry(entryItem);

            // assert
            Assert.That(results, Is.Null);
        }

        private Entry CreateEntry(string name)
        {
            return new Entry
            {
                Uri = new ItemUri(ID.NewID, Language.Parse("en"), Sitecore.Data.Version.Latest, "mock"),
                Title = name,
                EntryDate = new DateTime(2016, 04, 06)
            };
        }

        private (Dictionary<string, Item> Items, IBlogManager BlogManager, IEntryManager EntryManager) SetupManagerMocks(
            bool returnBlogItem,
            Entry[] page1Entries,
            Entry[] page2Entries = null,
            Entry[] page3Entries = null
        )
        {
            var entries = page1Entries.AsEnumerable();

            if (page2Entries != null)
                entries = entries.Concat(page2Entries);

            if (page3Entries != null)
                entries = entries.Concat(page3Entries);

            var items = new Dictionary<string, Item>();

            foreach (var entry in entries)
            {
                var itemMock = ItemFactory.CreateItem(entry.Uri.ItemID);
                var dateField = FieldFactory.CreateField(itemMock.Object, ID.NewID, "Entry Date", DateUtil.ToIsoDate(entry.EntryDate));
                ItemFactory.AddFields(itemMock, new[] { dateField });

                items.Add(entry.Title, itemMock.Object);
            }

            var blogItem = ItemFactory.CreateItem();
            var blogHomeItem = new BlogHomeItem(blogItem.Object, null);

            var blogManager = Mock.Of<IBlogManager>(x =>
                x.GetCurrentBlog(It.IsAny<Item>()) == (returnBlogItem ? blogHomeItem : null)
            );

            var entryManager = Mock.Of<IEntryManager>(x =>
                x.GetBlogEntries(blogHomeItem, It.IsAny<EntryCriteria>(), It.IsAny<ListOrder>()) == new SearchResults<Model.Entry>(entries.ToList(), false)
            );

            return (items, blogManager, entryManager);
        }
    }
}
