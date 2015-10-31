using System.Collections.Generic;

namespace Sitecore.Modules.WeBlog.Components.TagCloud
{
    public interface ITagCloudCore
    {
        Dictionary<string, int> Tags { get; set; }
        string GetTagWeightClass(int tagWeight);
        string GetTagUrl(string tag);
    }
}