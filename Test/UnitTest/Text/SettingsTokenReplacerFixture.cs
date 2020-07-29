using Moq;
using NUnit.Framework;
using Sitecore.Data;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Text;
using System;

namespace Sitecore.Modules.WeBlog.UnitTest.Text
{
    [TestFixture]
    public class SettingsTokenReplacerFixture
    {
        [Test]
        public void Ctor_BlogManagerIsNull_ThrowsException()
        {
            // arrange
            TestDelegate sutAction = () => new SettingsTokenReplacer(null, Mock.Of<IBlogSettingsResolver>());

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.That(ex.ParamName, Is.EqualTo("blogManager"));
        }

        [Test]
        public void Ctor_SettingsResolverIsNull_ThrowsException()
        {
            // arrange
            TestDelegate sutAction = () => new SettingsTokenReplacer(Mock.Of<IBlogManager>(), null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.That(ex.ParamName, Is.EqualTo("blogSettingsResolver"));
        }

        [TestCase(null)]
        [TestCase("")]
        public void ContainsToken_TextIsNullOrEmpty_ReturnsFalse(string input)
        {
            // arrange
            var sut = new SettingsTokenReplacer(Mock.Of<IBlogManager>(), Mock.Of<IBlogSettingsResolver>());

            // act
            var result = sut.ContainsToken(input);

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ContainsToken_TextDoesNotContainToken_ReturnsFalse()
        {
            // arrange
            var sut = new SettingsTokenReplacer(Mock.Of<IBlogManager>(), Mock.Of<IBlogSettingsResolver>());

            // act
            var result = sut.ContainsToken("lorem ipsum");

            // assert
            Assert.That(result, Is.False);
        }

        [TestCase("query:.././/*[@@templateid='$weblogsetting(EntryTemplateID)']")]
        [TestCase("$weblogsetting(lorem, ipsum)")]
        public void ContainsToken_TextContainsToken_ReturnsTrue(string input)
        {
            // arrange
            var sut = new SettingsTokenReplacer(Mock.Of<IBlogManager>(), Mock.Of<IBlogSettingsResolver>());

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
            var sut = new SettingsTokenReplacer(Mock.Of<IBlogManager>(), Mock.Of<IBlogSettingsResolver>());
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
            var sut = new SettingsTokenReplacer(Mock.Of<IBlogManager>(), Mock.Of<IBlogSettingsResolver>());
            var contextItem = ItemFactory.CreateItem().Object;

            // act
            var result = sut.Replace(input, contextItem);

            // assert
            Assert.That(result, Is.EqualTo(input));
        }

        [Test]
        public void Replace_InputContainsEntryTemplateID_ReturnsEntryTemplateID()
        {
            // arrange
            var entryTemplateId = ID.NewID;
            var settings = Mock.Of<IWeBlogSettings>(x => x.EntryTemplateIds == new[] { entryTemplateId });
            var settingsResolver = Mock.Of<IBlogSettingsResolver>(x => x.Resolve(It.IsAny<BlogHomeItem>()) == new BlogSettings(settings));

            var input = "$weblogsetting(EntryTemplateID)";
            var sut = new SettingsTokenReplacer(Mock.Of<IBlogManager>(), settingsResolver);
            var contextItem = ItemFactory.CreateItem().Object;

            // act
            var result = sut.Replace(input, contextItem);

            // assert
            Assert.That(result, Is.EqualTo(entryTemplateId.ToString()));
        }

        [Test]
        public void Replace_InputContainsCommentTemplateID_ReturnsCommentTemplateID()
        {
            // arrange
            var commentTemplateId = ID.NewID;
            var settings = Mock.Of<IWeBlogSettings>(x => x.CommentTemplateIds == new[] { commentTemplateId });
            var settingsResolver = Mock.Of<IBlogSettingsResolver>(x => x.Resolve(It.IsAny<BlogHomeItem>()) == new BlogSettings(settings));

            var input = "$weblogsetting(commenttemplateid)";
            var sut = new SettingsTokenReplacer(Mock.Of<IBlogManager>(), settingsResolver);
            var contextItem = ItemFactory.CreateItem().Object;

            // act
            var result = sut.Replace(input, contextItem);

            // assert
            Assert.That(result, Is.EqualTo(commentTemplateId.ToString()));
        }

        [Test]
        public void Replace_InputContainsCategoryTemplateID_ReturnsCategoryTemplateID()
        {
            // arrange
            var categoryTemplateId = ID.NewID;
            var settings = Mock.Of<IWeBlogSettings>(x => x.CategoryTemplateIds == new[] { categoryTemplateId });
            var settingsResolver = Mock.Of<IBlogSettingsResolver>(x => x.Resolve(It.IsAny<BlogHomeItem>()) == new BlogSettings(settings));

            var input = "$weblogsetting(CATEGORYtemplateid)";
            var sut = new SettingsTokenReplacer(Mock.Of<IBlogManager>(), settingsResolver);
            var contextItem = ItemFactory.CreateItem().Object;

            // act
            var result = sut.Replace(input, contextItem);

            // assert
            Assert.That(result, Is.EqualTo(categoryTemplateId.ToString()));
        }

        [Test]
        public void Replace_TokenWithDifferentCasing_ReturnsWithTokenReplaced()
        {
            // arrange
            var entryTemplateId = ID.NewID;
            var settings = Mock.Of<IWeBlogSettings>(x => x.EntryTemplateIds == new[] { entryTemplateId });
            var settingsResolver = Mock.Of<IBlogSettingsResolver>(x => x.Resolve(It.IsAny<BlogHomeItem>()) == new BlogSettings(settings));

            var input = "query:.././/*[@@templateid='$WeBlogSetting(EntryTemplateID)']";
            var sut = new SettingsTokenReplacer(Mock.Of<IBlogManager>(), settingsResolver);
            var contextItem = ItemFactory.CreateItem().Object;

            // act
            var result = sut.Replace(input, contextItem);

            // assert
            Assert.That(result, Is.EqualTo($"query:.././/*[@@templateid='{entryTemplateId}']"));
        }

        [Test]
        public void Replace_TokenInsideOtherText_ReturnsWithTokenReplaced()
        {
            // arrange
            var entryTemplateId = ID.NewID;
            var settings = Mock.Of<IWeBlogSettings>(x => x.EntryTemplateIds == new[] { entryTemplateId });
            var settingsResolver = Mock.Of<IBlogSettingsResolver>(x => x.Resolve(It.IsAny<BlogHomeItem>()) == new BlogSettings(settings));

            var input = "query:.././/*[@@templateid='$weblogsetting(EntryTemplateID)']";
            var sut = new SettingsTokenReplacer(Mock.Of<IBlogManager>(), settingsResolver);
            var contextItem = ItemFactory.CreateItem().Object;

            // act
            var result = sut.Replace(input, contextItem);

            // assert
            Assert.That(result, Is.EqualTo($"query:.././/*[@@templateid='{entryTemplateId}']"));
        }

        [Test]
        public void Replace_InputContainsMultipleTokens_ReturnsWithTokensReplaced()
        {
            // arrange
            var categoryTemplateId = ID.NewID;
            var commentTemplateId = ID.NewID;

            var settings = Mock.Of<IWeBlogSettings>(x => 
                x.CategoryTemplateIds == new[] { categoryTemplateId } &&
                x.CommentTemplateIds == new[] { commentTemplateId }
            );
            var settingsResolver = Mock.Of<IBlogSettingsResolver>(x => x.Resolve(It.IsAny<BlogHomeItem>()) == new BlogSettings(settings));

            var input = "query:.././/*[@@templateid='$weblogsetting(CommentTemplateID)' OR @@templateid='$weblogsetting(categoryTemplateID)']";
            var sut = new SettingsTokenReplacer(Mock.Of<IBlogManager>(), settingsResolver);
            var contextItem = ItemFactory.CreateItem().Object;

            // act
            var result = sut.Replace(input, contextItem);

            // assert
            Assert.That(result, Is.EqualTo($"query:.././/*[@@templateid='{commentTemplateId}' OR @@templateid='{categoryTemplateId}']"));
        }
    }
}
