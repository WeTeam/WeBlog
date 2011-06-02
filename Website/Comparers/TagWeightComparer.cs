using System.Collections.Generic;

namespace Sitecore.Modules.WeBlog.Comparers
{
   public class TagWeightComparer : IComparer<KeyValuePair<string, int>>
   {
       /// <summary>
       /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
       /// </summary>
       /// <param name="x">The first object to compare.</param>
       /// <param name="y">The second object to compare.</param>
       /// <returns>
       /// Value
       /// Condition
       /// Less than zero
       /// <paramref name="x"/> is less than <paramref name="y"/>.
       /// Zero
       /// <paramref name="x"/> equals <paramref name="y"/>.
       /// Greater than zero
       /// <paramref name="x"/> is greater than <paramref name="y"/>.
       /// </returns>
	      public int Compare(KeyValuePair<string, int> x, KeyValuePair<string, int> y)
	      {
	         return x.Value.CompareTo(y.Value);
	      }
	   }
	}