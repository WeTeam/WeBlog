using System.IO;
using System.Linq;
using System.Web;
using NUnit.Framework;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using Mod = Sitecore.Modules.WeBlog.Extensions;

namespace Sitecore.Modules.WeBlog.Test.Extensions
{
    [TestFixture]
    [Category("ItemExtensions")]
    public class ItemExtensions
    {
        private Item m_testRoot = null;
        private Item m_testTemplateRoot = null;
        private Item m_base1 = null;
        private Item m_base2 = null;
        private Item m_d11 = null;
        private Item m_d12 = null;
        private Item m_d2 = null;
        private TemplateItem m_baseTemplate = null;
        private TemplateItem m_d1Template = null;
        private TemplateItem m_d2Template = null;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            // Create test content
            var templateHome = Context.Database.GetItem("/sitecore/templates/user defined");
            var home = Context.Database.GetItem("/sitecore/content/home");
            using (new SecurityDisabler())
            {
                using (new EventDisabler())
                {
                    templateHome.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\templates.xml")), false, PasteMode.Overwrite);
                    home.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\template content.xml")), false, PasteMode.Overwrite);
                }
            }

            // Retrieve created templates
            m_testTemplateRoot = templateHome.Axes.GetChild("Test Templates");
            m_baseTemplate = Context.Database.GetTemplate("user defined/test templates/base");
            m_d1Template = Context.Database.GetTemplate("user defined/test templates/d1");
            m_d2Template = Context.Database.GetTemplate("user defined/test templates/d2");

            // Retrieve created content items
            m_testRoot = home.Axes.GetChild("Test Root");
            m_base1 = m_testRoot.Axes.GetChild("Base1");
            m_base2 = m_testRoot.Axes.GetChild("Base2");
            m_d11 = m_testRoot.Axes.GetChild("D1-1");
            m_d12 = m_testRoot.Axes.GetChild("D1-2");
            m_d2 = m_testRoot.Axes.GetChild("D2");
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            using (new SecurityDisabler())
            {
                if (m_testRoot != null)
                    m_testRoot.Delete();

                if (m_testTemplateRoot != null)
                    m_testTemplateRoot.Delete();
            }
        }

        [Test]
        public void TemplateIsOrBasedOn_TemplateIs()
        {
            Assert.IsTrue(Mod.ItemExtensions.TemplateIsOrBasedOn(m_d2, m_d2Template));
        }

        [Test]
        public void TemplateIsOrBasedOn_BasedOnDirectly()
        {
            Assert.IsTrue(Mod.ItemExtensions.TemplateIsOrBasedOn(m_d12, m_baseTemplate));
        }

        [Test]
        public void TemplateIsOrBasedOn_BasedOnIndirectly()
        {
            Assert.IsTrue(Mod.ItemExtensions.TemplateIsOrBasedOn(m_d2, m_baseTemplate));
        }

        [Test]
        public void TemplateIsOrBasedOn_NotBasedOn()
        {
            Assert.IsFalse(Mod.ItemExtensions.TemplateIsOrBasedOn(m_testTemplateRoot, m_baseTemplate));
        }

        [Test]
        public void TemplateIsOrBasedOn_NullTemplate()
        {
            Assert.IsFalse(Mod.ItemExtensions.TemplateIsOrBasedOn(m_testTemplateRoot, (TemplateItem)null));
        }

        [Test]
        public void TemplateIsOrBasedOn_NullItem()
        {
            Assert.IsFalse(Mod.ItemExtensions.TemplateIsOrBasedOn((Item)null, m_baseTemplate));
        }

        [Test]
        public void FindItemsByTemplateOrDerivedTemplate_BaseTemplate()
        {
            var items = Mod.ItemExtensions.FindItemsByTemplateOrDerivedTemplate(m_testRoot, m_baseTemplate);
            Assert.AreEqual(5, items.Length);

            var ids = (from item in items select item.ID).ToArray();
            Assert.Contains(m_base1.ID, ids);
            Assert.Contains(m_base2.ID, ids);
            Assert.Contains(m_d11.ID, ids);
            Assert.Contains(m_d12.ID, ids);
            Assert.Contains(m_d2.ID, ids);
        }

        [Test]
        public void FindItemsByTemplateOrDerivedTemplate_DerivedDirectly()
        {
            var items = Mod.ItemExtensions.FindItemsByTemplateOrDerivedTemplate(m_testRoot, m_d1Template);
            Assert.AreEqual(3, items.Length);

            var ids = (from item in items select item.ID).ToArray();
            Assert.Contains(m_d11.ID, ids);
            Assert.Contains(m_d12.ID, ids);
            Assert.Contains(m_d2.ID, ids);
        }

        [Test]
        public void FindItemsByTemplateOrDerivedTemplate_DerivedInDirectly()
        {
            var items = Mod.ItemExtensions.FindItemsByTemplateOrDerivedTemplate(m_testRoot, m_d2Template);
            Assert.AreEqual(1, items.Length);

            var ids = (from item in items select item.ID).ToArray();
            Assert.Contains(m_d2.ID, ids);
        }

        [Test]
        public void FindItemsByTemplateOrDerivedTemplate_NullRoot()
        {
            var items = Mod.ItemExtensions.FindItemsByTemplateOrDerivedTemplate(null, m_baseTemplate);
            Assert.AreEqual(0, items.Length);
        }

        [Test]
        public void FindItemsByTemplateOrDerivedTemplate_NullTemplate()
        {
            var items = Mod.ItemExtensions.FindItemsByTemplateOrDerivedTemplate(m_testRoot, null);
            Assert.AreEqual(0, items.Length);
        }
    }
}