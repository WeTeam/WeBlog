using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using CookComputing.XmlRpc;
using NUnit.Framework;
using Sitecore.ContentSearch;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Search;
using Sitecore.Security.AccessControl;
using Sitecore.Security.Accounts;
using Sitecore.SecurityModel;
using Mod = Sitecore.Modules.WeBlog;

namespace Sitecore.Modules.WeBlog.Test
{
    [TestFixture]
    [Category("MetaBlogApi")]
    public class MetaBlogApi : UnitTestBase
    {
        private static Random m_random = new Random();
        private const string PASSWORD = "password1";

        private User m_userAuthor = null;
        private User m_userNothing = null;
        private Item m_testRoot = null;
        private Item m_blog1 = null;
        private Item m_blog2 = null;
        private Item m_blog3 = null;
        private Mod.MetaBlogApi m_api = null;

        [TestFixtureSetUp()]
        public void TestFixtureSetUp()
        {
            // Create test content
            using (new SecurityDisabler())
            {
                m_testContentRoot.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\MetaBlog content.xml")), true, PasteMode.Overwrite);

                // Retrieve created content items
                m_testRoot = m_testContentRoot.Axes.GetChild("test content");
                m_blog1 = m_testRoot.Axes.GetChild("blog1");
                m_blog2 = m_testRoot.Axes.GetChild("blog2");
                m_blog3 = m_testRoot.Axes.GetChild("blog3");

                // Ensure blog 1 entries. Current NewsMover has a bug which is removing them as they are created.
                // Remove the following section once the bug has been fixed
                // START: Workaround
                var template = m_blog1.Database.Templates[Settings.EntryTemplateID];
                var entry11Check = m_blog1.Axes.GetDescendant("Entry11");

                if (entry11Check == null)
                    m_blog1.Add("Entry11", template);

                var entry12Check = m_blog1.Axes.GetDescendant("Entry12");

                if (entry12Check == null)
                {
                    System.Threading.Thread.Sleep(2000);
                    m_blog1.Add("Entry12", template);
                }
                // END: Workaround

                // Create test users
                // Use random usernames to ensure we're not trying to create users that might already exist
                m_userAuthor = Sitecore.Security.Accounts.User.Create("sitecore\\user" + m_random.Next(999999), PASSWORD);
                m_userNothing = Sitecore.Security.Accounts.User.Create("sitecore\\user" + m_random.Next(999999), PASSWORD);

                // Add users to roles
                m_userAuthor.Roles.Add(Role.FromName("sitecore\\Sitecore Client Authoring"));

                var rules = new AccessRuleCollection();
                rules.Add(AccessRule.Create(m_userAuthor, AccessRight.ItemWrite, PropagationType.Any, AccessPermission.Allow));
                rules.Add(AccessRule.Create(m_userAuthor, AccessRight.ItemDelete, PropagationType.Any, AccessPermission.Allow));
                rules.Add(AccessRule.Create(m_userAuthor, AccessRight.ItemCreate, PropagationType.Any, AccessPermission.Allow));

                m_blog1.Security.SetAccessRules(rules);
                m_blog2.Security.SetAccessRules(rules);

                ContentHelper.PublishItemAndRequiredAncestors(m_blog1, Sitecore.Configuration.Factory.GetDatabase("web"));

                var entry11 = m_blog1.Axes.GetDescendant("Entry11");
                ContentHelper.PublishItemAndRequiredAncestors(entry11, Sitecore.Configuration.Factory.GetDatabase("web"));

                var entry12 = m_blog1.Axes.GetDescendant("Entry12");
                ContentHelper.PublishItemAndRequiredAncestors(entry12, Sitecore.Configuration.Factory.GetDatabase("web"));

                // Rebuild the search index to ensure all manager calls work as expected
#if FEATURE_CONTENT_SEARCH
                var index = ContentSearchManager.GetIndex(Settings.SearchIndexName);
                index.Rebuild();
#else
                var index = SearchManager.GetIndex(Settings.SearchIndexName);
                index.Rebuild();
#endif
            }

            m_api = new Mod.MetaBlogApi();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            if (m_testRoot != null)
            {
                using (new SecurityDisabler())
                {
                    // Publish to remove test content from the web DB
                    ContentHelper.PublishItem(m_testRoot.Parent);
                }
            }

            using (new SecurityDisabler())
            {
                Membership.DeleteUser(m_userAuthor.Name);
                Membership.DeleteUser(m_userNothing.Name);
            }
        }

        [Test]
        public void GetUsersBlogs_ValidUserMultiple()
        {
            var result = m_api.getUsersBlogs("test", m_userAuthor.Name, PASSWORD);

            Assert.AreEqual(2, result.Length);

            var ids = (from entry in result
                        select entry["blogid"] as string).ToArray();

            Assert.Contains(m_blog1.ID.ToString(), ids);
            Assert.Contains(m_blog2.ID.ToString(), ids);
        }

        [Test]
        public void GetUsersBlogs_ValidUserNone()
        {
            var result = m_api.getUsersBlogs("test", m_userNothing.Name, PASSWORD);
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        [ExpectedException("System.Security.Authentication.InvalidCredentialException")]
        public void GetUsersBlogs_InvalidUser()
        {
            var result = m_api.getUsersBlogs("test", "sitecore\\a", "a");
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void GetCategories_ValidUser()
        {
            var result = m_api.getCategories(m_blog1.ID.ToString(), m_userAuthor.Name, PASSWORD);

            Assert.AreEqual(2, result.Length);

            var names = (from entry in result
                         select entry["title"] as string).ToArray();

            Assert.Contains("Category11", names);
            Assert.Contains("Category12", names);
        }

        [Test]
        [ExpectedException("System.Security.Authentication.InvalidCredentialException")]
        public void GetCategories_InvalidUser()
        {
            var result = m_api.getCategories(m_blog1.ID.ToString(), "sitecore\\a", "a");
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void GetRecentPosts_ValidUser_Blog1()
        {
            var result = m_api.getRecentPosts(m_blog1.ID.ToString(), m_userAuthor.Name, PASSWORD, 5);

            Assert.AreEqual(2, result.Length);

            var names = (from entry in result
                         select entry["title"] as string).ToArray();

            Assert.Contains("Entry11", names);
            Assert.Contains("Entry12", names);
        }

        [Test]
        public void GetRecentPosts_ValidUser_Blog1Limited()
        {
            var result = m_api.getRecentPosts(m_blog1.ID.ToString(), m_userAuthor.Name, PASSWORD, 1);

            Assert.AreEqual(1, result.Length);

            var names = (from entry in result
                         select entry["title"] as string).ToArray();

            Assert.Contains("Entry11", names);
        }

        [Test]
        public void GetRecentPosts_ValidUser_NotOnBlog3()
        {
            var result = m_api.getRecentPosts(m_blog3.ID.ToString(), m_userAuthor.Name, PASSWORD, 100);

            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void NewPost_ValidUser()
        {
            var entryContent = new XmlRpcStruct();
            entryContent.Add("title", "the title");
            entryContent.Add("description", "the description");

            var newId = m_api.newPost(m_blog1.ID.ToString(), m_userAuthor.Name, PASSWORD, entryContent, false);

            Assert.IsNotNullOrEmpty(newId);

            var newItem = m_blog1.Database.GetItem(newId);

            try
            {   
                Assert.AreEqual("the title", newItem["title"]);
                Assert.AreEqual("the description", newItem["content"]);
            }
            finally
            {
                using (new SecurityDisabler())
                {
                    newItem.Delete();
                }
            }
        }

        [Test]
        public void NewPost_ValidUserFuturePublish()
        {
            var publishDate = new DateTime(2020, 10, 10, 13, 12, 12);

            var entryContent = new XmlRpcStruct();
            entryContent.Add("title", "the title");
            entryContent.Add("description", "the description");
            entryContent.Add("dateCreated", publishDate);

            var newId = m_api.newPost(m_blog1.ID.ToString(), m_userAuthor.Name, PASSWORD, entryContent, false);

            Assert.IsNotNullOrEmpty(newId);

            var newItem = m_blog1.Database.GetItem(newId);

            try
            {
                Assert.AreEqual(publishDate, newItem.Publishing.PublishDate.ToLocalTime());
            }
            finally
            {
                using (new SecurityDisabler())
                {
                    newItem.Delete();
                }
            }
        }

        [Test]
        [ExpectedException("System.Security.Authentication.InvalidCredentialException")]
        public void NewPost_InvalidUser()
        {
            var entryContent = new XmlRpcStruct();
            entryContent.Add("title", "the title");
            entryContent.Add("description", "the description");

            var newId = m_api.newPost(m_blog1.ID.ToString(), "sitecore\\a", "a", entryContent, false);

            Assert.IsNullOrEmpty(newId);
        }

        [Test]
        public void EditPost_ExistingEntry()
        {
            // Create entry
            var entryContent = new XmlRpcStruct();
            entryContent.Add("title", "the title");
            entryContent.Add("description", "the description");

            var newId = m_api.newPost(m_blog1.ID.ToString(), m_userAuthor.Name, PASSWORD, entryContent, false);

            Assert.IsNotNullOrEmpty(newId);

            // Edit the entry
            var publishDate = new DateTime(2020, 3, 6);
            var updatedContent = new XmlRpcStruct();
            updatedContent.Add("description", "updated");
            updatedContent.Add("dateCreated", publishDate);

            var result = m_api.editPost(newId, m_userAuthor.Name, PASSWORD, updatedContent, false);
            Assert.IsTrue(result);

            var newItem = m_blog1.Database.GetItem(newId);

            try
            {
                Assert.AreEqual("the title", newItem["title"]);
                Assert.AreEqual("updated", newItem["content"]);

              //var expectedDate = newItem.Publishing.PublishDate.ToLocalTime();

                Assert.AreEqual(publishDate, newItem.Publishing.PublishDate.ToLocalTime());
            }
            finally
            {
                using (new SecurityDisabler())
                {
                    newItem.Delete();
                }
            }
        }

        [Test]
        public void EditPost_InvalidEntry()
        {
            // Edit the entry
            var updatedContent = new XmlRpcStruct();
            updatedContent.Add("description", "updated");

            var result = m_api.editPost(ID.NewID.ToString(), m_userAuthor.Name, PASSWORD, updatedContent, false);
            Assert.IsFalse(result);
        }

        [Test]
        [ExpectedException("System.Security.Authentication.InvalidCredentialException")]
        public void EditPost_InvalidUser()
        {
            // Edit the entry
            var updatedContent = new XmlRpcStruct();
            updatedContent.Add("description", "updated");

            var entry11 = m_blog1.Axes.GetDescendant("Entry11");

            var result = m_api.editPost(entry11.ID.ToString(), "sitecore\\a", "a", updatedContent, false);
            Assert.IsFalse(result);
        }

        [Test]
        public void GetPost_ExistingEntry()
        {
            var entry11 = m_blog1.Axes.GetDescendant("Entry11");
            var content = m_api.getPost(entry11.ID.ToString(), m_userAuthor.Name, PASSWORD);

            Assert.AreEqual(entry11["title"], content["title"]);
            Assert.AreEqual(entry11["content"], content["description"]);
            Assert.AreEqual(entry11.ID.ToString(), content["guid"]);
        }

        [Test]
        public void GetPost_InvalidEntry()
        {
            var content = m_api.getPost(ID.NewID.ToString(), m_userAuthor.Name, PASSWORD);

            Assert.IsNull(content["title"]);
            Assert.IsNull(content["content"]);
            Assert.IsNull(content["guid"]);
        }

        [Test]
        [ExpectedException("System.Security.Authentication.InvalidCredentialException")]
        public void GetPost_InvalidUser()
        {
            var entry11 = m_blog1.Axes.GetDescendant("Entry11");
            var content = m_api.getPost(entry11.ID.ToString(), "sitecore\\a", "a");
        }

        [Test]
        public void DeletePost_ExistingEntry()
        {
            // Create entry
            var entryContent = new XmlRpcStruct();
            entryContent.Add("title", "the title");
            entryContent.Add("description", "the description");

            var newId = m_api.newPost(m_blog1.ID.ToString(), m_userAuthor.Name, PASSWORD, entryContent, false);

            Assert.IsNotNullOrEmpty(newId);

            // Delete the entry
            var result = m_api.deletePost("test", newId, m_userAuthor.Name, PASSWORD, false);
            Assert.IsTrue(result);
        }

        [Test]
        public void DeletePost_InvalidEntry()
        {
            var result = m_api.deletePost("test", ID.NewID.ToString(), m_userAuthor.Name, PASSWORD, false);
            Assert.IsFalse(result);
        }

        [Test]
        [ExpectedException("System.Security.Authentication.InvalidCredentialException")]
        public void DeletePost_InvalidUser()
        {
            var newId = m_api.deletePost("test", ID.NewID.ToString(), "sitecore\\a", "a", false);
        }
    }
}