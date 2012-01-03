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
            if(fieldtextItem != null)
                fieldtextItem.DataSource = ManagerFactory.BlogManagerInstance.GetCurrentBlog().ID.ToString();

            if(HyperlinkBlog != null)
                HyperlinkBlog.NavigateUrl = LinkManager.GetItemUrl(ManagerFactory.BlogManagerInstance.GetCurrentBlog().InnerItem);

            // Add the title to the page
            Page.Title = CurrentBlog.Title.Text;
        }
        #endregion
    }
}