using Sitecore.Abstractions;
using Sitecore.DependencyInjection;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using System;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts
{
    [AllowDependencyInjection]
    public partial class Blog : BaseSublayout
    {
        protected IBlogManager BlogManager { get; }

        protected BaseLinkManager LinkManager { get; }

        protected ThemeItem ThemeItem
        {
            get
            {
                return CurrentBlog.Theme.Item;
            }
        }

        public Blog(IBlogManager blogManager, BaseLinkManager linkManager)
        {
            BlogManager = blogManager;
            LinkManager = linkManager;
        }

        public Blog()
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var currentBlog = BlogManager.GetCurrentBlog();

            if (FieldTextItem != null)
                FieldTextItem.DataSource = currentBlog?.ID.ToString();

            if (HyperlinkBlog != null)
                HyperlinkBlog.NavigateUrl = LinkManager.GetItemUrl(currentBlog);

            // Add the title to the page
            Page.Title = CurrentBlog.Title.Text;
        }
    }
}