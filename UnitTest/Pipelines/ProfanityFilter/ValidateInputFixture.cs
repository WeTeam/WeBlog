using System.Collections.Generic;
using NUnit.Framework;
using Sitecore.Modules.WeBlog.Pipelines;
using Sitecore.Modules.WeBlog.Pipelines.ProfanityFilter;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.ProfanityFilter
{
    [TestFixture]
    public class ValidateInputFixture
    {
        [TestCase(true, "lorem ipsum", true, TestName = "Proper, whole words")]
        [TestCase(true, "Dolor sit", true, TestName = "Proper, whole words 2")]
        [TestCase(true, "lorem ipsum x y", false, TestName = "Improper, whole words")]
        [TestCase(true, "lorem ipsum #x! #y!", false, TestName = "Improper, whole words 2")]
        [TestCase(false, "lorem ipsum", true, TestName = "Proper, partial words")]
        [TestCase(false, "lorem ipsum xy", false, TestName = "Proper, partial words")]
        public void Test(bool wholeWords, string input, bool isAllowed)
        {
            var processor = new ValidateInput
            {
                WholeWordsOnly = wholeWords
            };

            var args = new ProfanityFilterArgs
            {
                Input = input,
                WordList = new List<string> {"x", "y"}
            };

            processor.Process(args);

            if(isAllowed)
                Assert.That(args.Valid, Is.True);
            else
                Assert.That(args.Valid, Is.False);
        }
    }
}