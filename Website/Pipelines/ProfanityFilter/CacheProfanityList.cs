using System;
using System.Linq;
using Sitecore.Modules.WeBlog.Caching;

namespace Sitecore.Modules.WeBlog.Pipelines.ProfanityFilter
{
    public class CacheProfanityList : IProfanityFilterProcessor
    {
        public void Process(ProfanityFilterArgs args)
        {
            if (args.WordList.Any() && CacheManager.ProfanityFilter.WorList == null)
            {
                CacheManager.ProfanityFilter.WorList = args.WordList;
            }
        }
    }
}