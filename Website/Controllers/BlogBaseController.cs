using System.ComponentModel;
using System.Web.Mvc;
using Sitecore.Modules.WeBlog.layouts;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class BlogBaseController : Controller
    {
        public RenderingParameterHelper RenderingParameterHelper { get; set; }

        public BlogBaseController()
        {
            RenderingParameterHelper = new RenderingParameterHelper(this, true);
        }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool VaryByBlog
        {
            get;
            set;
        }
    }
}