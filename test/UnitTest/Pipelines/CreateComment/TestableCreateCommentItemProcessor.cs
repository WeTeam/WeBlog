using System;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Pipelines.CreateComment;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.CreateComment
{
    internal class TestableCreateCommentItemProcessor : CreateCommentItem
    {
        public TestableCreateCommentItemProcessor(IBlogManager blogManager)
            : base(blogManager)
        { }

        protected override DateTime GetDateTime()
        {
            return new DateTime(2012, 03, 04, 14, 37, 23);
        }
    }
}
