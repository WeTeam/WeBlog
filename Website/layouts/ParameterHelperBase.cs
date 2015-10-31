using System;
using System.Collections.Specialized;
using System.Linq.Expressions;
using Sitecore.Diagnostics;
using Sitecore.Reflection;

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
                try
                {
                    ReflectionUtil.SetProperty(presentationComponent, key, Parameters[key]);
                }
                catch (Exception e)
                {
                    Log.Error("WeBlog.ParameterHelperBase: Unable to set rendering/sublayout property", e, this);
                    Type propertyType = ReflectionUtil.GetPropertyInfo(presentationComponent, key).PropertyType;
                    ReflectionUtil.SetProperty(presentationComponent, key, GetDefaultValueForType(propertyType));
                }
            }
        }

        public static object GetDefaultValueForType(Type type)
        {
            if (type == null) return null;
            Expression<Func<object>> e = Expression.Lambda<Func<object>>(Expression.Convert(Expression.Default(type), typeof(object)));
            return e.Compile()();
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