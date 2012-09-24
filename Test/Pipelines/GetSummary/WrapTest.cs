using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NUnit.Framework;
using Sitecore.Modules.WeBlog.Pipelines.GetSummary;

namespace Sitecore.Modules.WeBlog.Test.Pipelines.GetSummary
{
    [TestFixture]
    [Category("GetSummary Wrap")]
    public class WrapTest
    {
        [Test]
        public void Unwrapped_OnlyRequiredWrap()
        {
            var processor = new Wrap();
            processor.OnlyWhenRequired = true;
            processor.WrappingTag = "span";

            var args = new GetSummaryArgs();
            args.Summary = "lorem ipsum";

            processor.Process(args);

            Assert.AreEqual("<span>lorem ipsum</span>", args.Summary);
        }

        [Test]
        public void AlreadyWrapped_OnlyRequiredWrap()
        {
            var processor = new Wrap();
            processor.OnlyWhenRequired = true;
            processor.WrappingTag = "span";

            var args = new GetSummaryArgs();
            args.Summary = "<span>lorem ipsum</span>";

            processor.Process(args);

            Assert.AreEqual("<span>lorem ipsum</span>", args.Summary);
        }

        [Test]
        public void Unwrapped_AlwaysWrap()
        {
            var processor = new Wrap();
            processor.OnlyWhenRequired = false;
            processor.WrappingTag = "span";

            var args = new GetSummaryArgs();
            args.Summary = "lorem ipsum";

            processor.Process(args);

            Assert.AreEqual("<span>lorem ipsum</span>", args.Summary);
        }

        [Test]
        public void AlreadyWrapped_AlwaysWrap()
        {
            var processor = new Wrap();
            processor.OnlyWhenRequired = false;
            processor.WrappingTag = "span";

            var args = new GetSummaryArgs();
            args.Summary = "<span>lorem ipsum</span>";

            processor.Process(args);

            Assert.AreEqual("<span><span>lorem ipsum</span></span>", args.Summary);
        }

        [Test]
        public void EmptySummary()
        {
            var processor = new Wrap();
            processor.OnlyWhenRequired = true;
            processor.WrappingTag = "p";

            var args = new GetSummaryArgs();
            args.Summary = string.Empty;

            processor.Process(args);

            Assert.AreEqual("<p></p>", args.Summary);
        }
    }
}