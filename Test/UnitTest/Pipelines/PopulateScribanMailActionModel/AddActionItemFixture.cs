using NUnit.Framework;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.PopulateScribanMailActionModel
{
    [TestFixture]
    public class AddActionItemFixture
    {
        [Test]
        public void Process_WhenCalled_AddsActionDefinitionItem()
        {
            // arrange
            var actionItem = ItemFactory.CreateItem();
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            workflowPipelineArgs.ProcessorItem = actionItem.Object;

            var args = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            var sut = new AddActionItem();

            // act
            sut.Process(args);

            // assert
            var value = args.GetModel(AddActionItem.ModelKey);
            Assert.That(((ProcessorItem)value).ID, Is.EqualTo(actionItem.Object.ID));
        }
    }
}
