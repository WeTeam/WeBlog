using System;
using Moq;
using NUnit.Framework;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Modules.WeBlog.Search;

namespace Sitecore.Modules.WeBlog.UnitTest.Managers
{
    [TestFixture]
    public class TagManagerFixture
    {
        [Test]
        public void GetTagsForBlog_NullItem()
        {
            var manager = new TagManager();
            var tags = manager.GetTagsForBlog(null);

            Assert.That(tags, Is.Empty);
        }

        [Test]
        public void GetTagsForBlog_NoTags()
        {
            using (var db = new Db
            {
                new DbItem("blog1")
            })
            {
                var blogItem = db.GetItem("/sitecore/content/blog1");

                var entryManager = MockEntryManager(blogItem, new Entry(), new Entry());

                var sut = new TagManager(entryManager);

                var tags = sut.GetTagsForBlog(blogItem);

                Assert.That(tags, Is.Empty);
            }
        }

        [Test]
        public void GetTagsForBlog_SomeEntriesNoTags()
        {
            // limit datestamp to date only so comparison doesn't fail with milliseconds.
            var dateStamp = DateTime.UtcNow.Date;

            using (var db = new Db
            {
                new DbItem("blog1")
            })
            {
                var blogItem = db.GetItem("/sitecore/content/blog1");
                
                var entryManager = MockEntryManager(
                    blogItem,
                    new Entry { Tags = new[] { "lorem", "ipsum" }, EntryDate = dateStamp },
                    new Entry { Tags = new string[0], EntryDate = dateStamp },
                    new Entry { Tags = new[] { "dolor" }, EntryDate = dateStamp }
                );

                var sut = new TagManager(entryManager);

                var tags = sut.GetTagsForBlog(blogItem);

                Assert.That(tags, Is.EqualTo(new[]
                {
                    new Tag("lorem", dateStamp, 1),
                    new Tag("ipsum", dateStamp, 1),
                    new Tag("dolor", dateStamp, 1)
                }));
            }
        }

        [Test]
        public void GetTagsForBlog_SingleTagsOnEntries()
        {
            // limit datestamp to date only so comparison doesn't fail with milliseconds.
            var dateStamp = DateTime.UtcNow.Date;

            using (var db = new Db
            {
                new DbItem("blog1")
            })
            {
                var blogItem = db.GetItem("/sitecore/content/blog1");

                var entryManager = MockEntryManager(
                    blogItem,
                    new Entry { Tags = new[] { "lorem" }, EntryDate = dateStamp },
                    new Entry { Tags = new[] { "ipsum" }, EntryDate = dateStamp }
                );

                var sut = new TagManager(entryManager);
                var tags = sut.GetTagsForBlog(blogItem);

                Assert.That(tags, Is.EqualTo(new[]
                {
                    new Tag("lorem", dateStamp, 1),
                    new Tag("ipsum", dateStamp, 1)
                }));
            }
        }

        [Test]
        public void GetTagsForBlog_MultipleTagsOnEntries()
        {
            // limit datestamp to date only so comparison doesn't fail with milliseconds.
            var dateStamp = DateTime.UtcNow.Date;

            using (var db = new Db
            {
                new DbItem("blog1")
            })
            {
                var blogItem = db.GetItem("/sitecore/content/blog1");

                var entryManager = MockEntryManager(
                    blogItem,
                    new Entry { Tags = new[] { "lorem", "ipsum", "dolor" }, EntryDate = dateStamp },
                    new Entry { Tags = new[] { "sit", "amed", "pluto" }, EntryDate = dateStamp }
                );

                var sut = new TagManager(entryManager);
                var tags = sut.GetTagsForBlog(blogItem);

                Assert.That(tags, Is.EqualTo(new[]
                {
                    new Tag("lorem", dateStamp, 1),
                    new Tag("ipsum", dateStamp, 1),
                    new Tag("dolor", dateStamp, 1),
                    new Tag("sit", dateStamp, 1),
                    new Tag("amed", dateStamp, 1),
                    new Tag("pluto", dateStamp, 1)
                }));
            }
        }

        [Test]
        public void GetTagsForBlog_MultipleUse()
        {
            // limit datestamp to date only so comparison doesn't fail with milliseconds.
            var dateStamp = DateTime.UtcNow.Date;

            using (var db = new Db
            {
                new DbItem("blog1")
            })
            {
                var blogItem = db.GetItem("/sitecore/content/blog1");

                var entryManager = MockEntryManager(
                    blogItem,
                    new Entry { Tags = new[] { "lorem", "ipsum" }, EntryDate = dateStamp },
                    new Entry { Tags = new[] { "lorem", "dolor" }, EntryDate = dateStamp },
                    new Entry { Tags = new[] { "lorem", "dolor" }, EntryDate = dateStamp }
                );

                var sut = new TagManager(entryManager);
                var tags = sut.GetTagsForBlog(blogItem);

                Assert.That(tags, Is.EqualTo(new[]
                {
                    new Tag("lorem", dateStamp, 3),
                    new Tag("dolor", dateStamp, 2),
                    new Tag("ipsum", dateStamp, 1)
                    
                }));
            }
        }

        [Test]
        public void GetTagsForEntry_NullEntryItem()
        {
            var manager = new TagManager();
            var tags = manager.GetTagsForEntry((EntryItem)null);
            Assert.That(tags, Is.Empty);
        }

        [Test]
        public void GetTagsForEntry_NullEntry()
        {
            var manager = new TagManager();
            var tags = manager.GetTagsForEntry((Entry)null);
            Assert.That(tags, Is.Empty);
        }

        [Test]
        public void GetTagsForEntry_NoTags()
        {
            using (var db = new Db
            {
                new DbItem("entry")
            })
            {
                var entryItem = db.GetItem("/sitecore/content/entry");
                var manager = new TagManager();
                var tags = manager.GetTagsForEntry(entryItem);
                Assert.That(tags, Is.Empty);
            }
        }

        [Test]
        public void GetTagsForEntry_SingleTag()
        {
            // limit datestamp to date only so comparison doesn't fail with milliseconds.
            var dateStamp = DateTime.UtcNow.Date;

            using (var db = new Db
            {
                new DbItem("entry")
                {
                    new DbField("Tags")
                    {
                        Value = "lorem"
                    },
                    new DbField("Entry Date") { Value = DateUtil.ToIsoDate(dateStamp) }
                }
            })
            {
                var entryItem = db.GetItem("/sitecore/content/entry");
                var manager = new TagManager();
                var tags = manager.GetTagsForEntry(entryItem);

                Assert.That(tags, Is.EqualTo(new[]
                {
                    new Tag("lorem", dateStamp, 1)
                }));
            }
        }

        [Test]
        public void GetTagsForEntry_MultipleTags()
        {
            // limit datestamp to date only so comparison doesn't fail with milliseconds.
            var dateStamp = System.DateTime.UtcNow.Date;

            using (var db = new Db
            {
                new DbItem("entry")
                {
                    new DbField("Tags")
                    {
                        Value = "dolor, lorem, ipsum, lorem"
                    },
                    new DbField("Entry Date") { Value = DateUtil.ToIsoDate(dateStamp) }
                }
            })
            {
                var entryItem = db.GetItem("/sitecore/content/entry");
                var manager = new TagManager();
                var tags = manager.GetTagsForEntry(entryItem);

                Assert.That(tags, Is.EqualTo(new[]
                {
                    new Tag("dolor", dateStamp, 1),
                    new Tag("lorem", dateStamp, 1),
                    new Tag("ipsum", dateStamp, 1)
                }));
            }
        }

        private IEntryManager MockEntryManager(Item blogItem, params Entry[] entries)
        {
            return Mock.Of<IEntryManager>(x =>
                x.GetBlogEntries(blogItem, EntryCriteria.AllEntries, ListOrder.Descending) == new SearchResults<Entry>(entries, false)
            );
        }
    }
}
