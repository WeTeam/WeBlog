using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Search;

namespace Sitecore.Modules.WeBlog.Components
{
    public class PostListCore : IPostListCore
    {
        protected IEnumerable<EntryItem> _entries;

        protected IWeBlogSettings Settings = null;

        protected BlogHomeItem CurrentBlog { get; set; }

        protected NameValueCollection QueryString { get; set; }

        public IAuthorsCore AuthorsCore { get; set; }

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
            get { return (StartIndex + TotalToShow) < _entries.Count(); }
        }

        public IEnumerable<EntryItem> Entries
        {
            get
            {
                if (_entries == null)
                {
                    _entries = GetEntries();
                }
                return TotalToShow == 0 ? _entries.Skip(StartIndex) : _entries.Skip(StartIndex).Take(TotalToShow);
            }
        }

        public string ViewMoreHref => BuildViewMoreHref();

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="currentBlog">The blog being listed.</param>
        /// <param name="settings">The settings to use. If null, the default settings will be used.</param>
        public PostListCore(BlogHomeItem currentBlog, IWeBlogSettings settings = null, IAuthorsCore authorsCore = null)
        {
            CurrentBlog = currentBlog;
            Settings = settings ?? new WeBlogSettings();
            AuthorsCore = authorsCore ?? new AuthorsCore(CurrentBlog);
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
            var author = QueryString["author"];
            if (!string.IsNullOrEmpty(tag))
            {
                entries = ManagerFactory.EntryManagerInstance.GetBlogEntries(CurrentBlog, int.MaxValue, tag, null, null, null);
            }
            else if (author != null)
            {
                entries = ManagerFactory.EntryManagerInstance.GetBlogEntries(CurrentBlog)
                    .GroupBy(item => AuthorsCore.GetUserFullName(item))
                    .FirstOrDefault(items => AuthorsCore.GetAuthorIdentity(items.Key) == author);
            }
            else if (Context.Item.TemplateIsOrBasedOn(Settings.CategoryTemplateIds))
            {
                entries = ManagerFactory.EntryManagerInstance.GetBlogEntries(CurrentBlog, int.MaxValue, null, Context.Item.Name);
            }
            else if (!string.IsNullOrEmpty(sort))
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
                entries = ManagerFactory.EntryManagerInstance.GetBlogEntries(CurrentBlog);
            }
            return entries ?? new EntryItem[0];
        }

        protected virtual string BuildViewMoreHref()
        {
            string tag = QueryString["tag"];
            string sort = QueryString["sort"];
            var author = QueryString["author"];
            string blogUrl = LinkManager.GetItemUrl(Context.Item);
            var viewMoreHref = blogUrl + "?count=" + (TotalToShow + CurrentBlog.DisplayItemCountNumeric);
            if (tag != null)
            {
                viewMoreHref += "&tag=" + HttpUtility.UrlEncode(tag);
            }
            if (sort != null)
            {
                viewMoreHref += "&sort=" + HttpUtility.UrlEncode(sort);
            }

            if (author != null)
            {
                viewMoreHref += "&author=" + HttpUtility.UrlEncode(author);
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