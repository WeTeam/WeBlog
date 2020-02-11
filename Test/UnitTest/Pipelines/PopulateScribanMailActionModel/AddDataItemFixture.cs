using NUnit.Framework;
using Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.PopulateScribanMailActionModel
{
    [TestFixture]
    public class AddDataItemFixture
    {
        [Test]
        public void Process_WhenCalled_AddsDataItem()
        {
            // arrange
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            var args = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            var sut = new AddDataItem();

            // act
            sut.Process(args);

            // assert
            var value = args.GetModel(AddDataItem.ModelKey);
            Assert.That(value, Is.SameAs(workflowPipelineArgs.DataItem));
        }
    }
}
