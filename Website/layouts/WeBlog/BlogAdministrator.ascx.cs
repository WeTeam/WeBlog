using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Web;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class BlogAdministrator : BaseSublayout
    {
        #region Page Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Sitecore.Context.User.IsAuthenticated && !Page.IsPostBack)
            {
                if(LoggedInPanel != null)
                    LoggedInPanel.Visible = true;

                FillThemeDropDownlist();

                if(Theme != null)
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
            if (Theme != null)
            {
                var db = Sitecore.Context.ContentDatabase ?? Sitecore.Context.Database;

                var themeRoot = db.GetItem(Settings.ThemesRoot);

                if (themeRoot != null)
                {
                    foreach (Item itm in themeRoot.GetChildren())
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
            }
        }
        #endregion
    }
}