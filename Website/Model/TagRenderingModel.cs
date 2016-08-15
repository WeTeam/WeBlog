namespace Sitecore.Modules.WeBlog.Model
{
    public class TagRenderingModel : Tag
    {
        public string Url { get; set; }
        public string Weight { get; set; }

        public TagRenderingModel(Tag tag)
        {
            Name = tag.Name;
            Count = tag.Count;
            LastUsed = tag.LastUsed;
        }
    }
}