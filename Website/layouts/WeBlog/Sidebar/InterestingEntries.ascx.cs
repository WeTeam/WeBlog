using System;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Search;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class BlogInterestingEntries : BaseSublayout
    {
        private string m_title = string.Empty;

        /// <summary>
        /// Gets or sets the title to display. If empty, reads the title from the dictionary
        /// </summary>
        public string Title
        {
            get
            {
                if (!string.IsNullOrEmpty(m_title))
                    return m_title;
                else
                    return Sitecore.Modules.WeBlog.Globalization.Translator.Render("POPULAR_POSTS");
            }

            set
            {
                m_title = value;
            }
        }

        /// <summary>
        /// Gets or sets the algorithm to use for selecting items to display
        /// </summary>
        public InterestingEntriesAlgorithm Algorithm
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the algorithm implementation to use. Only used when Algorithm = InterestingEntriesAlgorithm.Custom
        /// </summary>
        public IInterstingEntriesAlgorithm CustomAlgorithm
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the algorithm to use in a textual fashion
        /// </summary>
        public string Mode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum number of items to show
        /// </summary>
        public int MaximumCount
        {
            get;
            set;
        }

        public BlogInterestingEntries()
        {
            MaximumCount = 10;
            Algorithm = InterestingEntriesAlgorithm.Comments;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetProperties();
            PopulateList();
        }

        protected virtual void SetProperties()
        {
            if (!string.IsNullOrEmpty(Mode))
            {
                try
                {
                    Algorithm = (InterestingEntriesAlgorithm)Enum.Parse(typeof(InterestingEntriesAlgorithm), Mode);
                }
                catch (ArgumentException ex)
                {
                    Log.Warn("Failed to parse Mode as InterestingEntriesAlgorithm: " + Mode, ex, this);
                }
            }
        }

        protected virtual void PopulateList()
        {
            var blogItem = ManagerFactory.BlogManagerInstance.GetCurrentBlog();

            switch (Algorithm)
            {
                case InterestingEntriesAlgorithm.Comments:
                    ItemList.DataSource = ManagerFactory.EntryManagerInstance.GetPopularEntriesByComment(blogItem, MaximumCount);
                    break;

                case InterestingEntriesAlgorithm.PageViews:
                    ItemList.DataSource = ManagerFactory.EntryManagerInstance.GetPopularEntriesByView(blogItem, MaximumCount);
                    break;

                case InterestingEntriesAlgorithm.Custom:
                    ItemList.DataSource = CustomAlgorithm.GetEntries(blogItem, MaximumCount);
                    break;
            }

            ItemList.DataBind();
        }
    }
}