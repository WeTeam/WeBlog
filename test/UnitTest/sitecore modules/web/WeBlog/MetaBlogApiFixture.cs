using System;
using System.Security.Authentication;
using CookComputing.XmlRpc;
using Moq;
using NUnit.Framework;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Sitecore.FakeDb.Sites;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Sites;

namespace Sitecore.Modules.WeBlog.UnitTest.sitecore_modules.web.WeBlog
{
    [TestFixture]
    public class MetaBlogApiFixture
    {
        [Test]
        public void getUsersBlogs_Unauthenticated()
        {
            var api = CreateNonAuthenticatingApi();
            Assert.That(() => {
                api.getUsersBlogs("app", "user", "password");
            }, Throws.InstanceOf<InvalidCredentialException>());
        }

        [Test]
        public void getUsersBlogs_NoBlogs()
        {
            var username = "johnny";

            var blogManager = Mock.Of<IBlogManager>(x => 
                x.GetUserBlogs(username) == new BlogHomeItem[0]
                );

            var api = CreateAuthenticatingApi(blogManager);
            var blogsStruct = api.getUsersBlogs("app", username, "password");

            Assert.That(blogsStruct, Is.Empty);
        }

        [Test]
        public void getUsersBlogs_SingleBlog()
        {
            var username = "johnny";

            using (var db = new Db
            {
                new DbItem("blog1")
                {
                    new DbField("Title") { Value = "Lorem" }
                }
            })
            {
                var blog1 = db.GetItem("/sitecore/content/blog1");

                var blogManager = Mock.Of<IBlogManager>(x =>
                    x.GetUserBlogs(username) == new BlogHomeItem[] { blog1 }
                    );

                var api = CreateAuthenticatingApi(blogManager);
                var blogsStruct = api.getUsersBlogs("app", username, "password");

                var expected = new[]
                {
                    new XmlRpcStruct
                    {
                        {"blogid", blog1.ID.ToString()},
                        {"blogName", "Lorem"},
                        {"url", "/en/sitecore/content/blog1.aspx"},
                    }
                };

                Assert.That(blogsStruct, Is.EqualTo(expected));
            }
        }

        [Test]
        public void getUsersBlogs_MultipleBlogs()
        {
            var username = "johnny";

            using (var db = new Db
            {
                new DbItem("blog1")
                {
                    new DbField("Title") { Value = "Lorem" }
                },
                new DbItem("blog2")
                {
                    new DbField("Title") { Value = "Ipsum" }
                }
            })
            {
                var blog1 = db.GetItem("/sitecore/content/blog1");
                var blog2 = db.GetItem("/sitecore/content/blog2");

                var blogManager = Mock.Of<IBlogManager>(x =>
                    x.GetUserBlogs(username) == new BlogHomeItem[] { blog1, blog2 }
                    );

                var api = CreateAuthenticatingApi(blogManager);
                var blogsStruct = api.getUsersBlogs("app", username, "password");

                var expected = new[]
                {
                    new XmlRpcStruct
                    {
                        {"blogid", blog1.ID.ToString()},
                        {"blogName", "Lorem"},
                        {"url", "/en/sitecore/content/blog1.aspx"},
                    },
                    new XmlRpcStruct
                    {
                        {"blogid", blog2.ID.ToString()},
                        {"blogName", "Ipsum"},
                        {"url", "/en/sitecore/content/blog2.aspx"},
                    }
                };

                Assert.That(blogsStruct, Is.EqualTo(expected));
            }
        }

        [Test]
        public void getCategories_Unauthenticated()
        {
            var api = CreateNonAuthenticatingApi();
            Assert.That(() => {
                api.getCategories(ID.NewID.ToString(), "user", "password");
            }, Throws.InstanceOf<InvalidCredentialException>());
        }

        [Test]
        public void getCategories_UnknownBlog()
        {
            var api = new TestableMetaBlogApi(null, null)
            {
                AuthenticateFunction = (u, p) => { },
                GetBlogFunction = id => null
            };

            var categoriesStruct = api.getCategories(ID.NewID.ToString(), "user", "password");

            Assert.That(categoriesStruct, Is.Empty);
        }

        [Test]
        public void getCategories_NoCategories()
        {
            using (var db = new Db
            {
                new DbItem("blog")
            })
            {
                var blog = db.GetItem("/sitecore/content/blog");

                var categoryManager = Mock.Of<ICategoryManager>(x =>
                    x.GetCategories(blog) == new CategoryItem[0]
                    );

                var api = new TestableMetaBlogApi(null, categoryManager)
                {
                    AuthenticateFunction = (u, p) => { },
                    GetBlogFunction = id => blog
                };

                var categoriesStruct = api.getCategories(blog.ID.ToString(), "user", "password");

                Assert.That(categoriesStruct, Is.Empty);
            }
        }

        [Test]
        public void getCategories_CategoriesPresent()
        {
            using (var db = new Db
            {
                new DbItem("blog")
                {
                    new DbItem("Categories")
                    {
                        new DbItem("alpha")
                        {
                            new DbField("Title") { Value = "Alpha" }
                        },
                        new DbItem("beta")
                        {
                            new DbField("Title") { Value = "Beta" }
                        },
                        new DbItem("gamma")
                        {
                            new DbField("Title") { Value = "Gamma" }
                        }
                    }
                }
            })
            {
                var blog = db.GetItem("/sitecore/content/blog");

                var categoryAlpha = db.GetItem("/sitecore/content/blog/categories/alpha");
                var categoryBeta = db.GetItem("/sitecore/content/blog/categories/beta");
                var categoryGamma = db.GetItem("/sitecore/content/blog/categories/gamma");

                var categoryManager = Mock.Of<ICategoryManager>(x => 
                    x.GetCategories(blog) == new CategoryItem[] { categoryAlpha, categoryBeta, categoryGamma }
                    );

                var api = new TestableMetaBlogApi(null, categoryManager)
                {
                    AuthenticateFunction = (u, p) => { },
                    GetBlogFunction = id => blog
                };

                var categoriesStruct = api.getCategories(blog.ID.ToString(), "user", "password");

                var expected = new[]
                {
                    new XmlRpcStruct
                    {
                        {"categoryid", categoryAlpha.ID.ToString()},
                        {"title", "Alpha"},
                        {"description", "Description is not available"},
                    },
                    new XmlRpcStruct
                    {
                        {"categoryid", categoryBeta.ID.ToString()},
                        {"title", "Beta"},
                        {"description", "Description is not available"},
                    },
                    new XmlRpcStruct
                    {
                        {"categoryid", categoryGamma.ID.ToString()},
                        {"title", "Gamma"},
                        {"description", "Description is not available"},
                    }
                };

                Assert.That(categoriesStruct, Is.EqualTo(expected));
            }
        }

        [Test]
        public void getRecentPosts_Unauthenticated()
        {
            var api = CreateNonAuthenticatingApi();
            Assert.That(() => {
                api.getRecentPosts(ID.NewID.ToString(), "user", "password", 5);
            }, Throws.InstanceOf<InvalidCredentialException>());
        }

        [Test]
        public void getRecentPosts_UnknownBlog()
        {
            var api = CreateAuthenticatingApi();
            api.GetBlogFunction = id => null;

            var postsStruct = api.getRecentPosts(ID.NewID.ToString(), "user", "password", 2);

            Assert.That(postsStruct, Is.Empty);
        }

        [Test]
        public void getRecentPosts_NoneAvailable()
        {
            var date = DateTime.UtcNow.Date;

            using (var db = new Db
            {
                new DbItem("Blog")
            })
            {
                var blog = db.GetItem("/sitecore/content/Blog");

                var entryManager = Mock.Of<IEntryManager>(x =>
                    x.GetBlogEntries(It.IsAny<Item>(), It.IsAny<int>(), null, null, null, null) == new EntryItem[0]
                );

                var api = CreateAuthenticatingApi(null, null, entryManager);
                api.GetBlogFunction = id => blog;

                var postsStruct = api.getRecentPosts(blog.ID.ToString(), "user", "password", 1);

                Assert.That(postsStruct, Is.Empty);
            }
        }

        [Test]
        public void getRecentPosts_Multiple()
        {
            var date = DateTime.UtcNow.Date;

            using (var db = new Db
            {
                new DbItem("Blog")
                {
                    new DbItem("entry1")
                    {
                        new DbField("Title") { Value = "Lorem" },
                        new DbField("Content") { Value = "Lorem ipsum dolor sit" },
                        new DbField("Entry Date") { Value = DateUtil.ToIsoDate(date) },
                        new DbField("Tags") { Value = "Deimos" },
                    },
                    new DbItem("entry2")
                    {
                        new DbField("Title") { Value = "Ipsum" },
                        new DbField("Content") { Value = "ipsum dolor sit" },
                        new DbField("Entry Date") { Value = DateUtil.ToIsoDate(date.AddMinutes(1)) },
                        new DbField("Tags") { Value = "Luna" },
                    }
                }
            })
            {
                var blog = db.GetItem("/sitecore/content/Blog");
                var entry1 = db.GetItem("/sitecore/content/Blog/entry1");
                var entry2 = db.GetItem("/sitecore/content/Blog/entry2");

                var entryManager = Mock.Of<IEntryManager>(x =>
                    x.GetBlogEntries(It.IsAny<Item>(), It.IsAny<int>(), null, null, null, null) == new EntryItem[] { entry1, entry2 }
                );

                var api = CreateAuthenticatingApi(null, null, entryManager);
                api.GetBlogFunction = id => blog;

                var site = new FakeSiteContext("test");
                XmlRpcStruct[] postsStruct = null;

                using (new SiteContextSwitcher(site))
                {
                    postsStruct = api.getRecentPosts(blog.ID.ToString(), "user", "password", 2);
                }

                var expected = new[]
                {new XmlRpcStruct
                    {
                        {"title", "Lorem"},
                        {"link", "/en/sitecore/content/Blog/entry1.aspx"},
                        {"description", ""}, // Description is empty because the renderField pipeline is empty
                        {"pubDate", date},
                        {"guid", entry1.ID.ToString()},
                        {"postid", entry1.ID.ToString()},
                        {"keywords", "Deimos"},
                        {"author", Context.User.Name}
                    },
                    new XmlRpcStruct
                    {
                        {"title", "Ipsum"},
                        {"link", "/en/sitecore/content/Blog/entry2.aspx"},
                        {"description", ""}, // Description is empty because the renderField pipeline is empty
                        {"pubDate", date.AddMinutes(1)},
                        {"guid", entry2.ID.ToString()},
                        {"postid", entry2.ID.ToString()},
                        {"keywords", "Luna"},
                        {"author", Context.User.Name}
                    }
                };

                Assert.That(postsStruct, Is.EqualTo(expected));
            }
        }

        [Test]
        public void getTemplate_Unauthenticated()
        {
            var api = CreateNonAuthenticatingApi();
            Assert.That(() => {
                api.getTemplate("app", ID.NewID.ToString(), "user", "password", "none");
            }, Throws.InstanceOf<InvalidCredentialException>());
        }

        [Test]
        public void getTemplate_NoRights()
        {
            using (var db = new Db
            {
                new DbItem("Blog")
                {
                    Access = { CanWrite = false }
                }
            })
            {
                var blog = db.GetItem("/sitecore/content/Blog");
                var api = CreateAuthenticatingApi();

                Assert.That(() =>
                {
                    api.getTemplate("app", blog.ID.ToString(), "user", "password", "none");
                }, Throws.InstanceOf<InvalidCredentialException>());
            }
        }

        [Test]
        public void newPost_Unauthenticated()
        {
            var api = CreateNonAuthenticatingApi();
            Assert.That(() => {
                api.newPost(ID.NewID.ToString(), "user", "password", null, false);
            }, Throws.InstanceOf<InvalidCredentialException>());
        }

        [Test]
        public void newPost_NoRights()
        {
            using (var db = new Db
            {
                new DbItem("Blog")
                {
                    Access = { CanWrite = false }
                }
            })
            {
                var blog = db.GetItem("/sitecore/content/Blog");
                var api = CreateAuthenticatingApi();

                var post = new XmlRpcStruct
                {
                    { "title", "Deimos" }
                };

                Assert.That(() =>
                {
                    api.newPost(blog.ID.ToString(), "user", "password", post, false);
                }, Throws.InstanceOf<InvalidCredentialException>());
            }
        }

        [Test]
        public void editPost_Unauthenticated()
        {
            var api = CreateNonAuthenticatingApi();
            Assert.That(() => {
                api.editPost(ID.NewID.ToString(), "user", "password", null, false);
            }, Throws.InstanceOf<InvalidCredentialException>());
        }

        [Test]
        public void editPost_NoRights()
        {
            using (var db = new Db
            {
                new DbItem("Blog")
                {
                    new DbItem("entry1")
                    {
                        Access = { CanWrite = false }
                    }
                }
            })
            {
                var entry1 = db.GetItem("/sitecore/content/Blog/entry1");
                var entryManager = Mock.Of<IEntryManager>(x =>
                    x.GetBlogEntries(It.IsAny<Item>(), It.IsAny<int>(), null, null, null, null) ==
                    new EntryItem[] { entry1 }
                    );

                var api = CreateAuthenticatingApi(null, null, entryManager);

                var post = new XmlRpcStruct
                {
                    { "title", "Deimos" }
                };

                Assert.That(() =>
                {
                    api.editPost(entry1.ID.ToString(), "user", "password", post, false);
                }, Throws.InstanceOf<InvalidCredentialException>());
            }
        }

        [Test]
        public void getPost_Unauthenticated()
        {

            var api = CreateNonAuthenticatingApi();
            Assert.That(() => {
                api.getPost(ID.NewID.ToString(), "user", "password");
            }, Throws.InstanceOf<InvalidCredentialException>());
        }

        [Test]
        public void getPost_NoRights()
        {
            using (var db = new Db
            {
                new DbItem("Blog")
                {
                    new DbItem("entry1")
                    {
                        Access = { CanWrite = false }
                    }
                }
            })
            {
                var entry1 = db.GetItem("/sitecore/content/Blog/entry1");
                var entryManager = Mock.Of<IEntryManager>(x =>
                    x.GetBlogEntries(It.IsAny<Item>(), It.IsAny<int>(), null, null, null, null) ==
                    new EntryItem[] { entry1 }
                    );

                var api = CreateAuthenticatingApi(null, null, entryManager);

                Assert.That(() =>
                {
                    api.getPost(entry1.ID.ToString(), "user", "password");
                }, Throws.InstanceOf<InvalidCredentialException>());
            }
        }

        [Test]
        public void deletePost_Unauthenticated()
        {
            var api = CreateNonAuthenticatingApi();
            Assert.That(() => {
                api.deletePost("app", ID.NewID.ToString(), "user", "password", false);
            }, Throws.InstanceOf<InvalidCredentialException>());
        }

        [Test]
        public void deletePost_NoRights()
        {
            using (var db = new Db
            {
                new DbItem("Blog")
                {
                    new DbItem("entry1")
                    {
                        Access = { CanWrite = false }
                    }
                }
            })
            {
                var entry1 = db.GetItem("/sitecore/content/Blog/entry1");
                var entryManager = Mock.Of<IEntryManager>(x =>
                    x.GetBlogEntries(It.IsAny<Item>(), It.IsAny<int>(), null, null, null, null) ==
                    new EntryItem[] { entry1 }
                    );

                var api = CreateAuthenticatingApi(null, null, entryManager);

                Assert.That(() =>
                {
                    api.deletePost("app", entry1.ID.ToString(), "user", "password", true);
                }, Throws.InstanceOf<InvalidCredentialException>());
            }
        }

        [Test]
        public void newMediaObject_Unauthenticated()
        {
            var api = CreateNonAuthenticatingApi();
            Assert.That(() => {
                api.newMediaObject(ID.NewID.ToString(), "user", "password", null);
            }, Throws.InstanceOf<InvalidCredentialException>());
        }

        private TestableMetaBlogApi CreateNonAuthenticatingApi()
        {
            return new TestableMetaBlogApi(null, null)
            {
                AuthenticateFunction = (u, p) =>
                {
                    throw new InvalidCredentialException("Invalid credentials. Access denied");
                }
            };
        }

        private TestableMetaBlogApi CreateAuthenticatingApi(IBlogManager blogManager = null, ICategoryManager categoryManager = null, IEntryManager entryManager = null)
        {
            return new TestableMetaBlogApi(blogManager, categoryManager, entryManager)
            {
                AuthenticateFunction = (u, p) => { }
            };
        }
    }
}
