using System;
using System.IO;
using System.Security.Authentication;
using CookComputing.XmlRpc;
using Moq;
using NUnit.Framework;
using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Sitecore.FakeDb.Sites;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Resources.Media;
using Sitecore.Sites;
using Sitecore.Modules.WeBlog.Search;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Links;

#if SC93
using Sitecore.Links.UrlBuilders;
#endif

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

#if SC93
                var linkManager = Mock.Of<BaseLinkManager>(x =>
                    x.GetItemUrl(It.IsAny<Item>(), It.IsAny<ItemUrlBuilderOptions>()) == "link"
                );
#else
                var linkManager = Mock.Of<BaseLinkManager>(x =>
                    x.GetItemUrl(It.IsAny<Item>(), It.IsAny<UrlOptions>()) == "link"
                );
#endif

                var blogManager = Mock.Of<IBlogManager>(x =>
                    x.GetUserBlogs(username) == new BlogHomeItem[]
                    {
                        new BlogHomeItem(blog1, linkManager, null)
                    }
                );

                var api = CreateAuthenticatingApi(blogManager, linkManager: linkManager);
                var blogsStruct = api.getUsersBlogs("app", username, "password");

                var expected = new[]
                {
                    new XmlRpcStruct
                    {
                        {"blogid", blog1.ID.ToString()},
                        {"blogName", "Lorem"},
                        {"url", "link"}
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

#if SC93
                var linkManager = Mock.Of<BaseLinkManager>(x =>
                    x.GetItemUrl(It.Is<Item>(y => y.Name == "blog1"), It.IsAny<ItemUrlBuilderOptions>()) == "link1" &&
                    x.GetItemUrl(It.Is<Item>(y => y.Name == "blog2"), It.IsAny<ItemUrlBuilderOptions>()) == "link2"
                );
#else
                var linkManager = Mock.Of<BaseLinkManager>(x =>
                    x.GetItemUrl(It.Is<Item>(y => y.Name == "blog1"), It.IsAny<UrlOptions>()) == "link1" &&
                    x.GetItemUrl(It.Is<Item>(y => y.Name == "blog2"), It.IsAny<UrlOptions>()) == "link2"
                );
#endif

                var blogManager = Mock.Of<IBlogManager>(x =>
                    x.GetUserBlogs(username) == new BlogHomeItem[]
                    {
                        new BlogHomeItem(blog1, linkManager, null),
                        new BlogHomeItem(blog2, linkManager, null)
                    }
                );

                var api = CreateAuthenticatingApi(blogManager, linkManager: linkManager);
                var blogsStruct = api.getUsersBlogs("app", username, "password");

                var expected = new[]
                {
                    new XmlRpcStruct
                    {
                        {"blogid", blog1.ID.ToString()},
                        {"blogName", "Lorem"},
                        {"url", "link1"}
                    },
                    new XmlRpcStruct
                    {
                        {"blogid", blog2.ID.ToString()},
                        {"blogName", "Ipsum"},
                        {"url", "link2"}
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
            using (var db = new Db
            {
                new DbItem("Blog")
            })
            {
                var blog = db.GetItem("/sitecore/content/Blog");

                var entryManager = MockEntryManager();

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
                var entryItem1 = db.GetItem("/sitecore/content/Blog/entry1");
                var entryItem2 = db.GetItem("/sitecore/content/Blog/entry2");

                var entry1 = new Entry
                {
                    Uri = entryItem1.Uri
                };

                var entry2 = new Entry
                {
                    Uri = entryItem2.Uri
                };

                var entryManager = MockEntryManager(entry1, entry2);

#if SC93
                var linkManager = Mock.Of<BaseLinkManager>(x =>
                    x.GetItemUrl(It.Is<Item>(y => y.Name == "entry1"), It.IsAny<ItemUrlBuilderOptions>()) == "link1" &&
                    x.GetItemUrl(It.Is<Item>(y => y.Name == "entry2"), It.IsAny<ItemUrlBuilderOptions>()) == "link2"
                );
#else
                var linkManager = Mock.Of<BaseLinkManager>(x =>
                    x.GetItemUrl(It.Is<Item>(y => y.Name == "entry1"), It.IsAny<UrlOptions>()) == "link1" &&
                    x.GetItemUrl(It.Is<Item>(y => y.Name == "entry2"), It.IsAny<UrlOptions>()) == "link2"
                );
#endif

                var api = CreateAuthenticatingApi(null, null, entryManager, linkManager: linkManager);
                api.GetBlogFunction = id => blog;

                var site = new FakeSiteContext("test");
                XmlRpcStruct[] postsStruct = null;

                using (new SiteContextSwitcher(site))
                {
                    postsStruct = api.getRecentPosts(blog.ID.ToString(), "user", "password", 2);
                }

                var expected = new[]
                {
                    new XmlRpcStruct
                    {
                        {"title", "Lorem"},
                        {"link", "link1"},
                        {"description", ""}, // Description is empty because the renderField pipeline is empty
                        {"pubDate", date},
                        {"guid", entryItem1.ID.ToString()},
                        {"postid", entryItem1.ID.ToString()},
                        {"keywords", "Deimos"},
                        {"author", Context.User.Name}
                    },
                    new XmlRpcStruct
                    {
                        {"title", "Ipsum"},
                        {"link", "link2"},
                        {"description", ""}, // Description is empty because the renderField pipeline is empty
                        {"pubDate", date.AddMinutes(1)},
                        {"guid", entryItem2.ID.ToString()},
                        {"postid", entryItem2.ID.ToString()},
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
        public void getTemplate_HasRights()
        {
            using (var db = new Db
            {
                new DbItem("Blog")
            })
            {
                var blog = db.GetItem("/sitecore/content/Blog");
                var api = CreateAuthenticatingApi();

                var output = api.getTemplate("app", blog.ID.ToString(), "user", "password", "none");
                Assert.That(output, Does.Contain("<html>").IgnoreCase);
                Assert.That(output, Does.Contain("$BlogTitle$").IgnoreCase);
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
        public void newPost_NoTitle()
        {
            using (var db = new Db
            {
                new DbItem("Blog")
            })
            {
                var blog = db.GetItem("/sitecore/content/Blog");
                var api = CreateAuthenticatingApi();

                var post = new XmlRpcStruct
                {
                    { "description", "Deimos" },
                    { "categories", "luna" },
                    { "dateCreated", "2013-02-07" }
                };

                Assert.That(() =>
                {
                    api.newPost(blog.ID.ToString(), "user", "password", post, false);
                }, Throws.InstanceOf<ArgumentException>());
            }
        }

        [Test]
        public void newPost_InvalidBlogId()
        {
            using (var db = new Db())
            {
                var api = CreateAuthenticatingApi();
                api.GetContentDatabaseFunction = () => db.Database;

                var post = new XmlRpcStruct
                {
                    {"title", "Lorem"},
                    {"description", "Deimos"},
                    {"categories", "luna"},
                    {"dateCreated", "2013-02-07"}
                };

                var newPostId = api.newPost(ID.NewID.ToString(), "user", "password", post, false);
                Assert.That(newPostId, Is.Empty);
            }
        }

        [Test]
        public void newPost_ValidStruct()
        {
            var entryTemplateId = ID.NewID;
            var blogRootTemplate = CreateBlogRootTemplate();

            var settings = Mock.Of<IWeBlogSettings>(x =>
                x.BlogTemplateIds == new[] { blogRootTemplate.ID }
            );

            using (var db = new Db
            {
                new DbTemplate(entryTemplateId)
                {
                    new DbField("Title"),
                    new DbField("Content"),
                    new DbField("Category")
                },
                blogRootTemplate,
                new DbItem("blog", ID.NewID, blogRootTemplate.ID)
                {
                    new DbField("Defined Entry Template")
                    {
                        Value = entryTemplateId.ToString()
                    }
                }
            })
            {
                var blog = db.GetItem("/sitecore/content/blog");
                var api = CreateAuthenticatingApi(settings: settings);
                api.GetContentDatabaseFunction = () => db.Database;

                var post = new XmlRpcStruct
                {
                    {"title", "Lorem"},
                    {"description", "Deimos"},
                    {"categories", "luna"},
                    {"dateCreated", "2013-02-07"}
                };

                var newPostId = api.newPost(blog.ID.ToString(), "user", "password", post, false);
                Assert.That(newPostId, Is.Not.Empty);

                var parsedId = ID.Parse(newPostId);
                Assert.That(parsedId, Is.Not.Null);
                Assert.That(parsedId, Is.Not.EqualTo(ID.Null));
                Assert.That(parsedId, Is.Not.EqualTo(ID.Undefined));
            }
        }

        [Test]
        public void newPost_FuturePublishDate()
        {
            var entryTemplateId = ID.NewID;
            var date = DateTime.UtcNow.Date.AddMonths(1);
            var blogRootTemplate = CreateBlogRootTemplate();
            var settings = Mock.Of<IWeBlogSettings>(x => 
                x.BlogTemplateIds == new[] { blogRootTemplate.ID }
            );

            using (var db = new Db
            {
                new DbTemplate(entryTemplateId)
                {
                    new DbField("Title"),
                    new DbField("Content")
                },
                blogRootTemplate,
                new DbItem("blog", ID.NewID, blogRootTemplate.ID)
                {
                    new DbField("Defined Entry Template")
                    {
                        Value = entryTemplateId.ToString()
                    }
                }
            })
            {
                var blog = db.GetItem("/sitecore/content/blog");
                var api = CreateAuthenticatingApi(settings: settings);
                api.GetContentDatabaseFunction = () => db.Database;

                var post = new XmlRpcStruct
                {
                    {"title", "Lorem"},
                    {"description", "Deimos"},
                    {"dateCreated", date}
                };

                var newPostId = api.newPost(blog.ID.ToString(), "user", "password", post, false);
                var postItem = db.GetItem(newPostId);

                var postDate = postItem.Publishing.PublishDate.ToLocalTime();

                Assert.That(postDate, Is.EqualTo(date));
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
                var entryItem1 = db.GetItem("/sitecore/content/Blog/entry1");
                var entry1 = new Entry {Uri = entryItem1.Uri};

                var entryManager = MockEntryManager(entry1);

                var api = CreateAuthenticatingApi(null, null, entryManager);

                var post = new XmlRpcStruct
                {
                    { "title", "Deimos" }
                };

                Assert.That(() =>
                {
                    api.editPost(entryItem1.ID.ToString(), "user", "password", post, false);
                }, Throws.InstanceOf<InvalidCredentialException>());
            }
        }

        [Test]
        public void editPost_InvalidPost()
        {
            var api = CreateAuthenticatingApi();

            var post = new XmlRpcStruct
            {
                { "title", "Deimos" }
            };

            using (new Db())
            {
                var result = api.editPost(ID.NewID.ToString(), "user", "password", post, false);
                Assert.That(result, Is.False);
            }
        }

        [Test]
        public void editPost_ValidPost()
        {
            var entryTemplateId = ID.NewID;

            using (var db = new Db
            {
                new DbTemplate(entryTemplateId)
                {
                    new DbField("Title"),
                    new DbField("Content"),
                    new DbField("Category")
                },
                new DbItem("Blog")
                {
                    new DbItem("entry1", ID.NewID, entryTemplateId)
                    {
                        new DbField("Title")
                        {
                            Value = "Some old title"
                        }
                    }
                }
            })
            {
                var entryItem1 = db.GetItem("/sitecore/content/Blog/entry1");
                var entry1 = new Entry {Uri = entryItem1.Uri};

                var entryManager = MockEntryManager(entry1);

                var api = CreateAuthenticatingApi(null, null, entryManager);

                var post = new XmlRpcStruct
                {
                    {"title", "Lorem"},
                    {"description", "Deimos"}
                };

                var result = api.editPost(entryItem1.ID.ToString(), "user", "password", post, false);
                Assert.That(result, Is.True);

                entryItem1.Reload();
                Assert.That(entryItem1["title"], Is.EqualTo("Lorem"));
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
                var entryItem1 = db.GetItem("/sitecore/content/Blog/entry1");
                var entry1 = new Entry {Uri = entryItem1.Uri};

                var entryManager = MockEntryManager(entry1);

                var api = CreateAuthenticatingApi(null, null, entryManager);

                Assert.That(() =>
                {
                    api.getPost(entryItem1.ID.ToString(), "user", "password");
                }, Throws.InstanceOf<InvalidCredentialException>());
            }
        }

        [Test]
        public void getPost_InvalidPost()
        {
            var api = CreateAuthenticatingApi(null, null, null);

            using (new Db())
            {
                var post = api.getPost(ID.NewID.ToString(), "user", "password");
                Assert.That(post, Is.Empty);
            }
        }

        [Test]
        public void getPost_ExistingPost()
        {
            var date = DateTime.UtcNow.Date;

            using (var db = new Db
            {
                new DbItem("Blog")
                {
                    new DbItem("entry1")
                    {
                        new DbField("title")
                        {
                            Value = "Luna"
                        },
                        new DbField("content")
                        {
                            Value = "This world doesn't need no opera"
                        },
                        new DbField("Entry Date")
                        {
                            Value = DateUtil.ToIsoDate(date)
                        }
                    }
                }
            })
            {
                var entryItem1 = db.GetItem("/sitecore/content/Blog/entry1");
                var entry1 = new Entry {Uri = entryItem1.Uri};

                var entryManager = MockEntryManager(entry1);

#if SC93
                var linkManager = Mock.Of<BaseLinkManager>(x =>
                    x.GetItemUrl(It.IsAny<Item>(), It.IsAny<ItemUrlBuilderOptions>()) == "the-link"
                );
#else
                var linkManager = Mock.Of<BaseLinkManager>(x =>
                    x.GetItemUrl(It.IsAny<Item>(), It.IsAny<UrlOptions>()) == "the-link"
                );
#endif

                var api = CreateAuthenticatingApi(null, null, entryManager, linkManager: linkManager);

                var post = api.getPost(entryItem1.ID.ToString(), "user", "password");

                var expected = new XmlRpcStruct
                {
                    {"title", "Luna"},
                    {"link", "the-link"},
                    {"description", "This world doesn't need no opera"},
                    {"pubDate", date},
                    {"guid", entryItem1.ID.ToString()},
                    {"author", Context.User.Name}
                };

                Assert.That(post, Is.EqualTo(expected));
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
                var entryItem1 = db.GetItem("/sitecore/content/Blog/entry1");
                var entry1 = new Entry {Uri = entryItem1.Uri};

                var entryManager = MockEntryManager(entry1);

                var api = CreateAuthenticatingApi(null, null, entryManager);

                Assert.That(() =>
                {
                    api.deletePost("app", entryItem1.ID.ToString(), "user", "password", true);
                }, Throws.InstanceOf<InvalidCredentialException>());
            }
        }

        [Test]
        public void deletePost_ValidPost()
        {
            using (var db = new Db
            {
                new DbItem("Blog")
                {
                    new DbItem("entry1")
                }
            })
            {
                var entry1 = db.GetItem("/sitecore/content/Blog/entry1");
                var entryManager = Mock.Of<IEntryManager>(x =>
                    x.DeleteEntry(entry1.ID.ToString(), It.IsAny<Database>())
                    );

                var api = CreateAuthenticatingApi(null, null, entryManager);
                api.GetContentDatabaseFunction = () => db.Database;

                var result = api.deletePost("app", entry1.ID.ToString(), "user", "password", true);
                Assert.That(result, Is.True);
            }
        }

        [Test]
        public void deletePost_InvalidPost()
        {
            using (var db = new Db())
            {
                var api = CreateAuthenticatingApi();

                var result = api.deletePost("app", ID.NewID.ToString(), "user", "password", true);
                Assert.That(result, Is.False);
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

        [Test]
        public void newMediaObject_NoRights()
        {
            using (var db = new Db
            {
                new DbTemplate(),
                new DbItem("Blog")
                {
                    Access = { CanWrite = false }
                },
                new DbItem("Modules")
                {
                    ParentID = ItemIDs.MediaLibraryRoot,
                    Access = { CanCreate = false },
                    Children = { new DbItem("image") }
                }
            })
            {
                var blog = db.GetItem("/sitecore/content/Blog");
                var image = db.GetItem("/sitecore/media library/Modules/image");

                var payload = new XmlRpcStruct
                {
                    { "name", "the media" },
                    { "type", "image/gif" },
                    { "bits", GetSampleImageData() }
                };

                var mediaManager = Mock.Of<BaseMediaManager>(x =>
                    x.Creator == Mock.Of<MediaCreator>(y =>
                        y.CreateFromStream(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<MediaCreatorOptions>()) == image
                    )
                );

                var api = CreateAuthenticatingApi(mediaManager: mediaManager);

                api.GetContentDatabaseFunction = () => db.Database;

                Assert.That(() =>
                {
                   api.newMediaObject(blog.ID.ToString(), "user", "password", payload);
                }, Throws.InstanceOf<InvalidCredentialException>());
            }
        }

        [Test]
        public void newMediaObject_Valid()
        {
            using (var db = new Db
            {
                new DbTemplate(),
                new DbItem("Blog"),
                new DbItem("Modules")
                {
                    ParentID = ItemIDs.MediaLibraryRoot,
                    Access = { CanWrite = false },
                    Children = { new DbItem("image") }
                }
            })
            {
                var blog = db.GetItem("/sitecore/content/Blog");
                var image = db.GetItem("/sitecore/media library/Modules/image");

                var mediaCreator = new Mock<MediaCreator>();
                mediaCreator.Setup(x => x.CreateFromStream(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<MediaCreatorOptions>())).Returns(image).Verifiable();

                var mediaManager = Mock.Of<BaseMediaManager>(x => 
                    x.Creator == mediaCreator.Object &&
#if SC93
                    x.GetMediaUrl(It.IsAny<MediaItem>(), It.IsAny<MediaUrlBuilderOptions>()) == "fake-url"
#else
                    x.GetMediaUrl(It.IsAny<MediaItem>(), It.IsAny<MediaUrlOptions>()) == "fake-url"
#endif
                );

                var api = CreateAuthenticatingApi(mediaManager: mediaManager);

                api.GetContentDatabaseFunction = () => db.Database;

                var payload = new XmlRpcStruct
                {
                    { "name", "the media" },
                    { "type", "image/gif" },
                    { "bits", GetSampleImageData() }
                };

				var urlStruct = api.newMediaObject(blog.ID.ToString(), "user", "password", payload);
				Assert.That(urlStruct["url"], Is.EqualTo("fake-url"));

                mediaCreator.Verify();
            }
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

        private TestableMetaBlogApi CreateAuthenticatingApi(
            IBlogManager blogManager = null,
            ICategoryManager categoryManager = null,
            IEntryManager entryManager = null,
            IWeBlogSettings settings = null,
            BaseMediaManager mediaManager = null,
            BaseLinkManager linkManager = null
            )
        {
            return new TestableMetaBlogApi(
                blogManager,
                categoryManager,
                entryManager,
                settings,
                mediaManager,
                linkManager
                )
            {
                AuthenticateFunction = (u, p) => { }
            };
        }

        private byte[] GetSampleImageData()
        {
            var base64 =
                "R0lGODlhEAAQAMQAAORHHOVSKudfOulrSOp3WOyDZu6QdvCchPGolfO0o/XBs/fNwfjZ0frl3/zy7////wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACH5BAkAABAALAAAAAAQABAAAAVVICSOZGlCQAosJ6mu7fiyZeKqNKToQGDsM8hBADgUXoGAiqhSvp5QAnQKGIgUhwFUYLCVDFCrKUE1lBavAViFIDlTImbKC5Gm2hB0SlBCBMQiB0UjIQA7";
            return System.Convert.FromBase64String(base64);
        }

        private DbTemplate CreateBlogRootTemplate()
        {
            return new DbTemplate(ID.NewID)
            {
                new DbField("Defined Entry Template"),
                new DbField("Defined Category Template"),
                new DbField("Defined Comment Template")
            };
        }

        private IEntryManager MockEntryManager(params Entry[] entries)
        {
            return Mock.Of<IEntryManager>(x =>
                x.GetBlogEntries(It.IsAny<Item>(), It.IsAny<EntryCriteria>(), ListOrder.Descending) == new SearchResults<Entry>(entries, false)
            );
        }
    }
}
