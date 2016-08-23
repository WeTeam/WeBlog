using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Extensions;

namespace Sitecore.Modules.WeBlog.Components
{
    public class EntryTagsCore : IEntryTagsCore
    {
        protected BlogHomeItem CurrentBlog { get; set; }

        public EntryTagsCore(BlogHomeItem currentBlog)
        {
            CurrentBlog = currentBlog;
        }

        /// <summary>
        /// Get the URL for a tag
        /// </summary>
        /// <param name="tag">The tag to get the URL for</param>
        /// <returns>The URL to the tag</returns>
        public virtual string GetTagUrl(string tag)
        {
            return CurrentBlog.InnerItem.GetUrl() + "?tag=" + tag;
        }
    }
}