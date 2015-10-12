using System;
using System.Collections.Specialized;

namespace Sitecore.Modules.WeBlog.layouts
{
    public abstract class ParameterHelperBase<T>
    {
        /// <summary>
        /// Gets or sets the parameters passed to the sublayout.
        /// </summary>
        protected NameValueCollection Parameters { get; set; }

        /// <summary>
        /// Apply parameters passed to the presentation component as properties of the sublayout/rendering.
        /// </summary>
        /// <param name="presentationComponent">The user control (WebForms) or controller (MVC).</param>
        protected void ApplyParameters(T presentationComponent)
        {
            foreach (string key in Parameters.Keys)
            {
                Reflection.ReflectionUtil.SetProperty(presentationComponent, key, Parameters[key]);
            }
        }

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

            string result = Parameters[key];

            if (String.IsNullOrEmpty(result))
            {
                return String.Empty;
            }

            return result;
        }
    }
}