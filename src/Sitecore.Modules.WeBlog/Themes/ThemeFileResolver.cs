using Sitecore.Abstractions;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Model;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.Modules.WeBlog.Themes
{
    public class ThemeFileResolver : IThemeFileResolver
    {
        private BaseTemplateManager _templateManager = null;

        public ThemeFileResolver(BaseTemplateManager templateManager)
        {
            Assert.ArgumentNotNull(templateManager, nameof(templateManager));
            _templateManager = templateManager;
        }

        public ThemeFiles Resolve(ThemeItem themeItem)
        {
            // todo: implement caching with clear on publish.

            var stylesheets = new List<ThemeInclude>();
            var scripts = new List<ThemeScriptInclude>();

            var children = themeItem?.InnerItem.GetChildren() ?? Enumerable.Empty<Item>();

            foreach (Item item in children)
            {
                if (_templateManager.TemplateIsOrBasedOn(item, StylesheetItem.TemplateId))
                {
                    var include = CreateThemeInclude(item);
                    stylesheets.Add(include);
                }

                if (_templateManager.TemplateIsOrBasedOn(item, ScriptItem.TemplateId))
                {
                    var include = CreateThemeScriptInclude(item);
                    scripts.Add(include);
                }
            }

            return new ThemeFiles(stylesheets, scripts);
        }

        protected ThemeInclude CreateThemeInclude(FileItem item)
        {
            return new ThemeInclude(item.Url);
        }

        protected ThemeScriptInclude CreateThemeScriptInclude(ScriptItem item)
        {
            return new ThemeScriptInclude(item.Url, item.FallbackUrl, item.VerificationObject);
        }
    }
}
