using Moq;
using NUnit.Framework;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Sitecore.Globalization;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Modules.WeBlog.Pipelines;
using Sitecore.Modules.WeBlog.Pipelines.CreateComment;
using Sitecore.Sites;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.CreateComment
{
    [TestFixture]
    public class CreateCommentItemFixture
    {
        [Test]
        public void NullArgs()
        {
            var processor = new CreateCommentItem();
            Assert.That(() => processor.Process(null), Throws.ArgumentNullException);
        }

        [Test]
        public void NullDatabase()
        {
            var processor = new CreateCommentItem();
            using (var db = new Db
            {
                new DbItem("entry")
            })
            {
                var entry = db.GetItem("/sitecore/content/entry");
                var args = new CreateCommentArgs
                {
                    Comment = new Comment(),
                    EntryID = entry.ID,
                    Language = Language.Parse("da")
                };

                Assert.That(() => processor.Process(args), Throws.InvalidOperationException);
            }
        }

        [Test]
        public void NullComment()
        {
            var processor = new CreateCommentItem();
            using (var db = new Db
            {
                new DbItem("entry")
            })
            {
                var entry = db.GetItem("/sitecore/content/entry");
                var args = new CreateCommentArgs
                {
                    Database = entry.Database,
                    EntryID = entry.ID,
                    Language = Language.Parse("da")
                };

                Assert.That(() => processor.Process(args), Throws.InvalidOperationException);
            }
        }

        [Test]
        public void NullEntryId()
        {
            var processor = new CreateCommentItem();
            using (var db = new Db
            {
                new DbItem("entry")
            })
            {
                var entry = db.GetItem("/sitecore/content/entry");
                var args = new CreateCommentArgs
                {
                    Database = entry.Database,
                    Comment = new Comment(),
                    Language = Language.Parse("da")
                };

                Assert.That(() => processor.Process(args), Throws.InvalidOperationException);
            }
        }

        [Test]
        public void NullLanguage()
        {
            var processor = new CreateCommentItem();
            using (var db = new Db
            {
                new DbItem("entry")
            })
            {
                var entry = db.GetItem("/sitecore/content/entry");
                var args = new CreateCommentArgs
                {
                    Database = entry.Database,
                    Comment = new Comment(),
                    EntryID = entry.ID
                };

                Assert.That(() => processor.Process(args), Throws.InvalidOperationException);
            }
        }

        [Test]
        public void InvalidEntryID()
        {
            var blogTemplateId = ID.NewID;

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, blogTemplateId)
            })
            {
                var blog = db.GetItem("/sitecore/content/blog");

                var blogManager = Mock.Of<IBlogManager>(x =>
                    x.GetCurrentBlog(It.IsAny<Item>()) == new BlogHomeItem(blog, null)
                    );
                var processor = new CreateCommentItem(blogManager);

                var args = new CreateCommentArgs
                {
                    Database = blog.Database,
                    Comment = new Comment
                    {
                        AuthorName = "commenter",
                        AuthorEmail = "c@mail.com"
                    },
                    EntryID = ID.NewID,
                    Language = Language.Parse("da")
                };

                processor.Process(args);

                Assert.That(args.CommentItem, Is.Null);
            }
        }

        [Test]
        public void NewComment()
        {
            var commentTemplate = CreateCommentTemplate();
            var settings = CreateSettings(commentTemplate.ID);

            using (var db = new Db
            {
                commentTemplate,
                new DbItem("blog")
                {
                    new DbItem("entry")
                }
            })
            {
                var blog = db.GetItem("/sitecore/content/blog");
                var entry = db.GetItem("/sitecore/content/blog/entry");

                var blogManager = Mock.Of<IBlogManager>(x => 
                    x.GetCurrentBlog(entry) == new BlogHomeItem(blog, settings)
                    );
                var processor = new CreateCommentItem(blogManager);
                
                var args = new CreateCommentArgs
                {
                    Database = entry.Database,
                    Comment = new Comment
                    {
                        AuthorName = "commenter",
                        AuthorEmail = "c@mail.com",
                        Text = "the comment"
                    },
                    EntryID = entry.ID,
                    Language = Language.Parse("da")
                };

                processor.Process(args);

                Assert.That(args.CommentItem, Is.Not.Null);
                Assert.That(args.CommentItem.InnerItem.Axes.IsDescendantOf(entry), Is.True);
            }
        }

        [Test]
        public void MultipleCommentsSameName()
        {
            var commentTemplate = CreateCommentTemplate();
            var settings = CreateSettings(commentTemplate.ID);

            using (var db = new Db
            {
                commentTemplate,
                new DbItem("blog")
                {
                    new DbItem("entry")
                }
            })
            {
                var blog = db.GetItem("/sitecore/content/blog");
                var entry = db.GetItem("/sitecore/content/blog/entry");

                var blogManager = Mock.Of<IBlogManager>(x =>
                    x.GetCurrentBlog(entry) == new BlogHomeItem(blog, settings)
                    );
                var processor = new TestableCreateCommentItemProcessor(blogManager);

                var args = new CreateCommentArgs
                {
                    Database = entry.Database,
                    Comment = new Comment
                    {
                        AuthorName = "commenter",
                        AuthorEmail = "c@mail.com",
                        Text = "the comment"
                    },
                    EntryID = entry.ID,
                    Language = Language.Parse("da")
                };

                processor.Process(args);

                Assert.That(args.CommentItem, Is.Not.Null);
                Assert.That(args.CommentItem.InnerItem.Axes.IsDescendantOf(entry), Is.True);

                var firstComment = args.CommentItem;

                processor.Process(args);

                Assert.That(args.CommentItem, Is.Not.Null);
                Assert.That(args.CommentItem.InnerItem.Axes.IsDescendantOf(entry), Is.True);
                Assert.That(args.CommentItem.ID, Is.Not.EqualTo(firstComment.ID));
                Assert.That(args.CommentItem.InnerItem.Name, Is.Not.EqualTo(firstComment.InnerItem.Name));
                Assert.That(args.CommentItem.InnerItem.Name, Does.Match(".+\\d$"));
            }
        }

        [Test]
        [Description("The processor uses Sitefore query, so this test ensures no issues in that part")]
        public void DashesInPath()
        {
            var commentTemplate = CreateCommentTemplate();
            var settings = CreateSettings(commentTemplate.ID);

            using (var db = new Db
            {
                commentTemplate,
                new DbItem("my-blog")
                {
                    new DbItem("the-entry")
                }
            })
            {
                var blog = db.GetItem("/sitecore/content/my-blog");
                var entry = db.GetItem("/sitecore/content/my-blog/the-entry");

                var blogManager = Mock.Of<IBlogManager>(x =>
                    x.GetCurrentBlog(entry) == new BlogHomeItem(blog, settings)
                    );
                var processor = new CreateCommentItem(blogManager);

                var args = new CreateCommentArgs
                {
                    Database = entry.Database,
                    Comment = new Comment
                    {
                        AuthorName = "commenter",
                        AuthorEmail = "c@mail.com",
                        Text = "the comment"
                    },
                    EntryID = entry.ID,
                    Language = Language.Parse("da")
                };

                processor.Process(args);

                Assert.That(args.CommentItem, Is.Not.Null);
                Assert.That(args.CommentItem.InnerItem.Axes.IsDescendantOf(entry), Is.True);
            }
        }

        [Test]
        public void CustomFields()
        {
            var commentTemplate = new DbTemplate(ID.NewID)
            {
                new DbField("Name"),
                new DbField("Email"),
                new DbField("Comment"),
                new DbField("Website"),
                new DbField("IP Address"),
                new DbField("field1"),
                new DbField("field2")
            };

            var settings = CreateSettings(commentTemplate.ID);

            using (var db = new Db
            {
                commentTemplate,
                new DbItem("blog")
                {
                    new DbItem("entry")
                }
            })
            {
                var blog = db.GetItem("/sitecore/content/blog");
                var entry = db.GetItem("/sitecore/content/blog/entry");

                var blogManager = Mock.Of<IBlogManager>(x =>
                    x.GetCurrentBlog(entry) == new BlogHomeItem(blog, settings)
                    );
                var processor = new CreateCommentItem(blogManager);

                var args = new CreateCommentArgs
                {
                    Database = entry.Database,
                    Comment = new Comment
                    {
                        AuthorName = "commenter",
                        AuthorEmail = "c@mail.com",
                        Text = "the comment",
                        Fields =
                        {
                            { "field1", "value1" },
                            { "field2", "value2" },
                        }
                    },
                    EntryID = entry.ID,
                    Language = Language.Parse("da")
                };

                processor.Process(args);

                Assert.That(args.CommentItem, Is.Not.Null);
                Assert.That(args.CommentItem.InnerItem.Axes.IsDescendantOf(entry), Is.True);
                Assert.That(args.CommentItem["field1"], Is.EqualTo("value1"));
                Assert.That(args.CommentItem["field2"], Is.EqualTo("value2"));
            }
        }

        private DbTemplate CreateCommentTemplate()
        {
            return new DbTemplate(ID.NewID)
            {
                new DbField("Name"),
                new DbField("Email"),
                new DbField("Comment"),
                new DbField("Website"),
                new DbField("IP Address"),
            };
        }

        private IWeBlogSettings CreateSettings(ID commentTemplateId)
        {
            return Mock.Of<IWeBlogSettings>(x =>
                x.CommentTemplateIds == new [] { commentTemplateId }
            );
        }
    }
}
