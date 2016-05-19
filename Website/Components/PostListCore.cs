using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Search;

namespace Sitecore.Modules.WeBlog.Components
{
    public class PostListCore : IPostListCore
    {
        protected IEnumerable<EntryItem> entries;

        protected IWeBlogSettings Settings = null;

        protected BlogHomeItem CurrentBlog { get; set; }

        protected NameValueCollection QueryString { get; set; }

        /// <summary>
        /// Gets or sets the total number of entries to show
        /// </summary>
        public int TotalToShow { get; set; }

        /// <summary>
        /// Gets or sets the entry index to start binding at.
        /// </summary>
        public int StartIndex { get; set; }

        public bool ShowViewMoreLink
        {
            get { return (StartIndex + TotalToShow) < entries.Count(); }
        }

        public IEnumerable<EntryItem> Entries
        {
            get
            {
                if (entries == null)
                {
                    entries = GetEntries();
                }
                return TotalToShow == 0 ? entries.Skip(StartIndex) : entries.Skip(StartIndex).Take(TotalToShow);
            }
        }

        public string ViewMoreHref
        {
            get { return BuildViewMoreHref(); }
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="currentBlog">The blog being listed.</param>
        /// <param name="settings">The settings to use. If null, the default settings will be used.</param>
        public PostListCore(BlogHomeItem currentBlog, IWeBlogSettings settings = null)
        {
            CurrentBlog = currentBlog;
            Settings = settings ?? new WeBlogSettings();
        }

        public virtual void Initialize(NameValueCollection queryString)
        {
            QueryString = queryString;

            StartIndex = GetFromQueryString("startIndex");
            int requestedToShow = GetFromQueryString("count");
            TotalToShow = requestedToShow > 0 ? requestedToShow : CurrentBlog.DisplayItemCountNumeric;
        }

        protected virtual IEnumerable<EntryItem> GetEntries()
        {
            IEnumerable<EntryItem> entries;
            string tag = QueryString["tag"];
            string sort = QueryString["sort"];
            if (!String.IsNullOrEmpty(tag))
            {
                entries = ManagerFactory.EntryManagerInstance.GetBlogEntries(tag);
            }
            else if (Context.Item.TemplateIsOrBasedOn(Settings.CategoryTemplateIds))
            {
                entries = ManagerFactory.EntryManagerInstance.GetBlogEntryByCategorie(CurrentBlog.ID, Context.Item.Name);
            }
            else if (!String.IsNullOrEmpty(sort))
            {
                var algorithm = InterestingEntriesCore.GetAlgororithmFromString(sort, InterestingEntriesAlgorithm.Custom);
                if (algorithm != InterestingEntriesAlgorithm.Custom)
                {
                    var core = new InterestingEntriesCore(ManagerFactory.EntryManagerInstance, algorithm);
                    entries = core.GetEntries(CurrentBlog, int.MaxValue);
                }
                else
                {
                    entries = new EntryItem[0];
                }
            }
            else
            {
                entries = ManagerFactory.EntryManagerInstance.GetBlogEntries();
            }
            return entries;
        }

        protected virtual string BuildViewMoreHref()
        {
            string tag = QueryString["tag"];
            string sort = QueryString["sort"];
            string blogUrl = Links.LinkManager.GetItemUrl(Context.Item);
            var viewMoreHref = blogUrl + "?count=" + (TotalToShow + CurrentBlog.DisplayItemCountNumeric);
            if (tag != null)
            {
                viewMoreHref += "&tag=" + HttpUtility.UrlEncode(tag);
            }
            if (sort != null)
            {
                viewMoreHref += "&sort=" + HttpUtility.UrlEncode(sort);
            }
            return viewMoreHref;
        }

        [Obsolete("Use the templates defined in settings.")]
        protected virtual TemplateItem GetCategoryTemplate()
        {
            var categoryTemplateId = Settings.CategoryTemplateIds.First();
            var categoryTemplate = new TemplateItem(Context.Database.GetItem(categoryTemplateId));
            return categoryTemplate;
        }

        protected int GetFromQueryString(string key, int defaultValue = 0)
        {
            string rawValue = QueryString[key] ?? defaultValue.ToString();
            int result;
            int.TryParse(rawValue, out result);
            return result;
        }
    }
}