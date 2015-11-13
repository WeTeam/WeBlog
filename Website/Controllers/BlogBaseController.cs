using System;
using System.Web.Mvc;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class BlogBaseController : Controller
    {
        private EntryItem _currentEntry;
        private Item _datasource;

        public EntryItem CurrentEntry
        {
            get { return _currentEntry ?? (_currentEntry = new EntryItem(DataSourceItem)); }
        }

        public Item DataSourceItem
        {
            get
            {
                if (_datasource == null)
                {
                    var renderingContext = RenderingContext.CurrentOrNull;
                    if (renderingContext != null)
                    {
                        if (String.IsNullOrEmpty(renderingContext.Rendering.DataSource))
                        {
                            _datasource = renderingContext.ContextItem;
                        }
                        else
                        {
                            _datasource = Context.Database.GetItem(renderingContext.Rendering.DataSource);
                        }
                    }
                }
                return _datasource;
            }
        }
    }
}