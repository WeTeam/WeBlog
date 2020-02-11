using Moq;
using NUnit.Framework;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.PopulateScribanMailActionModel
{
    [TestFixture]
    public class AddBlogFixture
    {
        [Test]
        public void Process_NoBlogs_DoesNotAddToModel()
        {
            // arrange
            var blogManager = Mock.Of<IBlogManager>();
            var sut = new AddBlog(blogManager);
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            var args = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);

            // act
            sut.Process(args);

            // assert
            var blogExists = args.ModelContains(AddBlog.ModelKey);
            Assert.That(blogExists, Is.False);
        }

        [Test]
        public void Process_BlogPresent_AddsBlog()
        {
            // arrange
            var blogItem = ItemFactory.CreateItem();
            var settings = Mock.Of<IWeBlogSettings>();

            var blogManager = Mock.Of<IBlogManager>(x =>
                x.GetCurrentBlog(It.IsAny<Item>()) == new BlogHomeItem(blogItem.Object, settings)
            );

            var sut = new AddBlog(blogManager);
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs(blogItem.Object);
            var args = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);

            // act
            sut.Process(args);

            // assert
            var value = args.GetModel(AddBlog.ModelKey);
            Assert.That(((BlogHomeItem)value).ID, Is.EqualTo(blogItem.Object.ID));
        }
    }
}
