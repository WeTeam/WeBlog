using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Sitecore.Links;
using Sitecore.Modules.Eviblog.Items;
using Sitecore.Modules.Eviblog.Managers;
using Sitecore.Modules.Eviblog.Utilities;

namespace Sitecore.Modules.Eviblog.UserControls
{
    public class BlogPostList : UserControl
    {

        #region Fields

        public Items.Blog currentBlog = BlogManager.GetCurrentBlog();
        protected ListView ListView1;
        protected HtmlAnchor ancViewMore;

        #endregion

        #region Page Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            string totalToShowStr = Request.QueryString["count"] ?? currentBlog.DisplayItemCount.ToString();
            int totalToShow = 0;
            int.TryParse(totalToShowStr, out totalToShow);

            string startIndexStr = Request.QueryString["startIndex"] ?? "0";
            int startIndex = 0;
            int.TryParse(startIndexStr, out startIndex);

            string tag = Request.QueryString["tag"];
            BindEntries(startIndex, totalToShow, tag);

            string blogUrl = Sitecore.Links.LinkManager.GetItemUrl(Sitecore.Context.Item);
            ancViewMore.HRef = blogUrl + "?count=" + (totalToShow + currentBlog.DisplayItemCount);
            if (tag != null)
            {
                ancViewMore.HRef += "&tag=" + Server.UrlEncode(tag);
            }
        }

        #endregion

        #region Eventhandlers

        private void ListView1_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            Entry objEntry = (Entry) ((ListViewDataItem) e.Item).DataItem;

            PlaceHolder PostedDate = (PlaceHolder) e.Item.FindControl("PostedDate");
            PostedDate.Controls.Add(new LiteralControl(objEntry.Created.ToString("dddd, MMMM d, yyyy")));
            PlaceHolder PostedBy = (PlaceHolder)e.Item.FindControl("PostedBy");
            PostedBy.Controls.Add(new LiteralControl(objEntry.CreatedBy.LocalName));

            HyperLink postTitle = (HyperLink)e.Item.FindControl("lnkBlogPostTitle");
            postTitle.Text = objEntry.Title;
            postTitle.NavigateUrl = objEntry.Url;

			Literal txtIntroduction = (Literal)e.Item.FindControl("txtIntroduction");
			txtIntroduction.Text = objEntry.Introduction;

            HyperLink postLink = (HyperLink) e.Item.FindControl("BlogPostLink");
            postLink.NavigateUrl = objEntry.Url;

            if (!objEntry.DisableComments)
            {
                PlaceHolder commentsPlaceholder = (PlaceHolder) e.Item.FindControl("CommentsPlaceholder");
                commentsPlaceholder.Controls.Add(
                    new LiteralControl("Comments " + "(" + CommentManager.GetCommentsCount(objEntry.ID) + ")"));
            }
        }

        #endregion

        #region Utility

        private void BindEntries(int startIndex, int totalToShow, string tag)
        {
            IEnumerable<Entry> entries;
            if (Sitecore.Context.Item.TemplateID == ItemConstants.Templates.Category)
            {
                entries = EntryManager.GetBlogEntryByCategorie(Sitecore.Context.Item.ID);
            }
            else if (tag.HasValue())
            {
                entries = EntryManager.GetBlogEntries(tag);
            }
            else
            {
                entries = EntryManager.GetBlogEntries(Sitecore.Context.Item.ID);
            }
            if (totalToShow == 0)
            {
                totalToShow = entries.Count();
            }
            if ((startIndex + totalToShow) >= entries.Count())
            {
                ancViewMore.Visible = false;
            }
            entries = entries.Skip(startIndex).Take(totalToShow);
            ListView1.DataSource = entries;
            ListView1.ItemDataBound += ListView1_ItemDataBound;
            ListView1.DataBind();
        }

        #endregion
    }
}