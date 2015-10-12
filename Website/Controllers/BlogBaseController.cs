using System.ComponentModel;
using System.Web.Mvc;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Modules.WeBlog.layouts;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class BlogBaseController : Controller
    {
        private BlogHomeItem _currentBlog;
        public RenderingParameterHelper RenderingParameterHelper { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool VaryByBlog { get; set; }

        public BlogHomeItem CurrentBlog
        {
            get { return _currentBlog ?? (_currentBlog = ManagerFactory.BlogManagerInstance.GetCurrentBlog()); }
        }

        public BlogBaseController()
        {
            RenderingParameterHelper = new RenderingParameterHelper(this, true);
        }
    }
}