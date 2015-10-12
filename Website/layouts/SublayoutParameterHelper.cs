//------------------------------------------------------------------------------------------------- 
// <copyright file="SublayoutParameterHelper.cs" company="Sitecore Shared Source">
// Copyright (c) Sitecore.  All rights reserved.
// </copyright>
// <summary>Defines the SublayoutParameterHelper type.</summary>
// <license>
// http://sdn.sitecore.net/Resources/Shared%20Source/Shared%20Source%20License.aspx
// </license>
// <url>http://trac.sitecore.net/SublayoutParameterHelper/</url>
//-------------------------------------------------------------------------------------------------

using System;
using System.Web.UI;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.layouts;
using Sitecore.Web.UI.WebControls;

namespace Sitecore.Sharedsource.Web.UI.Sublayouts
{
    /// <summary>
    /// Helper class to parse parameters and data source passed to a sublayout.
    /// </summary>
    public class SublayoutParameterHelper : ParameterHelperBase<UserControl>
    {
        #region Private members

        /// <summary>
        /// Sublayout data source item.
        /// </summary>
        private Item dataSourceItem = null;

        #endregion

        #region Public constructors

        /// <summary>
        /// Initializes a new instance of the SublayoutParameterHelper class. Parses
        /// parameters and applies properties as directed.
        /// </summary>
        /// <param name="control">Pass "this" from the user control</param>
        /// <param name="applyProperties">Set user control properties corresponding to
        /// parameter names to parameter values.</param>
        public SublayoutParameterHelper(UserControl control, bool applyProperties)
        {
            BindingControl = control.Parent as Sitecore.Web.UI.WebControls.Sublayout;

            // Parse parameters passed to the sc:sublayout control.
            if (BindingControl == null)
            {
                return;
            }

            Parameters = Sitecore.Web.WebUtil.ParseUrlParameters(BindingControl.Parameters);

            if (applyProperties)
            {
                ApplyProperties(control);
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Sets the path to the Sitecore data source item.
        /// </summary>
        public string DataSource
        {
            set
            {
                DataSourceItem = Context.Database.GetItem(value);
            }
        }

        /// <summary>
        /// Gets or sets the data source item.
        /// </summary>
        public Item DataSourceItem
        {
            get
            {
                // If the data source has not been set 
                // and this code can access properties of the binding control
                if (dataSourceItem == null)
                {
                    if (BindingControl == null || String.IsNullOrEmpty(BindingControl.DataSource))
                    {
                        dataSourceItem = Context.Item;
                    }
                    else if (Context.Database != null)
                    {
                        dataSourceItem =
                          Context.Database.GetItem(BindingControl.DataSource);
                    }
                }

                return dataSourceItem;
            }
            set
            {
                dataSourceItem = value;
            }
        }

        #endregion

        #region Protected properties

        /// <summary>
        /// Gets or sets the sublayout control used when user control binds to Sitecore
        /// placeholder,  or using sc:sublayout control. Otherwiwse Null (when user control
        /// is bound using ASP.NET).
        /// </summary>
        protected Sublayout BindingControl
        {
            get;
            set;
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Apply parameters passed to the sublayout as properties of the user control.
        /// </summary>
        /// <param name="control">The user control.</param>
        protected void ApplyProperties(UserControl control)
        {
            ApplyParameters(control);
            if (String.IsNullOrEmpty(BindingControl.DataSource))
            {
                return;
            }

            Reflection.ReflectionUtil.SetProperty(control, "datasource", DataSourceItem.Paths.FullPath);
            Reflection.ReflectionUtil.SetProperty(control, "datasourceitem", DataSourceItem);
        }
        #endregion
    }
}
