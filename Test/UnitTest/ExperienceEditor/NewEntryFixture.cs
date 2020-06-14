using Moq;
using NUnit.Framework;
using Sitecore.Abstractions;
using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.Modules.WeBlog.ExperienceEditor;
using Sitecore.Modules.WeBlog.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Modules.WeBlog.UnitTest.ExperienceEditor
{
    [TestFixture]
    public class NewEntryFixture
    {
        [Test]
        public void Ctor_BlogManagerIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new NewEntry(null, Mock.Of<BaseItemManager>());

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(new TestDelegate(sutAction));
            Assert.That(ex.ParamName, Is.EqualTo("blogManager"));
        }

        [Test]
        public void Ctor_ItemManagerIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new NewEntry(Mock.Of<IBlogManager>(), null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(new TestDelegate(sutAction));
            Assert.That(ex.ParamName, Is.EqualTo("itemManager"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("invalid/name")]
        public void ProcessRequest_InvalidItemName_ReturnsEmptyResult(string itemName)
        {
            // arrange
            var blogManager = Mock.Of<IBlogManager>();
            var itemManager = Mock.Of<BaseItemManager>();
            var sut = new NewEntry(blogManager, itemManager);

            sut.RequestContext = new ItemContext
            {
                Argument = itemName
            };

            // act
            var result = sut.ProcessRequest();

            // assert
            Assert.That(result.Value, Is.Null);
        }

        // todo: complete tests
    }
}
