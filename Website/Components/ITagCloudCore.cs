using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.Components
{
    public interface ITagCloudCore
    {
        Tag[] Tags { get; set; }
        string[] GetSortNames(string sortingOptions);
        string GetTagWeightClass(int tagWeight);
        string GetTagUrl(string tag);
    }
}