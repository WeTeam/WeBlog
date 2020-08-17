using System;
using Moq;
using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.UnitTest.sitecore_modules.web.WeBlog
{
    public class TestableMetaBlogApi : MetaBlogApi
    {
        public Action<string, string> AuthenticateFunction { get; set; }

        public Func<Database> GetContentDatabaseFunction { get; set; }

        public Func<string, BlogHomeItem> GetBlogFunction { get; set; }

        public TestableMetaBlogApi(
            IBlogManager blogManager = null,
            ICategoryManager categoryManager = null,
            IEntryManager entryManager = null,
            IWeBlogSettings settings = null,
            BaseMediaManager mediaManager = null,
            BaseLinkManager linkManager = null,
            IBlogSettingsResolver blogSettingsResolver = null
        )
            : base(
                  blogManager ?? Mock.Of<IBlogManager>(),
                  categoryManager ?? Mock.Of<ICategoryManager>(),
                  entryManager ?? Mock.Of<IEntryManager>(),
                  settings ?? Mock.Of<IWeBlogSettings>(),
                  mediaManager ?? Mock.Of<BaseMediaManager>(),
                  linkManager ?? Mock.Of<BaseLinkManager>(),
                  blogSettingsResolver ?? Mock.Of<IBlogSettingsResolver>()
                  )
        {
        }

        protected override void Authenticate(string username, string password)
        {
            if (AuthenticateFunction != null)
                AuthenticateFunction(username, password);
            else
                base.Authenticate(username, password);
        }

        protected override Database GetContentDatabase()
        {
            if (GetContentDatabaseFunction != null)
                return GetContentDatabaseFunction();

            return base.GetContentDatabase();
        }

        protected override BlogHomeItem GetBlog(string blogId)
        {
            if(GetBlogFunction != null)
                return GetBlogFunction(blogId);

            return base.GetBlog(blogId);
        }
    }
}
