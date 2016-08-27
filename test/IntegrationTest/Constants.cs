using Sitecore.Data;

namespace Sitecore.Modules.WeBlog.IntegrationTest
{
    public static class Constants
    {
        public const string UnitTestDatabase = "master";
        public const string FolderTemplate = "common/folder";

        public static class Templates
        {
            public static readonly TemplateID EntryTemplateId = new TemplateID(ID.Parse("{BE9675B1-4951-4E11-B935-A698227B6A97}")); // MVC entry
            public static readonly TemplateID CategoryTemplateId = new TemplateID(ID.Parse("{6C455315-01BF-4E2E-9BA3-31B5695E9C77}")); // MVC category
            public static readonly TemplateID CommentTemplateId = new TemplateID(ID.Parse("{70949D4E-35D8-4581-A7A2-52928AA119D5}")); // Comment
        }

        public static class Branches
        {
            public static readonly BranchId BlogBranchId = new BranchId(ID.Parse("{8F890A99-5AD0-48B9-B930-B44BEC524840}")); // MVC blog branch
        }
    }
}