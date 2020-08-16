using Moq;
using NUnit.Framework;
using Sitecore.Analytics;
using Sitecore.Analytics.Core;
using Sitecore.Analytics.Tracking;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Caching;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Modules.WeBlog.Pipelines.ValidateComment;
using System.Collections.Specialized;
using System.Linq;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.ValidateComment
{
    [TestFixture]
    public class ContactNotRobotFixture
    {
        [SetUp]
        public void SetUp()
        {
            // Add dictionary entries to cache
            var cache = Mock.Of<TranslatorCache>(x =>
                x.FindEntry(Constants.TranslationPhrases.ErrorOccurredTryAgain) == CreateDictionaryEntryItem("An error occurred. Please try again later.")
            );

            CacheManager.SetCache("WeBlog [translator]", cache);
        }

        private Item CreateDictionaryEntryItem(string phrase)
        {
            var item = ItemFactory.CreateItem();
            var field = FieldFactory.CreateField(item.Object, ID.NewID, "Phrase", phrase);
            ItemFactory.AddFields(item, new[] { field });
            return item.Object;
        }

        [Test]
        public void Process_ContactIsRobot_AddsErrorToArgs()
        {
            // arrange
            var args = new ValidateCommentArgs(new Comment(), new NameValueCollection());
            var sut = new ContactNotRobot();

            var tracker = CreateTracker(ContactClassification.RobotBoundary);

            // act
            using (new TrackerSwitcher(tracker))
            {
                sut.Process(args);
            }

            // assert
            var errorText = args.Errors.First();
            Assert.That(errorText, Is.EqualTo("An error occurred. Please try again later."));
        }

        [Test]
        public void Process_ContactIsHuman_NoErrorsInArgs()
        {
            // arrange
            var args = new ValidateCommentArgs(new Comment(), new NameValueCollection());
            var sut = new ContactNotRobot();

            var tracker = CreateTracker(ContactClassification.Unidentified);

            // act
            using (new TrackerSwitcher(tracker))
            {
                sut.Process(args);
            }

            // assert
            Assert.That(args.Errors, Is.Empty);
        }

        private ITracker CreateTracker(int classifiaction)
        {
            return Mock.Of<ITracker>(trackerMock =>
                trackerMock.Contact == Mock.Of<Contact>(contact =>
                    contact.System == Mock.Of<IContactSystemInfoContext>(ctx =>
                        ctx.Classification == classifiaction
                    )
                )
            );
        }
    }
}
