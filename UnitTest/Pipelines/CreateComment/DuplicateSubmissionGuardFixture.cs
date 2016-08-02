using Moq;
using NUnit.Framework;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.Globalization;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Modules.WeBlog.Pipelines;
using Sitecore.Modules.WeBlog.Pipelines.CreateComment;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.CreateComment
{
    [TestFixture]
    public class DuplicateSubmissionGuardFixture
    {
        [Test]
        public void NullArgs()
        {
            var processor = new DuplicateSubmissionGuard();
            Assert.That(() => processor.Process(null), Throws.ArgumentNullException);
        }

        [Test]
        public void NullDatabase()
        {
            var processor = new DuplicateSubmissionGuard();

            using (var db = new Db
            {
                new DbItem("blog")
                {
                    new DbItem("entry")
                }
            })
            {
                var entry = db.GetItem("/sitecore/content/blog/entry");
                var args = new CreateCommentArgs
                {
                    EntryID = entry.ID,
                    Language = Language.Parse("de"),
                    Comment = new Comment
                    {
                        AuthorEmail = "a@mail.net",
                        AuthorName = "commentor",
                        Text = "the comment"
                    }
                };

                Assert.That(() => processor.Process(args), Throws.InvalidOperationException);
            }
        }

        [Test]
        public void NullEntryId()
        {
            var processor = new DuplicateSubmissionGuard();

            using (var db = new Db
            {
                new DbItem("blog")
                {
                    new DbItem("entry")
                }
            })
            {
                var entry = db.GetItem("/sitecore/content/blog/entry");
                var args = new CreateCommentArgs
                {
                    Database = entry.Database,
                    Language = Language.Parse("de"),
                    Comment = new Comment
                    {
                        AuthorEmail = "a@mail.net",
                        AuthorName = "commentor",
                        Text = "the comment"
                    }
                };

                Assert.That(() => processor.Process(args), Throws.InvalidOperationException);
            }
        }

        [Test]
        public void NullLanguage()
        {
            var processor = new DuplicateSubmissionGuard();

            using (var db = new Db
            {
                new DbItem("blog")
                {
                    new DbItem("entry")
                }
            })
            {
                var entry = db.GetItem("/sitecore/content/blog/entry");
                var args = new CreateCommentArgs
                {
                    Database = entry.Database,
                    EntryID = entry.ID,
                    Comment = new Comment
                    {
                        AuthorEmail = "a@mail.net",
                        AuthorName = "commentor",
                        Text = "the comment"
                    }
                };

                Assert.That(() => processor.Process(args), Throws.InvalidOperationException);
            }
        }

        [Test]
        public void NullComment()
        {
            var processor = new DuplicateSubmissionGuard();

            using (var db = new Db
            {
                new DbItem("blog")
                {
                    new DbItem("entry")
                }
            })
            {
                var entry = db.GetItem("/sitecore/content/blog/entry");
                var args = new CreateCommentArgs
                {
                    Database = entry.Database,
                    EntryID = entry.ID,
                    Language = Language.Parse("de")
                };

                Assert.That(() => processor.Process(args), Throws.InvalidOperationException);
            }
        }

        [Test]
        public void InvalidEntryId()
        {
            var processor = new DuplicateSubmissionGuard();

            using (var db = new Db
            {
                new DbItem("blog")
                {
                    new DbItem("entry")
                }
            })
            {
                var entry = db.GetItem("/sitecore/content/blog/entry");
                var args = new CreateCommentArgs
                {
                    Database = entry.Database,
                    EntryID = entry.ID,
                    Language = Language.Parse("de"),
                    Comment = new Comment
                    {
                        AuthorEmail = "a@mail.net",
                        AuthorName = "commentor",
                        Text = "the comment"
                    }
                };

                processor.Process(args);

                Assert.That(args.Aborted, Is.False);
            }
        }

        [Test]
        public void NoDuplicateComment()
        {
            using (var db = new Db
            {
                new DbTemplate(Settings.CommentTemplateID)
                {
                    new DbField("Name"),
                    new DbField("Email"),
                    new DbField("Comment"),
                    new DbField("Website"),
                    new DbField("IP Address"),
                },
                new DbItem("blog")
                {
                    new DbItem("entry")
                }
            })
            {
                var blog = db.GetItem("/sitecore/content/blog");
                var entry = db.GetItem("/sitecore/content/blog/entry");

                var blogManager = Mock.Of<IBlogManager>(x =>
                    x.GetCurrentBlog(entry) == new BlogHomeItem(blog)
                    );

                var processor = new DuplicateSubmissionGuard(blogManager);

                var args = new CreateCommentArgs
                {
                    Database = entry.Database,
                    EntryID = entry.ID,
                    Language = Language.Parse("de"),
                    Comment = new Comment
                    {
                        AuthorEmail = "a@mail.net",
                        AuthorName = "commentor",
                        Text = "the comment"
                    }
                };

                processor.Process(args);

                Assert.That(args.Aborted, Is.False);
            }
        }

        [Test]
        public void CommentExists()
        {
            using (var db = new Db
            {
                new DbTemplate(Settings.CommentTemplateID)
                {
                    new DbField("Name"),
                    new DbField("Email"),
                    new DbField("Comment"),
                    new DbField("Website"),
                    new DbField("IP Address"),
                },
                new DbItem("blog")
                {
                    new DbItem("entry")
                    {
                        new DbItem("comment", ID.NewID, Settings.CommentTemplateID)
                        {
                            new DbField("Name")
                            {
                                { "de", "commentor" }
                            },
                            new DbField("Email")
                            {
                                { "de", "a@mail.net" }
                            },
                            new DbField("Comment")
                            {
                                { "de", "the comment" }
                            },
                        }
                    }
                }
            })
            {
                var blog = db.GetItem("/sitecore/content/blog");
                var entry = db.GetItem("/sitecore/content/blog/entry");

                var blogManager = Mock.Of<IBlogManager>(x =>
                    x.GetCurrentBlog(entry) == new BlogHomeItem(blog)
                    );

                var processor = new DuplicateSubmissionGuard(blogManager);

                var args = new CreateCommentArgs
                {
                    Database = entry.Database,
                    EntryID = entry.ID,
                    Language = Language.Parse("de"),
                    Comment = new Comment
                    {
                        AuthorEmail = "a@mail.net",
                        AuthorName = "commentor",
                        Text = "the comment"
                    }
                };

                processor.Process(args);

                Assert.That(args.Aborted, Is.True);
            }
        }

        [Test]
        public void DifferentText()
        {
            using (var db = new Db
            {
                new DbTemplate(Settings.CommentTemplateID)
                {
                    new DbField("Name"),
                    new DbField("Email"),
                    new DbField("Comment"),
                    new DbField("Website"),
                    new DbField("IP Address"),
                },
                new DbItem("blog")
                {
                    new DbItem("entry")
                    {
                        new DbItem("comment", ID.NewID, Settings.CommentTemplateID)
                        {
                            new DbField("Name")
                            {
                                { "de", "commentor" }
                            },
                            new DbField("Email")
                            {
                                { "de", "a@mail.net" }
                            },
                            new DbField("Comment")
                            {
                                { "de", "the comment" }
                            },
                        }
                    }
                }
            })
            {
                var blog = db.GetItem("/sitecore/content/blog");
                var entry = db.GetItem("/sitecore/content/blog/entry");

                var blogManager = Mock.Of<IBlogManager>(x =>
                    x.GetCurrentBlog(entry) == new BlogHomeItem(blog)
                    );

                var processor = new DuplicateSubmissionGuard(blogManager);

                var args = new CreateCommentArgs
                {
                    Database = entry.Database,
                    EntryID = entry.ID,
                    Language = Language.Parse("de"),
                    Comment = new Comment
                    {
                        AuthorEmail = "a@mail.net",
                        AuthorName = "commentor",
                        Text = "the comment updated"
                    }
                };

                processor.Process(args);

                Assert.That(args.Aborted, Is.False);
            }
        }
    }
}
