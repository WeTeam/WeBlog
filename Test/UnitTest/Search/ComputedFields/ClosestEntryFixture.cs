using Moq;
using NUnit.Framework;
using Sitecore.Buckets.Util;
using Sitecore.ContentSearch;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Search.ComputedFields;
using System.Linq;

namespace Sitecore.Modules.WeBlog.UnitTest.Search.ComputedFields
{
    [TestFixture]
    public class ClosestEntryFixture
    {
        [Test]
        public void ComputeFieldValue_ArgumentNotAnItem_ReturnsNull()
        {
            // arrange
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);
            var sut = new ClosestEntry(manager);
            var objectToIndex = new AbstractIndexable();

            // act
            var result = sut.ComputeFieldValue(objectToIndex);

            // assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void ComputeFieldValue_ItemNotUnderEntry_ReturnsNull()
        {
            // arrange
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);
            var sut = new ClosestEntry(manager);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First())
                    },
                    new DbItem("someitem")
                }
            })
            {
                var item = db.GetItem("/sitecore/content/blog/someitem");

                // act
                var result = sut.ComputeFieldValue(new SitecoreIndexableItem(item));

                // assert
                Assert.That(result, Is.Null);
            }
        }

        [Test]
        public void ComputeFieldValue_ItemIsEntry_ReturnsEntryItemUri()
        {
            // arrange
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);
            var sut = new ClosestEntry(manager);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First())
                    }
                }
            })
            {
                var item = db.GetItem("/sitecore/content/blog/2016/entry1");

                // act
                var result = sut.ComputeFieldValue(new SitecoreIndexableItem(item));

                // assert
                Assert.That(result, Is.EqualTo(item.Uri));
            }
        }

        [Test]
        public void ComputeFieldValue_ItemUnderEntry_ReturnsEntryItemUri()
        {
            // arrange
            var settings = MockSettings(ID.NewID);
            var manager = new TestableEntryManager(settings, 1);
            var sut = new ClosestEntry(manager);

            using (var db = new Db
            {
                new DbItem("blog", ID.NewID, settings.BlogTemplateIds.First()) {
                    new DbItem("2016", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                    {
                        new DbItem("entry1", ID.NewID, settings.EntryTemplateIds.First())
                        {
                            new DbItem("2017", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                            {
                                new DbItem("12", ID.NewID, BucketConfigurationSettings.BucketTemplateId)
                                {
                                    new DbItem("comment", ID.NewID, settings.CommentTemplateIds.First())
                                }
                            }
                        }
                    }
                }
            })
            {
                var entryItem = db.GetItem("/sitecore/content/blog/2016/entry1");
                var item = db.GetItem("/sitecore/content/blog/2016/entry1/2017/12/comment");

                // act
                var result = sut.ComputeFieldValue(new SitecoreIndexableItem(item));

                // assert
                Assert.That(result, Is.EqualTo(entryItem.Uri));
            }
        }

        private IWeBlogSettings MockSettings(params ID[] entryTemplateIds)
        {
            return Mock.Of<IWeBlogSettings>(x =>
                x.BlogTemplateIds == new[] { ID.NewID, ID.NewID } &&
                x.CategoryTemplateIds == new[] { ID.NewID, ID.NewID } &&
                x.EntryTemplateIds == entryTemplateIds &&
                x.CommentTemplateIds == new[] { ID.NewID } &&
                x.SearchIndexName == "WeBlog"
            );
        }
    }
}
