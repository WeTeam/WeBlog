using Moq;
using NUnit.Framework;
using Sitecore.Caching;
using Sitecore.Data;
using Sitecore.Modules.WeBlog.Caching;

namespace Sitecore.Modules.WeBlog.UnitTest.Caching
{
#if FEATURE_ABSTRACTIONS
    [TestFixture]
    public class ProfanityFilterCacheFixture
    {
        [Test]
        public void WordListGet_DatabaseIsNullAndNothingSet_ReturnsNull()
        {
            // arrange
            var innerCache = Mock.Of<ICache>();
            var sut = new ProfanityFilterCache(innerCache, null);

            // act
            var words = sut.WordList;

            // assert
            Assert.That(words, Is.Null);
        }

        [Test]
        public void WordListGet_NothingSet_ReturnsNull()
        {
            // arrange
            var innerCache = Mock.Of<ICache>();
            var database = Mock.Of<Database>(x => x.Name == "database");
            var sut = new ProfanityFilterCache(innerCache, database);

            // act
            var words = sut.WordList;

            // assert
            Assert.That(words, Is.Null);
        }

        [Test]
        public void WordListGet_InnerCacheHasEntries_ReturnsWords()
        {
            // arrange
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            var innerCache = Mock.Of<ICache>(x =>
                x.GetValue("wordlist_database") == "lorem|ipsum" &&
                x["wordlist_database"] == "lorem|ipsum"
            );
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            var database = Mock.Of<Database>(x => x.Name == "database");
            var sut = new ProfanityFilterCache(innerCache, database);

            // act
            var words = sut.WordList;

            // assert
            Assert.That(words, Is.EquivalentTo(new[] { "lorem", "ipsum" }));
        }

        [Test]
        public void WordListSet_WordsSet_SavesWordsToInnerCache()
        {
            // arrange
            var innerCache = new Mock<ICache>();
            innerCache.Setup(x => x.Enabled).Returns(true);

            var database = Mock.Of<Database>(x => x.Name == "database");
            var sut = new ProfanityFilterCache(innerCache.Object, database);

            // act
            sut.WordList = new[] { "lorem", "ipsum" };

            // assert
            innerCache.Verify(x => x.Add("wordlist_database", "lorem|ipsum"));
        }

        [Test]
        public void WordListSet_DatabaseIsNullAndWordsSet_SavesWordsToInnerCache()
        {
            // arrange
            var innerCache = new Mock<ICache>();
            innerCache.Setup(x => x.Enabled).Returns(true);

            var sut = new ProfanityFilterCache(innerCache.Object, null);

            // act
            sut.WordList = new[] { "lorem", "ipsum" };

            // assert
            innerCache.Verify(x => x.Add("wordlist_master", "lorem|ipsum"));
        }
    }
#endif
}
