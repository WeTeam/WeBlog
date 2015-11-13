using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Web.UI.WebControls;

namespace Sitecore.Modules.WeBlog.Items.Custom
{
    public abstract class CustomFieldBase<T> where T : CustomField
    {
        protected T InnerField;
        protected Item InternalItem;

        /// <summary>
        /// Raw field value
        /// </summary>
        public string Raw
        {
            get { return (object) InnerField == null ? string.Empty : InnerField.Value; }
        }

        /// <summary>
        /// Field as rendered by the field editor
        /// </summary>
        public string Rendered
        {
            get { return (object) InnerField == null ? string.Empty : FieldRenderer.Render(InternalItem, InnerField.InnerField.Name); }
        }

        public T Field
        {
            get { return InnerField; }
        }

        protected CustomFieldBase(Item item, T field)
        {
            InnerField = field;
            InternalItem = item;
        }
    }
}