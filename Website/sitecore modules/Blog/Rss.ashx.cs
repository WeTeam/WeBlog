using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using RssToolkit.Rss;
using System.Web.SessionState;
using Sitecore.Modules.Blog.Managers;
using Sitecore.Data;
using Sitecore.Modules.Blog.Items.Blog;
using Sitecore.Data.Items;

namespace Sitecore.Modules.Blog
{
    public class RssHandler : RssToolkit.Rss.RssDocumentHttpHandler, IRequiresSessionState
    {
        public string blogID = string.Empty;
        public int Total = 0;

        protected override void PopulateRss(string channelName, string userName)
        {
            if (HttpContext.Current.Request.QueryString.ToString() != string.Empty)
            {
                blogID = HttpContext.Current.Request.QueryString["blogid"].ToString();
                Total = System.Convert.ToInt32(HttpContext.Current.Request.QueryString["count"]); ;

                GetBlogEntries(blogID);
            }
            else
            {
                Get10LatestEntries();
            }

        }

        /// <summary>
        /// Gets the blog entries.
        /// </summary>
        /// <param name="blogID">The blog ID.</param>
        private void GetBlogEntries(string blogID)
        {
            Item currentBlogItem = Sitecore.Context.Database.GetItem(blogID);
            if (currentBlogItem != null)
            {
                Sitecore.Modules.Blog.Items.Blog.BlogItem currentBlog = new Sitecore.Modules.Blog.Items.Blog.BlogItem(currentBlogItem);

                Rss.Channel = new RssChannel();
                Rss.Version = "2.0";
                Rss.Channel.Title = currentBlog.Title.Text;
                Rss.Channel.PubDate = currentBlog.InnerItem.Statistics.Created.ToString();
                Rss.Channel.LastBuildDate = DateTime.Now.ToUniversalTime().ToString();
                Rss.Channel.WebMaster = currentBlog.Email.Text;
                Rss.Channel.Description = currentBlog.Title.Text;
                Rss.Channel.Link = HttpContext.Current.Request.RawUrl;

                var currentBlogEntries = EntryManager.GetBlogEntries(currentBlogItem, 10, null, null);
                Rss.Channel.Items = new List<RssItem>();

                foreach (EntryItem entry in currentBlogEntries)
                {
                    RssItem item = new RssItem();
                    item.Title = entry.Title.Text;
                    item.Description = entry.Introduction.Text;
                    item.Link = entry.Url;
                    Rss.Channel.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// Get the 10 latest entries.
        /// </summary>
        private void Get10LatestEntries()
        {
            Rss.Channel = new RssChannel();
            Rss.Version = "2.0";
            Rss.Channel.Title = "The 10 latest entries from all blogs in the " + Sitecore.Context.Site.HostName + " website";
            Rss.Channel.PubDate = Sitecore.Context.Site.DisplayDate.ToUniversalTime().ToString();
            Rss.Channel.LastBuildDate = DateTime.Now.ToUniversalTime().ToString();
            Rss.Channel.WebMaster = "";
            Rss.Channel.Description = "";
            Rss.Channel.Link = HttpContext.Current.Request.RawUrl;

            var LatestBlogEntries = EntryManager.GetBlogEntries(10);
            Rss.Channel.Items = new List<RssItem>();

            foreach (EntryItem entry in LatestBlogEntries)
            {
                RssItem item = new RssItem();
                item.Title = entry.Title.Text;
                item.Description = entry.Introduction.Text;
                item.Link = entry.Url;
                Rss.Channel.Items.Add(item);
            }
        }
    }
}