using System;
using System.Collections.Specialized;

namespace Sitecore.Modules.WeBlog.Utilities
{
    public class SublayoutParamHelper
    {

        // All known parameters passed to the sublayout.

        NameValueCollection _params = null;

        // Sublayout data source item.

        Sitecore.Data.Items.Item _dataSource = null;

        // Sublayout control used when user control binds to Sitecore placeholder, 
        // or using sc:sublayout control.
        // Otherwiwse null (when user control is bound using ASP.NET).

        Sitecore.Web.UI.WebControls.Sublayout _sublayout = null;

        /// <summary>
        /// Constructor parses parameters and applies properties as directed.
        /// </summary>
        /// <param name="control">Pass "this" from the user control</param>
        /// <param name="applyProperties">Set user control properties corresponding to parameter names to parameter values.</param>

        public SublayoutParamHelper(System.Web.UI.UserControl control, bool applyProperties )
        {
            _sublayout = control.Parent as Sitecore.Web.UI.WebControls.Sublayout;

            // Parse parameters passed to the sc:sublayout control.

            if (_sublayout != null)
            {
                _params = Sitecore.Web.WebUtil.ParseUrlParameters(_sublayout.Parameters);

                if (applyProperties)
                {
                    foreach (string key in _params.Keys)
                    {
                        Sitecore.Reflection.ReflectionUtil.SetProperty(control, key, _params[key]);
                    }

                    if (_dataSource != null)
                    {
                        Sitecore.Reflection.ReflectionUtil.SetProperty(control, "datasource", DataSourceItem.Paths.FullPath);
                        Sitecore.Reflection.ReflectionUtil.SetProperty(control, "datasourceitem", DataSourceItem);
                    }
                }
            }
        }

        public SublayoutParamHelper(Sitecore.Web.UI.WebControls.Sublayout sublayout)
        {
            _sublayout = sublayout;
        }

        /// <summary>
        /// Return the value of a specific parameter.
        /// </summary>
        /// <param name="key">Parameter name.</param>
        /// <returns>Value of specified parameter.</returns>

        public string GetParam( string key )
        {
            key.Trim().ToLower();
            string result = _params[key.Trim().ToLower()];

            if (String.IsNullOrEmpty( result ))
            {
                result = String.Empty;
            }

            return (System.Web.HttpUtility.UrlDecode(result));
        }

        /// <summary>
        /// Sitecore data source item.
        /// </summary>

        public string DataSource
        {
            set
            {
                _dataSource = Sitecore.Context.Database.Items[value];
            }
        }

        /// <summary>
        /// Sitecore data source item. NOTE: if the sublayout's datasource is invalid, the sublayout will be hidden.
        /// </summary>

        public Sitecore.Data.Items.Item DataSourceItem
        {
            get
            {
                if ( _dataSource == null && _sublayout != null )
                {
                    if ( String.IsNullOrEmpty( _sublayout.DataSource ))
                    {
                        _dataSource = Sitecore.Context.Item;
                    }
                    else
                    {
                        _dataSource = Sitecore.Context.Database.Items[_sublayout.DataSource];

                        // If the datasource is invalid, hide the sublayout.
                        if (_dataSource == null)
                        {
                            _sublayout.Visible = false;
                        }
                    }
                }

                return (_dataSource);
            }
        }
    }
}

