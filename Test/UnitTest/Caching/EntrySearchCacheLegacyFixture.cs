using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Modules.WeBlog.Search;

namespace Sitecore.Modules.WeBlog.UnitTest.Caching
{
    [TestFixture]
    public class EntrySearchCacheFixture
    {
        [Test]
        public void Ctor_NullSettings_DoesNotThrow()
        {
            Action sutAction = () => new EntrySearchCache(null);
            Assert.DoesNotThrow(new TestDelegate(sutAction));
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

        [Test]
        public void ClearCache_EntriesPreviouslySet_EntriesNoLongerSet()
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

            sut.Set(criteria, list);

            // act
            sut.ClearCache();

            // assert
            var result = sut.Get(criteria);
            Assert.That(result, Is.Null);
        }
    }
}
