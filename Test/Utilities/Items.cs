using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using System.IO;
using System.Web;
using Mod = Sitecore.Modules.WeBlog.Utilities;

namespace Sitecore.Modules.WeBlog.Test.Utilities
{
    [TestFixture]
    [Category("Items")]
    public class Items
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
            var templateHome = Sitecore.Context.Database.GetItem("/sitecore/templates/user defined");
            var home = Sitecore.Context.Database.GetItem("/sitecore/content/home");
            using (new SecurityDisabler())
            {
                templateHome.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\templates.xml")), false, PasteMode.Overwrite);
                home.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\template content.xml")), false, PasteMode.Overwrite);
            }

            // Retrieve created templates
            m_testTemplateRoot = templateHome.Axes.GetChild("Test Templates");
            m_baseTemplate = Sitecore.Context.Database.GetTemplate("user defined/test templates/base");
            m_d1Template = Sitecore.Context.Database.GetTemplate("user defined/test templates/d1");
            m_d2Template = Sitecore.Context.Database.GetTemplate("user defined/test templates/d2");

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
        public void MakeSafeItemName_AlreadySafe()
        {
            Assert.AreEqual("this is safe", Mod.Items.MakeSafeItemName("this is safe"));
        }

        [Test]
        public void MakeSafeItemName_InvalidChar()
        {
            Assert.AreEqual("now- is- saf-e", Mod.Items.MakeSafeItemName("now@ is+ saf\\e"));
        }

        [Test]
        public void MakeSafeItemName_Email()
        {
            Assert.AreEqual("alistair-deneys-codeflood-net", Mod.Items.MakeSafeItemName("alistair.deneys@codeflood.net"));
        }

        [Test]
        public void MakeSafeItemName_Empty()
        {
            Assert.AreEqual(string.Empty, Mod.Items.MakeSafeItemName(string.Empty));
        }

        [Test]
        public void TemplateIsOrBasedOn_TemplateIs()
        {
            Assert.IsTrue(Mod.Items.TemplateIsOrBasedOn(m_d2, m_d2Template));
        }

        [Test]
        public void TemplateIsOrBasedOn_BasedOnDirectly()
        {
            Assert.IsTrue(Mod.Items.TemplateIsOrBasedOn(m_d12, m_baseTemplate));
        }

        [Test]
        public void TemplateIsOrBasedOn_BasedOnIndirectly()
        {
            Assert.IsTrue(Mod.Items.TemplateIsOrBasedOn(m_d2, m_baseTemplate));
        }

        [Test]
        public void TemplateIsOrBasedOn_NotBasedOn()
        {
            Assert.IsFalse(Mod.Items.TemplateIsOrBasedOn(m_testTemplateRoot, m_baseTemplate));
        }

        [Test]
        public void TemplateIsOrBasedOn_NullTemplate()
        {
            Assert.IsFalse(Mod.Items.TemplateIsOrBasedOn(m_testTemplateRoot, (TemplateItem)null));
        }

        [Test]
        public void TemplateIsOrBasedOn_NullItem()
        {
            Assert.IsFalse(Mod.Items.TemplateIsOrBasedOn((Item)null, m_baseTemplate));
        }

        [Test]
        public void FindItemsByTemplateOrDerivedTemplate_BaseTemplate()
        {
            var items = Mod.Items.FindItemsByTemplateOrDerivedTemplate(m_testRoot, m_baseTemplate);
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
            var items = Mod.Items.FindItemsByTemplateOrDerivedTemplate(m_testRoot, m_d1Template);
            Assert.AreEqual(3, items.Length);

            var ids = (from item in items select item.ID).ToArray();
            Assert.Contains(m_d11.ID, ids);
            Assert.Contains(m_d12.ID, ids);
            Assert.Contains(m_d2.ID, ids);
        }

        [Test]
        public void FindItemsByTemplateOrDerivedTemplate_DerivedInDirectly()
        {
            var items = Mod.Items.FindItemsByTemplateOrDerivedTemplate(m_testRoot, m_d2Template);
            Assert.AreEqual(1, items.Length);

            var ids = (from item in items select item.ID).ToArray();
            Assert.Contains(m_d2.ID, ids);
        }

        [Test]
        public void FindItemsByTemplateOrDerivedTemplate_NullRoot()
        {
            var items = Mod.Items.FindItemsByTemplateOrDerivedTemplate(null, m_baseTemplate);
            Assert.AreEqual(0, items.Length);
        }

        [Test]
        public void FindItemsByTemplateOrDerivedTemplate_NullTemplate()
        {
            var items = Mod.Items.FindItemsByTemplateOrDerivedTemplate(m_testRoot, null);
            Assert.AreEqual(0, items.Length);
        }
    }
}