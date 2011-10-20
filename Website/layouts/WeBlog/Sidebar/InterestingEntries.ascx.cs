using System;
using Sitecore.Modules.WeBlog.Search;
using Sitecore.Modules.WeBlog.Utilities;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Diagnostics;

namespace Sitecore.Modules.WeBlog.layouts.WeBlog
{
    public partial class BlogInterestingEntries : System.Web.UI.UserControl
    {
        private string m_title = string.Empty;

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

        public InterestingEntriesAlgorithm Algorithm
        {
            get;
            set;
        }

        public IInterstingEntriesAlgorithm CustomAlgorithm
        {
            get;
            set;
        }

        public string Mode
        {
            get;
            set;
        }

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
            if (!Page.IsPostBack)
            {
                SetProperties();
                PopulateList();
            }
        }

        protected virtual void SetProperties()
        {
            var helper = new SublayoutParamHelper(this, true);

            if (!string.IsNullOrEmpty(Mode))
            {
                try
                {
                    Algorithm = (InterestingEntriesAlgorithm)Enum.Parse(typeof(InterestingEntriesAlgorithm), Mode);
                }
                catch (ArgumentException ex)
                {
                    Log.Warn("Failed to parse Mode as InterestingEntriesAlgorithm: " + Mode, this);
                }
            }
        }

        protected virtual void PopulateList()
        {
            var blogItem = BlogManager.GetCurrentBlog();

            switch (Algorithm)
            {
                case InterestingEntriesAlgorithm.Comments:
                    Items.DataSource = EntryManager.GetPopularEntriesByComment(blogItem, MaximumCount);
                    break;

                case InterestingEntriesAlgorithm.PageViews:
                    Items.DataSource = EntryManager.GetPopularEntriesByView(blogItem, MaximumCount);
                    break;

                case InterestingEntriesAlgorithm.Custom:
                    Items.DataSource = CustomAlgorithm.GetEntries(blogItem, MaximumCount);
                    break;
            }

            Items.DataBind();
        }
    }
}