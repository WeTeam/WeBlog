using System;
using System.Web.UI;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class Blog : BaseSublayout
    {
        #region Page methods
        protected void Page_Load(object sender, EventArgs e)
        {
            fieldtextItem.DataSource = BlogManager.GetCurrentBlog().ID.ToString();
            HyperlinkBlog.NavigateUrl = LinkManager.GetItemUrl(BlogManager.GetCurrentBlog().InnerItem);

            // Add the title to the page
            Page.Title = CurrentBlog.Title.Text;
        }
        #endregion
    }
}