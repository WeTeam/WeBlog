using Moq;
using NUnit.Framework;
using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.ExperienceEditor;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Mvc.Presentation;
using System;

namespace Sitecore.Modules.WeBlog.UnitTest.ExperienceEditor
{
    [TestFixture]
    public class NewEntryFixture
    {
        [Test]
        public void Ctor_BlogManagerIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new NewEntry(null, Mock.Of<BaseItemManager>(), Mock.Of<IBlogSettingsResolver>());

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(new TestDelegate(sutAction));
            Assert.That(ex.ParamName, Is.EqualTo("blogManager"));
        }

        [Test]
        public void Ctor_ItemManagerIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new NewEntry(Mock.Of<IBlogManager>(), null, Mock.Of<IBlogSettingsResolver>());

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(new TestDelegate(sutAction));
            Assert.That(ex.ParamName, Is.EqualTo("itemManager"));
        }

        [Test]
        public void Ctor_BlogSettingsResolverIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new NewEntry(Mock.Of<IBlogManager>(), Mock.Of<BaseItemManager>(), null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(new TestDelegate(sutAction));
            Assert.That(ex.ParamName, Is.EqualTo("blogSettingsResolver"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("invalid/name")]
        public void ProcessRequest_InvalidItemName_ReturnsEmptyResult(string itemName)
        {
            // arrange
            var blogManager = Mock.Of<IBlogManager>();
            var itemManager = Mock.Of<BaseItemManager>();
            var sut = new NewEntry(blogManager, itemManager, Mock.Of<IBlogSettingsResolver>());

            sut.RequestContext = new ItemContext
            {
                Argument = itemName
            };

            // act
            var result = sut.ProcessRequest();

            // assert
            Assert.That(result.Value, Is.Null);
        }

        [Test]
        public void ProcessRequest_ContextItemNotUnderBlog_DoesNotCreateAnyItems()
        {
            // arrange
            var contextItem = ItemFactory.CreateItem().Object;

            var blogManager = Mock.Of<IBlogManager>();
            var itemManager = new Mock<BaseItemManager>();
            var sut = new NewEntry(blogManager, itemManager.Object, Mock.Of<IBlogSettingsResolver>());

            var context = new ItemContext
            {
                Argument = "entry"
            };

            context.PopulateContextData(contextItem, Mock.Of<RenderingParametersResolver>());

            sut.RequestContext = context;

            // act
            var result = sut.ProcessRequest();

            // assert
            itemManager.Verify(x => x.AddFromTemplate(It.IsAny<string>(), It.IsAny<ID>(), It.IsAny<Item>()), Times.Never);
        }
        
        [Test]
        public void ProcessRequest_ContextItemUnderBlog_CreatesEntryItem()
        {
            // arrange
            var entryTemplateId = ID.NewID;
            var contextItem = ItemFactory.CreateItem().Object;
            var blogItem = ItemFactory.CreateItem().Object;
            var weblogSettings = Mock.Of<IWeBlogSettings>(x => x.EntryTemplateIds == new[] { entryTemplateId });

            var blogManager = Mock.Of<IBlogManager>(x =>
                x.GetCurrentBlog(contextItem) == (BlogHomeItem)blogItem
            );

            var newItem = ItemFactory.CreateItem().Object;

            var itemManager = new Mock<BaseItemManager>();
            itemManager.Setup(x => x.AddFromTemplate("entry", entryTemplateId, blogItem)).Returns(newItem).Verifiable();

            var settingsResolver = Mock.Of<IBlogSettingsResolver>(x => x.Resolve(It.IsAny<BlogHomeItem>()) == new BlogSettings(weblogSettings));
            var sut = new NewEntry(blogManager, itemManager.Object, settingsResolver);

            var context = new ItemContext
            {
                Argument = "entry"
            };

            context.PopulateContextData(contextItem, Mock.Of<RenderingParametersResolver>());

            sut.RequestContext = context;

            // act
            var result = sut.ProcessRequest();

            // assert
            itemManager.Verify();
            var id = GetItemIdFromAnonymousResult(result.Value);

            Assert.That(id, Is.EqualTo(newItem.ID.Guid));
        }

        private Guid GetItemIdFromAnonymousResult(object ob)
        {
            var type = ob.GetType();
            var propInfo = type.GetProperty("itemId");
            return (Guid)propInfo.GetValue(ob);
        }
    }
}
