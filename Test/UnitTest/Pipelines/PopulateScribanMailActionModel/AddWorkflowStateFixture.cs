using Moq;
using NUnit.Framework;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel;
using Sitecore.Workflows;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.PopulateScribanMailActionModel
{
    [TestFixture]
    public class AddWorkflowStateFixture
    {
        [Test]
        public void Process_WhenCalled_AddsState()
        {
            // arrange
            var dataItem = ItemFactory.CreateItem();
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs(dataItem.Object);
            var args = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);

            var workflowState = new WorkflowState(ID.NewID.ToString(), "state", "icon", true);
            var itemState = new Mock<ItemState>(dataItem.Object);
            itemState.Setup(x => x.GetWorkflowState()).Returns(workflowState);

            dataItem.Setup(x => x.State).Returns(itemState.Object);

            var sut = new AddWorkflowState();

            // act
            sut.Process(args);

            // assert
            var value = args.GetModel(AddWorkflowState.ModelKey);
            Assert.That(value, Is.SameAs(workflowState));
        }
    }
}
