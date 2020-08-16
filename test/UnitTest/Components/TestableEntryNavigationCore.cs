using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Managers;
using System.Collections.Generic;

namespace Sitecore.Modules.WeBlog.UnitTest.Components
{
    public class TestableEntryNavigationCore : EntryNavigationCore
    {
        private Dictionary<ItemUri, Item> _items = null;

        public TestableEntryNavigationCore(IBlogManager blogManager, IEntryManager entryManager, Dictionary<ItemUri, Item> items)
            : base(blogManager, entryManager)
        {
            _items = items;
        }

        protected override Item GetItem(ItemUri uri)
        {
            return _items[uri];
        }
    }
}
