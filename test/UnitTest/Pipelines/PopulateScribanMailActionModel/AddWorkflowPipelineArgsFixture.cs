using NUnit.Framework;
using Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.PopulateScribanMailActionModel
{
    [TestFixture]
    public class AddWorkflowPipelineArgsFixture
    {
        [Test]
        public void Process_WhenCalled_AddsWorkflowPipelineArgs()
        {
            // arrange
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            var args = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            var sut = new AddWorkflowPipelineArgs();

            // act
            sut.Process(args);

            // assert
            var value = args.GetModel(AddWorkflowPipelineArgs.ModelKey);
            Assert.That(value, Is.SameAs(workflowPipelineArgs));
        }
    }
}
