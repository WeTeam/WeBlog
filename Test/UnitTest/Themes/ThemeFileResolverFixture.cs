using NUnit.Framework;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Templates;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Themes;
using System;
using System.Linq;

namespace Sitecore.Modules.WeBlog.UnitTest.Themes
{
    [TestFixture]
    public class ThemeFileResolverFixture
    {
        [Test]
        public void Ctor_LinkManagerIsNull_Throws()
        {
            // arrange
            Action sutAction = () => new ThemeFileResolver(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(new TestDelegate(sutAction));
            Assert.That(ex.ParamName, Is.EqualTo("templateManager"));
        }

        [Test]
        public void Resolve_ThemeIsNull_ReturnsEmptyThemeFiles()
        {
            // arrange
            var templateManager = TemplateFactory.CreateTemplateManager(StylesheetItem.TemplateId);
            var sut = new ThemeFileResolver(templateManager);

            // act
            var result = sut.Resolve(null);

            // assert
            Assert.That(result.Stylesheets, Is.Empty);
            Assert.That(result.Scripts, Is.Empty);
        }

        [Test]
        public void Resolve_ThemeHasNoChildren_ReturnsEmptyThemeFiles()
        {
            // arrange
            var templateManager = TemplateFactory.CreateTemplateManager(StylesheetItem.TemplateId);
            var item = ItemFactory.CreateItem().Object;

            var sut = new ThemeFileResolver(templateManager);

            // act
            var result = sut.Resolve(item);

            // assert
            Assert.That(result.Stylesheets, Is.Empty);
            Assert.That(result.Scripts, Is.Empty);
        }

        [Test]
        public void Resolve_UnknownChildrenUnderTheme_ReturnsEmptyThemeFiles()
        {
            // arrange
            var templateManager = TemplateFactory.CreateTemplateManager(StylesheetItem.TemplateId);
            var stylesheetItemMock = ItemFactory.CreateItem();

            var itemMock = ItemFactory.CreateItem();
            var children = new ChildList(itemMock.Object, new[] { stylesheetItemMock.Object });
            itemMock.Setup(x => x.GetChildren()).Returns(children);

            var sut = new ThemeFileResolver(templateManager);

            // act
            var result = sut.Resolve(itemMock.Object);

            // assert
            Assert.That(result.Stylesheets, Is.Empty);
            Assert.That(result.Scripts, Is.Empty);
        }

        [Test]
        public void Resolve_StylesheetUnderTheme_IncludesStylesheet()
        {
            // arrange
            var templateManager = TemplateFactory.CreateTemplateManager(StylesheetItem.TemplateId);
            var stylesheetItemMock = ItemFactory.CreateItem(templateId: StylesheetItem.TemplateId);
            ItemFactory.SetIndexerField(stylesheetItemMock, StylesheetItem.Fields.Url, "/url");

            var itemMock = ItemFactory.CreateItem();
            var children = new ChildList(itemMock.Object, new[] { stylesheetItemMock.Object });
            itemMock.Setup(x => x.GetChildren()).Returns(children);

            var sut = new ThemeFileResolver(templateManager);

            // act
            var result = sut.Resolve(itemMock.Object);

            // assert
            Assert.That(result.Scripts, Is.Empty);
            Assert.That(result.Stylesheets.Count(), Is.EqualTo(1));

            var stylesheet = result.Stylesheets.First();
            Assert.That(stylesheet.Url, Is.EqualTo("/url"));
        }

        [Test]
        public void Resolve_DerivedStylesheetUnderTheme_IncludesStylesheet()
        {
            // arrange
            var derivedTemplateId = ID.NewID;
            var templates = new TemplateCollection();
            var stylesheetTemplate = TemplateFactory.CreateTemplate(StylesheetItem.TemplateId, null, templates);
            var derivedStylesheetTemplate = TemplateFactory.CreateTemplate(derivedTemplateId, StylesheetItem.TemplateId, templates);

            var templateManager = TemplateFactory.CreateTemplateManager(new[] { stylesheetTemplate, derivedStylesheetTemplate });
            var stylesheetItemMock = ItemFactory.CreateItem(templateId: derivedTemplateId);
            ItemFactory.SetIndexerField(stylesheetItemMock, StylesheetItem.Fields.Url, "/url");

            var itemMock = ItemFactory.CreateItem();
            var children = new ChildList(itemMock.Object, new[] { stylesheetItemMock.Object });
            itemMock.Setup(x => x.GetChildren()).Returns(children);

            var sut = new ThemeFileResolver(templateManager);

            // act
            var result = sut.Resolve(itemMock.Object);

            // assert
            Assert.That(result.Scripts, Is.Empty);
            Assert.That(result.Stylesheets.Count(), Is.EqualTo(1));

            var stylesheet = result.Stylesheets.First();
            Assert.That(stylesheet.Url, Is.EqualTo("/url"));
        }

        [Test]
        public void Resolve_ScriptUnderTheme_IncludesSscript()
        {
            // arrange
            var templateManager = TemplateFactory.CreateTemplateManager(ScriptItem.TemplateId);
            var scriptItemMock = ItemFactory.CreateItem(templateId: ScriptItem.TemplateId);
            ItemFactory.SetIndexerField(scriptItemMock, FileItem.Fields.Url, "/url");
            ItemFactory.SetIndexerField(scriptItemMock, ScriptItem.ScriptItemFields.FallbackUrl, "/fallbackurl");
            ItemFactory.SetIndexerField(scriptItemMock, ScriptItem.ScriptItemFields.VerificationObject, "object");

            var itemMock = ItemFactory.CreateItem();
            var children = new ChildList(itemMock.Object, new[] { scriptItemMock.Object });
            itemMock.Setup(x => x.GetChildren()).Returns(children);

            var sut = new ThemeFileResolver(templateManager);

            // act
            var result = sut.Resolve(itemMock.Object);

            // assert
            Assert.That(result.Stylesheets, Is.Empty);
            Assert.That(result.Scripts.Count(), Is.EqualTo(1));

            var script = result.Scripts.First();
            Assert.That(script.Url, Is.EqualTo("/url"));
            Assert.That(script.FallbackUrl, Is.EqualTo("/fallbackurl"));
            Assert.That(script.VerificationObject, Is.EqualTo("object"));
        }

        [Test]
        public void Resolve_DerivedScriptUnderTheme_IncludesScript()
        {
            // arrange
            var derivedTemplateId = ID.NewID;
            var templates = new TemplateCollection();
            var scriptTemplate = TemplateFactory.CreateTemplate(ScriptItem.TemplateId, null, templates);
            var derivedScriptTemplate = TemplateFactory.CreateTemplate(derivedTemplateId, ScriptItem.TemplateId, templates);

            var templateManager = TemplateFactory.CreateTemplateManager(new[] { scriptTemplate, derivedScriptTemplate });
            var scriptItemMock = ItemFactory.CreateItem(templateId: derivedTemplateId);
            ItemFactory.SetIndexerField(scriptItemMock, FileItem.Fields.Url, "/url");
            ItemFactory.SetIndexerField(scriptItemMock, ScriptItem.ScriptItemFields.FallbackUrl, "/fallbackurl");
            ItemFactory.SetIndexerField(scriptItemMock, ScriptItem.ScriptItemFields.VerificationObject, "object");

            var itemMock = ItemFactory.CreateItem();
            var children = new ChildList(itemMock.Object, new[] { scriptItemMock.Object });
            itemMock.Setup(x => x.GetChildren()).Returns(children);

            var sut = new ThemeFileResolver(templateManager);

            // act
            var result = sut.Resolve(itemMock.Object);

            // assert
            Assert.That(result.Stylesheets, Is.Empty);
            Assert.That(result.Scripts.Count(), Is.EqualTo(1));

            var script = result.Scripts.First();
            Assert.That(script.Url, Is.EqualTo("/url"));
            Assert.That(script.FallbackUrl, Is.EqualTo("/fallbackurl"));
            Assert.That(script.VerificationObject, Is.EqualTo("object"));
        }

        [Test]
        public void Resolve_StylesheetsAndScriptsUnderTheme_IncludesReferences()
        {
            // arrange
            var templateManager = TemplateFactory.CreateTemplateManager(ScriptItem.TemplateId, StylesheetItem.TemplateId);

            var stylesheetItem1Mock = ItemFactory.CreateItem(templateId: StylesheetItem.TemplateId);
            ItemFactory.SetIndexerField(stylesheetItem1Mock, StylesheetItem.Fields.Url, "/url");

            var stylesheetItem2Mock = ItemFactory.CreateItem(templateId: StylesheetItem.TemplateId);
            ItemFactory.SetIndexerField(stylesheetItem2Mock, StylesheetItem.Fields.Url, "/url2");

            var scriptItemMock = ItemFactory.CreateItem(templateId: ScriptItem.TemplateId);
            ItemFactory.SetIndexerField(scriptItemMock, FileItem.Fields.Url, "/url");
            ItemFactory.SetIndexerField(scriptItemMock, ScriptItem.ScriptItemFields.FallbackUrl, "/fallbackurl");
            ItemFactory.SetIndexerField(scriptItemMock, ScriptItem.ScriptItemFields.VerificationObject, "object");

            var itemMock = ItemFactory.CreateItem();
            var children = new ChildList(itemMock.Object, new[] { scriptItemMock.Object, stylesheetItem1Mock.Object, stylesheetItem2Mock.Object });
            itemMock.Setup(x => x.GetChildren()).Returns(children);

            var sut = new ThemeFileResolver(templateManager);

            // act
            var result = sut.Resolve(itemMock.Object);

            // assert
            Assert.That(result.Stylesheets.Count(), Is.EqualTo(2));
            Assert.That(result.Scripts.Count(), Is.EqualTo(1));

            var stylesheet1 = result.Stylesheets.First();
            Assert.That(stylesheet1.Url, Is.EqualTo("/url"));

            var stylesheet2 = result.Stylesheets.ElementAt(1);
            Assert.That(stylesheet2.Url, Is.EqualTo("/url2"));

            var script = result.Scripts.First();
            Assert.That(script.Url, Is.EqualTo("/url"));
            Assert.That(script.FallbackUrl, Is.EqualTo("/fallbackurl"));
            Assert.That(script.VerificationObject, Is.EqualTo("object"));
        }
    }
}
