using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Sitecore.Abstractions;
using Sitecore.Caching;
using Sitecore.Caching.Generics;
using Sitecore.Modules.WeBlog.Caching;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Modules.WeBlog.Search;

namespace Sitecore.Modules.WeBlog.UnitTest.Caching
{
    [TestFixture]
    public class EntrySearchCacheFixture
    {
        [Test]
        public void Ctor_NullCacheManager_DoesNotThrow()
        {
            Action sutAction = () => new EntrySearchCache(null, Mock.Of<IWeBlogSettings>());
            Assert.DoesNotThrow(new TestDelegate(sutAction));
        }

        [Test]
        public void Ctor_NullSettings_DoesNotThrow()
        {
            Action sutAction = () => new EntrySearchCache(Mock.Of<BaseCacheManager>(), null);
            Assert.DoesNotThrow(new TestDelegate(sutAction));
        }

        [Test]
        public void Get_NullCriteria_Throws()
        {
            // arrange
            var sut = new EntrySearchCache();

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(() => sut.Get(null, ListOrder.Descending));
            Assert.That(ex.ParamName, Is.EqualTo("criteria"));
        }

        [Test]
        public void Get_ForCriteriaNotSet_ReturnsNull()
        {
            // arrange
            var sut = new EntrySearchCache();
            var criteria = new EntryCriteria
            {
                PageNumber = 1,
                PageSize = 5
            };

            // act
            var result = sut.Get(criteria, ListOrder.Descending);

            // assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetSet_ForCriteriaWhichHasBeenSet_ReturnsListPreviouslySet()
        {
            // arrange
            var innerCache = new Cache<EntrySearchCacheKey>("test cache", 500)
            {
                Enabled = true
            };

            var cacheManager = Mock.Of<BaseCacheManager>(x => 
                x.GetNamedInstance<EntrySearchCacheKey>(It.IsAny<string>(), It.IsAny<long>(), true) == innerCache
            );

            var sut = new EntrySearchCache(cacheManager);
            var criteria = new EntryCriteria
            {
                PageNumber = 1,
                PageSize = 5
            };

            var list = new List<Entry>
            {
                new Entry
                {
                    Author = "admin",
                    Title = "lorem"
                }
            };

            var results = new SearchResults<Entry>(list, false);

            // act
            sut.Set(criteria, ListOrder.Descending, results);
            var result = sut.Get(criteria, ListOrder.Descending);

            // assert
            Assert.That(result.Results, Is.EquivalentTo(list));
        }

        [Test]
        public void GetSet_ForResultOrderWhichHasBeenSet_ReturnsNull()
        {
            // arrange
            var sut = new EntrySearchCache();
            var criteria = new EntryCriteria
            {
                PageNumber = 1,
                PageSize = 5
            };

            var list = new List<Entry>
            {
                new Entry
                {
                    Author = "admin",
                    Title = "lorem"
                }
            };

            var results = new SearchResults<Entry>(list, false);

            // act
            sut.Set(criteria, ListOrder.Descending, results);
            var result = sut.Get(criteria, ListOrder.Ascending);

            // assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Set_NullCriteria_Throws()
        {
            // arrange
            var sut = new EntrySearchCache();

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(() => sut.Set(null, ListOrder.Descending, SearchResults<Entry>.Empty));
            Assert.That(ex.ParamName, Is.EqualTo("criteria"));
        }

        [Test]
        public void Set_NullEntries_Throws()
        {
            // arrange
            var sut = new EntrySearchCache();

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(() => sut.Set(EntryCriteria.AllEntries, ListOrder.Descending, null));
            Assert.That(ex.ParamName, Is.EqualTo("entries"));
        }

        [Test]
        public void ClearCache_EntriesPreviouslySet_EntriesNoLongerSet()
        {
            // arrange
            var innerCache = new Cache<EntrySearchCacheKey>("test cache", 500)
            {
                Enabled = true
            };

            var cacheManager = Mock.Of<BaseCacheManager>(x =>
                x.GetNamedInstance<EntrySearchCacheKey>(It.IsAny<string>(), It.IsAny<long>(), true) == innerCache
            );

            var sut = new EntrySearchCache(cacheManager);
            var criteria = new EntryCriteria
            {
                PageNumber = 1,
                PageSize = 5
            };

            var list = new List<Entry>
            {
                new Entry
                {
                    Author = "admin",
                    Title = "lorem"
                }
            };

            var results = new SearchResults<Entry>(list, false);

            sut.Set(criteria, ListOrder.Descending, results);

            // act
            sut.ClearCache();

            // assert
            var result = sut.Get(criteria, ListOrder.Descending);
            Assert.That(result, Is.Null);
        }
    }
}
