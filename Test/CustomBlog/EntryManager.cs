using NUnit.Framework;

namespace Sitecore.Modules.WeBlog.Test.CustomBlog
{
    [TestFixture]
    [Category("CustomBlog.EntryManager")]
    public class EntryManager : Sitecore.Modules.WeBlog.Test.EntryManager
    {
        [TestFixtureSetUp]
        public void ChangeBlog()
        {
            Sitecore.Context.Database.SetupCustomBlogs(m_testRoot);
            //re-init to retrieve member items
            Initialize();
        }

        [TestFixtureTearDown]
        public void RemoveTemplates()
        {
            Sitecore.Context.Database.RemoveCustomTemplates();
        }

        [Ignore("Deprecated method not tested with new custom template functionality")]
        public override void MakeSortedEntriesList_InOrder()
        {
            return;
        }

        [Ignore("Deprecated method not tested with new custom template functionality")]
        public override void MakeSortedEntriesList_OutOfOrder()
        {
            return;
        }

        [Ignore("Deprecated method not tested with new custom template functionality")]
        public override void MakeSortedEntriesList_ReverseOrder()
        {
            return;
        }
    }
}