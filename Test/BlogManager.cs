using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using NUnit.Framework;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Security.AccessControl;
using Sitecore.SecurityModel;
using Mod = Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Test
{
    [TestFixture]
    [Category("BlogManager")]
    public class BlogManager : UnitTestBase
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
            using (new SecurityDisabler())
            {
                m_testContentRoot.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\blog manager content.xml")), true, PasteMode.Overwrite);
                Initialize();

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
        }

        protected void Initialize()
        {
            // Retrieve created content items
            m_testRoot = m_testContentRoot.Axes.GetChild("weblog testroot");
            m_blog1 = m_testRoot.Axes.GetChild("blog1");
            m_blog2 = m_testRoot.Axes.GetChild("blog2");

            m_entry11 = m_blog1.Axes.GetDescendant("entry1");
            m_comment111 = m_entry11.Axes.GetDescendant("comment1");

            m_entry21 = m_blog2.Axes.GetDescendant("entry1");
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            Membership.DeleteUser("sitecore\\" + TESTUSERNAME);
        }

        [Test]
        public void GetCurrentBlog_FromBlogItem()
        {
            Assert.AreEqual(m_blog1.ID, new Mod.BlogManager().GetCurrentBlog(m_blog1).ID);
        }

        [Test]
        public void GetCurrentBlog_OutsideBlogRoot()
        {
            Assert.IsNull(new Mod.BlogManager().GetCurrentBlog(m_testRoot));
        }

        [Test]
        public void GetCurrentBlog_FromEntryItem()
        {
            Assert.AreEqual(m_blog1.ID, new Mod.BlogManager().GetCurrentBlog(m_entry11).ID);
        }

        [Test]
        public void GetCurrentBlog_FromCommentItem()
        {
            Assert.AreEqual(m_blog1.ID, new Mod.BlogManager().GetCurrentBlog(m_comment111).ID);
        }

        [Test]
        public void GetCurrentBlog_FromSeparateBlogEntryItem()
        {
            Assert.AreNotEqual(m_blog1.ID, new Mod.BlogManager().GetCurrentBlog(m_entry21).ID);
        }

        [Test]
        public void GetAllBlogs()
        {
          var blogs = new Mod.BlogManager().GetAllBlogs(null).Where(x => m_testContentRoot.Axes.IsAncestorOf(x));

          Assert.That(blogs.Select(x => x.ID), Is.EquivalentTo(new[]
          {
            m_blog1.ID,
            m_blog2.ID
          }));
        }

        [Test]
        public void EnableRSS_BlogEnabled()
        {
            Assert.IsTrue(new Mod.BlogManager().EnableRSS(new Items.WeBlog.BlogHomeItem(m_blog1)));
        }

        [Test]
        public void EnableRSS_BlogDisabled()
        {
            Assert.IsFalse(new Mod.BlogManager().EnableRSS(new Items.WeBlog.BlogHomeItem(m_blog2)));
        }

        [Test]
        public void ShowEmailWithinComments_BlogEnabled()
        {
            Assert.IsTrue(new Mod.BlogManager().ShowEmailWithinComments(new Items.WeBlog.BlogHomeItem(m_blog1)));
        }

        [Test]
        public void ShowEmailWithinComments_BlogDisabled()
        {
            Assert.IsFalse(new Mod.BlogManager().ShowEmailWithinComments(new Items.WeBlog.BlogHomeItem(m_blog2)));
        }

        [Test]
        public void GetUserBlogs()
        {
            var blogs = new Mod.BlogManager().GetUserBlogs("sitecore\\" + TESTUSERNAME);
            Assert.AreEqual(1, blogs.Length);
            Assert.AreEqual(m_blog1.ID, blogs[0].ID);
        }
    }
}