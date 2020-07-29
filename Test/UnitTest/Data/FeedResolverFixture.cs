using Moq;
using NUnit.Framework;
using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data;
using System;
using System.Linq;

namespace Sitecore.Modules.WeBlog.UnitTest.Data
{
    [TestFixture]
    public class FeedResolverFixture
    {
        [Test]
        public void Ctor_TemplateManagerIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new FeedResolver(null, Mock.Of<IWeBlogSettings>());

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(new TestDelegate(sutAction));
            Assert.That(ex.ParamName, Is.EqualTo("templateManager"));
        }

        [Test]
        public void Ctor_WeBlogSettingsIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new FeedResolver(Mock.Of<BaseTemplateManager>(), null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(new TestDelegate(sutAction));
            Assert.That(ex.ParamName, Is.EqualTo("weBlogSettings"));
        }

        [Test]
        public void ResolveFor_BlogItemIsNull_ReturnsEmpty()
        {
            // arrange
            var templateManager = Mock.Of<BaseTemplateManager>();
            var settings = Mock.Of<IWeBlogSettings>();
            var sut = new FeedResolver(templateManager, settings);

            // act
            var result = sut.Resolve(null);

            // assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void ResolveFor_BlogContainsNoFeeds_ReturnsEmpty()
        {
            // arrange
            var templateManager = Mock.Of<BaseTemplateManager>();
            var settings = Mock.Of<IWeBlogSettings>();
            var sut = new FeedResolver(templateManager, settings);

            var blogItemMock = CreateMockBlogItem(true);

            // act
            var result = sut.Resolve(blogItemMock.Object);

            // assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void ResolveFor_BlogContainsSingleFeed_ReturnsFeed()
        {
            // arrange
            var feedTemplateId = ID.NewID;
            var templateManager = CreateTemplateManager(feedTemplateId);

            var settings = Mock.Of<IWeBlogSettings>(x =>
                x.RssFeedTemplateIds == new[] { feedTemplateId }
            );
            var sut = new FeedResolver(templateManager, settings);

            var blogItemMock = CreateMockBlogItem(true);
            var feedItem = ItemFactory.CreateItem(templateId: feedTemplateId).Object;

            ItemFactory.AddChildItems(blogItemMock, feedItem);

            // act
            var result = sut.Resolve(blogItemMock.Object);

            // assert
            var feedItems = result.ToList();

            Assert.That(feedItems.Count, Is.EqualTo(1));
            Assert.That(feedItems[0].ID, Is.EqualTo(feedItem.ID));
        }

        [Test]
        public void ResolveFor_RssDisabled_ReturnsEmpty()
        {
            // arrange
            var feedTemplateId = ID.NewID;
            var templateManager = CreateTemplateManager(feedTemplateId);

            var settings = Mock.Of<IWeBlogSettings>(x =>
                x.RssFeedTemplateIds == new[] { feedTemplateId }
            );
            var sut = new FeedResolver(templateManager, settings);

            var blogItemMock = CreateMockBlogItem(false);
            var feedItem = ItemFactory.CreateItem(templateId: feedTemplateId).Object;

            ItemFactory.AddChildItems(blogItemMock, feedItem);

            // act
            var result = sut.Resolve(blogItemMock.Object);

            // assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void ResolveFor_BlogContainsManyFeeds_ReturnsFeeds()
        {
            // arrange
            var feedTemplateId = ID.NewID;
            var templateManager = CreateTemplateManager(feedTemplateId);

            var settings = Mock.Of<IWeBlogSettings>(x =>
                x.RssFeedTemplateIds == new[] { feedTemplateId }
            );
            var sut = new FeedResolver(templateManager, settings);

            var blogItemMock = CreateMockBlogItem(true);
            var feedItem1 = ItemFactory.CreateItem(templateId: feedTemplateId).Object;
            var feedItem2 = ItemFactory.CreateItem(templateId: feedTemplateId).Object;
            var otherItem = ItemFactory.CreateItem().Object;

            ItemFactory.AddChildItems(blogItemMock, feedItem1, otherItem, feedItem2);

            // act
            var result = sut.Resolve(blogItemMock.Object);

            // assert
            var feedItems = result.ToList();

            Assert.That(feedItems.Count, Is.EqualTo(2));
            Assert.That(feedItems[0].ID, Is.EqualTo(feedItem1.ID));
            Assert.That(feedItems[1].ID, Is.EqualTo(feedItem2.ID));
        }

        private Mock<Item> CreateMockBlogItem(bool rssEnabled)
        {
            var itemMock = ItemFactory.CreateItem();
            var rssEnabledField = FieldFactory.CreateField(itemMock.Object, ID.NewID, "Enable RSS", rssEnabled ? "1" : "0");
            ItemFactory.AddFields(itemMock, new[] { rssEnabledField });

            return itemMock;
        }

        private BaseTemplateManager CreateTemplateManager(ID feedTemplateId)
        {
            var templates = new TemplateCollection();
            var template = TemplateFactory.CreateTemplate(feedTemplateId, ID.NewID, templates);
            var templateManager = TemplateFactory.CreateTemplateManager(new[] { template });

            return templateManager;
        }
    }
}
