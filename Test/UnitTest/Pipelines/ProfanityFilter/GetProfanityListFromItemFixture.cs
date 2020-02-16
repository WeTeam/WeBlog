using Moq;
using NUnit.Framework;
using Sitecore.Data;
using Sitecore.Modules.WeBlog.Pipelines;
using Sitecore.Modules.WeBlog.Pipelines.ProfanityFilter;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.ProfanityFilter
{
    [TestFixture]
    public class GetProfanityListFromItemFixture
    {
        [Test]
        public void Process_UnknownItem_DoesNotPopulateWords()
        {
            // arrange
            var database = Mock.Of<Database>();
            var sut = new GetProfanityListFromItem(database);
            var args = new ProfanityFilterArgs();

            // act
            sut.Process(args);

            // assert
            Assert.That(args.WordList, Is.Empty);
        }


        [Test]
        public void Process_ValidItem_PopulatesWords()
        {
            // arrange
            var database = new Mock<Database>();
            var itemMock = ItemFactory.CreateItem(database: database.Object);
            database.Setup(x => x.GetItem("item")).Returns(itemMock.Object);

            var field = FieldFactory.CreateField(itemMock.Object, ID.NewID, Constants.Fields.WordList, "lorem\nipsum");

            ItemFactory.AddFields(itemMock, new[] { field });

            var sut = new GetProfanityListFromItem(database.Object);
            sut.ItemPath = "item";
            var args = new ProfanityFilterArgs();

            // act
            sut.Process(args);

            // assert
            Assert.That(args.WordList, Is.EquivalentTo(new[] { "lorem", "ipsum" }));
        }

        [Test]
        public void Process_WordsAlreadyPopulated_DoesNothing()
        {
            // arrange
            var database = new Mock<Database>();
            var itemMock = ItemFactory.CreateItem(database: database.Object);
            database.Setup(x => x.GetItem("item")).Returns(itemMock.Object);

            var field = FieldFactory.CreateField(itemMock.Object, ID.NewID, Constants.Fields.WordList, "lorem\nipsum");

            ItemFactory.AddFields(itemMock, new[] { field });

            var sut = new GetProfanityListFromItem(database.Object);
            sut.ItemPath = "item";
            var args = new ProfanityFilterArgs();
            args.WordList = new[] { "dolor" };

            // act
            sut.Process(args);

            // assert
            Assert.That(args.WordList, Is.EquivalentTo(new[] { "dolor" }));
        }
    }
}
