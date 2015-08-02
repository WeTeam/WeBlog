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

        [Ignore("Deprecated method not tested with new custom template functionality")]
        public override void MakeSortedCommentsList_InOrder()
        {
            
        }

        [Ignore("Deprecated method not tested with new custom template functionality")]
        public override void MakeSortedCommentsList_OutOfOrder()
        {
            
        }

        [Ignore("Deprecated method not tested with new custom template functionality")]
        public override void MakeSortedCommentsList_ReverseOrder()
        {
            
        }

        [Ignore("Deprecated method not tested with new custom template functionality")]
        public override void MakeSortedCommentsList_WithNonComment()
        {
            
        }
    }
}