using Moq;
using NUnit.Framework;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.FakeDb.Links;
using Sitecore.Links;
using System.Linq;

namespace Sitecore.Modules.WeBlog.UnitTest.Extensions
{
    [TestFixture]
    public class ItemExtensionsFixture
    {
        [Test]
        public void FindAncestorByAnyTemplate_NullItem_ReturnsNull()
        {
            // arrange
            var templateId = ID.NewID;
            var templateManager = TemplateFactory.CreateTemplateManager(templateId);

            // act
            var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.FindAncestorByAnyTemplate(null, new[] { templateId }, templateManager);

            // assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void FindAncestorByAnyTemplate_NullTemplates_ReturnsNull()
        {
            // arrange
            var templateId = ID.NewID;
            var item = ItemFactory.CreateItem(templateId: templateId).Object;
            var templateManager = TemplateFactory.CreateTemplateManager(templateId);

            // act
            var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.FindAncestorByAnyTemplate(item, null, templateManager);

            // assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void FindAncestorByAnyTemplate_NoTemplates_ReturnsNull()
        {
            // arrange
            var templateId = ID.NewID;
            var item = ItemFactory.CreateItem(templateId: templateId).Object;
            var templateManager = TemplateFactory.CreateTemplateManager(templateId);

            // act
            var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.FindAncestorByAnyTemplate(item, Enumerable.Empty<ID>(), templateManager);

            // assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void FindAncestorByAnyTemplate_ItemBasedOnTemplate_ReturnsItem()
        {
            // arrange
            var templateId = ID.NewID;
            var item = ItemFactory.CreateItem(templateId: templateId).Object;
            var templateManager = TemplateFactory.CreateTemplateManager(templateId);

            // act
            var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.FindAncestorByAnyTemplate(item, new[] { templateId }, templateManager);

            // assert
            Assert.That(result.ID, Is.EqualTo(item.ID));
        }

        [Test]
        public void FindAncestorByAnyTemplate_ParentBasedOnTemplate_ReturnsParent()
        {
            // arrange
            var templateId = ID.NewID;
            var itemMock = ItemFactory.CreateItem();
            var parentItem = ItemFactory.CreateItem(templateId: templateId).Object;
            ItemFactory.SetParent(itemMock, parentItem);

            var templateManager = TemplateFactory.CreateTemplateManager(templateId);

            // act
            var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.FindAncestorByAnyTemplate(itemMock.Object, new[] { templateId }, templateManager);

            // assert
            Assert.That(result.ID, Is.EqualTo(parentItem.ID));
        }

        [Test]
        public void FindAncestorByAnyTemplate_AncestorBasedOnTemplate_ReturnsAncestor()
        {
            // arrange
            var templateId = ID.NewID;
            var itemMock = ItemFactory.CreateItem();
            var parentItemMock = ItemFactory.CreateItem();
            var grantParentItemMock = ItemFactory.CreateItem(templateId: templateId).Object;
            
            ItemFactory.SetParent(itemMock, parentItemMock.Object);
            ItemFactory.SetParent(parentItemMock, grantParentItemMock);

            var templateManager = TemplateFactory.CreateTemplateManager(templateId);

            // act
            var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.FindAncestorByAnyTemplate(itemMock.Object, new[] { templateId }, templateManager);

            // assert
            Assert.That(result.ID, Is.EqualTo(grantParentItemMock.ID));
        }

        [Test]
        public void FindAncestorByAnyTemplate_NoAncestorsBasedOnTemplate_ReturnsNull()
        {
            // arrange
            var templateId = ID.NewID;
            var itemMock = ItemFactory.CreateItem();
            var parentItem = ItemFactory.CreateItem().Object;
            ItemFactory.SetParent(itemMock, parentItem);

            var templateManager = TemplateFactory.CreateTemplateManager(templateId);

            // act
            var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.FindAncestorByAnyTemplate(itemMock.Object, new[] { templateId }, templateManager);

            // assert
            Assert.That(result, Is.Null);
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
