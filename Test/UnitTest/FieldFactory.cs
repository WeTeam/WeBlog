using Moq;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.UnitTest
{
    /// <summary>
    /// A factory used to create fields.
    /// </summary>
    internal static class FieldFactory
    {
        public static Field CreateField(Item item, ID id, string name, string value)
        {
            var fieldMock = new Mock<Field>(id, item);
            fieldMock.Setup(x => x.Name).Returns(name);
            fieldMock.Setup(x => x.Value).Returns(value);

            return fieldMock.Object;
        }
    }
}
