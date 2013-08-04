using Lucene.Net.Search;
using Sitecore.Search;

namespace Sitecore.Modules.WeBlog.Search.Search
{
    public class SortableIndexSearchContext : IndexSearchContext
    {
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
            return this.Search(Parse(query, context), sort);
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
            return this.Search(Prepare(Translate(query), context), sort);
        }
        public SearchHits Search(PreparedQuery query, Sort sort)
        {
            return new SearchHits(Searcher.Search(query.Query, sort));
        }
    }
}
