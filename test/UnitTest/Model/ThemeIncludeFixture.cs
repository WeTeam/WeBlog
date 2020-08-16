using NUnit.Framework;
using Sitecore.Modules.WeBlog.Model;
using System;

namespace Sitecore.Modules.WeBlog.UnitTest.Model
{
    [TestFixture]
    public class ThemeIncludeFixture
    {
        [TestCase(null)]
        [TestCase("")]
        public void Ctor_UrlIsEmptyOrNull_SetsUrlPropertyToEmpty(string url)
        {
            // arrange, act
            var sut = new ThemeInclude(null);

            // assert
            Assert.That(sut.Url, Is.Empty);
        }

        [Test]
        public void Ctor_UrlIsNotEmpty_UrlPropertyIsSet()
        {
            // arrange, act
            var sut = new ThemeInclude("/url");

            // assert
            Assert.That(sut.Url, Is.EqualTo("/url"));
        }
    }
}
