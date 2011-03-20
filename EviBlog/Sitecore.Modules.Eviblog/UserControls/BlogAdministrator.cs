using System;
using System.Security.Authentication;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Links;
using Sitecore.Modules.Eviblog.Managers;
using Sitecore.Security.Authentication;
using Sitecore.Web;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.Configuration;
using Sitecore.Collections;
using Sitecore.Modules.Eviblog.Utilities;

namespace Sitecore.Modules.Eviblog.UserControls
{
    public class BlogAdministrator : UserControl
    {
        #region Fields

        protected Panel AnonymousPanel;
        protected Panel ErrorPanel;
        protected Literal LiteralError;
        protected Panel LoggedInPanel;
        protected DropDownList Theme;
        public string Password;
        protected Web.UI.WebControls.Text titleAdministration;
        protected TextBox txtPassword;
        protected TextBox txtUserName;
        public string UserName;
        public Sitecore.Modules.Eviblog.Items.Blog currentBlog = BlogManager.GetCurrentBlog();
        public Sitecore.Modules.Eviblog.Items.Settings blogSettings = BlogManager.GetCurrentBlogSettings();

        #endregion

        #region Page Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            titleAdministration.Item = BlogManager.GetCurrentBlogItem();

            if (Sitecore.Context.User.IsAuthenticated && !Page.IsPostBack)
            {
                LoggedInPanel.Visible = true;

                FillThemeDropDownlist();

                Theme.SelectedIndexChanged += new EventHandler(Theme_SelectedIndexChanged);
            }
        }

        void Theme_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentBlog.BeginEdit();
            currentBlog.Theme = Theme.SelectedValue;
            currentBlog.EndEdit();
            
            WebUtil.ReloadPage();
        }

        private void FillThemeDropDownlist()
        {
            Database current;

            if (Sitecore.Context.PageMode.IsPageEditor)
                current = Factory.GetDatabase("web");
            else
                current = Sitecore.Context.Database;
            
            ChildList currentThemes = current.GetItem("/sitecore/system/Modules/EviBlog/Themes").Children;

            foreach (Item itm in currentThemes)
            {
                ListItem listItem = new ListItem();
                listItem.Text = itm.Name;
                listItem.Value = itm.ID.ToString();
                if (itm.ID.ToString() == currentBlog.Theme)
                {
                    listItem.Selected = true;
                }
                Theme.Items.Add(listItem);
            }
        }
        #endregion
    }
}