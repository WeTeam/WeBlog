using System;
using System.Web.UI;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class Blog : BaseSublayout
    {
        #region Page methods
        protected void Page_Load(object sender, EventArgs e)
        {
            if (fieldtextItem != null)
                fieldtextItem.DataSource = ManagerFactory.BlogManagerInstance.GetCurrentBlog().SafeGet(x => x.ID).SafeGet(x => x.ToString());

            if (HyperlinkBlog != null)
                HyperlinkBlog.NavigateUrl = ManagerFactory.BlogManagerInstance.GetCurrentBlog().SafeGet(x => x.Url);

            // Add the title to the page
            Page.Title = CurrentBlog.Title.Text;
        }
        #endregion
    }
}