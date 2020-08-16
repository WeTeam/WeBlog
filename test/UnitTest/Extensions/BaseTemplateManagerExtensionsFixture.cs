using NUnit.Framework;
using Sitecore.Data;
using Sitecore.Data.Templates;
using Sitecore.Modules.WeBlog.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.Modules.WeBlog.UnitTest.Extensions
{
    [TestFixture]
    public class BaseTemplateManagerExtensionsFixture
    {
        [Test]
        public void TemplateIsOrBasedOn_ItemIsNull_ReturnsFalse()
        {
            // arrange
            var templateId = ID.NewID;
            var templateManager = TemplateFactory.CreateTemplateManager(templateId);

            // act
            var result = templateManager.TemplateIsOrBasedOn(null, new[] { templateId });

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TemplateIsOrBasedOn_TemplatesIsNull_ReturnsFalse()
        {
            // arrange
            var templateId = ID.NewID;
            var templateManager = TemplateFactory.CreateTemplateManager(templateId);
            var itemMock = ItemFactory.CreateItem(templateId);

            // act
            var result = templateManager.TemplateIsOrBasedOn(itemMock.Object, (IEnumerable<ID>)null);

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TemplateIsOrBasedOn_TemplatesIsEmpty_ReturnsFalse()
        {
            // arrange
            var templateId = ID.NewID;
            var templateManager = TemplateFactory.CreateTemplateManager(templateId);
            var itemMock = ItemFactory.CreateItem(templateId);

            // act
            var result = templateManager.TemplateIsOrBasedOn(itemMock.Object, Enumerable.Empty<ID>());

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TemplateIsOrBasedOn_InvalidTemplate_ReturnsFalse()
        {
            // arrange
            var templateId = ID.NewID;
            var templateManager = TemplateFactory.CreateTemplateManager(templateId);
            var itemMock = ItemFactory.CreateItem(templateId);

            // act
            var result = templateManager.TemplateIsOrBasedOn(itemMock.Object, new[] { ID.NewID });

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TemplateIsOrBasedOn_TemplateMatches_ReturnsTrue()
        {
            // arrange
            var templateId = ID.NewID;
            var templateManager = TemplateFactory.CreateTemplateManager(templateId);
            var itemMock = ItemFactory.CreateItem(templateId: templateId);

            // act
            var result = templateManager.TemplateIsOrBasedOn(itemMock.Object, new[] { templateId });

            // assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void TemplateIsOrBasedOnSingle_TemplateMatches_ReturnsTrue()
        {
            // arrange
            var templateId = ID.NewID;
            var templateManager = TemplateFactory.CreateTemplateManager(templateId);
            var itemMock = ItemFactory.CreateItem(templateId: templateId);

            // act
            var result = templateManager.TemplateIsOrBasedOn(itemMock.Object, templateId);

            // assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void TemplateIsOrBasedOn_TemplateDoesNotMatch_ReturnsFalse()
        {
            // arrange
            var templateId1 = ID.NewID;
            var templateId2 = ID.NewID;
            var templateManager = TemplateFactory.CreateTemplateManager(templateId1, templateId2);
            var itemMock = ItemFactory.CreateItem(templateId2);

            // act
            var result = templateManager.TemplateIsOrBasedOn(itemMock.Object, new[] { templateId1 });

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TemplateIsOrBasedOnSingle_TemplateDoesNotMatch_ReturnsFalse()
        {
            // arrange
            var templateId1 = ID.NewID;
            var templateId2 = ID.NewID;
            var templateManager = TemplateFactory.CreateTemplateManager(templateId1, templateId2);
            var itemMock = ItemFactory.CreateItem(templateId2);

            // act
            var result = templateManager.TemplateIsOrBasedOn(itemMock.Object, templateId1);

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TemplateIsOrBasedOn_DerivedTemplateMatches_ReturnsTrue()
        {
            // arrange
            var baseTemplateId = ID.NewID;
            var templateId = ID.NewID;

            var templates = new TemplateCollection();
            var baseTemplate = TemplateFactory.CreateTemplate(baseTemplateId, null, templates);
            var template = TemplateFactory.CreateTemplate(templateId, baseTemplateId, templates);

            var templateManager = TemplateFactory.CreateTemplateManager(new[] { baseTemplate, template });
            var itemMock = ItemFactory.CreateItem(templateId: templateId);

            // act
            var result = templateManager.TemplateIsOrBasedOn(itemMock.Object, new[] { baseTemplateId });

            // assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void TemplateIsOrBasedOn_DerivedTemplateChainMatches_ReturnsTrue()
        {
            // arrange
            var baseTemplateId1 = ID.NewID;
            var baseTemplateId2 = ID.NewID;
            var baseTemplateId3 = ID.NewID;
            var templateId = ID.NewID;

            var templates = new TemplateCollection();
            var baseTemplate1 = TemplateFactory.CreateTemplate(baseTemplateId1, null, templates);
            var baseTemplate2 = TemplateFactory.CreateTemplate(baseTemplateId2, baseTemplateId1, templates);
            var baseTemplate3 = TemplateFactory.CreateTemplate(baseTemplateId3, baseTemplateId2, templates);
            var template = TemplateFactory.CreateTemplate(templateId, baseTemplateId3, templates);

            var templateManager = TemplateFactory.CreateTemplateManager(new[] { baseTemplate1, baseTemplate2, baseTemplate3, template });
            var itemMock = ItemFactory.CreateItem(templateId: templateId);

            // act
            var result = templateManager.TemplateIsOrBasedOn(itemMock.Object, new[] { baseTemplateId1 });

            // assert
            Assert.That(result, Is.True);
        }
    }
}
