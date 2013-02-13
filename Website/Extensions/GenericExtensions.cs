using System;

namespace Sitecore.Modules.WeBlog.Extensions
{
    public static class GenericExtensions
    {
        /// <summary>
        /// Safely evaluate an expression, checking to ensure the object is not null
        /// </summary>
        /// <typeparam name="T">The return type of the expression</typeparam>
        /// <param name="ob">The object to call the expression on</param>
        /// <param name="func">The expression to evaluate</param>
        /// <returns>The outcome of the expression if the object is not null, otherwise the default value of the return type</returns>
        public static T SafeGet<T, Y>(this Y ob, Func<Y, T> func)
        {
            if (ob != null)
                return func(ob);
            else
                return default(T);
        }
    }
}