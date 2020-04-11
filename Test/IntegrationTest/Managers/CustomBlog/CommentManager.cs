using NUnit.Framework;

namespace Sitecore.Modules.WeBlog.IntegrationTest.Managers.CustomBlog
{
    [TestFixture]
    [Category("CustomBlog.CommentManager")]
    public class CommentManager : CommentManagerFixture
    {
        [TestFixtureSetUp]
        public void ChangeBlog()
        {
            Sitecore.Context.Database.SetupCustomBlogs(TestContentRoot);
        }

        [TestFixtureTearDown]
        public void RemoveTemplates()
        {
            Sitecore.Context.Database.RemoveCustomTemplates();
        }
    }
}