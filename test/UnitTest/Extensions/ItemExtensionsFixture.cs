using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.FakeDb.Links;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Extensions;

namespace Sitecore.Modules.WeBlog.UnitTest.Extensions
{
    [TestFixture]
    public class ItemExtensionsFixture
    {
        #region TemplateIsOrBasedOn

        [Test]
        public void TemplateIsOrBasedOn_NullItem()
        {
            var templateId = ID.NewID;

            using (var db = new Db()
            {
                new DbTemplate("dummy", templateId)
            })
            {
                var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.TemplateIsOrBasedOn(null, new[] {templateId});
                Assert.That(result, Is.False);
            }
        }

        [Test]
        public void TemplateIsOrBasedOn_NullTemplates()
        {
            using (var db = new Db()
            {
                new DbItem("theitem", ID.NewID)
            })
            {
                var item = db.GetItem("/sitecore/content/theitem");
                var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.TemplateIsOrBasedOn(item, (IEnumerable<ID>)null);
                Assert.That(result, Is.False);
            }
        }

        [Test]
        public void TemplateIsOrBasedOn_NoTemplates()
        {
            using (var db = new Db()
            {
                new DbItem("theitem", ID.NewID)
            })
            {
                var item = db.GetItem("/sitecore/content/theitem");
                var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.TemplateIsOrBasedOn(item, Enumerable.Empty<ID>());
                Assert.That(result, Is.False);
            }
        }

        [Test]
        public void TemplateIsOrBasedOn_InvalidTemplate()
        {
            using (var db = new Db()
            {
                new DbItem("theitem", ID.NewID)
            })
            {
                var item = db.GetItem("/sitecore/content/theitem");
                var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.TemplateIsOrBasedOn(item, new[] { ID.NewID } );
                Assert.That(result, Is.False);
            }
        }

        [Test]
        public void TemplateIsOrBasedOn_ValidTemplate()
        {
            var templateId = ID.NewID;

            using (var db = new Db()
            {
                new DbItem("theitem", ID.NewID, templateId)
            })
            {
                var item = db.GetItem("/sitecore/content/theitem");
                var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.TemplateIsOrBasedOn(item, new[] { templateId });
                Assert.That(result, Is.True);
            }
        }

        [Test]
        public void TemplateIsOrBasedOn_ValidDerivedTemplate()
        {
            var baseTemplateId = ID.NewID;
            var templateId = ID.NewID;

            using (var db = new Db()
            {
                new DbTemplate("base template", baseTemplateId),
                new DbTemplate("dummy", templateId)
                {
                    BaseIDs = new [] { baseTemplateId }
                },
                new DbItem("theitem", ID.NewID, templateId)
            })
            {
                var item = db.GetItem("/sitecore/content/theitem");
                var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.TemplateIsOrBasedOn(item, new[] { baseTemplateId });
                Assert.That(result, Is.True);
            }
        }

        [Test]
        public void TemplateIsOrBasedOn_ValidMultipleDerivedTemplate()
        {
            var baseTemplateId = ID.NewID;
            var baseTemplateId2 = ID.NewID;
            var baseTemplateId3 = ID.NewID;
            var templateId = ID.NewID;

            using (var db = new Db()
            {
                new DbTemplate("base template", baseTemplateId),
                new DbTemplate("base template 2", baseTemplateId2)
                {
                    BaseIDs = new [] { baseTemplateId }
                },
                new DbTemplate("base template 3", baseTemplateId3)
                {
                    BaseIDs = new [] { baseTemplateId2 }
                },
                new DbTemplate("dummy", templateId)
                {
                    BaseIDs = new [] { baseTemplateId3 }
                },
                new DbItem("theitem", ID.NewID, templateId)
            })
            {
                var item = db.GetItem("/sitecore/content/theitem");
                var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.TemplateIsOrBasedOn(item, new[] { baseTemplateId });
                Assert.That(result, Is.True);
            }
        }

        [Test]
        public void TemplateIsOrBasedOn_ValidSingleTemplate()
        {
            var templateId = ID.NewID;

            using (var db = new Db()
            {
                new DbItem("theitem", ID.NewID, templateId)
            })
            {
                var item = db.GetItem("/sitecore/content/theitem");
                var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.TemplateIsOrBasedOn(item, templateId);
                Assert.That(result, Is.True);
            }
        }

        [Test]
        public void TemplateIsOrBasedOn_InvalidSingleTemplate()
        {
            using (var db = new Db()
            {
                new DbItem("theitem")
            })
            {
                var item = db.GetItem("/sitecore/content/theitem");
                var result = Sitecore.Modules.WeBlog.Extensions.ItemExtensions.TemplateIsOrBasedOn(item, ID.NewID);
                Assert.That(result, Is.False);
            }
        }

        #endregion TemplateIsOrBasedOn

        #region FindAncestorByAnyTemplate

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

        #endregion FindAncestorByAnyTemplate

        #region FindItemsByTemplateOrDerivedTemplate

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

        #endregion
    }
}
