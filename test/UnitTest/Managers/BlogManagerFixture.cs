using System;
using System.Linq;
using System.Web.Security;
using Moq;
using NUnit.Framework;
using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.FakeDb.Links;
using Sitecore.FakeDb.Security.Accounts;
using Sitecore.FakeDb.Security.Web;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Security.AccessControl;

namespace Sitecore.Modules.WeBlog.UnitTest.Managers
{
    [TestFixture]
    public class BlogManagerFixture
    {
        private const string _validUsername = "sitecore\\alfred";

        [Test]
        public void Ctor_LinkManagerIsNull_ThrowsException()
        {
            // arrange
            var settings = Mock.Of<IWeBlogSettings>();
            Action sutAction = () => new BlogManager(null, settings);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(new TestDelegate(sutAction));
            Assert.That(ex.ParamName, Is.EqualTo("linkManager"));
        }

        [Test]
        public void GetCurrentBlog_NullItem()
        {
            var settings = Mock.Of<IWeBlogSettings>(x =>
                x.BlogTemplateIds == new[] { ID.NewID }
            );

            var manager = CreateBlogManager(settings: settings);
            var resultItem = manager.GetCurrentBlog(null);

            Assert.That(resultItem, Is.Null);
        }

        [TestCase("/sitecore/content/blog/entries/entry", "/sitecore/content/blog", TestName = "Under blog root")]
        [TestCase("/sitecore/content/blog", "/sitecore/content/blog", TestName = "On blog root")]
        [TestCase("/sitecore/content", null, TestName = "Outside blog root")]
        public void GetCurrentBlog(string startPath, string expectedPath)
        {
            var blogTemplateId = ID.NewID;
            var entryTemplateId = ID.NewID;

            var settings = Mock.Of<IWeBlogSettings>(x =>
                x.BlogTemplateIds == new[] { blogTemplateId } &&
                x.EntryTemplateIds == new[] { entryTemplateId }
            );

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, blogTemplateId)
                {
                    new DbItem("entries")
                    {
                        new DbItem("entry", ID.NewID, entryTemplateId)
                    }
                }
            })
            {
                var startItem = db.GetItem(startPath);
                var manager = CreateBlogManager(settings: settings);
                var resultItem = manager.GetCurrentBlog(startItem);

                if(expectedPath == null)
                {
                    Assert.That(resultItem, Is.Null);
                }
                else
                {
                    Assert.That(resultItem.ID, Is.EqualTo(resultItem.ID));
                }
            };
        }

        [TestCase("/sitecore/content/blog/entries/entry", "/sitecore/content/blog", TestName = "Under blog root - custom templates")]
        [TestCase("/sitecore/content/blog", "/sitecore/content/blog", TestName = "On blog root - custom templates")]
        public void GetCurrentBlog_CustomTemplates(string startPath, string expectedPath)
        {
            var blogTemplateId = ID.NewID;
            var customBlogtemplateId = ID.NewID;
            var entryTemplateId = ID.NewID;
            var customEntryTemplateId = ID.NewID;

            var settings = Mock.Of<IWeBlogSettings>(x =>
                x.BlogTemplateIds == new[] { ID.NewID, ID.NewID, blogTemplateId, ID.NewID } &&
                x.EntryTemplateIds == new[] { ID.NewID, entryTemplateId, ID.NewID }
            );

            using (var db = new Db
            {
                new DbTemplate(blogTemplateId),
                new DbTemplate(entryTemplateId),
                new DbTemplate(customBlogtemplateId)
                {
                    BaseIDs = new [] { blogTemplateId }
                },
                new DbTemplate(customEntryTemplateId)
                {
                    BaseIDs = new [] { entryTemplateId }
                },
                new DbItem("blog", ID.NewID, customBlogtemplateId)
                {
                    new DbItem("entries")
                    {
                        new DbItem("entry", ID.NewID, customEntryTemplateId)
                    }
                }
            })
            {
                var startItem = db.GetItem(startPath);
                var manager = CreateBlogManager(settings: settings);
                var resultItem = manager.GetCurrentBlog(startItem);

                Assert.That(resultItem.ID, Is.EqualTo(resultItem.ID));
            };
        }

        [Test]
        public void GetUserBlogs_NoBlogs()
        {
            var settings = Mock.Of<IWeBlogSettings>(x =>
                x.ContentRootPath == "/sitecore/content" &&
                x.BlogTemplateIds == new[] { ID.NewID }
            );

            var manager = CreateBlogManager(settings: settings);

            var membershipProvider = Mock.Of<MembershipProvider>(x => 
                x.GetUser(_validUsername, true) == new FakeMembershipUser()
            );

            using (var db = new Db())
            {
                var item = db.GetItem("/sitecore/content");

                using (new MembershipSwitcher(membershipProvider))
                {
                    var blogs = manager.GetUserBlogs(_validUsername);

                    Assert.That(blogs, Is.Empty);
                }
            }
        }

        [Test]
        public void GetUserBlogs_NoAccess()
        {
            var blogTemplateId = ID.NewID;
            var blog2Id = ID.NewID;
            var blog3Id = ID.NewID;

            var settings = Mock.Of<IWeBlogSettings>(x =>
                x.ContentRootPath == "/sitecore/content" &&
                x.BlogTemplateIds == new[] { ID.NewID, blogTemplateId }
            );

            var manager = CreateBlogManager(settings: settings);

            var membershipProvider = Mock.Of<MembershipProvider>(x =>
                x.GetUser(_validUsername, true) == new FakeMembershipUser()
            );

            using (var db = new Db
            {
                new DbItem("blog1", ID.NewID, blogTemplateId),
                new DbItem("blog2", blog2Id, blogTemplateId)
                {
                    Access =
                    {
                        CanWrite = true
                    }
                },
                new DbItem("blog3", blog3Id, blogTemplateId)
                {
                    Access =
                    {
                        CanWrite = true
                    }
                }
            })
            {
                var blog1 = db.GetItem("/sitecore/content/blog1");
                var blog2 = db.GetItem("/sitecore/content/blog2");
                var blog3 = db.GetItem("/sitecore/content/blog3");

                using (new MembershipSwitcher(membershipProvider))
                {
                    var user = Sitecore.Security.Accounts.User.FromName(_validUsername, true);

                    var authorizationProvider = Mock.Of<AuthorizationProvider>(x =>
                        x.GetAccess(blog1, user, AccessRight.ItemWrite) == new Sitecore.FakeDb.Security.AccessControl.DenyAccessResult() &&
                        x.GetAccess(blog2, user, AccessRight.ItemWrite) == new Sitecore.FakeDb.Security.AccessControl.DenyAccessResult() &&
                        x.GetAccess(blog3, user, AccessRight.ItemWrite) == new Sitecore.FakeDb.Security.AccessControl.DenyAccessResult()
                    );

                    using (new AuthorizationSwitcher(authorizationProvider))
                    {
                        var blogTemplate = blog1.Database.GetTemplate(blogTemplateId);

                        var linkDb = Mock.Of<LinkDatabase>(x =>
                        x.GetReferrers(blogTemplate) == new[]
                        {
                            new ItemLink("master", blog2Id, ID.Null, "master", blogTemplateId, blogTemplateId.ToString()),
                            new ItemLink("master", blog3Id, ID.Null, "master", blogTemplateId, blogTemplateId.ToString())
                        });

                        using (new LinkDatabaseSwitcher(linkDb))
                        {
                            var blogs = manager.GetUserBlogs(_validUsername);
                            var ids = from blog in blogs select blog.ID;

                            Assert.That(ids, Is.EquivalentTo(new[] { blog2Id, blog3Id }));
                        }
                    }
                }
            }
        }

        [Test]
        public void GetUserBlogs_Multiple()
        {
            var blogTemplateId = ID.NewID;
            var blog2Id = ID.NewID;
            var blog3Id = ID.NewID;

            var settings = Mock.Of<IWeBlogSettings>(x =>
                x.ContentRootPath == "/sitecore/content" &&
                x.BlogTemplateIds == new[] { ID.NewID, blogTemplateId }
            );

            var manager = CreateBlogManager(settings: settings);

            var membershipProvider = Mock.Of<MembershipProvider>(x =>
                x.GetUser(_validUsername, true) == new FakeMembershipUser()
            );

            using (var db = new Db
            {
                new DbItem("blog1", ID.NewID, blogTemplateId),
                new DbItem("blog2", blog2Id, blogTemplateId)
                {
                    Access =
                    {
                        CanWrite = true
                    }
                },
                new DbItem("blog3", blog3Id, blogTemplateId)
                {
                    Access =
                    {
                        CanWrite = true
                    }
                }
            })
            {
                var blog1 = db.GetItem("/sitecore/content/blog1");
                var blog2 = db.GetItem("/sitecore/content/blog2");
                var blog3 = db.GetItem("/sitecore/content/blog3");

                using (new MembershipSwitcher(membershipProvider))
                {
                    var user = Sitecore.Security.Accounts.User.FromName(_validUsername, true);

                    var authorizationProvider = Mock.Of<AuthorizationProvider>(x =>
                        x.GetAccess(blog1, user, AccessRight.ItemWrite) == new Sitecore.FakeDb.Security.AccessControl.DenyAccessResult() &&
                        x.GetAccess(blog2, user, AccessRight.ItemWrite) == new Sitecore.FakeDb.Security.AccessControl.AllowAccessResult() &&
                        x.GetAccess(blog3, user, AccessRight.ItemWrite) == new Sitecore.FakeDb.Security.AccessControl.AllowAccessResult()
                    );

                    using (new AuthorizationSwitcher(authorizationProvider))
                    {
                        var blogTemplate = blog1.Database.GetTemplate(blogTemplateId);

                        var linkDb = Mock.Of<LinkDatabase>(x =>
                        x.GetReferrers(blogTemplate) == new[]
                        {
                            new ItemLink("master", blog2Id, ID.Null, "master", blogTemplateId, blogTemplateId.ToString()),
                            new ItemLink("master", blog3Id, ID.Null, "master", blogTemplateId, blogTemplateId.ToString())
                        });

                        using (new LinkDatabaseSwitcher(linkDb))
                        {
                            var blogs = manager.GetUserBlogs(_validUsername);
                            var ids = from blog in blogs select blog.ID;

                            Assert.That(ids, Is.EquivalentTo(new[] {blog2Id, blog3Id}));
                        }
                    }
                }
            }
        }

        [Test]
        public void GetAllBlogs_None()
        {
            var settings = Mock.Of<IWeBlogSettings>(x => 
                x.ContentRootPath == "/sitecore/content" &&
                x.BlogTemplateIds == new[] { ID.NewID }
            );

            var manager = CreateBlogManager(settings: settings);

            using (var db = new Db())
            {
                var item = db.GetItem("/sitecore/content");
                var blogs = manager.GetAllBlogs(item.Database);

                Assert.That(blogs, Is.Empty);
            }
        }

        [Test]
        public void GetAllBlogs_Multiple()
        {
            var blogTemplateId = ID.NewID;
            var blog1Id = ID.NewID;
            var blog2Id = ID.NewID;

            var settings = Mock.Of<IWeBlogSettings>(x =>
                x.ContentRootPath == "/sitecore/content" &&
                x.BlogTemplateIds == new[] { ID.NewID, blogTemplateId, ID.NewID }
            );

            var manager = CreateBlogManager(settings: settings);

            using (var db = new Db
            {
                new DbItem("an item"),
                new DbItem("another item"),
                new DbItem("blog 1", blog1Id, blogTemplateId),
                new DbItem("blog 2", blog2Id, blogTemplateId)
            })
            {
                var item = db.GetItem("/sitecore/content");
                var blogTemplate = item.Database.GetTemplate(blogTemplateId);

                var linkDb = Mock.Of<LinkDatabase>(x =>
                    x.GetReferrers(blogTemplate) == new[]
                    {
                        new ItemLink("master", blog1Id, ID.Null, "master", blogTemplateId, blogTemplateId.ToString()),
                        new ItemLink("master", blog2Id, ID.Null, "master", blogTemplateId, blogTemplateId.ToString())
                    });

                using (new LinkDatabaseSwitcher(linkDb))
                {
                    var blogs = manager.GetAllBlogs(item.Database);
                    var ids = from blog in blogs select blog.ID;

                    Assert.That(ids, Is.EquivalentTo(new[] {blog1Id, blog2Id}));
                }
            }
        }

        [Test]
        public void GetAllBlogs_ExcludeBlogOutsideContentRoot()
        {
            var blogTemplateId = ID.NewID;
            var blog1Id = ID.NewID;
            var blog2Id = ID.NewID;

            var settings = Mock.Of<IWeBlogSettings>(x =>
                x.ContentRootPath == "/sitecore/content/alpha" &&
                x.BlogTemplateIds == new[] { ID.NewID, blogTemplateId, ID.NewID }
            );

            var manager = CreateBlogManager(settings: settings);

            using (var db = new Db
            {
                new DbItem("alpha")
                {
                    new DbItem("blog 1", blog1Id, blogTemplateId)
                },
                new DbItem("beta")
                {
                    new DbItem("blog 2", blog2Id, blogTemplateId)
                }
            })
            {
                var item = db.GetItem("/sitecore/content/alpha");
                var blogTemplate = item.Database.GetTemplate(blogTemplateId);

                var linkDb = Mock.Of<LinkDatabase>(x =>
                    x.GetReferrers(blogTemplate) == new[]
                    {
                        new ItemLink("master", blog1Id, ID.Null, "master", blogTemplateId, blogTemplateId.ToString()),
                        new ItemLink("master", blog2Id, ID.Null, "master", blogTemplateId, blogTemplateId.ToString())
                    });

                using (new LinkDatabaseSwitcher(linkDb))
                {
                    var blogs = manager.GetAllBlogs(item.Database);
                    var ids = from blog in blogs select blog.ID;

                    Assert.That(ids, Is.EquivalentTo(new[] { blog1Id }));
                }
            }
        }

        [TestCase(true, TestName = "RSS enabled")]
        [TestCase(false, TestName = "RSS disabled")]
        public void EnableRSS(bool enabled)
        {
            var blogTemplateId = ID.NewID;

            var settings = Mock.Of<IWeBlogSettings>(x =>
                x.ContentRootPath == "/sitecore/content" &&
                x.BlogTemplateIds == new[] { ID.NewID, blogTemplateId, ID.NewID }
            );

            var manager = CreateBlogManager(settings: settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, blogTemplateId)
                {
                    { "Enable RSS", enabled ? "1" : string.Empty }
                }
            })
            {
                var item = db.GetItem("/sitecore/content/blog");
                var result = manager.EnableRSS(item);
                
                if(enabled)
                    Assert.That(result, Is.True);
                else
                {
                    Assert.That(result, Is.False);
                }
            }
        }

        [Test]
        public void EnableRSS_NullBlog()
        {
            var blogTemplateId = ID.NewID;

            var settings = Mock.Of<IWeBlogSettings>(x =>
                x.ContentRootPath == "/sitecore/content" &&
                x.BlogTemplateIds == new[] { ID.NewID, blogTemplateId, ID.NewID }
            );

            var manager = CreateBlogManager(settings: settings);
            var result = manager.EnableRSS(null);

            Assert.That(result, Is.False);
        }

        [TestCase(true, TestName = "Show Email in Comments Enabled")]
        [TestCase(false, TestName = "Show Email in Comments Disabled")]
        public void ShowEmailWithinComments(bool enabled)
        {
            var blogTemplateId = ID.NewID;

            var settings = Mock.Of<IWeBlogSettings>(x =>
                x.ContentRootPath == "/sitecore/content" &&
                x.BlogTemplateIds == new[] { ID.NewID, blogTemplateId, ID.NewID }
            );

            var manager = CreateBlogManager(settings: settings);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, blogTemplateId)
                {
                    { "Show Email Within Comments", enabled ? "1" : string.Empty }
                }
            })
            {
                var item = db.GetItem("/sitecore/content/blog");
                var result = manager.ShowEmailWithinComments(item);

                if (enabled)
                    Assert.That(result, Is.True);
                else
                {
                    Assert.That(result, Is.False);
                }
            }
        }

        [Test]
        public void ShowEmailWithinComments_NullBlog()
        {
            var blogTemplateId = ID.NewID;

            var settings = Mock.Of<IWeBlogSettings>(x =>
                x.ContentRootPath == "/sitecore/content" &&
                x.BlogTemplateIds == new[] { ID.NewID, blogTemplateId, ID.NewID }
            );

            var manager = CreateBlogManager(settings: settings);
            var result = manager.ShowEmailWithinComments(null);

            Assert.That(result, Is.False);
        }

        private BlogManager CreateBlogManager(BaseLinkManager linkManager = null, IWeBlogSettings settings = null)
        {
            return new BlogManager(linkManager ?? Mock.Of<BaseLinkManager>(), settings ?? Mock.Of<IWeBlogSettings>());
        }
    }
}
