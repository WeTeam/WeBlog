using System.IO;
using System.Linq;
using System.Web;
using NUnit.Framework;
using Sitecore.Caching;
using Sitecore.Data;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.SecurityModel;
using Mod = Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Test
{
    [TestFixture]
    [Category("CategoryManager")]
    public class CategoryManager : UnitTestBase
    {
        private Item m_testRoot = null;
        private Item m_blog1 = null;
        private Item m_categoryRoot = null;
        private Item m_category1 = null;
        private Item m_category2 = null;
        private Item m_category3 = null;
        private Item m_category4 = null;
        private Item m_entry1 = null;
        private Item m_entry2 = null;
        private Item m_comment1 = null;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            // Create test content
            using (new SecurityDisabler())
            {
                // Don't change IDs as blog item references category by ID
                using (new EventDisabler())
                {
                    m_testContentRoot.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\category manager content.xml")), false, PasteMode.Overwrite);

                    // Had some weirdness with field values not being seen
                    CacheManager.ClearAllCaches();
                }
            }
            Initialize();
        }

        protected void Initialize()
        {
            // Retrieve created content items
            m_testRoot = m_testContentRoot.Axes.GetChild("blog test root");
            m_blog1 = m_testRoot.Axes.GetChild("blog1");

            m_categoryRoot = m_blog1.Axes.GetChild("categories");
            m_category1 = m_categoryRoot.Axes.GetChild("category1");
            m_category2 = m_categoryRoot.Axes.GetChild("category2");
            m_category3 = m_categoryRoot.Axes.GetChild("category3");
            m_category4 = m_categoryRoot.Axes.GetChild("category4");

            m_entry1 = m_blog1.Axes.GetDescendant("entry1");
            m_entry2 = m_blog1.Axes.GetDescendant("entry2");

            m_comment1 = m_entry1.Axes.GetDescendant("comment1");
        }

        [Test]
        public void GetCategories_Null()
        {
            var categories = new Mod.CategoryManager().GetCategories((Item)null);
            Assert.AreEqual(0, categories.Length);
        }

        [Test]
        public void GetCategories_OutsideBlogRoot()
        {
            var home = Sitecore.Context.Database.GetItem("/sitecore/content/home");
            var categories = new Mod.CategoryManager().GetCategories(home);
            Assert.AreEqual(0, categories.Length);
        }

        [Test]
        public void GetCategories_OutsideBlogRoot_ById()
        {
            var home = Sitecore.Context.Database.GetItem("/sitecore/content/home");
            var categories = new Mod.CategoryManager().GetCategories(home.ID.ToString());
            Assert.AreEqual(0, categories.Length);
        }

        [Test]
        public void GetCategories_OnBlogItem()
        {
            VerifyAllCategories(new Mod.CategoryManager().GetCategories(m_blog1));
        }

        [Test]
        public void GetCategories_OnEntry1Item()
        {
            VerifyAllCategories(new Mod.CategoryManager().GetCategories(m_entry1));
        }

        [Test]
        public void GetCategories_OnEntry1Item_ById()
        {
            VerifyAllCategories(new Mod.CategoryManager().GetCategories(m_entry1.ID.ToString()));
        }

        [Test]
        public void GetCategories_OnEntry2Item()
        {
            VerifyAllCategories(new Mod.CategoryManager().GetCategories(m_entry2));
        }

        [Test]
        public void GetCategories_OnCommentItem()
        {
            VerifyAllCategories(new Mod.CategoryManager().GetCategories(m_comment1));
        }

        [Test]
        public void GetCategories_OnCommentItem_ById()
        {
            VerifyAllCategories(new Mod.CategoryManager().GetCategories(m_comment1.ID.ToString()));
        }

        [Test]
        public void GetCategories_OnCategoryItem()
        {
            VerifyAllCategories(new Mod.CategoryManager().GetCategories(m_category2));
        }

        private void VerifyAllCategories(CategoryItem[] categories)
        {
            var ids = (from category in categories
                       select category.ID).ToArray();

            Assert.AreEqual(4, ids.Length);
            Assert.Contains(m_category1.ID, ids);
            Assert.Contains(m_category2.ID, ids);
            Assert.Contains(m_category3.ID, ids);
            Assert.Contains(m_category4.ID, ids);
        }

        [Test]
        public void GetCategoriesByEntryID_NonEntryItem()
        {
            var categories = new Mod.CategoryManager().GetCategoriesByEntryID(m_blog1.ID);
            Assert.AreEqual(0, categories.Length);
        }

        [Test]
        public void GetCategoriesByEntryID_Entry1()
        {
            var categories = new Mod.CategoryManager().GetCategoriesByEntryID(m_entry1.ID);
            Assert.AreEqual(2, categories.Length);

            var ids = (from category in categories
                       select category.ID).ToArray();

            Assert.Contains(m_category1.ID, ids);
            Assert.Contains(m_category3.ID, ids);
        }

        [Test]
        public void GetCategoriesByEntryID_Entry2()
        {
            var categories = new Mod.CategoryManager().GetCategoriesByEntryID(m_entry2.ID);
            Assert.AreEqual(2, categories.Length);

            var ids = (from category in categories
                       select category.ID).ToArray();

            Assert.Contains(m_category2.ID, ids);
            Assert.Contains(m_category3.ID, ids);
        }

        [Test]
        public void GetCategoriesByEntryID_InvalidID()
        {
            var categories = new Mod.CategoryManager().GetCategoriesByEntryID(ID.NewID);
            Assert.AreEqual(0, categories.Length);
        }

        [Test]
        public void GetCategoryRoot_BelowBlog()
        {
            var root = new Mod.CategoryManager().GetCategoryRoot(m_entry1);
            Assert.IsNotNull(root);
            Assert.AreEqual(m_categoryRoot.ID, root.ID);
        }

        [Test]
        public void GetCategoryRoot_OnBlog()
        {
            var root = new Mod.CategoryManager().GetCategoryRoot(m_blog1);
            Assert.IsNotNull(root);
            Assert.AreEqual(m_categoryRoot.ID, root.ID);
        }

        [Test]
        public void GetCategoryRoot_OutsideBlog()
        {
            var home = Sitecore.Context.Database.GetItem("/sitecore/content/home");
            var root = new Mod.CategoryManager().GetCategoryRoot(home);
            Assert.IsNull(root);
        }

        [Test]
        public void GetCategory_ByValidNameFromBlog()
        {
            var category = new Mod.CategoryManager().GetCategory(m_blog1, m_category3.Name);
            Assert.IsNotNull(category);
            Assert.AreEqual(m_category3.ID, category.ID);
        }

        [Test]
        public void GetCategory_ByInvalidNameFromBlog()
        {
            var category = new Mod.CategoryManager().GetCategory(m_blog1, "blah");
            Assert.IsNull(category);
        }

        [Test]
        public void GetCategory_ByUppercaseValidNameFromBlog()
        {
            var category = new Mod.CategoryManager().GetCategory(m_blog1, m_category3.Name.ToUpper());
            Assert.IsNotNull(category);
            Assert.AreEqual(m_category3.ID, category.ID);
        }

        [Test]
        public void GetCategory_ByValidNameFromEntry()
        {
            var category = new Mod.CategoryManager().GetCategory(m_entry2, m_category3.Name);
            Assert.IsNotNull(category);
            Assert.AreEqual(m_category3.ID, category.ID);
        }
    }
}