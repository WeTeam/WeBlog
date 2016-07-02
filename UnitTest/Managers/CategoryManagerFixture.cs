using System.Linq;
using Moq;
using NUnit.Framework;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.SecurityModel;

namespace Sitecore.Modules.WeBlog.UnitTest.Managers
{
    [TestFixture]
    public class CategoryManagerFixture
    {
        [TestCase(null, false, TestName = "Null Item")]
        [TestCase("/sitecore/content", false, TestName = "Outside blog")]
        [TestCase("/sitecore/content/blog", true, TestName = "On blog")]
        [TestCase("/sitecore/content/blog/a folder/entry1", true, TestName = "Below blog")]
        public void GetCategoryRoot(string startItemPath, bool expectCategoryRoot)
        {
            var settings = MockSettings(ID.NewID);
            var manager = new CategoryManager(settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First())
                {
                    new DbItem("Categories", ID.NewID, ID.NewID)
                    {
                        new DbItem("alpha", ID.NewID, settings.CategoryTemplateIds.First()),
                        new DbItem("beta", ID.NewID, settings.CategoryTemplateIds.First()),
                        new DbItem("gamma", ID.NewID, settings.CategoryTemplateIds.First())
                    },
                    new DbItem("a folder", ID.NewID, ID.NewID)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                var startItem = string.IsNullOrEmpty(startItemPath) ? null : db.GetItem(startItemPath);
                var categoryRoot = manager.GetCategoryRoot(startItem);

                if (expectCategoryRoot)
                {
                    Assert.That(categoryRoot, Is.Not.Null);
                    Assert.That(categoryRoot.Name, Is.EqualTo("Categories"));
                }
                else
                    Assert.That(categoryRoot, Is.Null);
            }
        }

        [TestCase(null, false, TestName = "Null Item")]
        [TestCase("/sitecore/content", false, TestName = "Outside blog")]
        [TestCase("/sitecore/content/blog", true, TestName = "On blog")]
        [TestCase("/sitecore/content/blog/a folder/entry1", true, TestName = "Below blog")]
        public void GetCategories(string startItemPath, bool expectCategories)
        {
            var settings = MockSettings(ID.NewID);
            var manager = new CategoryManager(settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First())
                {
                    new DbItem("Categories", ID.NewID, ID.NewID)
                    {
                        new DbItem("alpha", ID.NewID, settings.CategoryTemplateIds.First()),
                        new DbItem("beta", ID.NewID, settings.CategoryTemplateIds.First()),
                        new DbItem("gamma", ID.NewID, settings.CategoryTemplateIds.First())
                    },
                    new DbItem("a folder", ID.NewID, ID.NewID)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                var startItem = string.IsNullOrEmpty(startItemPath) ? null : db.GetItem(startItemPath);
                var categories = manager.GetCategories(startItem);
                var categoryNames = from c in categories select c.Name;

                if (expectCategories)
                    Assert.That(categoryNames, Is.EquivalentTo(new [] { "alpha", "beta", "gamma" }));
                else
                    Assert.That(categoryNames, Is.Empty);
            }
        }

        [Test]
        public void GetCategories_NoCategories()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new CategoryManager(settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First())
                {
                    new DbItem("Categories", ID.NewID, ID.NewID),
                    new DbItem("Entries", ID.NewID, ID.NewID)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                var entryItem = db.GetItem("/sitecore/content/blog/entries/entry1");
                var categories = manager.GetCategories(entryItem);

                Assert.That(categories, Is.Empty);
            }
        }

        [Test]
        public void GetCategories_MixedTemplates()
        {
            var settings = MockSettings(ID.NewID, ID.NewID, ID.NewID);
            var manager = new CategoryManager(settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First())
                {
                    new DbItem("Categories", ID.NewID, ID.NewID)
                    {
                        new DbItem("alpha", ID.NewID, settings.CategoryTemplateIds.ElementAt(0)),
                        new DbItem("beta", ID.NewID, settings.CategoryTemplateIds.ElementAt(1)),
                        new DbItem("gamma", ID.NewID, settings.CategoryTemplateIds.ElementAt(2))
                    },
                    new DbItem("Entries", ID.NewID, ID.NewID)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                var entryItem = db.GetItem("/sitecore/content/blog/entries/entry1");
                var categories = manager.GetCategories(entryItem);
                var categoryNames = from c in categories select c.Name;

                Assert.That(categoryNames, Is.EquivalentTo(new[] { "alpha", "beta", "gamma" }));
            }
        }

        [Test]
        public void GetCategories_MixedDerivedTemplates()
        {
            var baseBaseTemplateId = ID.NewID;
            var baseTemplateId = ID.NewID;
            var categoryTemplateId = ID.NewID;
            
            var settings = MockSettings(baseBaseTemplateId, baseTemplateId, categoryTemplateId);
            var manager = new CategoryManager(settings);

            using (var db = new Db
            {
                new DbTemplate(baseBaseTemplateId),
                new DbTemplate(baseTemplateId)
                {
                    BaseIDs = new[] { baseBaseTemplateId }
                },
                new DbTemplate(categoryTemplateId)
                {
                    BaseIDs = new[] { baseTemplateId }
                },
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First())
                {
                    new DbItem("Categories", ID.NewID, ID.NewID)
                    {
                        new DbItem("alpha", ID.NewID, categoryTemplateId),
                        new DbItem("beta", ID.NewID, baseTemplateId),
                        new DbItem("gamma", ID.NewID, baseBaseTemplateId)
                    },
                    new DbItem("Entries", ID.NewID, ID.NewID)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                var entryItem = db.GetItem("/sitecore/content/blog/entries/entry1");
                var categories = manager.GetCategories(entryItem);
                var categoryNames = from c in categories select c.Name;

                Assert.That(categoryNames, Is.EquivalentTo(new[] { "alpha", "beta", "gamma" }));
            }
        }

        [TestCase("/sitecore/content", "alpha", false, TestName = "Outside blog")]
        [TestCase("/sitecore/content/blog", "beta", true, TestName = "On blog")]
        [TestCase("/sitecore/content/blog/entries/entry1", "gamma", true, TestName = "Below blog")]
        [TestCase("/sitecore/content/blog/entries/entry1", "lorem", false, TestName = "Unknown category")]
        public void GetCategory(string startItemPath, string categoryName, bool shouldFindCategory)
        {
            var settings = MockSettings(ID.NewID);
            var manager = new CategoryManager(settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First())
                {
                    new DbItem("Categories", ID.NewID, ID.NewID)
                    {
                        new DbItem("alpha", ID.NewID, settings.CategoryTemplateIds.First()),
                        new DbItem("beta", ID.NewID, settings.CategoryTemplateIds.First()),
                        new DbItem("gamma", ID.NewID, settings.CategoryTemplateIds.First())
                    },
                    new DbItem("Entries", ID.NewID, ID.NewID)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                var startItem = string.IsNullOrEmpty(startItemPath) ? null : db.GetItem(startItemPath);
                var categoryItem = manager.GetCategory(startItem, categoryName);

                if (shouldFindCategory)
                {
                    Assert.That(categoryItem, Is.Not.Null);
                    Assert.That(categoryItem.Name, Is.EqualTo(categoryName));
                }
                else
                    Assert.That(categoryItem, Is.Null);
            }
        }

        [Test]
        public void GetCategory_NoCategories()
        {
            var settings = MockSettings(ID.NewID);
            var manager = new CategoryManager(settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First())
                {
                    new DbItem("Categories", ID.NewID, ID.NewID),
                    new DbItem("Entries", ID.NewID, ID.NewID)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                var entryItem = db.GetItem("/sitecore/content/blog/entries/entry1");
                var categoryItem = manager.GetCategory(entryItem, "alpha");

                Assert.That(categoryItem, Is.Null);
            }
        }

        [TestCase(null, "lorem", false, TestName = "Null Item")]
        [TestCase("/sitecore/content", "lorem", false, TestName = "Outside blog")]
        [TestCase("/sitecore/content/blog", "lorem", true, TestName = "On blog new")]
        [TestCase("/sitecore/content/blog", "alpha", true, TestName = "On blog existing")]
        [TestCase("/sitecore/content/blog/entries/entry1", "alpha", true, TestName = "On entry new")]
        [TestCase("/sitecore/content/blog/entries/entry1", "lorem", true, TestName = "On entry existing")]
        public void Add(string contextItemPath, string name, bool expectCategory)
        {
            var settings = MockSettings(ID.NewID, ID.NewID);
            var manager = new CategoryManager(settings);

            using (var db = new Db
            {
                new DbTemplate("category1", settings.CategoryTemplateIds.ElementAt(0))
                {
                    new DbField("Title")
                },
                new DbTemplate("category2", settings.CategoryTemplateIds.ElementAt(1))
                {
                    new DbField("Title")
                },
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First())
                {
                    new DbItem("Categories", ID.NewID, ID.NewID)
                    {
                        new DbItem("alpha", ID.NewID, settings.CategoryTemplateIds.ElementAt(0)),
                        new DbItem("beta", ID.NewID, settings.CategoryTemplateIds.ElementAt(1)),
                        new DbItem("gamma", ID.NewID, settings.CategoryTemplateIds.ElementAt(0))
                    },
                    new DbItem("Entries", ID.NewID, ID.NewID)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                var entryItem = contextItemPath != null ? db.GetItem(contextItemPath) : null;
                CategoryItem categoryItem = null;

                using (new SecurityDisabler())
                {
                    categoryItem = manager.Add(name, entryItem);
                }

            if (expectCategory)
                {
                    Assert.That(categoryItem, Is.Not.Null);
                    Assert.That(categoryItem.Name, Is.EqualTo(name));
                }
                else
                    Assert.That(categoryItem, Is.Null);
            }
        }

        [Test]
        public void GetCategoriesForEntry_NullItem()
        {
            var settings = MockSettings(ID.NewID, ID.NewID);
            var manager = new CategoryManager(settings);

            var categories = manager.GetCategoriesForEntry(null);
            Assert.That(categories, Is.Empty);
        }

        [Test]
        public void GetCategoriesForEntry_NotAnEntryItem()
        {
            var settings = MockSettings(ID.NewID, ID.NewID);
            var manager = new CategoryManager(settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First())
                {
                    new DbItem("Categories", ID.NewID, ID.NewID)
                    {
                        new DbItem("alpha", ID.NewID, settings.CategoryTemplateIds.ElementAt(0)),
                        new DbItem("beta", ID.NewID, settings.CategoryTemplateIds.ElementAt(1)),
                        new DbItem("gamma", ID.NewID, settings.CategoryTemplateIds.ElementAt(0))
                    },
                    new DbItem("Entries", ID.NewID, ID.NewID)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                var item = db.GetItem("/sitecore/content/blog");
                var categories = manager.GetCategoriesForEntry(item);
                Assert.That(categories, Is.Empty);
            }
        }

        [Test]
        public void GetCategoriesForEntry_NoCategoriesOnEntry()
        {
            var settings = MockSettings(ID.NewID, ID.NewID);
            var manager = new CategoryManager(settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First())
                {
                    new DbItem("Categories", ID.NewID, ID.NewID)
                    {
                        new DbItem("alpha", ID.NewID, settings.CategoryTemplateIds.ElementAt(0)),
                        new DbItem("beta", ID.NewID, settings.CategoryTemplateIds.ElementAt(1)),
                        new DbItem("gamma", ID.NewID, settings.CategoryTemplateIds.ElementAt(0))
                    },
                    new DbItem("Entries", ID.NewID, ID.NewID)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                var item = db.GetItem("/sitecore/content/blog/entries/entry1");
                var categories = manager.GetCategoriesForEntry(item);
                Assert.That(categories, Is.Empty);
            }
        }

        [Test]
        public void GetCategoriesForEntry_WithCategories()
        {
            var settings = MockSettings(ID.NewID, ID.NewID);
            var manager = new CategoryManager(settings);

            var categoryFieldId = ID.NewID;
            var cat1Id = ID.NewID;
            var cat2Id = ID.NewID;

            using (var db = new Db
            {
                new DbTemplate("entry", settings.EntryTemplateIds.First())
                {
                    new DbField("Category", categoryFieldId)
                },
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First())
                {
                    new DbItem("Categories", ID.NewID, ID.NewID)
                    {
                        new DbItem("alpha", cat1Id, settings.CategoryTemplateIds.ElementAt(0)),
                        new DbItem("beta", cat2Id, settings.CategoryTemplateIds.ElementAt(1)),
                        new DbItem("gamma", ID.NewID, settings.CategoryTemplateIds.ElementAt(0))
                    },
                    new DbItem("Entries", ID.NewID, ID.NewID)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First())
                        {
                            new DbField(categoryFieldId)
                            {
                                Value = cat1Id + "|" + cat2Id
                            }
                        }
                    }
                }
            })
            {
                var item = db.GetItem("/sitecore/content/blog/entries/entry1");
                var categories = manager.GetCategoriesForEntry(item);
                var categoryNames = from c in categories select c.Name;

                Assert.That(categoryNames, Is.EquivalentTo(new[] { "alpha", "beta" }));
            }
        }

        private IWeBlogSettings MockSettings(params ID[] categoryTemplateIds)
        {
            return Mock.Of<IWeBlogSettings>(x =>
                x.BlogTemplateIds == new[] { ID.NewID, ID.NewID } &&
                x.CategoryTemplateIds == categoryTemplateIds &&
                x.EntryTemplateIds == new[]{ ID.NewID }
            );
        }
    }
}
