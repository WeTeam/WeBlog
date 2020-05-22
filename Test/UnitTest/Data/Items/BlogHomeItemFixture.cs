﻿using Moq;
using NUnit.Framework;
using Sitecore.Abstractions;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using System;

namespace Sitecore.Modules.WeBlog.UnitTest.Data.Items
{
    [TestFixture]
    public class BlogHomeItemFixture
    {
        [Test]
        public void Ctor_NullItem_ThrowsException()
        {
            // arrange
            var linkManager = Mock.Of<BaseLinkManager>();
            var settings = Mock.Of<IWeBlogSettings>();
            Action sutAction = () => new BlogHomeItem(null, linkManager, settings);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(new TestDelegate(sutAction));
            Assert.That(ex.ParamName, Is.EqualTo("innerItem"));
        }

        [Test]
        public void Ctor_NullLinkManager_ThrowsException()
        {
            // arrange
            var item = ItemFactory.CreateItem();
            var settings = Mock.Of<IWeBlogSettings>();
            Action sutAction = () => new BlogHomeItem(item.Object, null, settings);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(new TestDelegate(sutAction));
            Assert.That(ex.ParamName, Is.EqualTo("linkManager"));
        }
    }
}