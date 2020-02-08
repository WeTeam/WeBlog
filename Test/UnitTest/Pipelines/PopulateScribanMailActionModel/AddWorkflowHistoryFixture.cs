using Moq;
using NUnit.Framework;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel;
using Sitecore.Workflows;
using System;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.PopulateScribanMailActionModel
{
    [TestFixture]
    public class AddWorkflowHistoryFixture
    {
        [Test]
        public void Process_ItemHasNoWorkflow_HistoryObjectIsEmpty()
        {
            // arrange
            var dataItemMock = ItemFactory.CreateItem();
            var itemState = new Mock<ItemState>(dataItemMock.Object);
            dataItemMock.Setup(x => x.State).Returns(itemState.Object);

            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs(dataItemMock.Object);
            var args = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            var sut = new AddWorkflowHistory();

            // act
            sut.Process(args);

            // assert
            var value = args.GetModel(AddWorkflowHistory.ModelKey);
            Assert.That((WorkflowEvent[])value, Is.Empty);
        }

        [Test]
        public void Process_ItemHasNoWorkflowHistory_HistoryObjectIsEmpty()
        {
            // arrange
            var dataItemMock = ItemFactory.CreateItem();
            AddItemHistory(dataItemMock, new ID[0]);

            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs(dataItemMock.Object);
            var args = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            var sut = new AddWorkflowHistory();

            // act
            sut.Process(args);

            // assert
            var value = args.GetModel(AddWorkflowHistory.ModelKey);
            Assert.That((WorkflowEvent[])value, Is.Empty);
        }

        [Test]
        public void Process_ItemHasWorkflowHistory_AddsWorkflowHistory()
        {
            // arrange
            var dataItemMock = ItemFactory.CreateItem();
            var workflowEvents = AddItemHistory(dataItemMock, new[] { ID.NewID, ID.NewID });

            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs(dataItemMock.Object);
            var args = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            var sut = new AddWorkflowHistory();

            // act
            sut.Process(args);

            // assert
            var value = args.GetModel(AddWorkflowHistory.ModelKey);
            Assert.That((WorkflowEvent[])value, Is.EquivalentTo(workflowEvents));
        }

        private WorkflowEvent[] AddItemHistory(Mock<Item> item, ID[] workflowStates)
        {
            WorkflowEvent[] events = null;

            if (workflowStates.Length == 0)
                events = new WorkflowEvent[0];
            else
            {
                events = new WorkflowEvent[workflowStates.Length - 1];
                for (var i = 0; i < workflowStates.Length - 1; i++)
                {
                    events[i] = new WorkflowEvent(workflowStates[i].ToString(), workflowStates[i + 1].ToString(), "comment", "user", DateTime.UtcNow);
                }
            }

            var workflow = Mock.Of<IWorkflow>(x =>
                x.GetHistory(item.Object) == events
            );

            var initialStateId = ID.NewID.ToString();
            if (workflowStates.Length > 0)
                initialStateId = workflowStates[workflowStates.Length - 1].ToString();

            var workflowState = new WorkflowState(initialStateId, "state", "icon", true);

            var itemState = new Mock<ItemState>(item.Object);
            itemState.Setup(x => x.GetWorkflow()).Returns(workflow);
            itemState.Setup(x => x.GetWorkflowState()).Returns(workflowState);

            item.Setup(x => x.State).Returns(itemState.Object);

            return events;
        }
    }
}
