using System.Collections.Generic;
using NUnit.Framework;
using Sitecore.Modules.WeBlog.Pipelines;
using Sitecore.Modules.WeBlog.Pipelines.ProfanityFilter;

namespace Sitecore.Modules.WeBlog.Test.Pipelines.ProfanityFilter
{
    [TestFixture]
    [Category("ProfanityFilter ValidateInput")]
    public class ValidateInputTest
    {
        [Test]
        public void Proper_Input_WholeWords()
        {
            var processor = new ValidateInput();
            processor.WholeWordsOnly = true;

            var args = new ProfanityFilterArgs();
            args.Input = "lorem ipsum";
            args.WordList = new List<string> {"x", "y"};

            processor.Process(args);

            Assert.True(args.Valid);
        }

        [Test]
        public void Proper_Input_WholeWords2()
        {
            var processor = new ValidateInput();
            processor.WholeWordsOnly = true;

            var args = new ProfanityFilterArgs();
            args.Input = "lorem ipsumx";
            args.WordList = new List<string> { "x", "y" };

            processor.Process(args);

            Assert.True(args.Valid);
        }

        [Test]
        public void ImProper_Input_WholeWords()
        {
            var processor = new ValidateInput();
            processor.WholeWordsOnly = true;

            var args = new ProfanityFilterArgs();
            args.Input = "lorem ipsum x y";
            args.WordList = new List<string> { "x", "y" };

            processor.Process(args);

            Assert.False(args.Valid);
        }

        [Test]
        public void ImProper_Input_WholeWords2()
        {
            var processor = new ValidateInput();
            processor.WholeWordsOnly = true;

            var args = new ProfanityFilterArgs();
            args.Input = "lorem ipsum #x! #y!";
            args.WordList = new List<string> { "x", "y" };

            processor.Process(args);

            Assert.False(args.Valid);
        }

        [Test]
        public void Proper_Input()
        {
            var processor = new ValidateInput();
            processor.WholeWordsOnly = false;

            var args = new ProfanityFilterArgs();
            args.Input = "lorem ipsum";
            args.WordList = new List<string> { "x", "y" };

            processor.Process(args);

            Assert.True(args.Valid);
        }

        [Test]
        public void ImProper_Input()
        {
            var processor = new ValidateInput();
            processor.WholeWordsOnly = false;

            var args = new ProfanityFilterArgs();
            args.Input = "lorem ipsum xy";
            args.WordList = new List<string> { "x", "y" };

            processor.Process(args);

            Assert.False(args.Valid);
        }
    }
}