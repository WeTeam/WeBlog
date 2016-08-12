using System.Collections.Generic;
using NUnit.Framework;
using Sitecore.Modules.WeBlog.Pipelines;
using Sitecore.Modules.WeBlog.Pipelines.ProfanityFilter;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.ProfanityFilter
{
    [TestFixture]
    public class FilterWordsFixture
    {
        [TestCase(true, "-_-", "lorem ipsum", "lorem ipsum", TestName = "Proper, whole words")]
        [TestCase(true, "-_-", "Dolor sit", "Dolor sit", TestName = "Proper, whole words 2")]
        [TestCase(true, "-_-", "lorem ipsum x y", "lorem ipsum -_- -_-", TestName = "Improper, whole words")]
        [TestCase(true, "-_-", "lorem ipsum #x! #y!", "lorem ipsum #-_-! #-_-!", TestName = "Improper, whole words 2")]
        [TestCase(false, "-_-", "lorem ipsum", "lorem ipsum", TestName = "Proper, partial words")]
        [TestCase(false, "-_-", "lorem ipsum xy", "lorem ipsum -_--_-", TestName = "Improper, partial words")]
        public void Test(bool wholeWords, string replacement, string input, string expected)
        {
            var processor = new FilterWords
            {
                WholeWordsOnly = wholeWords,
                Replacement = replacement
            };

            var args = new ProfanityFilterArgs
            {
                Input = input,
                WordList = new List<string> {"x", "y"}
            };

            processor.Process(args);

            Assert.That(args.Input, Is.EqualTo(expected));
        }
    }
}