using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Modules.Blog.Managers;
using Sitecore.Data;
using Sitecore.Web;
using Sitecore.Collections;
using Sitecore.Data.Items;
using Sitecore.Configuration;

namespace Sitecore.Modules.Blog.Layouts
{
    public partial class BlogAdministrator : BaseSublayout
    {
        #region Page Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            titleAdministration.Item = CurrentBlog.InnerItem;

            if (Sitecore.Context.User.IsAuthenticated && !Page.IsPostBack)
            {
                LoggedInPanel.Visible = true;

                FillThemeDropDownlist();

                Theme.SelectedIndexChanged += new EventHandler(Theme_SelectedIndexChanged);
            }
        }

        void Theme_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentBlog.BeginEdit();
            CurrentBlog.Theme.Field.Value = Theme.SelectedValue;
            CurrentBlog.EndEdit();
            WebUtil.ReloadPage();
        }

        private void FillThemeDropDownlist()
        {
            Database current;

            if (Sitecore.Context.PageMode.IsPageEditor)
                current = Factory.GetDatabase("web");
            else
                current = Sitecore.Context.Database;
            
            Item themeRoot = current.GetItem(Sitecore.Configuration.Settings.GetSetting("Blog.ThemesRoot"));

            if(themeRoot != null)

            foreach (Item itm in themeRoot.Children)
            {
                ListItem listItem = new ListItem();
                listItem.Text = itm.Name;
                listItem.Value = itm.ID.ToString();
                if (itm.ID.ToString() == CurrentBlog.Theme.Raw)
                {
                    listItem.Selected = true;
                }
                Theme.Items.Add(listItem);
            }
        }
        #endregion
    }
}