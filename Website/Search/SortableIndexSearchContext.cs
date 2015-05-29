using Lucene.Net.Search;
using Sitecore.Search;

namespace Sitecore.Modules.WeBlog.Search.Search
{
    public class SortableIndexSearchContext : IndexSearchContext
    {
        protected const int DefaultMaximumResults = 500;

        public SortableIndexSearchContext(ILuceneIndex index)
        {
            Initialize(index, true);
        }
        
        public SearchHits Search(string query, Sort sort)
        {
            return Search(query, SearchContext.Empty, sort);
        }
        public SearchHits Search(string query, ISearchContext context, Sort sort)
        {
            return Search(Parse(query, context), sort);
        }
        public SearchHits Search(Query query, Sort sort)
        {
            return Search(query, SearchContext.Empty, sort);
        }
        public SearchHits Search(Query query, ISearchContext context, Sort sort)
        {
            return Search(Prepare(query, context), sort);
        }
        public SearchHits Search(QueryBase query, Sort sort)
        {
            return Search(query, SearchContext.Empty, sort);
        }
        public SearchHits Search(QueryBase query, ISearchContext context, Sort sort)
        {
            return Search(Prepare(Translate(query), context), sort);
        }
        public SearchHits Search(PreparedQuery query, Sort sort, int maxResults = DefaultMaximumResults)
        {
#if SC62 || SC64 || SC66
            return new SearchHits(Searcher.Search(query.Query, sort));
          //return new SearchHits(Searcher.Search(query.Query));
#else
            return new SearchHits(Searcher.Search(query.Query, null, maxResults, sort), Searcher.IndexReader);
#endif
        }
    }
}
