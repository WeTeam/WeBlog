using Sitecore.Modules.WeBlog.Caching;

namespace Sitecore.Modules.WeBlog.Pipelines.ProfanityFilter
{
    public class GetProfanityListFromCache : IProfanityFilterProcessor
    {
        public void Process(ProfanityFilterArgs args)
        {
            if (CacheManager.ProfanityFilter.WorList != null)
            {
                args.WordList = CacheManager.ProfanityFilter.WorList;
            }
        }
    }
}