using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using NUnit.Framework;
using System;

namespace Sitecore.Modules.WeBlog.Test.CustomBlog
{
    [TestFixture]
    [Category("CustomBlog.BlogManager")]
    public class BlogManager : Sitecore.Modules.WeBlog.Test.BlogManager
    {
        [TestFixtureSetUp]
        public void ChangeBlog()
        {
            Sitecore.Context.Database.SetupCustomBlogs("weblog testroot");
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