using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Reflection;

namespace Sitecore.Modules.WeBlog.WebControls
{
    public class BlogSublayoutRenderingType : Sitecore.Web.UI.SublayoutRenderingType
    {
        public override System.Web.UI.Control GetControl(System.Collections.Specialized.NameValueCollection parameters, bool assert)
        {
            var sublayout = new BlogSublayout();
            foreach (string key in parameters.Keys)
            {
                ReflectionUtil.SetProperty(sublayout, key, parameters[key]);
            }
            return sublayout;
        }
    }
}