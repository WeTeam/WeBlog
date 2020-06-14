using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.ExperienceEditor
{
    public class NewEntry : CreateItem
    {
        public NewEntry(IBlogManager blogManager, BaseItemManager itemManager)
            : base(blogManager, itemManager)
        {
        }

        protected override ID GetTemplateId(BlogHomeItem blogItem)
        {
            return blogItem.BlogSettings.EntryTemplateID;
        }

        protected override Item GetParentItem(BlogHomeItem blogItem)
        {
            return blogItem;
        }
    }
}