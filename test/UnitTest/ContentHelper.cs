using NUnit.Framework;

namespace Sitecore.Modules.WeBlog.UnitTest
{
    [TestFixture]
    public class ContentHelper
    {
        [TestCase("/sitecore/content/Home/r1/2016/December/Mile Post/Comment at 20161219 174023 by True Religion")]
        public void EscapePath(string path)
        {
            var result = WeBlog.ContentHelper.EscapePath(path);
            var expected = "/#sitecore#/#content#/#Home#/#r1#/#2016#/#December#/#Mile Post#/#Comment at 20161219 174023 by True Religion#";
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
