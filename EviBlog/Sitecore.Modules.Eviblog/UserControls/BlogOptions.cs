using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Modules.Eviblog.Managers;

namespace Sitecore.Modules.Eviblog.UserControls
{
    public class BlogOptions : UserControl
    {
        #region Fields

        protected CheckBox CheckBoxCommentsEmail;
        protected CheckBox CheckBoxEnableComments;
        protected CheckBox CheckBoxEnableRSS;
        public Items.Blog currentBlog = BlogManager.GetCurrentBlog();
        protected Panel EditModePanel;

        #endregion

        #region Page Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Sitecore.Context.PageMode.IsPageEditorEditing)
            {
                EditModePanel.Visible = true;

                CheckBoxEnableRSS.Checked = currentBlog.EnableRSS;
                CheckBoxCommentsEmail.Checked = currentBlog.ShowEmailWithinComments;
            }
        }

        #endregion
    }
}