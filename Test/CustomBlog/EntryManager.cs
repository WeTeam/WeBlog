using NUnit.Framework;

namespace Sitecore.Modules.WeBlog.Test.CustomBlog
{
    [TestFixture]
    [Category("CustomBlog.EntryManager")]
    public class EntryManager : Sitecore.Modules.WeBlog.Test.EntryManagerFixture
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