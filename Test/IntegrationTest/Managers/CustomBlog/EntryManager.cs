using NUnit.Framework;

namespace Sitecore.Modules.WeBlog.IntegrationTest.Managers.CustomBlog
{
    [TestFixture]
    [Category("CustomBlog.EntryManager")]
    public class EntryManager : EntryManagerFixture
    {
        [TestFixtureSetUp]
        public void ChangeBlog()
        {
            /*Sitecore.Context.Database.SetupCustomBlogs(m_testRoot);
            //re-init to retrieve member items
            Initialize();*/
        }

        [TestFixtureTearDown]
        public void RemoveTemplates()
        {
            Sitecore.Context.Database.RemoveCustomTemplates();
        }
    }
}