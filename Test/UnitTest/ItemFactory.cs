using Moq;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.UnitTest
{
    /// <summary>
    /// A factory used to create items.
    /// </summary>
    internal static class ItemFactory
    {
        public static Mock<Item> CreateItem(ID templateId = null, Database database = null)
        {
            if(database == null)
                database = Mock.Of<Database>();

            var itemMock = new Mock<Item>(ID.NewID, ItemData.Empty, database);

            if (templateId != (ID)null)
                itemMock.Setup(x => x.TemplateID).Returns(templateId);

            // Set all field access to return empty string
            itemMock.Setup(x => x[It.IsAny<string>()]).Returns(string.Empty);

            return itemMock;
        }

        public static void SetField(Mock<Item> itemMock, string fieldName, string fieldValue)
        {
            itemMock.Setup(x => x[fieldName]).Returns(fieldValue);
        }

        public static void SetField(Mock<Item> itemMock, ID fieldId, string fieldValue)
        {
            itemMock.Setup(x => x[fieldId]).Returns(fieldValue);
        }
    }
}
