using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using NUnit.Framework;
using Sitecore.Data.Items;
using Sitecore.Modules.Blog.Items;
using Sitecore.Security.AccessControl;
using Sitecore.SecurityModel;
using Mod = Sitecore.Modules.Blog.Managers;

namespace Sitecore.Modules.Blog.Test
{
    [TestFixture]
    [Category("BlogManager")]
    public class BlogManager
    {
        private const string TESTUSERNAME = "test1";

        private Item m_testRoot = null;
        private Item m_blog1 = null;
        private Item m_blog2 = null;
        private Item m_entry11 = null;
        private Item m_entry21 = null;
        private Item m_comment111 = null;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            // Create test content
            var home = Sitecore.Context.Database.GetItem("/sitecore/content/home");
            using (new SecurityDisabler())
            {
                home.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\blog manager content.xml")), true, PasteMode.Overwrite);
            }

            // Retrieve created content items
            m_testRoot = home.Axes.GetChild("eviblog testroot");
            m_blog1 = m_testRoot.Axes.GetChild("blog1");
            m_blog2 = m_testRoot.Axes.GetChild("blog2");

            m_entry11 = m_blog1.Axes.GetChild("entry1");
            m_comment111 = m_entry11.Axes.GetChild("comment1");

            m_entry21 = m_blog2.Axes.GetChild("entry1");

            // Create test user
            try
            {
                var user = Sitecore.Security.Accounts.User.Create("sitecore\\" + TESTUSERNAME, TESTUSERNAME);
                Roles.AddUserToRole("sitecore\\" + TESTUSERNAME, "sitecore\\sitecore client authoring");

                var accessRule = AccessRule.Create(user, AccessRight.ItemWrite, PropagationType.Any, AccessPermission.Allow);
                var accessRules = new AccessRuleCollection();
                accessRules.Add(accessRule);
                m_blog1.Security.SetAccessRules(accessRules);
            }
            catch
            {
                Membership.DeleteUser("sitecore\\" + TESTUSERNAME);
            }
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            using (new SecurityDisabler())
            {
                if (m_testRoot != null)
                    m_testRoot.Delete();
            }

            Membership.DeleteUser("sitecore\\" + TESTUSERNAME);
        }

        [Test]
        public void GetCurrentBlog_FromBlogItem()
        {
            Assert.AreEqual(m_blog1.ID, Mod.BlogManager.GetCurrentBlog(m_blog1).ID);
        }

        [Test]
        public void GetCurrentBlog_OutsideBlogRoot()
        {
            Assert.IsNull(Mod.BlogManager.GetCurrentBlog(m_testRoot));
        }

        [Test]
        public void GetCurrentBlog_FromEntryItem()
        {
            Assert.AreEqual(m_blog1.ID, Mod.BlogManager.GetCurrentBlog(m_entry11).ID);
        }

        [Test]
        public void GetCurrentBlog_FromCommentItem()
        {
            Assert.AreEqual(m_blog1.ID, Mod.BlogManager.GetCurrentBlog(m_comment111).ID);
        }

        [Test]
        public void GetCurrentBlog_FromSeparateBlogEntryItem()
        {
            Assert.AreNotEqual(m_blog1.ID, Mod.BlogManager.GetCurrentBlog(m_entry21).ID);
        }

        [Test]
        public void GetAllBlogs()
        {
            var blogIds = (from blog in Mod.BlogManager.GetAllBlogs()
                       select blog.ID).ToArray();

            Assert.AreEqual(2, blogIds.Length);
            Assert.Contains(m_blog1.ID, blogIds);
            Assert.Contains(m_blog2.ID, blogIds);
        }

        [Test]
        public void EnableRSS_BlogEnabled()
        {
            Assert.IsTrue(Mod.BlogManager.EnableRSS(new Items.Blog.BlogItem(m_blog1)));
        }

        [Test]
        public void EnableRSS_BlogDisabled()
        {
            Assert.IsFalse(Mod.BlogManager.EnableRSS(new Items.Blog.BlogItem(m_blog2)));
        }

        [Test]
        public void ShowEmailWithinComments_BlogEnabled()
        {
            Assert.IsTrue(Mod.BlogManager.ShowEmailWithinComments(new Items.Blog.BlogItem(m_blog1)));
        }

        [Test]
        public void ShowEmailWithinComments_BlogDisabled()
        {
            Assert.IsFalse(Mod.BlogManager.ShowEmailWithinComments(new Items.Blog.BlogItem(m_blog2)));
        }

        [Test]
        public void GetUserBlogs()
        {
            var blogs = Mod.BlogManager.GetUserBlogs("sitecore\\" + TESTUSERNAME);
            Assert.AreEqual(1, blogs.Length);
            Assert.AreEqual(m_blog1.ID, blogs[0].ID);
        }
    }
}