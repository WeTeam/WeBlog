using NUnit.Framework;

namespace Sitecore.Modules.WeBlog.Test.CustomBlog
{
    [TestFixture]
    [Category("CustomBlog.CommentManager")]
    public class CommentManager : Sitecore.Modules.WeBlog.Test.CommentManager
    {
        [TestFixtureSetUp]
        public void ChangeBlog()
        {
            Sitecore.Context.Database.SetupCustomBlogs(m_testContentRoot);
            //re-init to retrieve member items
            Initialize();
        }

        [TestFixtureTearDown]
        public void RemoveTemplates()
        {
            Sitecore.Context.Database.RemoveCustomTemplates();
        }
    }
}