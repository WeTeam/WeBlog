using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.Data.Templates;
using Sitecore.FakeDb;
using Sitecore.FakeDb.Links;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Extensions;

namespace Sitecore.Modules.WeBlog.UnitTest.Extensions
{
    [TestFixture]
    public class ItemExtensionsFixture
    {
        [Test]
        [Obsolete("API has been deprecated.")]
        public void TemplateIsOrBasedOn_ItemIsNull_ReturnsFalse()
        {
            // arrange
            var templateId = ID.NewID;
            var templateManager = TemplateFactory.CreateTemplateManager(templateId);

            // act
            var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.TemplateIsOrBasedOn(null, templateManager, new[] { templateId });

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        [Obsolete("API has been deprecated.")]
        public void TemplateIsOrBasedOn_TemplatesIsNull_ReturnsFalse()
        {
            // arrange
            var templateId = ID.NewID;
            var templateManager = TemplateFactory.CreateTemplateManager(templateId);
            var itemMock = ItemFactory.CreateItem(templateId);

            // act
            var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.TemplateIsOrBasedOn(itemMock.Object, templateManager, (IEnumerable<ID>)null);

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        [Obsolete("API has been deprecated.")]
        public void TemplateIsOrBasedOn_TemplatesIsEmpty_ReturnsFalse()
        {
            // arrange
            var templateId = ID.NewID;
            var templateManager = TemplateFactory.CreateTemplateManager(templateId);
            var itemMock = ItemFactory.CreateItem(templateId);

            // act
            var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.TemplateIsOrBasedOn(itemMock.Object, templateManager, Enumerable.Empty<ID>());

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        [Obsolete("API has been deprecated.")]
        public void TemplateIsOrBasedOn_InvalidTemplate_ReturnsFalse()
        {
            // arrange
            var templateId = ID.NewID;
            var templateManager = TemplateFactory.CreateTemplateManager(templateId);
            var itemMock = ItemFactory.CreateItem(templateId);

            // act
            var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.TemplateIsOrBasedOn(itemMock.Object, templateManager, new[] { ID.NewID });

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        [Obsolete("API has been deprecated.")]
        public void TemplateIsOrBasedOn_TemplateMatches_ReturnsTrue()
        {
            // arrange
            var templateId = ID.NewID;
            var templateManager = TemplateFactory.CreateTemplateManager(templateId);
            var itemMock = ItemFactory.CreateItem(templateId: templateId);

            // act
            var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.TemplateIsOrBasedOn(itemMock.Object, templateManager, new[] { templateId });

            // assert
            Assert.That(result, Is.True);
        }

        [Test]
        [Obsolete("API has been deprecated.")]
        public void TemplateIsOrBasedOn_TemplateDoesNotMatch_ReturnsFalse()
        {
            // arrange
            var templateId1 = ID.NewID;
            var templateId2 = ID.NewID;
            var templateManager = TemplateFactory.CreateTemplateManager(templateId1, templateId2);
            var itemMock = ItemFactory.CreateItem(templateId2);

            // act
            var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.TemplateIsOrBasedOn(itemMock.Object, templateManager, new[] { templateId1 });

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        [Obsolete("API has been deprecated.")]
        public void TemplateIsOrBasedOn_DerivedTemplateMatches_ReturnsTrue()
        {
            // arrange
            var baseTemplateId = ID.NewID;
            var templateId = ID.NewID;

            var templates = new TemplateCollection();
            var baseTemplate = TemplateFactory.CreateTemplate(baseTemplateId, null, templates);
            var template = TemplateFactory.CreateTemplate(templateId, baseTemplateId, templates);

            var templateManager = TemplateFactory.CreateTemplateManager(new[] { baseTemplate, template });
            var itemMock = ItemFactory.CreateItem(templateId: templateId);

            // act
            var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.TemplateIsOrBasedOn(itemMock.Object, templateManager, new[] { baseTemplateId });

            // assert
            Assert.That(result, Is.True);
        }

        [Test]
        [Obsolete("API has been deprecated.")]
        public void TemplateIsOrBasedOn_DerivedTemplateChainMatches_ReturnsTrue()
        {
            // arrange
            var baseTemplateId1 = ID.NewID;
            var baseTemplateId2 = ID.NewID;
            var baseTemplateId3 = ID.NewID;
            var templateId = ID.NewID;

            var templates = new TemplateCollection();
            var baseTemplate1 = TemplateFactory.CreateTemplate(baseTemplateId1, null, templates);
            var baseTemplate2 = TemplateFactory.CreateTemplate(baseTemplateId2, baseTemplateId1, templates);
            var baseTemplate3 = TemplateFactory.CreateTemplate(baseTemplateId3, baseTemplateId2, templates);
            var template = TemplateFactory.CreateTemplate(templateId, baseTemplateId3, templates);

            var templateManager = TemplateFactory.CreateTemplateManager(new[] { baseTemplate1, baseTemplate2, baseTemplate3, template });
            var itemMock = ItemFactory.CreateItem(templateId: templateId);

            // act
            var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.TemplateIsOrBasedOn(itemMock.Object, templateManager, new[] { baseTemplateId1 });

            // assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void FindAncestorByAnyTemplate_NullItem()
        {
            var templateId = ID.NewID;

            using (var db = new Db()
            {
                new DbTemplate("dummy", templateId)
            })
            {
                var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.FindAncestorByAnyTemplate(null, new[] { templateId });
                Assert.That(result, Is.Null);
            }
        }

        [Test]
        public void FindAncestorByAnyTemplate_NullTemplates()
        {
            var itemId = ID.NewID;

            using (var db = new Db()
            {
                new DbItem("theitem", itemId)
            })
            {
                var item = db.GetItem("/sitecore/content/theitem");
                var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.FindAncestorByAnyTemplate(item, null);
                Assert.That(result, Is.Null);
            }
        }

        [Test]
        public void FindAncestorByAnyTemplate_NoTemplates()
        {
            var itemId = ID.NewID;

            using (var db = new Db()
            {
                new DbItem("theitem", itemId)
            })
            {
                var item = db.GetItem("/sitecore/content/theitem");
                var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.FindAncestorByAnyTemplate(item, Enumerable.Empty<ID>());
                Assert.That(result, Is.Null);
            }
        }

        [Test]
        public void FindAncestorByAnyTemplate_Exists_Item()
        {
            var templateId = ID.NewID;

            using (var db = new Db()
            {
                new DbItem("theitem", ID.NewID, templateId)
            })
            {
                var item = db.GetItem("/sitecore/content/theitem");
                var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.FindAncestorByAnyTemplate(item, new[] { templateId });
                Assert.That(result, Is.Not.Null);
            }
        }

        [Test]
        public void FindAncestorByAnyTemplate_Exists_Parent()
        {
            var templateId = ID.NewID;

            using (var db = new Db()
            {
                new DbItem("parent", ID.NewID, templateId)
                {
                    new DbItem("theitem")
                }
            })
            {
                var item = db.GetItem("/sitecore/content/parent/theitem");
                var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.FindAncestorByAnyTemplate(item, new[] { templateId });
                Assert.That(result, Is.Not.Null);
            }
        }

        [Test]
        public void FindAncestorByAnyTemplate_Exists_GrandGrandParent()
        {
            var templateId = ID.NewID;

            using (var db = new Db()
            {
                new DbItem("ggp", ID.NewID, templateId)
                {
                    new DbItem("gp")
                    {
                        new DbItem("parent")
                        {
                            new DbItem("theitem")
                        }
                    }
                }
            })
            {
                var item = db.GetItem("/sitecore/content/ggp/gp/parent/theitem");
                var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.FindAncestorByAnyTemplate(item, new[] { templateId });
                Assert.That(result, Is.Not.Null);
            }
        }

        [Test]
        public void FindAncestorByAnyTemplate_NotExists()
        {
            using (var db = new Db()
            {
                new DbItem("theitem")
            })
            {
                var item = db.GetItem("/sitecore/content/theitem");
                var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.FindAncestorByAnyTemplate(item, new[] { ID.NewID });
                Assert.That(result, Is.Null);
            }
        }

        [Test]
        public void FindItemsByTemplateOrDerivedTemplate_NullItem()
        {
            var templateId = ID.NewID;

            using (var db = new Db()
            {
                new DbItem("blog1", ID.NewID, templateId),
                new DbItem("normal item")
            })
            {
                var root = db.GetItem("/sitecore/content");
                var template = root.Database.GetTemplate(templateId);
                var blog1 = db.GetItem("/sitecore/content/blog1");


                // Setup LinkDatabase
                var linkDb = Mock.Of<Sitecore.Links.LinkDatabase>(x =>
                    x.GetReferrers(template) == new[]
                    {
                        new ItemLink("master", blog1.ID, ID.Null, "master", templateId, templateId.ToString())
                    });

                using (new LinkDatabaseSwitcher(linkDb))
                {
                    var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.FindItemsByTemplateOrDerivedTemplate(null, template);

                    Assert.That(result.Length, Is.EqualTo(0));
                }
            }
        }

        [Test]
        public void FindItemsByTemplateOrDerivedTemplate_NullTemplate()
        {
            var templateId = ID.NewID;

            using (var db = new Db()
            {
                new DbItem("blog1", ID.NewID, templateId),
                new DbItem("normal item")
            })
            {
                var root = db.GetItem("/sitecore/content");
                var template = root.Database.GetTemplate(templateId);
                var blog1 = db.GetItem("/sitecore/content/blog1");


                // Setup LinkDatabase
                var linkDb = Mock.Of<Sitecore.Links.LinkDatabase>(x =>
                    x.GetReferrers(template) == new[]
                    {
                        new ItemLink("master", blog1.ID, ID.Null, "master", templateId, templateId.ToString())
                    });

                using (new LinkDatabaseSwitcher(linkDb))
                {
                    var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.FindItemsByTemplateOrDerivedTemplate(root, null);

                    Assert.That(result.Length, Is.EqualTo(0));
                }
            }
        }

        [Test]
        public void FindItemsByTemplateOrDerivedTemplate_ItemsBasedOnTemplate()
        {
            var templateId = ID.NewID;

            using (var db = new Db()
            {
                new DbItem("blog1", ID.NewID, templateId),
                new DbItem("blog2", ID.NewID, templateId),
                new DbItem("normal item")
            })
            {
                var root = db.GetItem("/sitecore/content");
                var template = root.Database.GetTemplate(templateId);

                var blog1 = db.GetItem("/sitecore/content/blog1");
                var blog2 = db.GetItem("/sitecore/content/blog2");

                // Setup LinkDatabase
                var linkDb = Mock.Of<Sitecore.Links.LinkDatabase>(x =>
                    x.GetReferrers(template) == new[]
                    {
                        new ItemLink("master", blog1.ID, ID.Null, "master", templateId, templateId.ToString()),
                        new ItemLink("master", blog2.ID, ID.Null, "master", templateId, templateId.ToString()),
                    });

                using (new LinkDatabaseSwitcher(linkDb))
                {
                    var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.FindItemsByTemplateOrDerivedTemplate(root, template);

                    Assert.That(result.Length, Is.EqualTo(2));
                    Assert.That(result[0].Name, Is.EqualTo("blog1"));
                    Assert.That(result[1].Name, Is.EqualTo("blog2"));
                }
            }
        }

        [Test]
        public void FindItemsByTemplateOrDerivedTemplate_ItemsBasedOnDerivedTemplate()
        {
            var baseTemplateId = ID.NewID;
            var templateId = ID.NewID;

            using (var db = new Db()
            {
                new DbTemplate(baseTemplateId),
                new DbTemplate(templateId)
                {
                    BaseIDs = new [] { baseTemplateId }
                },
                new DbItem("blog1", ID.NewID, templateId),
                new DbItem("blog2", ID.NewID, templateId),
                new DbItem("normal item")
            })
            {
                var root = db.GetItem("/sitecore/content");
                var template = root.Database.GetTemplate(templateId);
                var baseTemplate = root.Database.GetTemplate(baseTemplateId);

                var blog1 = db.GetItem("/sitecore/content/blog1");
                var blog2 = db.GetItem("/sitecore/content/blog2");

                // Setup LinkDatabase
                var linkDb = Mock.Of<Sitecore.Links.LinkDatabase>(x =>
                    x.GetReferrers(baseTemplate) == new[]
                    {
                        new ItemLink("master", templateId, FieldIDs.BaseTemplate, "master", baseTemplateId, baseTemplateId.ToString()),
                    } &&
                    x.GetReferrers(template) == new[]
                    {
                        new ItemLink("master", blog1.ID, ID.Null, "master", templateId, templateId.ToString()),
                        new ItemLink("master", blog2.ID, ID.Null, "master", templateId, templateId.ToString()),
                    });

                using (new LinkDatabaseSwitcher(linkDb))
                {
                    var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.FindItemsByTemplateOrDerivedTemplate(root, baseTemplate);

                    Assert.That(result.Length, Is.EqualTo(2));
                    Assert.That(result[0].Name, Is.EqualTo("blog1"));
                    Assert.That(result[1].Name, Is.EqualTo("blog2"));
                }
            }
        }

        [Test]
        public void FindItemsByTemplateOrDerivedTemplate_ItemsBasedOnManyDerivedTemplates()
        {
            var baseBaseTemplateId = ID.NewID;
            var baseTemplateId = ID.NewID;
            var templateId = ID.NewID;

            using (var db = new Db()
            {
                new DbTemplate(baseBaseTemplateId),
                new DbTemplate(baseTemplateId)
                {
                    BaseIDs = new [] { baseBaseTemplateId }
                },
                new DbTemplate(templateId)
                {
                    BaseIDs = new [] { baseTemplateId }
                },
                new DbItem("blog1", ID.NewID, templateId),
                new DbItem("blog2", ID.NewID, templateId),
                new DbItem("blog3", ID.NewID, baseTemplateId),
                new DbItem("blog4", ID.NewID, baseBaseTemplateId),
                new DbItem("normal item")
            })
            {
                var root = db.GetItem("/sitecore/content");
                var template = root.Database.GetTemplate(templateId);
                var baseTemplate = root.Database.GetTemplate(baseTemplateId);
                var baseBaseTemplate = root.Database.GetTemplate(baseBaseTemplateId);

                var blog1 = db.GetItem("/sitecore/content/blog1");
                var blog2 = db.GetItem("/sitecore/content/blog2");
                var blog3 = db.GetItem("/sitecore/content/blog3");
                var blog4 = db.GetItem("/sitecore/content/blog4");

                // Setup LinkDatabase
                var linkDb = Mock.Of<Sitecore.Links.LinkDatabase>(x =>
                    x.GetReferrers(baseBaseTemplate) == new[]
                    {
                        new ItemLink("master", baseTemplateId, FieldIDs.BaseTemplate, "master", baseBaseTemplateId, baseBaseTemplate.ToString()),
                    } &&
                    x.GetReferrers(baseTemplate) == new[]
                    {
                        new ItemLink("master", templateId, FieldIDs.BaseTemplate, "master", baseTemplateId, baseTemplateId.ToString()),
                    } &&
                    x.GetReferrers(baseBaseTemplate) == new[]
                    {
                        new ItemLink("master", blog1.ID, ID.Null, "master", templateId, templateId.ToString()),
                        new ItemLink("master", blog2.ID, ID.Null, "master", templateId, templateId.ToString()),
                        new ItemLink("master", blog3.ID, ID.Null, "master", baseTemplateId, templateId.ToString()),
                        new ItemLink("master", blog4.ID, ID.Null, "master", baseBaseTemplateId, templateId.ToString()),
                    });

                using (new LinkDatabaseSwitcher(linkDb))
                {
                    var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.FindItemsByTemplateOrDerivedTemplate(root, baseBaseTemplate);

                    Assert.That(result.Length, Is.EqualTo(4));
                    Assert.That(result[0].Name, Is.EqualTo("blog1"));
                    Assert.That(result[1].Name, Is.EqualTo("blog2"));
                    Assert.That(result[2].Name, Is.EqualTo("blog3"));
                    Assert.That(result[3].Name, Is.EqualTo("blog4"));
                }
            }
        }
    }
}
