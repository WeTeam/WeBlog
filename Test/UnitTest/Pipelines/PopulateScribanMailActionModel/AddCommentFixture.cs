using NUnit.Framework;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.PopulateScribanMailActionModel
{
    [TestFixture]
    public class AddCommentFixture
    {
        [Test]
        public void Process_CommentItemIsNull_AddsNothing()
        {
            // arrange
            var dataItemMock = ItemFactory.CreateItem();
            var sut = new AddComment();
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs(dataItemMock.Object);
            var args = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);

            // act
            sut.Process(args);

            // assert
            var commentExists = args.ModelContains(AddComment.ModelKey);
            Assert.That(commentExists, Is.False);
        }

        [Test]
        public void Process_CommentItemIsNotNull_AddsComment()
        {
            // arrange
            var dataItemMock = ItemFactory.CreateItem();
            var sut = new AddComment();
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs(dataItemMock.Object);
            var args = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            args.CommentItem = new CommentItem(dataItemMock.Object);

            // act
            sut.Process(args);

            // assert
            var comment = args.GetModel(AddComment.ModelKey);
            Assert.That(((CommentItem)comment).ID, Is.EqualTo(dataItemMock.Object.ID));
        }
    }
}
