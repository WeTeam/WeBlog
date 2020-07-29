using DocumentFormat.OpenXml.Office.CustomUI;
using Moq;
using NUnit.Framework;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.UnitTest.Text
{
    [TestFixture]
    public class ContextTokenReplacerFixture
    {
        [Test]
        public void Ctor_CategoryManagerIsNull_ThrowsException()
        {
            // arrange
            TestDelegate sutAction = () => new ContextTokenReplacer(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.That(ex.ParamName, Is.EqualTo("categoryManager"));
        }

        [TestCase(null)]
        [TestCase("")]
        public void ContainsToken_TextIsNullOrEmpty_ReturnsFalse(string input)
        {
            // arrange
            var sut = new ContextTokenReplacer(Mock.Of<ICategoryManager>());

            // act
            var result = sut.ContainsToken(input);

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ContainsToken_TextDoesNotContainToken_ReturnsFalse()
        {
            // arrange
            var sut = new ContextTokenReplacer(Mock.Of<ICategoryManager>());

            // act
            var result = sut.ContainsToken("lorem ipsum");

            // assert
            Assert.That(result, Is.False);
        }

        [TestCase("query:.././/*[@@templateid='$weblogcontext(EntryTemplateID)']")]
        [TestCase("$weblogcontext(categoryroot)")]
        public void ContainsToken_TextContainsToken_ReturnsTrue(string input)
        {
            // arrange
            var sut = new ContextTokenReplacer(Mock.Of<ICategoryManager>());

            // act
            var result = sut.ContainsToken(input);

            // assert
            Assert.That(result, Is.True);
        }

        [TestCase(null)]
        [TestCase("")]
        public void Replace_TextIsEmptyOrNull_ReturnsEmptyString(string input)
        {
            // arrange
            var sut = new ContextTokenReplacer(Mock.Of<ICategoryManager>());
            var contextItem = ItemFactory.CreateItem().Object;

            // act
            var result = sut.Replace(input, contextItem);

            // assert
            Assert.That(result, Is.Empty);
        }

        [TestCase("$other(nothing)")]
        [TestCase("blank")]
        public void Replace_SettingsTokenNotPresent_ReturnsTextUnaltered(string input)
        {
            // arrange
            var sut = new ContextTokenReplacer(Mock.Of<ICategoryManager>());
            var contextItem = ItemFactory.CreateItem().Object;

            // act
            var result = sut.Replace(input, contextItem);

            // assert
            Assert.That(result, Is.EqualTo(input));
        }

        [Test]
        public void Replace_InputContainsCategoryRoot_ReturnsCategoryRootPath()
        {
            // arrange
            var categoryRootItem = ItemFactory.CreateItem();
            ItemFactory.SetPath(categoryRootItem, "/path");
            
            var categoryManager = Mock.Of<ICategoryManager>(x =>
                x.GetCategoryRoot(It.IsAny<Sitecore.Data.Items.Item>()) == categoryRootItem.Object
            );

            var input = "$weblogcontext(categoryroot)";
            var sut = new ContextTokenReplacer(categoryManager);
            var contextItem = ItemFactory.CreateItem().Object;

            // act
            var result = sut.Replace(input, contextItem);

            // assert
            Assert.That(result, Is.EqualTo("/path"));
        }

        [Test]
        public void Replace_InputContainsCategoryRootWithDifferentCasing_ReturnsCategoryRootPath()
        {
            // arrange
            var categoryRootItem = ItemFactory.CreateItem();
            ItemFactory.SetPath(categoryRootItem, "/path");

            var categoryManager = Mock.Of<ICategoryManager>(x =>
                x.GetCategoryRoot(It.IsAny<Sitecore.Data.Items.Item>()) == categoryRootItem.Object
            );

            var input = "$WeBlogContext(CategoryRoot)";
            var sut = new ContextTokenReplacer(categoryManager);
            var contextItem = ItemFactory.CreateItem().Object;

            // act
            var result = sut.Replace(input, contextItem);

            // assert
            Assert.That(result, Is.EqualTo("/path"));
        }

        [Test]
        public void Replace_InputContainsUnknownToken_ReturnsInputUnchanged()
        {
            // arrange
            var categoryRootItem = ItemFactory.CreateItem();
            ItemFactory.SetPath(categoryRootItem, "/path");

            var categoryManager = Mock.Of<ICategoryManager>(x =>
                x.GetCategoryRoot(It.IsAny<Sitecore.Data.Items.Item>()) == categoryRootItem.Object
            );

            var input = "$weblogcontext(bler)";
            var sut = new ContextTokenReplacer(categoryManager);
            var contextItem = ItemFactory.CreateItem().Object;

            // act
            var result = sut.Replace(input, contextItem);

            // assert
            Assert.That(result, Is.EqualTo("$weblogcontext(bler)"));
        }

        [Test]
        public void Replace_TokenInsideOtherText_ReturnsCategoryRootPath()
        {
            // arrange
            var categoryRootItem = ItemFactory.CreateItem();
            ItemFactory.SetPath(categoryRootItem, "/path");

            var categoryManager = Mock.Of<ICategoryManager>(x =>
                x.GetCategoryRoot(It.IsAny<Sitecore.Data.Items.Item>()) == categoryRootItem.Object
            );

            var input = "head:.$weblogcontext(categoryroot)/tail";
            var sut = new ContextTokenReplacer(categoryManager);
            var contextItem = ItemFactory.CreateItem().Object;

            // act
            var result = sut.Replace(input, contextItem);

            // assert
            Assert.That(result, Is.EqualTo("head:./path/tail"));
        }

        [Test]
        public void Replace_InputContainsMultipleTokens_ReturnsWithTokensReplaced()
        {
            // arrange
            var categoryRootItem = ItemFactory.CreateItem();
            ItemFactory.SetPath(categoryRootItem, "/path");

            var categoryManager = Mock.Of<ICategoryManager>(x =>
                x.GetCategoryRoot(It.IsAny<Sitecore.Data.Items.Item>()) == categoryRootItem.Object
            );

            var input = " '$weblogcontext(categoryroot)'  '$weblogcontext(categoryroot)' ";
            var sut = new ContextTokenReplacer(categoryManager);
            var contextItem = ItemFactory.CreateItem().Object;

            // act
            var result = sut.Replace(input, contextItem);

            // assert
            Assert.That(result, Is.EqualTo(" '/path'  '/path' "));
        }
    }
}
