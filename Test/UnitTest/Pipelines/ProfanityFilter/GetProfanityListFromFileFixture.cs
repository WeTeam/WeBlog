using NUnit.Framework;
using Sitecore.Modules.WeBlog.Pipelines;
using Sitecore.Modules.WeBlog.Pipelines.ProfanityFilter;
using System.IO;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.ProfanityFilter
{
    [TestFixture]
    public class GetProfanityListFromFileFixture
    {
        [Test]
        public void Process_WordListNotNull_DoesNothing()
        {
            // arrange
            var sut = new GetProfanityListFromFile();
            var args = new ProfanityFilterArgs();
            args.WordList = new[] { "lorem", "ipsum" };

            // act
            sut.Process(args);

            // assert
            Assert.That(args.WordList, Is.EquivalentTo(new[] { "lorem", "ipsum" }));
        }

        [Test]
        public void Process_FileNotFound_DoesNothing()
        {
            // arrange
            var sut = new GetProfanityListFromFile();
            sut.FilePath = "invalidfile.txt";
            var args = new ProfanityFilterArgs();

            // act
            sut.Process(args);

            // assert
            Assert.That(args.WordList, Is.Empty);
        }

        [Test]
        public void Process_FileExistsButIsEmpty_DoesNothing()
        {
            // This is an integration test. Sorry.
            // arrange
            File.WriteAllText("empty.txt", string.Empty);
            var sut = new GetProfanityListFromFile();
            sut.FilePath = "invalidfile.txt";
            var args = new ProfanityFilterArgs();

            // act
            sut.Process(args);

            // assert
            Assert.That(args.WordList, Is.Empty);
        }

        [Test]
        public void Process_FileWithWords_PopulatesWordList()
        {
            // This is an integration test. Sorry.
            // arrange
            File.WriteAllText("words.txt", "lorem\r\nipsum");
            var sut = new GetProfanityListFromFile();
            sut.FilePath = "words.txt";
            var args = new ProfanityFilterArgs();

            // act
            sut.Process(args);

            // assert
            Assert.That(args.WordList, Is.EquivalentTo(new[] { "lorem", "ipsum" }));
        }
    }
}
