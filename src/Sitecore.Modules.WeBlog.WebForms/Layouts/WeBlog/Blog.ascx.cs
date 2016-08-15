using System;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts.WeBlog
{
    public partial class Blog : BaseSublayout
    {
        protected ThemeItem ThemeItem
        {
            get
            {
                return CurrentBlog.Theme.Item;
            }
        }

        #region Page methods
        protected void Page_Load(object sender, EventArgs e)
        {
            if (FieldTextItem != null)
                FieldTextItem.DataSource = ManagerFactory.BlogManagerInstance.GetCurrentBlog().SafeGet(x => x.ID).SafeGet(x => x.ToString());

            if (HyperlinkBlog != null)
                HyperlinkBlog.NavigateUrl = ManagerFactory.BlogManagerInstance.GetCurrentBlog().SafeGet(x => x.Url);

            // Add the title to the page
            Page.Title = CurrentBlog.Title.Text;
        }
        #endregion
    }
}