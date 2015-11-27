using System.Collections.Generic;
using NUnit.Framework;
using Sitecore.Modules.WeBlog.Pipelines;
using Sitecore.Modules.WeBlog.Pipelines.ProfanityFilter;

namespace Sitecore.Modules.WeBlog.Test.Pipelines.ProfanityFilter
{
    [TestFixture]
    [Category("ProfanityFilter FilterWords")]
    public class FilterWordsTest
    {
        [Test]
        public void Proper_Input_WholeWords()
        {
            var processor = new FilterWords();
            processor.WholeWordsOnly = true;
            processor.Replacement = "-_-";

            var args = new ProfanityFilterArgs();
            args.Input = "lorem ipsum";
            args.WordList = new List<string> {"x", "y"};

            processor.Process(args);

            Assert.AreEqual("lorem ipsum", args.Input);
        }

        [Test]
        public void Proper_Input_WholeWords2()
        {
            var processor = new FilterWords();
            processor.WholeWordsOnly = true;
            processor.Replacement = "-_-";

            var args = new ProfanityFilterArgs();
            args.Input = "lorem ipsumx";
            args.WordList = new List<string> { "x", "y" };

            processor.Process(args);

            Assert.AreEqual("lorem ipsumx", args.Input);
        }

        [Test]
        public void ImProper_Input_WholeWords()
        {
            var processor = new FilterWords();
            processor.WholeWordsOnly = true;
            processor.Replacement = "-_-";

            var args = new ProfanityFilterArgs();
            args.Input = "lorem ipsum x y";
            args.WordList = new List<string> { "x", "y" };

            processor.Process(args);

            Assert.AreEqual("lorem ipsum -_- -_-", args.Input);
        }

        [Test]
        public void ImProper_Input_WholeWords2()
        {
            var processor = new FilterWords();
            processor.WholeWordsOnly = true;
            processor.Replacement = "-_-";

            var args = new ProfanityFilterArgs();
            args.Input = "lorem ipsum #x! #y!";
            args.WordList = new List<string> { "x", "y" };

            processor.Process(args);

            Assert.AreEqual("lorem ipsum #-_-! #-_-!", args.Input);
        }

        [Test]
        public void Proper_Input()
        {
            var processor = new FilterWords();
            processor.WholeWordsOnly = false;
            processor.Replacement = "-_-";

            var args = new ProfanityFilterArgs();
            args.Input = "lorem ipsum";
            args.WordList = new List<string> { "x", "y" };

            processor.Process(args);

            Assert.AreEqual("lorem ipsum", args.Input);
        }

        [Test]
        public void ImProper_Input()
        {
            var processor = new FilterWords();
            processor.WholeWordsOnly = false;
            processor.Replacement = "-_-";

            var args = new ProfanityFilterArgs();
            args.Input = "lorem ipsum xy";
            args.WordList = new List<string> { "x", "y" };

            processor.Process(args);

            Assert.AreEqual("lorem ipsum -_--_-", args.Input);
        }
    }
}