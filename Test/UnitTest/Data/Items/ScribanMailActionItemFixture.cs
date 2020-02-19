using NUnit.Framework;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Data.Items;
using System;
using System.Collections.Generic;

namespace Sitecore.Modules.WeBlog.UnitTest.Data.Items
{
    [TestFixture]
    public class ScribanMailActionItemFixture
    {
        [Test]
        public void ImplicitOperatorFromItem_NullItem_ReturnsNull()
        {
            // arrange, act
            ScribanMailActionItem item = (Item)null;

            // assert
            Assert.That(item, Is.Null);
        }

        [Test]
        public void ImplicitOperatorFromItem_ValidItem_ReturnsScribanMailActionItem()
        {
            // arrange
            var item = ItemFactory.CreateItem().Object;

            // act
            ScribanMailActionItem sut = item;

            // assert
            Assert.That(sut.InnerItem, Is.EqualTo(item));
        }

        [Test]
        public void ImplicitOperatorFromScribanMailActionItem_NullItem_ReturnsNull()
        {
            // arrange, act
            Item item = (ScribanMailActionItem)null;

            // assert
            Assert.That(item, Is.Null);
        }

        [Test]
        public void ImplicitOperatorFromScribanMailActionItem_ValidItem_ReturnsItem()
        {
            // arrange
            var item = ItemFactory.CreateItem().Object;
            var sut = new ScribanMailActionItem(item);

            // act
            Item returnedItem = sut;

            // assert
            Assert.That(returnedItem, Is.EqualTo(item));
        }

        [TestCaseSource(nameof(Properties_ItemDoesNotHaveField_ReturnsEmptyString_DataSource))]
        public void Properties_ItemDoesNotHaveField_ReturnsEmptyString(Func<ScribanMailActionItem, string> propertyAccessor)
        {
            // arrange
            var item = ItemFactory.CreateItem().Object;
            var sut = new ScribanMailActionItem(item);

            // act
            var result = propertyAccessor(sut);

            // assert
            Assert.That(result, Is.Empty);
        }

        private static IEnumerable<TestCaseData> Properties_ItemDoesNotHaveField_ReturnsEmptyString_DataSource()
        {
            yield return new TestCaseData(new Func<ScribanMailActionItem, string>(sut => sut.To)).SetName("Properties_ItemDoesNotHaveField_ReturnsEmptyString_To");
            yield return new TestCaseData(new Func<ScribanMailActionItem, string>(sut => sut.From)).SetName("Properties_ItemDoesNotHaveField_ReturnsEmptyString_From");
            yield return new TestCaseData(new Func<ScribanMailActionItem, string>(sut => sut.Message)).SetName("Properties_ItemDoesNotHaveField_ReturnsEmptyString_Message");
            yield return new TestCaseData(new Func<ScribanMailActionItem, string>(sut => sut.Subject)).SetName("Properties_ItemDoesNotHaveField_ReturnsEmptyString_Subject");
        }

        [TestCaseSource(nameof(Properties_ItemHasField_ReturnsFieldValue_DataSource))]
        public void Properties_ItemHasField_ReturnsFieldValue(string fieldName, string fieldValue, Func<ScribanMailActionItem, string> propertyAccessor)
        {
            // arrange
            var itemMock = ItemFactory.CreateItem();
            ItemFactory.SetIndexerField(itemMock, fieldName, fieldValue);

            var item = itemMock.Object;
            var sut = new ScribanMailActionItem(item);

            // act
            var result = propertyAccessor(sut);

            // assert
            Assert.That(result, Is.EqualTo(fieldValue));
        }

        private static IEnumerable<TestCaseData> Properties_ItemHasField_ReturnsFieldValue_DataSource()
        {
            yield return new TestCaseData("to", "value", new Func<ScribanMailActionItem, string>(sut => sut.To)).SetName("Properties_ItemHasField_ReturnsFieldValue_To");
            yield return new TestCaseData("from", "value", new Func<ScribanMailActionItem, string>(sut => sut.From)).SetName("Properties_ItemHasField_ReturnsFieldValue_From");
            yield return new TestCaseData("message", "lorem ipsum", new Func<ScribanMailActionItem, string>(sut => sut.Message)).SetName("Properties_ItemHasField_ReturnsFieldValue_Message");
            yield return new TestCaseData("subject", "lorem ipsum", new Func<ScribanMailActionItem, string>(sut => sut.Subject)).SetName("Properties_ItemHasField_ReturnsFieldValue_Subject");
        }
    }
}
