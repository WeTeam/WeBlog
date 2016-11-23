using System.Collections.Generic;
using System.Linq;
using Moq;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.ContentSearch.Security;

namespace Sitecore.Modules.WeBlog.UnitTest.Extensions
{
    public static class MockExtensions
    {
        public static void IndexItems<T>(this Mock<ISearchIndex> index, IEnumerable<T> items) where T : SearchResultItem
        {
            index.Setup(i => i.CreateSearchContext(It.IsAny<SearchSecurityOptions>())
            .GetQueryable<T>())
            .Returns(new EnumerableQuery<T>(items));
        }

        public static void IndexItems<T>(this IEnumerable<T> items, Mock<ISearchIndex> index)
            where T : SearchResultItem
        {
            index.IndexItems(items);
        }
    }
}
