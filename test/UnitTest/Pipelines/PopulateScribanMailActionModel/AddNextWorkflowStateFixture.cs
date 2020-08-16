using Moq;
using NUnit.Framework;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.PopulateScribanMailActionModel
{
    [TestFixture]
    public class AddNextWorkflowStateFixture
    {
        [Test]
        public void Process_NextStatePresent_AddsState()
        {
            // arrange
            var actionItem = CreateWorkflowItems();

            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            workflowPipelineArgs.ProcessorItem = new ProcessorItem(actionItem);

            var args = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            var sut = new AddNextWorkflowState();

            // act
            sut.Process(args);

            // assert
            var value = args.GetModel(AddNextWorkflowState.ModelKey);
            Assert.That(value, Is.EqualTo("next state"));
        }

        [Test]
        public void Process_NoNextState_AddsEmptyString()
        {
            // arrange
            var commandItem = ItemFactory.CreateItem();
            var actionItem = ItemFactory.CreateItem();
            actionItem.Setup(x => x.Parent).Returns(commandItem.Object);

            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            workflowPipelineArgs.ProcessorItem = actionItem.Object;

            var args = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            var sut = new AddNextWorkflowState();

            // act
            sut.Process(args);

            // assert
            var value = args.GetModel(AddNextWorkflowState.ModelKey);
            Assert.That(value, Is.Empty);
        }

        private Item CreateWorkflowItems()
        {
            var database = new Mock<Database>();
            database.Setup(x => x.Name).Returns("fake");

            var commandItem = ItemFactory.CreateItem(database: database.Object);

            var nextStateItem = ItemFactory.CreateItem(database: database.Object);
            nextStateItem.Setup(x => x.Name).Returns("next state");
            database.Setup(x => x.GetItem(nextStateItem.Object.ID)).Returns(nextStateItem.Object);
            database.Setup(x => x.GetItem(nextStateItem.Object.ID.ToString())).Returns(nextStateItem.Object);

            ItemFactory.SetIndexerField(commandItem, FieldIDs.NextState, nextStateItem.Object.ID.ToString());

            var actionItem = ItemFactory.CreateItem(database: database.Object);
            actionItem.Setup(x => x.Parent).Returns(commandItem.Object);

            return actionItem.Object;
        }
    }
}
