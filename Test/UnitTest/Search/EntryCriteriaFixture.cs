using NUnit.Framework;
using Sitecore.Modules.WeBlog.Search;
using System;
using System.Collections.Generic;

namespace Sitecore.Modules.WeBlog.UnitTest.Search
{
    [TestFixture]
    public class EntryCriteriaFixture
    {
        [TestCaseSource(nameof(EqualsDataSource))]
        public void Equals(EntryCriteria criteria, object other, bool expected)
        {
            // act
            var result = criteria.Equals(other);

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }

        private static IEnumerable<TestCaseData> EqualsDataSource => new[]
        {
            new TestCaseData(
                new EntryCriteria(),
                new object(),
                false).SetName("Equals_NonEntryCriteriaObject_NotEquals"),

            new TestCaseData(
                new EntryCriteria
                {
                    PageNumber = 4
                },
                new EntryCriteria
                {
                    PageNumber = 4
                },
                true).SetName("Equals_PageNumbersEqual_Equals"),

            new TestCaseData(
                new EntryCriteria
                {
                    PageNumber = 4
                },
                new EntryCriteria
                {
                    PageNumber = 3
                },
                false).SetName("Equals_PageNumbersNotEqual_NotEquals"),

            new TestCaseData(
                new EntryCriteria(),
                new EntryCriteria
                {
                    PageNumber = 3
                },
                false).SetName("Equals_PageNumberNotPopulated_NotEquals"),

            new TestCaseData(
                new EntryCriteria
                {
                    PageSize = 4
                },
                new EntryCriteria
                {
                    PageSize = 4
                },
                true).SetName("Equals_PageSizeEqual_Equals"),

            new TestCaseData(
                new EntryCriteria
                {
                    PageSize = 4
                },
                new EntryCriteria
                {
                    PageSize = 2
                },
                false).SetName("Equals_PageSizeNotEqual_NotEquals"),

            new TestCaseData(
                new EntryCriteria(),
                new EntryCriteria
                {
                    PageSize = 2
                },
                false).SetName("Equals_PageSizeNotPopulated_NotEquals"),

            new TestCaseData(
                new EntryCriteria
                {
                    Tag = "tag"
                },
                new EntryCriteria
                {
                    Tag = "tag"
                },
                true).SetName("Equals_TagEqual_Equals"),

            new TestCaseData(
                new EntryCriteria
                {
                    Tag = "tag"
                },
                new EntryCriteria
                {
                    Tag = ""
                },
                false).SetName("Equals_TagNotEqual_NotEquals"),

            new TestCaseData(
                new EntryCriteria(),
                new EntryCriteria
                {
                    Tag = "tag"
                },
                false).SetName("Equals_TagNotPopulated_NotEquals"),

            new TestCaseData(
                new EntryCriteria
                {
                    Category = "cat"
                },
                new EntryCriteria
                {
                    Category = "cat"
                },
                true).SetName("Equals_CategoryEqual_Equals"),

            new TestCaseData(
                new EntryCriteria
                {
                    Category = "cat"
                },
                new EntryCriteria
                {
                    Category = ""
                },
                false).SetName("Equals_CategoryNotEqual_NotEquals"),

            new TestCaseData(
                new EntryCriteria(),
                new EntryCriteria
                {
                    Category = "cat"
                },
                false).SetName("Equals_CategoryNotPopulated_NotEquals"),

            new TestCaseData(
                new EntryCriteria
                {
                    MinimumDate = new DateTime(2012, 2, 4)
                },
                new EntryCriteria
                {
                    MinimumDate = new DateTime(2012, 2, 4)
                },
                true).SetName("Equals_MinimumDateEqual_Equals"),

            new TestCaseData(
                new EntryCriteria
                {
                    MinimumDate = new DateTime(2012, 2, 3)
                },
                new EntryCriteria
                {
                    MinimumDate = new DateTime(2012, 2, 4)
                },
                false).SetName("Equals_MinimumDateNotEqual_NotEquals"),

            new TestCaseData(
                new EntryCriteria(),
                new EntryCriteria
                {
                    MinimumDate = new DateTime(2012, 2, 4)
                },
                false).SetName("Equals_MinimumDateNotPopulated_NotEquals"),

            new TestCaseData(
                new EntryCriteria
                {
                    MaximumDate = new DateTime(2012, 2, 4)
                },
                new EntryCriteria
                {
                    MaximumDate = new DateTime(2012, 2, 4)
                },
                true).SetName("Equals_MaximumDateEqual_Equals"),

            new TestCaseData(
                new EntryCriteria
                {
                    MaximumDate = new DateTime(2012, 2, 3)
                },
                new EntryCriteria
                {
                    MaximumDate = new DateTime(2012, 2, 4)
                },
                false).SetName("Equals_MaximumDateNotEqual_NotEquals"),

            new TestCaseData(
                new EntryCriteria(),
                new EntryCriteria
                {
                    MaximumDate = new DateTime(2012, 2, 4)
                },
                false).SetName("Equals_MaximumDateNotPopulated_NotEquals"),

            new TestCaseData(
                new EntryCriteria
                {
                    PageNumber = 3,
                    PageSize = 10,
                    Tag = "tag",
                    Category = "cat",
                    MinimumDate = new DateTime(2020, 11, 20),
                    MaximumDate = new DateTime(2020, 12, 20)
                },
                new EntryCriteria
                {
                    PageNumber = 3,
                    PageSize = 10,
                    Tag = "tag",
                    Category = "cat",
                    MinimumDate = new DateTime(2020, 11, 20),
                    MaximumDate = new DateTime(2020, 12, 20)
                },
                true).SetName("Equals_AllPropertiesEqual_Equals"),

            new TestCaseData(
                new EntryCriteria
                {
                    PageNumber = 3,
                    PageSize = 10,
                    Tag = "tag",
                    Category = "cat",
                    MinimumDate = new DateTime(2020, 11, 20),
                    MaximumDate = new DateTime(2020, 12, 20)
                },
                new EntryCriteria
                {
                    PageNumber = 3,
                    PageSize = 10,
                    Tag = "tag",
                    Category = "cat",
                    MinimumDate = new DateTime(2020, 11, 20),
                    MaximumDate = new DateTime(2020, 12, 21)
                },
                false).SetName("Equals_AllPropertiesNotEqual_NotEquals")
        };

        [Test]
        public void GetHashCode_NothingSet_ReturnsZero()
        {
            // arrange
            var sut = new EntryCriteria();

            // act
            var hashcode = sut.GetHashCode();

            // assert
            Assert.That(hashcode, Is.EqualTo(0));
        }

        [Test]
        public void GetHashCode_SameProperties_SameHashCode()
        {
            // arrange
            var sut1 = new EntryCriteria
            {
                PageNumber = 2,
                PageSize = 10,
                Tag = "tag"
            };

            var sut2 = new EntryCriteria
            {
                PageNumber = 2,
                PageSize = 10,
                Tag = "tag"
            };

            // act
            var hashcode1 = sut1.GetHashCode();
            var hashcode2 = sut2.GetHashCode();

            // assert
            Assert.That(hashcode1, Is.EqualTo(hashcode2));
        }

        [Test]
        public void GetHashCode_DifferentProperties_DifferentHashCode()
        {
            // arrange
            var sut1 = new EntryCriteria
            {
                PageNumber = 2,
                PageSize = 10,
                Tag = "tag"
            };

            var sut2 = new EntryCriteria
            {
                PageNumber = 2,
                PageSize = 10,
                Tag = "tag",
                Category = "cat"
            };

            // act
            var hashcode1 = sut1.GetHashCode();
            var hashcode2 = sut2.GetHashCode();

            // assert
            Assert.That(hashcode1, Is.Not.EqualTo(hashcode2));
        }
    }
}
