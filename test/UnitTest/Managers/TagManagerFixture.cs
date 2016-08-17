using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Sitecore.FakeDb;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Model;

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
                {
                    new DbItem("entry1"),
                    new DbItem("entry2")
                }
            })
            {
                var blogItem = db.GetItem("/sitecore/content/blog1");
                var entry1 = db.GetItem("/sitecore/content/blog1/entry1");
                var entry2 = db.GetItem("/sitecore/content/blog1/entry2");

                var entryManager = Mock.Of<EntryManager>(x =>
                    x.GetBlogEntries(blogItem) == new EntryItem[] { entry1, entry2 }
                );

                var manager = new TagManager(entryManager);
                var tags = manager.GetTagsForBlog(blogItem);

                Assert.That(tags, Is.Empty);
            }
        }

        [Test]
        public void GetTagsForBlog_SomeEntriesNoTags()
        {
            // limit datestamp to date only so comparison doesn't fail with milliseconds.
            var dateStamp = System.DateTime.UtcNow.Date;

            using (var db = new Db
            {
                new DbItem("blog1")
                {
                    new DbItem("entry1")
                    {
                        new DbField("Tags")
                        {
                            Value = "lorem, ipsum"
                        },
                        new DbField("Entry Date") { Value = DateUtil.ToIsoDate(dateStamp) }
                    },
                    new DbItem("entry2")
                    {
                        new DbField("Tags")
                        {
                            Value = "   "
                        },
                        new DbField("Entry Date") { Value = DateUtil.ToIsoDate(dateStamp) }
                    },
                    new DbItem("entry3")
                    {
                        new DbField("Tags")
                        {
                            Value = "dolor "
                        },
                        new DbField("Entry Date") { Value = DateUtil.ToIsoDate(dateStamp) }
                    }
                }
            })
            {
                var blogItem = db.GetItem("/sitecore/content/blog1");
                var entry1 = db.GetItem("/sitecore/content/blog1/entry1");
                var entry2 = db.GetItem("/sitecore/content/blog1/entry2");
                var entry3 = db.GetItem("/sitecore/content/blog1/entry3");

                var entryManager = Mock.Of<EntryManager>(x =>
                    x.GetBlogEntries(blogItem) == new EntryItem[] { entry1, entry2, entry3 }
                );

                var manager = new TagManager(entryManager);
                var tags = manager.GetTagsForBlog(blogItem);

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
            var dateStamp = System.DateTime.UtcNow.Date;

            using (var db = new Db
            {
                new DbItem("blog1")
                {
                    new DbItem("entry1")
                    {
                        new DbField("Tags")
                        {
                            Value = "lorem"
                        },
                        new DbField("Entry Date") { Value = DateUtil.ToIsoDate(dateStamp) }
                    },
                    new DbItem("entry2")
                    {
                        new DbField("Tags")
                        {
                            Value = "ipsum"
                        },
                        new DbField("Entry Date") { Value = DateUtil.ToIsoDate(dateStamp) }
                    }
                }
            })
            {
                var blogItem = db.GetItem("/sitecore/content/blog1");
                var entry1 = db.GetItem("/sitecore/content/blog1/entry1");
                var entry2 = db.GetItem("/sitecore/content/blog1/entry2");

                var entryManager = Mock.Of<EntryManager>(x =>
                    x.GetBlogEntries(blogItem) == new EntryItem[] { entry1, entry2 }
                );

                var manager = new TagManager(entryManager);
                var tags = manager.GetTagsForBlog(blogItem);

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
            var dateStamp = System.DateTime.UtcNow.Date;

            using (var db = new Db
            {
                new DbItem("blog1")
                {
                    new DbItem("entry1")
                    {
                        new DbField("Tags")
                        {
                            Value = "lorem, ipsum,       dolor     "
                        },
                        new DbField("Entry Date") { Value = DateUtil.ToIsoDate(dateStamp) }
                    },
                    new DbItem("entry2")
                    {
                        new DbField("Tags")
                        {
                            Value = "sit  ,  amed,   pluto "
                        },
                        new DbField("Entry Date") { Value = DateUtil.ToIsoDate(dateStamp) }
                    }
                }
            })
            {
                var blogItem = db.GetItem("/sitecore/content/blog1");
                var entry1 = db.GetItem("/sitecore/content/blog1/entry1");
                var entry2 = db.GetItem("/sitecore/content/blog1/entry2");

                var entryManager = Mock.Of<EntryManager>(x =>
                    x.GetBlogEntries(blogItem) == new EntryItem[] { entry1, entry2 }
                );

                var manager = new TagManager(entryManager);
                var tags = manager.GetTagsForBlog(blogItem);

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
            var dateStamp = System.DateTime.UtcNow.Date;

            using (var db = new Db
            {
                new DbItem("blog1")
                {
                    new DbItem("entry1")
                    {
                        new DbField("Tags")
                        {
                            Value = "lorem, ipsum"
                        },
                        new DbField("Entry Date") { Value = DateUtil.ToIsoDate(dateStamp) }
                    },
                    new DbItem("entry2")
                    {
                        new DbField("Tags")
                        {
                            Value = "lorem, dolor"
                        },
                        new DbField("Entry Date") { Value = DateUtil.ToIsoDate(dateStamp) }
                    },
                    new DbItem("entry3")
                    {
                        new DbField("Tags")
                        {
                            Value = "lorem, dolor"
                        },
                        new DbField("Entry Date") { Value = DateUtil.ToIsoDate(dateStamp) }
                    }
                }
            })
            {
                var blogItem = db.GetItem("/sitecore/content/blog1");
                var entry1 = db.GetItem("/sitecore/content/blog1/entry1");
                var entry2 = db.GetItem("/sitecore/content/blog1/entry2");
                var entry3 = db.GetItem("/sitecore/content/blog1/entry3");

                var entryManager = Mock.Of<EntryManager>(x =>
                    x.GetBlogEntries(blogItem) == new EntryItem[] { entry1, entry2, entry3 }
                );

                var manager = new TagManager(entryManager);
                var tags = manager.GetTagsForBlog(blogItem);

                Assert.That(tags, Is.EqualTo(new[]
                {
                    new Tag("lorem", dateStamp, 3),
                    new Tag("dolor", dateStamp, 2),
                    new Tag("ipsum", dateStamp, 1)
                    
                }));
            }
        }

        [Test]
        public void GetTagsForEntry_Null()
        {
            var manager = new TagManager();
            var tags = manager.GetTagsForEntry(null);
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
            var dateStamp = System.DateTime.UtcNow.Date;

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
        
        [Test]
        public void SortByWeight_Normal()
        {
            var weightedTags = new TagManager().SortByWeight(new string[] { "a", "a", "b", "c", "c", "c", "c", "a", "b" });
            var expectedTags = new Dictionary<string, int>
            {
                {"a", 3},
                {"b", 2},
                {"c", 4}
            };

            Assert.That(weightedTags, Is.EqualTo(expectedTags));
        }

        [Test]
        public void SortByWeight_SameWeight()
        {
            var weightedTags = new TagManager().SortByWeight(new string[] { "a", "b", "c", "A", "B", "C", "D", "d" });
            var expectedTags = new Dictionary<string, int>
            {
                {"a", 2},
                {"b", 2},
                {"c", 2},
                {"D", 2}
            };

            Assert.That(weightedTags, Is.EqualTo(expectedTags));
        }

        [Test]
        public void SortByWeight_Empty()
        {
            var weightedTags = new TagManager().SortByWeight(new string[0]);
            Assert.That(weightedTags, Is.Empty);
        }

        [Test]
        public void SortByWeight_Null()
        {
            var weightedTags = new TagManager().SortByWeight(null);
            Assert.That(weightedTags, Is.Empty);
        }

        [Test]
        public void SortByWeight_CaseInsensitive()
        {
            var weightedTags = new TagManager().SortByWeight(new[] { "mytag", "MyTag", "MYTAG", "myTaG" });
            Assert.That(weightedTags.Count, Is.EqualTo(1));
        }
    }
}
