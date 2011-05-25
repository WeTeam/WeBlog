using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Modules.Blog.Managers;

namespace Sitecore.Modules.Blog.Layouts
{
    public partial class BlogOptions : BaseSublayout
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Sitecore.Context.PageMode.IsPageEditorEditing)
            {
                EditModePanel.Visible = true;
                CheckBoxEnableRSS.Checked = CurrentBlog.EnableRSS.Checked;
                CheckBoxCommentsEmail.Checked = CurrentBlog.ShowEmailWithinComments.Checked;
            }
        }
    }
}