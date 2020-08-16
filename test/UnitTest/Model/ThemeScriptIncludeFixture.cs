using NUnit.Framework;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.UnitTest.Model
{
    [TestFixture]
    public class ThemeScriptIncludeFixture
    {
        [Test]
        public void Ctor_UrlIsNotNull_SetsProperty()
        {
            // arrange, act
            var sut = new ThemeScriptInclude("/url", "fallbackUrl", "object");

            // assert
            Assert.That(sut.Url, Is.EqualTo("/url"));
        }

        [TestCase(null)]
        [TestCase("")]
        public void Ctor_FallbackUrlIsNullOrEmpty_SetsPropertyToEmpty(string fallbackUrl)
        {
            // arrange, act
            var sut = new ThemeScriptInclude("/url", fallbackUrl, "object");

            // assert
            Assert.That(sut.FallbackUrl, Is.Empty);
        }

        [Test]
        public void Ctor_FallbackUrlIsNotEmpty_SetsProperty()
        {
            // arrange, act
            var sut = new ThemeScriptInclude("/url", "//fallback", "object");

            // assert
            Assert.That(sut.FallbackUrl, Is.EqualTo("//fallback"));
        }

        [TestCase(null)]
        [TestCase("")]
        public void Ctor_VerificationObjectIsNullOrEmpty_SetsPropertyToEmpty(string verificationObject)
        {
            // arrange, act
            var sut = new ThemeScriptInclude("/url", "//fallback", verificationObject);

            // assert
            Assert.That(sut.VerificationObject, Is.Empty);
        }

        [Test]
        public void Ctor_VerificationObjectIsNotEmpty_SetsProperty()
        {
            // arrange, act
            var sut = new ThemeScriptInclude("/url", "//fallback", "object");

            // assert
            Assert.That(sut.VerificationObject, Is.EqualTo("object"));
        }
    }
}
