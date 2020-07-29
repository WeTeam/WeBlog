using Moq;
using NUnit.Framework;
using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.Data.Templates;
using Sitecore.Modules.WeBlog.Configuration;
using System;

namespace Sitecore.Modules.WeBlog.UnitTest.Configuration
{
    [TestFixture]
    public class BlogSettingsResolverFixture
    {
        [Test]
        public void Ctor_TemplateManagerIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new BlogSettingsResolver(null, Mock.Of<IWeBlogSettings>());

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(new TestDelegate(sutAction));
            Assert.That(ex.ParamName, Is.EqualTo("templateManager"));
        }

        [Test]
        public void Ctor_WeBlogSettingsIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new BlogSettingsResolver(Mock.Of<BaseTemplateManager>(), null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(new TestDelegate(sutAction));
            Assert.That(ex.ParamName, Is.EqualTo("weBlogSettings"));
        }

        [Test]
        public void Resolve_ItemIsNull_ReturnsSettingsBasedOnWeBlogSettings()
        {
            // arrange
            // arrange
            var categoryTemplateId = ID.NewID;
            var entryTemplateId = ID.NewID;
            var commentTemplateId = ID.NewID;

            var weBlogsettings = Mock.Of<IWeBlogSettings>(x =>
                x.CategoryTemplateIds == new[] { categoryTemplateId } &&
                x.EntryTemplateIds == new[] { entryTemplateId } &&
                x.CommentTemplateIds == new[] { commentTemplateId }
            );

            var sut = new BlogSettingsResolver(Mock.Of<BaseTemplateManager>(), weBlogsettings);

            // act
            var result = sut.Resolve(null);

            // assert
            Assert.That(result.CategoryTemplateID, Is.EqualTo(categoryTemplateId));
            Assert.That(result.EntryTemplateID, Is.EqualTo(entryTemplateId));
            Assert.That(result.CommentTemplateID, Is.EqualTo(commentTemplateId));
        }

        [Test]
        public void Resolve_ItemNotBasedOnBlogTemplate_ReturnsSettingsBasedOnWeBlogSettings()
        {
            // arrange
            var categoryTemplateId = ID.NewID;
            var entryTemplateId = ID.NewID;
            var commentTemplateId = ID.NewID;

            var item = ItemFactory.CreateItem().Object;

            var weBlogsettings = Mock.Of<IWeBlogSettings>(x =>
                x.CategoryTemplateIds == new[] { categoryTemplateId } &&
                x.EntryTemplateIds == new[] { entryTemplateId } &&
                x.CommentTemplateIds == new[] { commentTemplateId }
            );

            var sut = new BlogSettingsResolver(Mock.Of<BaseTemplateManager>(), weBlogsettings);

            // act
            var result = sut.Resolve(item);

            // assert
            Assert.That(result.CategoryTemplateID, Is.EqualTo(categoryTemplateId));
            Assert.That(result.EntryTemplateID, Is.EqualTo(entryTemplateId));
            Assert.That(result.CommentTemplateID, Is.EqualTo(commentTemplateId));
        }

        [Test]
        public void Resolve_ItemBasedOnBlogTemplate_ReturnsSettingsFromBlog()
        {
            // arrange
            var categoryTemplateId = ID.NewID;
            var entryTemplateId = ID.NewID;
            var commentTemplateId = ID.NewID;
            var blogTemplateId = ID.NewID;

            var item = ItemFactory.CreateItem(templateId: blogTemplateId);

            var categoryField = FieldFactory.CreateField(item.Object, ID.NewID, "Defined Category Template", categoryTemplateId.ToString());
            var entryField = FieldFactory.CreateField(item.Object, ID.NewID, "Defined Entry Template", entryTemplateId.ToString());
            var commentField = FieldFactory.CreateField(item.Object, ID.NewID, "Defined Comment Template", commentTemplateId.ToString());
            ItemFactory.AddFields(item, new[] { categoryField, entryField, commentField });

            var templates = new TemplateCollection();
            var blogTemplate = TemplateFactory.CreateTemplate(blogTemplateId, null, templates);
            var templateManager = TemplateFactory.CreateTemplateManager(new[] { blogTemplate });

            var weBlogsettings = Mock.Of<IWeBlogSettings>(x =>
                x.CategoryTemplateIds == new[] { ID.NewID } &&
                x.EntryTemplateIds == new[] { ID.NewID } &&
                x.CommentTemplateIds == new[] { ID.NewID } &&
                x.BlogTemplateIds == new[] { blogTemplateId }
            );

            var sut = new BlogSettingsResolver(templateManager, weBlogsettings);

            // act
            var result = sut.Resolve(item.Object);

            // assert
            Assert.That(result.CategoryTemplateID, Is.EqualTo(categoryTemplateId));
            Assert.That(result.EntryTemplateID, Is.EqualTo(entryTemplateId));
            Assert.That(result.CommentTemplateID, Is.EqualTo(commentTemplateId));
        }

        [Test]
        public void Resolve_ItemBasedOnDerivedBlogTemplate_ReturnsSettingsFromBlog()
        {
            // arrange
            var categoryTemplateId = ID.NewID;
            var entryTemplateId = ID.NewID;
            var commentTemplateId = ID.NewID;
            var blogTemplateId = ID.NewID;
            var baseBlogTemplateId = ID.NewID;

            var templates = new TemplateCollection();
            var baseTemplate = TemplateFactory.CreateTemplate(baseBlogTemplateId, null, templates);
            var blogTemplate = TemplateFactory.CreateTemplate(blogTemplateId, baseBlogTemplateId, templates);
            var templateManager = TemplateFactory.CreateTemplateManager(new[] { blogTemplate });

            var item = ItemFactory.CreateItem(templateId: blogTemplateId);

            var categoryField = FieldFactory.CreateField(item.Object, ID.NewID, "Defined Category Template", categoryTemplateId.ToString());
            var entryField = FieldFactory.CreateField(item.Object, ID.NewID, "Defined Entry Template", entryTemplateId.ToString());
            var commentField = FieldFactory.CreateField(item.Object, ID.NewID, "Defined Comment Template", commentTemplateId.ToString());
            ItemFactory.AddFields(item, new[] { categoryField, entryField, commentField });

            var weBlogsettings = Mock.Of<IWeBlogSettings>(x =>
                x.CategoryTemplateIds == new[] { ID.NewID } &&
                x.EntryTemplateIds == new[] { ID.NewID } &&
                x.CommentTemplateIds == new[] { ID.NewID } &&
                x.BlogTemplateIds == new[] { baseBlogTemplateId }
            );

            var sut = new BlogSettingsResolver(templateManager, weBlogsettings);

            // act
            var result = sut.Resolve(item.Object);

            // assert
            Assert.That(result.CategoryTemplateID, Is.EqualTo(categoryTemplateId));
            Assert.That(result.EntryTemplateID, Is.EqualTo(entryTemplateId));
            Assert.That(result.CommentTemplateID, Is.EqualTo(commentTemplateId));
        }
    }
}
