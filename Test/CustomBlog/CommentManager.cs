using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using NUnit.Framework;
using System;

namespace Sitecore.Modules.WeBlog.Test.CustomBlog
{
    [TestFixture]
    [Category("CustomBlog.CommentManager")]
    public class CommentManager : Sitecore.Modules.WeBlog.Test.CommentManager
    {
        [TestFixtureSetUp]
        public void ChangeBlog()
        {
            Sitecore.Context.Database.SetupCustomBlogs("blog test root");
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