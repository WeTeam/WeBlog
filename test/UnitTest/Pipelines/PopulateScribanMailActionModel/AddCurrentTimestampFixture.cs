using NUnit.Framework;
using Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel;
using System;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.PopulateScribanMailActionModel
{
    [TestFixture]
    public class AddCurrentTimestampFixture
    {
        [Test]
        public void Process_WhenCalled_AddsCurrentTimestamp()
        {
            // arrange
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            var args = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            var sut = new AddCurrentTimestamp();

            // act
            sut.Process(args);

            // assert
            var value = args.GetModel(AddCurrentTimestamp.ModelKey);
            Assert.That(value, Is.EqualTo(DateTime.Now).Within(TimeSpan.FromSeconds(1)));
        }
    }
}
