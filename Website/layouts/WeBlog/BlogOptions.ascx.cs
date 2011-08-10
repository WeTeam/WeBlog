using System;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class BlogOptions : BaseSublayout
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Sitecore.Context.PageMode.IsPageEditorEditing)
            {
                if(EditModePanel != null)
                    EditModePanel.Visible = true;

                if(CheckBoxEnableRSS != null)
                    CheckBoxEnableRSS.Checked = CurrentBlog.EnableRSS.Checked;

                if(CheckBoxCommentsEmail != null)
                    CheckBoxCommentsEmail.Checked = CurrentBlog.ShowEmailWithinComments.Checked;
            }
        }
    }
}