using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.UnitTest.Managers
{
    [TestFixture]
    public class TagManagerFixture
    {
        [Test]
        public void GetTagsByBlog_NullItem()
        {
            var manager = new TagManager();
            var tags = manager.GetTagsByBlog((Item)null);

            Assert.That(tags, Is.Empty);
        }

        [Test]
        public void GetTagsByBlog_NoTags()
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
                var tags = manager.GetTagsByBlog(blogItem);

                Assert.That(tags, Is.Empty);
            }
        }

        [Test]
        public void GetTagsByBlog_SomeEntriesNoTags()
        {
            using (var db = new Db
            {
                new DbItem("blog1")
                {
                    new DbItem("entry1")
                    {
                        new DbField("Tags")
                        {
                            Value = "lorem, ipsum"
                        }
                    },
                    new DbItem("entry2")
                    {
                        new DbField("Tags")
                        {
                            Value = "   "
                        }
                    },
                    new DbItem("entry3")
                    {
                        new DbField("Tags")
                        {
                            Value = "dolor "
                        }
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
                var tags = manager.GetTagsByBlog(blogItem);

                Assert.That(tags, Is.EqualTo(new[] { "lorem", "ipsum", "dolor" }));
            }
        }

        [Test]
        public void GetTagsByBlog_SingleTagsOnEntries()
        {
            using (var db = new Db
            {
                new DbItem("blog1")
                {
                    new DbItem("entry1")
                    {
                        new DbField("Tags")
                        {
                            Value = "lorem"
                        }
                    },
                    new DbItem("entry2")
                    {
                        new DbField("Tags")
                        {
                            Value = "ipsum"
                        }
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
                var tags = manager.GetTagsByBlog(blogItem);

                Assert.That(tags, Is.EqualTo(new[] { "lorem", "ipsum" }));
            }
        }

        [Test]
        public void GetTagsByBlog_MultipleTagsOnEntries()
        {
            using (var db = new Db
            {
                new DbItem("blog1")
                {
                    new DbItem("entry1")
                    {
                        new DbField("Tags")
                        {
                            Value = "lorem, ipsum,       dolor     "
                        }
                    },
                    new DbItem("entry2")
                    {
                        new DbField("Tags")
                        {
                            Value = "sit  ,  amed,   pluto "
                        }
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
                var tags = manager.GetTagsByBlog(blogItem);

                Assert.That(tags, Is.EqualTo(new[] { "lorem", "ipsum", "dolor", "sit", "amed", "pluto" }));
            }
        }

        [Test]
        public void GetAllTags_Null()
        {
            var manager = new TagManager();
            var tags = manager.GetAllTags(null);

            Assert.That(tags, Is.Empty);
        }

        [Test]
        public void GetAllTags_NoTags()
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
                var tags = manager.GetAllTags(blogItem);

                Assert.That(tags, Is.Empty);
            }
        }

        [Test]
        public void GetAllTags_WithTags()
        {
            using (var db = new Db
            {
                new DbItem("blog1")
                {
                    new DbItem("entry1")
                    {
                        new DbField("tags")
                        {
                            Value = "lorem, ipsum"
                        }
                    },
                    new DbItem("entry2")
                    {
                        new DbField("tags")
                        {
                            Value = "ipsum, dolor"
                        }
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
                var tags = manager.GetAllTags(blogItem);
                var expectedTags = new Dictionary<string, int>
                {
                    { "ipsum", 2 },
                    { "lorem", 1 },
                    { "dolor", 1 }
                };

                Assert.That(tags, Is.EqualTo(expectedTags));
            }
        }

        [Test]
        public void GetTagsByEntry_Null()
        {
            var manager = new TagManager();
            var tags = manager.GetTagsByEntry(null);
            Assert.That(tags, Is.Empty);
        }

        [Test]
        public void GetTagsByEntry_NoTags()
        {
            using (var db = new Db
            {
                new DbItem("entry")
            })
            {
                var entryItem = db.GetItem("/sitecore/content/entry");
                var manager = new TagManager();
                var tags = manager.GetTagsByEntry(entryItem);
                Assert.That(tags, Is.Empty);
            }
        }

        [Test]
        public void GetTagsByEntry_SingleTag()
        {
            using (var db = new Db
            {
                new DbItem("entry")
                {
                    new DbField("Tags")
                    {
                        Value = "lorem"
                    }
                }
            })
            {
                var entryItem = db.GetItem("/sitecore/content/entry");
                var manager = new TagManager();
                var tags = manager.GetTagsByEntry(entryItem);

                var expectedTags = new Dictionary<string, int>
                {
                    { "lorem", 1 }
                };

                Assert.That(tags, Is.EqualTo(expectedTags));
            }
        }

        [Test]
        public void GetTagsByEntry_MultipleTags()
        {
            using (var db = new Db
            {
                new DbItem("entry")
                {
                    new DbField("Tags")
                    {
                        Value = "dolor, lorem, ipsum, lorem"
                    }
                }
            })
            {
                var entryItem = db.GetItem("/sitecore/content/entry");
                var manager = new TagManager();
                var tags = manager.GetTagsByEntry(entryItem);

                var expectedTags = new Dictionary<string, int>
                {
                    { "lorem", 2 },
                    { "dolor", 1 },
                    { "ipsum", 1 }
                };

                Assert.That(tags, Is.EqualTo(expectedTags));
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
            var weightedTags = new TagManager().SortByWeight(new string[] { "mytag", "MyTag", "MYTAG", "myTaG" });
            Assert.That(weightedTags.Count, Is.EqualTo(1));
        }
    }
}
