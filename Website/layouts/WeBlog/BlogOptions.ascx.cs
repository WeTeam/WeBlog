using System;

namespace Sitecore.Modules.WeBlog.Layouts
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