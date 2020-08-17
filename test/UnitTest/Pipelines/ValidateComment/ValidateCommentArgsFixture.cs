using NUnit.Framework;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Modules.WeBlog.Pipelines.ValidateComment;
using System;
using System.Collections.Specialized;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.ValidateComment
{
    [TestFixture]
    public class ValidateCommentArgsFixture
    {
        [Test]
        public void Ctor_CommentIsNull_ThrowsException()
        {
            // arrange
            TestDelegate sutAction = () => new ValidateCommentArgs(null, new NameValueCollection());

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.That(ex.ParamName, Is.EqualTo("comment"));
        }

        [Test]
        public void Ctor_FormIsNull_ThrowsException()
        {
            // arrange
            TestDelegate sutAction = () => new ValidateCommentArgs(new Comment(), null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.That(ex.ParamName, Is.EqualTo("form"));
        }

        [Test]
        public void Ctor_ValidParameters_SetsProperties()
        {
            // arrange
            var comment = new Comment();
            var form = new NameValueCollection();

            // act
            var sut = new ValidateCommentArgs(comment, form);

            // assert
            Assert.That(sut.Comment, Is.SameAs(comment));
            Assert.That(sut.Form, Is.SameAs(form));
        }
    }
}
