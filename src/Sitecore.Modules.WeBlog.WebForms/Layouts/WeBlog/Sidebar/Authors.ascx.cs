using System;
using System.Linq;
using Sitecore.Modules.WeBlog.Components;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts.Sidebar
{
    public partial class BlogAuthors : BaseSublayout
    {
        public IAuthorsCore AuthorsCore { get; set; }

        public BlogAuthors(IAuthorsCore authorsCore = null)
        {
            AuthorsCore = authorsCore ?? new AuthorsCore(CurrentBlog);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadAuthors();
        }

        public int MaximumCount { get; set; }

        protected virtual void LoadAuthors()
        {
            var authors = AuthorsCore.Users.Take(MaximumCount);
            if (!authors.Any())
            {
                if (PanelAuthors != null)
                {
                    PanelAuthors.Visible = false;
                }
            }
            else
            {
                if (ListViewAuthors != null)
                {
                    ListViewAuthors.DataSource = authors;
                    ListViewAuthors.DataBind();
                }
            }
        }

        protected string GetAuthorUrl(string item)
        {
            return AuthorsCore.GetAuthorUrl(item);
        }
    }
}