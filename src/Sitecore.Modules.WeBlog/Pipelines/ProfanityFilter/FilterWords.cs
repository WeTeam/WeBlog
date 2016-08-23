using System;
using System.Text.RegularExpressions;

namespace Sitecore.Modules.WeBlog.Pipelines.ProfanityFilter
{
    public class FilterWords : IProfanityFilterProcessor
    {
        public bool WholeWordsOnly { get; set; }
        public string Replacement { get; set; }

        public void Process(ProfanityFilterArgs args)
        {
            foreach (var p in args.WordList)
            {
                var indexOf = GetIndexOfProfanity(p, args.Input);
                if (indexOf >= 0)
                {
                    var begin = args.Input.Substring(0, indexOf);
                    var end = args.Input.Substring(indexOf + p.Length);
                    args.Input = begin + Replacement + end;
                }
            }
        }

        private int GetIndexOfProfanity(string profanity, string text)
        {
            int indexOf = -1;
            if (WholeWordsOnly)
            {
                var match = Regex.Match(text, String.Format(@"\b{0}\b", profanity), RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    indexOf = match.Index;
                }
            }
            else
            {
                indexOf = text.IndexOf(profanity, StringComparison.OrdinalIgnoreCase);
            }
            return indexOf;
        }
    }
}