using Sitecore.Sites;

namespace Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel
{
    /// <summary>
    /// A processor for the weBlogPopulateScribanMailActionModel pipeline which adds the current site to the model.
    /// </summary>
    public class AddSite
    {
        /// <summary>
        /// The key used to identify the model.
        /// </summary>
        public const string ModelKey = "site";

        private SiteContext _site = null;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        public AddSite()
        {
        }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="site">The site to add to the model.</param>
        public AddSite(SiteContext site)
        {
            _site = site;
        }

        public void Process(PopulateScribanMailActionModelArgs args)
        {
            args.AddModel(ModelKey, _site ?? Context.Site);
        }
    }
}