using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sitecore.Modules.WeBlog.Pipelines.ProfanityFilter
{
    public class ValidateInput : IProfanityFilterProcessor
    {
        public bool WholeWordsOnly { get; set; }

        public void Process(ProfanityFilterArgs args)
        {
            if (WholeWordsOnly)
            {
                args.Valid = !args.WordList
                    .Select(GetPattern)
                    .Any(p => Regex.Match(args.Input, p, RegexOptions.IgnoreCase).Success);
            }
            else
            {
                args.Valid = !args.WordList.Any(s => args.Input.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        protected virtual string GetPattern(string word)
        {
            return String.Format(@"\b{0}\b", word);
        }
    }
}