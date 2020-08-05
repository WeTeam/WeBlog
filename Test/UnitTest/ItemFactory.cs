using Moq;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System.Collections.Generic;

namespace Sitecore.Modules.WeBlog.UnitTest
{
    /// <summary>
    /// A factory used to create items.
    /// </summary>
    internal static class ItemFactory
    {
        public static Mock<Item> CreateItem(ID itemId = null, ID templateId = null, Database database = null)
        {
            if(database == null)
                database = Mock.Of<Database>(x => x.Name == "mock");

            var id = itemId ?? ID.NewID;

            var itemMock = new Mock<Item>(id, ItemData.Empty, database);
            itemMock.Setup(x => x.Uri).Returns(new ItemUri(id, database));

            if (templateId != (ID)null)
                itemMock.Setup(x => x.TemplateID).Returns(templateId);

            // Set all field access to return empty string
            itemMock.Setup(x => x[It.IsAny<string>()]).Returns(string.Empty);

            return itemMock;
        }

        public static void SetIndexerField(Mock<Item> itemMock, string fieldName, string fieldValue)
        {
            itemMock.Setup(x => x[fieldName]).Returns(fieldValue);
        }

        public static void SetIndexerField(Mock<Item> itemMock, ID fieldId, string fieldValue)
        {
            itemMock.Setup(x => x[fieldId]).Returns(fieldValue);
        }

        public static void AddFields(Mock<Item> itemMock, IEnumerable<Field> fields)
        {
            var fieldCollectionMock = new Mock<FieldCollection>(itemMock.Object);

            foreach (var field in fields)
            {
                fieldCollectionMock.Setup(x => x[field.ID]).Returns(field);
                fieldCollectionMock.Setup(x => x[field.Name]).Returns(field);

                itemMock.Setup(x => x[field.Name]).Returns(field.Value);
            }

            itemMock.Setup(x => x.Fields).Returns(fieldCollectionMock.Object);
        }

        public static void AddChildItems(Mock<Item> itemMock, params Item[] childItems)
        {
            var itemList = new ItemList();
            itemList.AddRange(childItems);

            var childList = new ChildList(itemMock.Object, itemList);

            itemMock.Setup(x => x.GetChildren()).Returns(childList);
        }

        public static void SetPath(Mock<Item> itemMock, string path)
        {
            var itemPath = new Mock<ItemPath>(itemMock.Object);
            itemPath.Setup(x => x.Path).Returns(path);

            itemMock.Setup(x => x.Paths).Returns(itemPath.Object);
        }

        public static void SetParent(Mock<Item> itemMock, Item parentItem)
        {
            itemMock.Setup(x => x.Parent).Returns(parentItem);
        }
    }
}
