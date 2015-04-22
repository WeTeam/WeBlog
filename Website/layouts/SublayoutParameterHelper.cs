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

namespace Sitecore.Sharedsource.Web.UI.Sublayouts
{
  using System;
  using System.Collections.Specialized;

  /// <summary>
  /// Helper class to parse parameters and data source passed to a sublayout.
  /// </summary>
  public class SublayoutParameterHelper
  {
    #region Private members

    /// <summary>
    /// Sublayout data source item.
    /// </summary>
    private Sitecore.Data.Items.Item dataSourceItem = null;

    #endregion

    #region Public constructors

    /// <summary>
    /// Initializes a new instance of the SublayoutParameterHelper class. Parses
    /// parameters and applies properties as directed.
    /// </summary>
    /// <param name="control">Pass "this" from the user control</param>
    /// <param name="applyProperties">Set user control properties corresponding to
    /// parameter names to parameter values.</param>
    public SublayoutParameterHelper(
      System.Web.UI.UserControl control,
      bool applyProperties)
    {
      this.BindingControl = control.Parent as Sitecore.Web.UI.WebControls.Sublayout;

      // Parse parameters passed to the sc:sublayout control.
      if (this.BindingControl == null)
      {
        return;
      }

      this.Parameters = Sitecore.Web.WebUtil.ParseUrlParameters(
        this.BindingControl.Parameters);

      if (applyProperties)
      {
        this.ApplyProperties(control);
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
        this.DataSourceItem = Sitecore.Context.Database.GetItem(value);
      }
    }

    /// <summary>
    /// Gets or sets the data source item.
    /// </summary>
    public Sitecore.Data.Items.Item DataSourceItem
    {
      get
      {
        // If the data source has not been set 
        // and this code can access properties of the binding control
        if (this.dataSourceItem == null)
        {
          if (this.BindingControl == null
              || String.IsNullOrEmpty(this.BindingControl.DataSource))
          {
            this.dataSourceItem = Sitecore.Context.Item;
          }
          else if (Sitecore.Context.Database != null)
          {
            this.dataSourceItem =
              Sitecore.Context.Database.GetItem(this.BindingControl.DataSource);
          }
        }

        return this.dataSourceItem;
      }

      set
      {
        this.dataSourceItem = value;
      }
    }

    #endregion

    #region Protected properties

    /// <summary>
    /// Gets or sets the parameters passed to the sublayout.
    /// </summary>
    protected NameValueCollection Parameters
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the sublayout control used when user control binds to Sitecore
    /// placeholder,  or using sc:sublayout control. Otherwiwse Null (when user control
    /// is bound using ASP.NET).
    /// </summary>
    protected Sitecore.Web.UI.WebControls.Sublayout BindingControl
    {
      get;
      set;
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Return the value of a specific parameter.
    /// </summary>
    /// <param name="key">Parameter name.</param>
    /// <returns>Value of specified parameter.</returns>
    public string GetParameter(string key)
    {
      if (this.Parameters == null)
      {
        return String.Empty;
      }

      string result = this.Parameters[key];

      if (String.IsNullOrEmpty(result))
      {
        return String.Empty;
      }

      return result;
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Apply parameters passed to the sublayout as properties of the user control.
    /// </summary>
    /// <param name="control">The user control.</param>
    protected void ApplyProperties(System.Web.UI.UserControl control)
    {
      foreach (string key in this.Parameters.Keys)
      {
        Sitecore.Reflection.ReflectionUtil.SetProperty(
          control,
          key,
          this.Parameters[key]);
      }

      if (String.IsNullOrEmpty(this.BindingControl.DataSource))
      {
        return;
      }

      Sitecore.Reflection.ReflectionUtil.SetProperty(
        control,
        "datasource",
        this.DataSourceItem.Paths.FullPath);
      Sitecore.Reflection.ReflectionUtil.SetProperty(
        control,
        "datasourceitem",
        this.DataSourceItem);
    }

    #endregion
  }
}
