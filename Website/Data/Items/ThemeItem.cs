using System.Collections.Generic;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Data.Fields;
using Sitecore.Modules.WeBlog.Extensions;

namespace Sitecore.Modules.WeBlog.Data.Items
{
    public class ThemeItem : CustomItem
    {
        public IEnumerable<FileItem> Stylesheets { get; protected set; }

        public IEnumerable<ScriptItem> Scripts { get; protected set; }

        public ThemeItem(Item innerItem) : base(innerItem)
        {
            ResolveThemeAssets();
        }

        public static implicit operator ThemeItem(Item innerItem)
        {
            return innerItem != null ? new ThemeItem(innerItem) : null;
        }

        public static implicit operator Item(ThemeItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        // to be removed once 2016 themes are done
        public CustomTextField FileLocation
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["File Location"]); }
        }

        protected void ResolveThemeAssets()
        {
            var scriptList = new List<ScriptItem>();
            var stylesheetList = new List<FileItem>();

            var children = InnerItem.GetChildren();

            foreach(Item item in children)
            {
                if (item.TemplateIsOrBasedOn(ScriptItem.TemplateId))
                    scriptList.Add(new ScriptItem(item));

                if (item.TemplateIsOrBasedOn(StylesheetItem.TemplateId))
                    stylesheetList.Add(new StylesheetItem(item));
            }

            Scripts = scriptList;
            Stylesheets = stylesheetList;
        }        
    }
}