using Moq;
using NUnit.Framework;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel;
using Sitecore.Security;
using Sitecore.Security.Accounts;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.PopulateScribanMailActionModel
{
    [TestFixture]
    public class AddEntryModelsFixture
    {
        private const string UserName = "sitecore\\user";

        [Test]
        public void Process_EntryItemIsNull_AddsNothing()
        {
            // arrange
            var (sut, dataItem) = CreateAddEntryModels();
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs(dataItem);
            var args = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);

            // act
            sut.Process(args);

            // assert
            var entryExists = args.ModelContains(AddEntryModels.EntryModelKey);
            Assert.That(entryExists, Is.False);

            var createdByExists = args.ModelContains(AddEntryModels.CreatedByModelKey);
            Assert.That(createdByExists, Is.False);

            var updatedByExists = args.ModelContains(AddEntryModels.UpdatedByModelKey);
            Assert.That(updatedByExists, Is.False);
        }

        [Test]
        public void Process_EntryItemIsNotNull_AddsEntry()
        {
            // arrange
            var (sut, dataItem) = CreateAddEntryModels();
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs(dataItem);
            var args = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            args.EntryItem = new EntryItem(dataItem);

            // act
            sut.Process(args);

            // assert
            var entry = args.GetModel(AddEntryModels.EntryModelKey);
            Assert.That(((EntryItem)entry).ID, Is.EqualTo(dataItem.ID));
        }

        [Test]
        public void Process_EntryItemIsNotNull_AddsCreatedByUser()
        {
            // arrange
            var (sut, dataItem) = CreateAddEntryModels();
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs(dataItem);
            var args = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            args.EntryItem = new EntryItem(dataItem);

            // act
            sut.Process(args);

            // assert
            var createdBy = args.GetModel(AddEntryModels.CreatedByModelKey);
            Assert.That(((UserProfile)createdBy).Name, Is.EqualTo(UserName));
        }

        [Test]
        public void Process_EntryItemIsNotNull_AddsUpdatedByUser()
        {
            // arrange
            var (sut, dataItem) = CreateAddEntryModels();
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs(dataItem);
            var args = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            args.EntryItem = new EntryItem(dataItem);

            // act
            sut.Process(args);

            // assert
            var updatedBy = args.GetModel(AddEntryModels.UpdatedByModelKey);
            Assert.That(((UserProfile)updatedBy).Name, Is.EqualTo(UserName));
        }

        private (AddEntryModels sut, Item dataItem) CreateAddEntryModels()
        {
            var dataItemMock = ItemFactory.CreateItem();

            var statistics = new Mock<ItemStatistics>(dataItemMock.Object);
            statistics.Setup(x => x.CreatedBy).Returns(UserName);
            statistics.Setup(x => x.UpdatedBy).Returns(UserName);

            dataItemMock.Setup(x => x.Statistics).Returns(statistics.Object);

            // Works with FakeDB
            var profile = Mock.Of<UserProfile>(x => x.Name == UserName);
            var user = new Mock<User>(UserName, true);
            user.Setup(x => x.Profile).Returns(profile);

            var sut = new TestAddEntryModels(user.Object);

            return (sut, dataItemMock.Object);
        }
    }
}
