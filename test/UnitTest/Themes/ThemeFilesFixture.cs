using NUnit.Framework;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Modules.WeBlog.Themes;
using System.Linq;

namespace Sitecore.Modules.WeBlog.UnitTest.Themes
{
    [TestFixture]
    public class ThemeFilesFixture
    {
        [Test]
        public void Ctor_StylesheetsIsNull_StylesheetsPropertyIsEmpty()
        {
            // arrange, act
            var sut = new ThemeFiles(null, Enumerable.Empty<ThemeScriptInclude>());

            // assert
            Assert.That(sut.Stylesheets, Is.Empty);
        }

        [Test]
        public void Ctor_ScriptsIsNull_ScriptsPropertyIsEmpty()
        {
            // arrange, act
            var sut = new ThemeFiles(Enumerable.Empty<ThemeInclude>(), null);

            // assert
            Assert.That(sut.Scripts, Is.Empty);
        }

        [Test]
        public void Ctor_WhenCalled_SetsStylesheetsProperty()
        {
            // arrange
            var stylesheet1 = new ThemeInclude("file1.css");
            var stylesheet2 = new ThemeInclude("file2.css");

            // act
            var sut = new ThemeFiles(new[] { stylesheet1, stylesheet2 }, Enumerable.Empty<ThemeScriptInclude>());

            // assert
            var stylesheets = sut.Stylesheets;
            Assert.That(stylesheets, Contains.Item(stylesheet1));
            Assert.That(stylesheets, Contains.Item(stylesheet2));
        }

        [Test]
        public void Ctor_WhenCalled_SetsScriptsProperty()
        {
            // arrange
            var script1 = new ThemeScriptInclude("url1.js", "fb1.js", "ob1");
            var script2 = new ThemeScriptInclude("url2.js", "fb2.js", "ob2");

            // act
            var sut = new ThemeFiles(Enumerable.Empty<ThemeInclude>(), new[] { script1, script2 });

            // assert
            var scripts = sut.Scripts;
            Assert.That(scripts, Contains.Item(script1));
            Assert.That(scripts, Contains.Item(script2));
        }
    }
}
