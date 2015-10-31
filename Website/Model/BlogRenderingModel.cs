using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.Model
{
    public class BlogRenderingModel
    {
        public Item BlogItem { get; set; }
        public string Hyperlink { get; set; }
    }
}