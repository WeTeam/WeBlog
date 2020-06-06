using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.DependencyInjection;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Modules.WeBlog.Search;

namespace Sitecore.Modules.WeBlog.Components
{
    public class PostListCore : IPostListCore
    {
        protected IList<EntryItem> _entries;

        protected IWeBlogSettings Settings = null;

        protected BlogHomeItem CurrentBlog { get; set; }

        protected NameValueCollection QueryString { get; set; }

        protected IEntryManager EntryManager { get; set; }

        protected BaseTemplateManager TemplateManager { get; set; }

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
            get;
            protected set;
        }

        public IEnumerable<EntryItem> Entries
        {
            get
            {
                if (_entries == null)
                    _entries = GetEntries();
                
                return _entries;
            }
        }

        public string ViewMoreHref => BuildViewMoreHref();

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="currentBlog">The blog being listed.</param>
        /// <param name="settings">The settings to use. If null, the default settings will be used.</param>
        /// <param name="authorsCore">The <see cref="IAuthorsCore"/> component to use to retrieve author information.</param>
        /// <param name="entryManager">The <see cref="IEntryManager"/> to use to access entries.</param>
        [Obsolete("Use ctor(BlogHomeItem, IWeBlogSettings, IAuthorsCore, IEntryManager, BaseTemplateManager) instead.")]
        public PostListCore(BlogHomeItem currentBlog, IWeBlogSettings settings = null, IAuthorsCore authorsCore = null, IEntryManager entryManager = null)
            : this(currentBlog, settings, authorsCore, entryManager, null)
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="currentBlog">The blog being listed.</param>
        /// <param name="settings">The settings to use. If null, the default settings will be used.</param>
        /// <param name="authorsCore">The <see cref="IAuthorsCore"/> component to use to retrieve author information.</param>
        /// <param name="entryManager">The <see cref="IEntryManager"/> to use to access entries.</param>
        /// <param name="templateManager">The <see cref="BaseTemplateManager"/> to use to access templates.</param>
        public PostListCore(BlogHomeItem currentBlog, IWeBlogSettings settings = null, IAuthorsCore authorsCore = null, IEntryManager entryManager = null, BaseTemplateManager templateManager = null)
        {
            CurrentBlog = currentBlog;
            Settings = settings ?? WeBlogSettings.Instance;
            AuthorsCore = authorsCore ?? new AuthorsCore(CurrentBlog);
            EntryManager = entryManager ?? ManagerFactory.EntryManagerInstance;
            TemplateManager = templateManager ?? ServiceLocator.ServiceProvider.GetService(typeof(BaseTemplateManager)) as BaseTemplateManager;
        }

        public virtual void Initialize(NameValueCollection queryString)
        {
            QueryString = queryString;

            StartIndex = GetFromQueryString("startIndex");
            int requestedToShow = GetFromQueryString("count");
            TotalToShow = requestedToShow > 0 ? requestedToShow : CurrentBlog.DisplayItemCountNumeric;
            if (TotalToShow == 0)
                TotalToShow = 10;
        }

        protected virtual IList<EntryItem> GetEntries()
        {
            SearchResults<Entry> results = null;
            string tag = QueryString["tag"];
            string sort = QueryString["sort"];
            var author = QueryString["author"];

            var criteria = new EntryCriteria
            {
                PageSize = TotalToShow,
                PageNumber = (StartIndex / TotalToShow) + 1
            };

            if (!string.IsNullOrEmpty(tag))
            {
                criteria.Tag = tag;
                results = EntryManager.GetBlogEntries(CurrentBlog, criteria, ListOrder.Descending);
            }
            else if (author != null)
            {
                // todo: need to implement entry criteria for author
                results = EntryManager.GetBlogEntries(CurrentBlog, EntryCriteria.AllEntries, ListOrder.Descending);

                var authorEntries = results.Results.GroupBy(item => AuthorsCore.GetUserFullName(item.Author))
                    .FirstOrDefault(items => AuthorsCore.GetAuthorIdentity(items.Key) == author).ToList();

                results = new SearchResults<Entry>(authorEntries, false);
            }
            else if (Context.Item.TemplateIsOrBasedOn(TemplateManager, Settings.CategoryTemplateIds))
            {
                criteria.Category = Context.Item.Name;
                results = EntryManager.GetBlogEntries(CurrentBlog, criteria, ListOrder.Descending);
            }
            else if (!string.IsNullOrEmpty(sort))
            {
                var algorithm = InterestingEntriesCore.GetAlgororithmFromString(sort, InterestingEntriesAlgorithm.Custom);
                if (algorithm != InterestingEntriesAlgorithm.Custom)
                {
                    var core = new InterestingEntriesCore(EntryManager, algorithm);
                    var maxCount = TotalToShow + (TotalToShow * StartIndex) + 1;
                    return core.GetEntries(CurrentBlog, maxCount);
                }
                else
                {
                    results = SearchResults<Entry>.Empty;
                }
            }
            else
            {
                results = EntryManager.GetBlogEntries(CurrentBlog, criteria, ListOrder.Descending);
            }

            var entryItems = new List<EntryItem>();

            foreach (var entry in results.Results)
            {
                var item = Database.GetItem(entry.Uri);
                if(item != null)
                    entryItems.Add(new EntryItem(item));
            }

            ShowViewMoreLink = results.HasMoreResults;

            return entryItems;
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

        protected int GetFromQueryString(string key, int defaultValue = 0)
        {
            string rawValue = QueryString[key] ?? defaultValue.ToString();
            int result;
            int.TryParse(rawValue, out result);
            return result;
        }
    }
}