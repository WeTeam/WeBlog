using Moq;
using NUnit.Framework;
using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel;
using System.Linq;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.PopulateScribanMailActionModel
{
    [TestFixture]
    public class ResolveItemsFixture
    {
        [Test]
        public void Process_DataItemNotEntryOrComment_ItemPropertiesRemainNull()
        {
            // arrange
            var (sut, dataItemMock, entryManager) = CreateResolveItems(ID.NewID, ID.NewID, ID.NewID);
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs(dataItemMock.Object);
            var args = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);

            // act
            sut.Process(args);

            // assert
            Assert.That(args.EntryItem, Is.Null);
            Assert.That(args.CommentItem, Is.Null);
        }

        [Test]
        public void Process_DataItemIsEntry_AddsEntry()
        {
            // arrange
            var entryTemplateId = ID.NewID;
            var (sut, dataItemMock, entryManager) = CreateResolveItems(entryTemplateId, entryTemplateId, ID.NewID);
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs(dataItemMock.Object);
            var args = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);

            // act
            sut.Process(args);

            // assert
            Assert.That(args.EntryItem.ID, Is.EqualTo(dataItemMock.Object.ID));
        }

        [Test]
        public void Process_DataItemIsComment_AddsCommentAndEntry()
        {
            // arrange
            var entryTemplateId = ID.NewID;
            var commentTemplateId = ID.NewID;

            var (sut, dataItemMock, entryManager) = CreateResolveItems(commentTemplateId, entryTemplateId, commentTemplateId);
            var entryItem = ItemFactory.CreateItem(templateId: entryTemplateId);
            dataItemMock.Setup(x => x.Parent).Returns(entryItem.Object);

            var commentUri = new ItemUri(dataItemMock.Object);
            dataItemMock.Setup(x => x.Uri).Returns(commentUri);

            entryManager.Setup(x => x.GetBlogEntryItemByCommentUri(commentUri)).Returns(entryItem.Object);

            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs(dataItemMock.Object);
            var args = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);

            // act
            sut.Process(args);

            // assert
            Assert.That(args.CommentItem.ID, Is.EqualTo(dataItemMock.Object.ID));
            Assert.That(args.EntryItem.ID, Is.EqualTo(entryItem.Object.ID));
        }

        [Test]
        public void Process_ItemsAlreadySet_DoesNotChangeItemProperties()
        {
            // arrange
            var entryTemplateId = ID.NewID;
            var commentTemplateId = ID.NewID;

            var (sut, dataItemMock, entryManager) = CreateResolveItems(commentTemplateId, entryTemplateId, commentTemplateId);
            var entryItem = ItemFactory.CreateItem(templateId: entryTemplateId);
            dataItemMock.Setup(x => x.Parent).Returns(entryItem.Object);

            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs(dataItemMock.Object);
            var args = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            
            var existingEntryItem = ItemFactory.CreateItem(templateId: entryTemplateId).Object;
            args.EntryItem = existingEntryItem;

            var existingCommentItem = ItemFactory.CreateItem(templateId: commentTemplateId).Object;
            args.CommentItem = existingCommentItem;

            // act
            sut.Process(args);

            // assert
            Assert.That(args.CommentItem.ID, Is.EqualTo(existingCommentItem.ID));
            Assert.That(args.EntryItem.ID, Is.EqualTo(existingEntryItem.ID));
        }

        private (ResolveItems sut, Mock<Item> dataItem, Mock<IEntryManager>) CreateResolveItems(ID dataItemTemplateId, ID entryTemplateId, ID commentTemplateId)
        {
            var settings = Mock.Of<IWeBlogSettings>(x =>
                x.EntryTemplateIds == new[] { entryTemplateId } &&
                x.CommentTemplateIds == new[] { commentTemplateId }
            );

            var dataItemMock = ItemFactory.CreateItem(templateId: dataItemTemplateId);

            var entryTemplateBuilder = new Template.Builder("entry", entryTemplateId, new TemplateCollection());
            var commentTemplateBuilder = new Template.Builder("comment", commentTemplateId, new TemplateCollection());

            var templateManager = Mock.Of<BaseTemplateManager>(x =>
                x.GetTemplate(settings.EntryTemplateIds.First(), dataItemMock.Object.Database) == entryTemplateBuilder.Template &&
                x.GetTemplate(settings.CommentTemplateIds.First(), dataItemMock.Object.Database) == commentTemplateBuilder.Template
            );

            var entryManager = new Mock<IEntryManager>();

            var sut = new ResolveItems(settings, templateManager, entryManager.Object);
            
            return (sut, dataItemMock, entryManager);
        }
    }
}
