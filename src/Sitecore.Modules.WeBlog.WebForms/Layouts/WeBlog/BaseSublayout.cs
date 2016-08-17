using System.ComponentModel;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Sharedsource.Web.UI.Sublayouts;
using Sitecore.Sites;
using Sitecore.Web.UI.WebControls;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts
{
    public class BaseSublayout : SublayoutBase
    {
        /// <summary>
        /// Gets or sets the current blog
        /// </summary>
        public BlogHomeItem CurrentBlog
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether caching for this component should vary by blog
        /// </summary>
        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool VaryByBlog
        {
            get;
            set;
        }

        public BaseSublayout()
        {
            CurrentBlog = ManagerFactory.BlogManagerInstance.GetCurrentBlog();
        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            // Alter the parameters so the cache key will be different per blog if caching and vary by blog are configured.
            // This is preferable to using a custom rendering type so the page editor still works.
            UpdateParametersForCaching();
        }

        public void UpdateParametersForCaching()
        {
            if (VaryByBlog)
            {
                var sublayoutWrapper = Parent as Sublayout;
                if (sublayoutWrapper != null)
                {
                    SiteContext site = Sitecore.Context.Site;
                    if (sublayoutWrapper.Cacheable && site != null && site.CacheHtml && CurrentBlog != null)
                    {
                        var key = "CacheVaryByBlogKey=" + CurrentBlog.SafeGet(x => x.ID).SafeGet(x => x.ToShortID()).SafeGet(x => x.ToString());
                        sublayoutWrapper.Parameters += (string.IsNullOrEmpty(sublayoutWrapper.Parameters))
                                                           ? string.Empty
                                                           : "&" + key;
                    }
                }
            }
        }
    }
}