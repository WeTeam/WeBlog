using Joel.Net;
using Moq;
using NUnit.Framework;
using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Pipelines;
using Sitecore.Modules.WeBlog.Pipelines.CreateComment;
using Sitecore.Workflows;

#if SC93
using Sitecore.Links.UrlBuilders;
#endif

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.CreateComment
{
    [TestFixture]
    public class AkismetSpamCheckFixture
    {
        [Test]
        public void Process_MissingApiKey_AkismetApiNotCalled()
        {
            // arrange
            var settings = Mock.Of<IWeBlogSettings>(x =>
                x.CommentWorkflowCommandSpam == "spamcommand"
            );
            var workflowProvider = CreateWorkflowProvider();
            var (sut, args, akismetApiMock) = CreateAkismetSpamCheck(settings, workflowProvider);

            // act
            sut.Process(args);

            // assert
            akismetApiMock.Verify(x => x.CommentCheck(It.IsAny<AkismetComment>()), Times.Never);
        }

        [Test]
        public void Process_MissingWorkflowCommand_AkismetApiNotCalled()
        {
            // arrange
            var settings = Mock.Of<IWeBlogSettings>(x =>
                x.AkismetAPIKey == "apikey"
            );
            var workflowProvider = CreateWorkflowProvider();
            var (sut, args, akismetApiMock) = CreateAkismetSpamCheck(settings, workflowProvider);

            // act
            sut.Process(args);

            // assert
            akismetApiMock.Verify(x => x.CommentCheck(It.IsAny<AkismetComment>()), Times.Never);
        }

        [Test]
        public void Process_WorkflowNotFound_AkismetApiNotCalled()
        {
            // arrange
            var settings = CreateSettings();
            var workflowProvider = Mock.Of<IWorkflowProvider>();
            var (sut, args, akismetApiMock) = CreateAkismetSpamCheck(settings, workflowProvider);

            // act
            sut.Process(args);

            // assert
            akismetApiMock.Verify(x => x.CommentCheck(It.IsAny<AkismetComment>()), Times.Never);
        }

        [Test]
        public void Process_WhenCalled_InitializesAkismetApi()
        {
            // arrange
            var settings = CreateSettings();
            var workflowProvider = CreateWorkflowProvider();
            var (sut, args, akismetApiMock) = CreateAkismetSpamCheck(settings, workflowProvider);

            // act
            sut.Process(args);

            // assert
            akismetApiMock.Verify(x => x.Init("apikey", "link", "WeBlog/4.1.0.0"));
        }

        [Test]
        public void Process_CommentIsSpam_ExecuteSpamWorkflowCommand()
        {
            // arrange
            var settings = CreateSettings();
            var workflow = new Mock<IWorkflow>();
            var workflowProvider = Mock.Of<IWorkflowProvider>(x =>
               x.GetWorkflow(It.IsAny<Item>()) == workflow.Object
            );

            var (sut, args, akismetApiMock) = CreateAkismetSpamCheck(settings, workflowProvider);
            akismetApiMock.Setup(x => x.CommentCheck(It.IsAny<AkismetComment>())).Returns(true);

            // act
            sut.Process(args);

            // assert
            workflow.Verify(x => x.Execute(settings.CommentWorkflowCommandSpam, args.CommentItem, It.IsAny<string>(), false, It.IsAny<object[]>()));
        }

        [Test]
        public void Process_CommentIsNotSpam_DoNotExecuteSpamWorkflowCommand()
        {
            // arrange
            var settings = CreateSettings();
            var workflow = new Mock<IWorkflow>();
            var workflowProvider = Mock.Of<IWorkflowProvider>(x =>
               x.GetWorkflow(It.IsAny<Item>()) == workflow.Object
            );

            var (sut, args, akismetApiMock) = CreateAkismetSpamCheck(settings, workflowProvider);
            akismetApiMock.Setup(x => x.CommentCheck(It.IsAny<AkismetComment>())).Returns(false);

            // act
            sut.Process(args);

            // assert
            workflow.Verify(x => x.Execute(It.IsAny<string>(), args.CommentItem, It.IsAny<string>(), false, It.IsAny<object[]>()), Times.Never);
        }

        [Test]
        public void Process_CommentIsSpam_AbortsPipeline()
        {
            // arrange
            var settings = CreateSettings();
            var workflowProvider = CreateWorkflowProvider();

            var (sut, args, akismetApiMock) = CreateAkismetSpamCheck(settings, workflowProvider);
            akismetApiMock.Setup(x => x.CommentCheck(It.IsAny<AkismetComment>())).Returns(true);

            // act
            sut.Process(args);

            // assert
            Assert.That(args.Aborted, Is.True);
        }

        private (AkismetSpamCheck processor, CreateCommentArgs args, Mock<IAkismet> akismetApiMock) CreateAkismetSpamCheck(IWeBlogSettings settings, IWorkflowProvider workflowProvider)
        {
            var linkManager = Mock.Of<BaseLinkManager>(x =>
#if SC93
                x.GetItemUrl(It.IsAny<Item>(), It.Is<ItemUrlBuilderOptions>(y => y.AlwaysIncludeServerUrl == true)) == "link"
#else
                x.GetItemUrl(It.IsAny<Item>(), It.Is<UrlOptions>(y => y.AlwaysIncludeServerUrl == true)) == "link"
#endif
            );

            var blogItem = ItemFactory.CreateItem().Object;

            var blogManager = Mock.Of<IBlogManager>(x =>
                x.GetCurrentBlog() == new BlogHomeItem(blogItem, settings)
            );

            var akismetApiMock = new Mock<IAkismet>();

            var processor = new AkismetSpamCheck(settings, blogManager, akismetApiMock.Object, linkManager);

            var item = CreateCommentItem();
            var database = Mock.Of<Database>(x =>
                x.WorkflowProvider == workflowProvider
            );

            var args = new CreateCommentArgs();
            args.CommentItem = new CommentItem(item);
            args.Database = database;

            return (processor, args, akismetApiMock);
        }

        private IWeBlogSettings CreateSettings()
        {
            return Mock.Of<IWeBlogSettings>(x =>
                x.AkismetAPIKey == "apikey" &&
                x.CommentWorkflowCommandSpam == "spamcommand"
            );
        }

        private IWorkflowProvider CreateWorkflowProvider()
        {
            return Mock.Of<IWorkflowProvider>(x =>
                x.GetWorkflow(It.IsAny<Item>()) == Mock.Of<IWorkflow>()
            );
        }

        private CommentItem CreateCommentItem()
        {
            var itemMock = ItemFactory.CreateItem();

            ItemFactory.AddFields(itemMock, new[]
            {
                FieldFactory.CreateField(itemMock.Object, ID.NewID, "IP Address", "127.0.0.1"),
                FieldFactory.CreateField(itemMock.Object, ID.NewID, "Comment", "comment"),
                FieldFactory.CreateField(itemMock.Object, ID.NewID, "Name", "name"),
                FieldFactory.CreateField(itemMock.Object, ID.NewID, "Email", "email"),
                FieldFactory.CreateField(itemMock.Object, ID.NewID, "Website", "website")
            });

            return new CommentItem(itemMock.Object);
        }
    }
}
