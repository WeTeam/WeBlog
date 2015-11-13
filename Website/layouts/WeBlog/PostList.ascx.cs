﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class BlogPostList : BaseSublayout
    {
        protected const string DEFAULT_POST_TEMPLATE = "/layouts/WeBlog/PostListEntry.ascx";

        protected Size m_imageMaxSize = Size.Empty;

        /// <summary>
        /// Gets or sets the total number of entries to show
        /// </summary>
        public int TotalToShow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the entry index to start binding at.
        /// </summary>
        public int StartIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path to the (override) template for posts in the list.
        /// </summary>
        public string PostTemplate
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(PostTemplate))
            {
                PostTemplate = DEFAULT_POST_TEMPLATE;
            }
            EntryList.ItemTemplate = Page.LoadTemplate(PostTemplate);

            m_imageMaxSize = CurrentBlog.MaximumThumbnailImageSizeDimension;

            string requestedToShowStr = Request.QueryString["count"] ?? "0";
            int requestedToShow = 0;
            int.TryParse(requestedToShowStr, out requestedToShow);
            TotalToShow = requestedToShow > 0 ? requestedToShow : CurrentBlog.DisplayItemCountNumeric;

            string startIndexStr = Request.QueryString["startIndex"] ?? "0";
            int startIndex = 0;
            int.TryParse(startIndexStr, out startIndex);
            StartIndex = startIndex;

            string tag = Request.QueryString["tag"];
            BindEntries(tag);
            string blogUrl = Sitecore.Links.LinkManager.GetItemUrl(Sitecore.Context.Item);

            if (ancViewMore != null)
            {
                ancViewMore.HRef = blogUrl + "?count=" + (TotalToShow + CurrentBlog.DisplayCommentSidebarCountNumeric);

                if (tag != null)
                {
                    ancViewMore.HRef += "&tag=" + Server.UrlEncode(tag);
                }
            }
        }

        protected virtual void BindEntries(string tag)
        {
            var categoryTemplateID = Settings.CategoryTemplateID;
            var categoryTemplate = new TemplateItem(Sitecore.Context.Database.GetItem(categoryTemplateID));

            IEnumerable<EntryItem> entries;
            if (!string.IsNullOrEmpty(tag))
            {
                entries = ManagerFactory.EntryManagerInstance.GetBlogEntries(tag);
                //TODO: Does this logic belong elsewhere? Possibly consolidate title logic somewhere.
                Page.Title = "Posts tagged \"" + tag + "\" | " + CurrentBlog.Title.Text;
            }
            else if (Sitecore.Context.Item.TemplateIsOrBasedOn(categoryTemplate))
            {
                CategoryItem category = Sitecore.Context.Item;
                entries = ManagerFactory.EntryManagerInstance.GetBlogEntryByCategorie(CurrentBlog.ID, Sitecore.Context.Item.Name);
                Page.Title = category.Title.Text + " | " + CurrentBlog.Title.Text;
            }
            else
            {
                entries = ManagerFactory.EntryManagerInstance.GetBlogEntries();
            }
            if (TotalToShow == 0)
            {
                TotalToShow = entries.Count();
            }
            if ((StartIndex + TotalToShow) >= entries.Count())
            {
                if(ancViewMore != null)
                    ancViewMore.Visible = false;
            }
            entries = entries.Skip(StartIndex).Take(TotalToShow);

            if (EntryList != null)
            {
                EntryList.DataSource = entries;
                EntryList.DataBind();
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
                    var imageControl = control as global::Sitecore.Web.UI.WebControls.Image;
                    imageControl.MaxWidth = m_imageMaxSize.Width;
                    imageControl.MaxHeight = m_imageMaxSize.Height;

                    var entry = dataItem.DataItem as EntryItem;
                    if (entry.ThumbnailImage.MediaItem == null)
                        imageControl.Field = "Image";
                }
            }
        }
    }
}