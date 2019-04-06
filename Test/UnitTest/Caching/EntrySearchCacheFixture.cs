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
            var sut = new EntrySearchCache(null, Mock.Of<IWeBlogSettings>());
        }

        [Test]
        public void Ctor_NullSettings_DoesNotThrow()
        {
            var sut = new EntrySearchCache(Mock.Of<BaseCacheManager>(), null);
        }

        [Test]
        public void Get_NullCriteria_Throws()
        {
            // arrange
            var sut = new EntrySearchCache();

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(() => sut.Get(null));
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
            var result = sut.Get(criteria);

            // assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetSet_ForCriteriaWhichHasBeenSet_ReturnsListPreviouslySet()
        {
            // arrange
            var innerCache = new Cache<EntryCriteria>("test cache", 500)
            {
                Enabled = true
            };

            var cacheManager = Mock.Of<BaseCacheManager>(x => 
                x.GetNamedInstance<EntryCriteria>(It.IsAny<string>(), It.IsAny<long>(), true) == innerCache
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

            // act
            sut.Set(criteria, list);
            var result = sut.Get(criteria);

            // assert
            Assert.That(result, Is.EquivalentTo(list));
        }

        [Test]
        public void Set_NullCriteria_Throws()
        {
            // arrange
            var sut = new EntrySearchCache();
            var list = new List<Entry>();

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(() => sut.Set(null, list));
            Assert.That(ex.ParamName, Is.EqualTo("criteria"));
        }

        [Test]
        public void Set_NullEntries_Throws()
        {
            // arrange
            var sut = new EntrySearchCache();

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(() => sut.Set(EntryCriteria.AllEntries, null));
            Assert.That(ex.ParamName, Is.EqualTo("entries"));
        }
    }
}
