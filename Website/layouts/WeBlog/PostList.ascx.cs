using System;
using System.Drawing;
using System.Web.UI.WebControls;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Data.Items;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class BlogPostList : BaseSublayout
    {
        protected const string DefaultPostTemplate = "/layouts/WeBlog/PostListEntry.ascx";

        protected Size ImageMaxSize = Size.Empty;
        public IPostListCore PostListCore { get; set; }

        public BlogPostList(IPostListCore postListCore = null)
        {
            PostListCore = postListCore ?? new PostListCore(CurrentBlog);
        }
        /// <summary>
        /// Gets or sets the path to the (override) template for posts in the list.
        /// </summary>
        public string PostTemplate { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            PostListCore.Initialize(Request.QueryString);
            if (string.IsNullOrEmpty(PostTemplate))
            {
                PostTemplate = DefaultPostTemplate;
            }
            EntryList.ItemTemplate = Page.LoadTemplate(PostTemplate);

            ImageMaxSize = CurrentBlog.MaximumThumbnailImageSizeDimension;

            if (EntryList != null)
            {
                EntryList.DataSource = PostListCore.Entries;
                EntryList.DataBind();
            }

            if (ancViewMore != null)
            {
                ancViewMore.Visible = PostListCore.ShowViewMoreLink;
                if (PostListCore.ShowViewMoreLink)
                {
                    ancViewMore.HRef = PostListCore.ViewMoreHref;
                }
            }
        }

        protected void EntryDataBound(object sender, ListViewItemEventArgs args)
        {
            if (args.Item.ItemType == ListViewItemType.DataItem)
            {
                var dataItem = args.Item as ListViewDataItem;
                var control = dataItem.FindControl("EntryImage");
                if (control != null)
                {
                    var imageControl = control as Web.UI.WebControls.Image;
                    imageControl.MaxWidth = ImageMaxSize.Width;
                    imageControl.MaxHeight = ImageMaxSize.Height;

                    var entry = dataItem.DataItem as EntryItem;
                    if (entry.ThumbnailImage.MediaItem == null)
                        imageControl.Field = "Image";
                }
            }
        }
    }
}