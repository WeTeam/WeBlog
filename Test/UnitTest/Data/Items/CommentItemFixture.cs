using Joel.Net;
using NUnit.Framework;
using Sitecore.Collections;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Sites;
using Sitecore.Web;

namespace Sitecore.Modules.WeBlog.UnitTest.Data.Items
{
    [TestFixture]
    public class CommentItemFixture
    {
        private readonly string EntryName = "entry";
        private readonly string AuthorName = "Lorem ipsum";
        private readonly string AuthorEmail = "lorem@ipsum.com";
        private readonly string CommentText = "the comment";
        private readonly string AuthorWebsite = "http://ipsum.com";
        private readonly string AuthorIpAddress = "127.0.0.1";

        [Test]
        public void ImplicitOperatorFromItem_NullItem_ReturnsNull()
        {
            // arrange, act
            CommentItem commentItem = (Item)null;

            // assert
            Assert.That(commentItem, Is.Null);
        }
        
        [Test]
        public void ImplicitOperatorFromItem_ItemWithCorrectFields_ReturnsCommentItem()
        {
            // arrange
            var siteContext = CreateSiteContext();

            using (new SiteContextSwitcher(siteContext))
            {
                using (var db = new Db
                {
                    CreateDbItem()
                })
                {
                    var entry = db.GetItem("/sitecore/content/" + EntryName);

                    // act
                    CommentItem commentItem = entry;

                    // assert
                    Assert.That(commentItem.Name.Raw, Is.EqualTo(AuthorName));
                    Assert.That(commentItem.Email.Raw, Is.EqualTo(AuthorEmail));
                    Assert.That(commentItem.Comment.Raw, Is.EqualTo(CommentText));
                    Assert.That(commentItem.Website.Raw, Is.EqualTo(AuthorWebsite));
                    Assert.That(commentItem.IpAddress.Raw, Is.EqualTo(AuthorIpAddress));
                }
            }
        }

        [Test]
        public void ImplicitOperatorToAkismetComment_NullItem_ReturnsNull()
        {
            // arrange, act
            AkismetComment comment = (CommentItem)null;

            // assert
            Assert.That(comment, Is.Null);
        }

        [Test]
        public void ImplicitOperatorToAkismetComment_ItemWithCorrectFields_ReturnsCommentItem()
        {
            // arrange
            var siteContext = CreateSiteContext();

            using (new SiteContextSwitcher(siteContext))
            {
                using (var db = new Db
                {
                    CreateDbItem()
                })
                {
                    var entry = db.GetItem("/sitecore/content/" + EntryName);
                    CommentItem commentItem = entry;

                    // act
                    AkismetComment akismetComment = commentItem;

                    // assert
                    Assert.That(akismetComment.Blog, Is.EqualTo(siteContext.HostName));
                    Assert.That(akismetComment.CommentAuthor, Is.EqualTo(AuthorName));
                    Assert.That(akismetComment.CommentAuthorEmail, Is.EqualTo(AuthorEmail));
                    Assert.That(akismetComment.CommentContent, Is.EqualTo(CommentText));
                    Assert.That(akismetComment.CommentAuthorUrl, Is.EqualTo(AuthorWebsite));
                    Assert.That(akismetComment.CommentType, Is.EqualTo("comment"));
                    Assert.That(akismetComment.UserIp, Is.EqualTo(AuthorIpAddress));
                }
            }
        }

        [Test]
        public void ImplicitOperatorToCommentContent_NullItem_ReturnsNull()
        {
            // arrange, act
            CommentContent commentContent = (CommentItem)null;

            // assert
            Assert.That(commentContent, Is.Null);
        }

        [Test]
        public void ImplicitOperatorToCommentContent_ItemWithCorrectFields_ReturnsNull()
        {
            // arrange
            using (var db = new Db
            {
                CreateDbItem()
            })
            {
                var entry = db.GetItem("/sitecore/content/" + EntryName);
                CommentItem commentItem = entry;

                // act
                CommentContent commentContent = commentItem;

                // assert
                Assert.That(commentContent.Uri, Is.EqualTo(entry.Uri));
                Assert.That(commentContent.AuthorName, Is.EqualTo(AuthorName));
                Assert.That(commentContent.AuthorWebsite, Is.EqualTo(AuthorWebsite));
                Assert.That(commentContent.AuthorEmail, Is.EqualTo(AuthorEmail));
                Assert.That(commentContent.Text, Is.EqualTo(CommentText));
            }
        }

        private SiteContext CreateSiteContext()
        {
            var siteInfo = new SiteInfo(new StringDictionary
            {
                { "hostName", "someurl" }
            });

            return new SiteContext(siteInfo);
        }

        private DbItem CreateDbItem()
        {
            return new DbItem(EntryName)
            {
                new DbField("Name") { Value = AuthorName },
                new DbField("Email") { Value = AuthorEmail },
                new DbField("Comment") { Value = CommentText },
                new DbField("Website") { Value = AuthorWebsite },
                new DbField("IP Address") { Value = AuthorIpAddress }
            };
        }
    }
}
