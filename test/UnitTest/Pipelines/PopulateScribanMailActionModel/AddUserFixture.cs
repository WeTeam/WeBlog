using NUnit.Framework;
using Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel;
using Sitecore.Security.Accounts;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.PopulateScribanMailActionModel
{
    [TestFixture]
    public class AddUserFixture
    {
        [Test]
        public void Process_WhenCalled_AddsUser()
        {
            // arrange
            var user = User.FromName("sitecore\\user", false);
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            var args = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            var sut = new AddUser(user);

            // act
            sut.Process(args);

            // assert
            var value = args.GetModel(AddUser.ModelKey);
            Assert.That(value, Is.EqualTo(user));
        }
    }
}
