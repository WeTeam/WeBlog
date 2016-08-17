using Sitecore.Data;

namespace Sitecore.Modules.WeBlog.Test
{
    public static class Constants
    {
        public const string UnitTestDatabase = "master";
        public const string EntryItemXmlTemplate = "<item name=\"Entry{0}\" tid=\"{{5FA92FF4-4AC2-48E2-92EB-E1E4914677B0}}\"><version language=\"en\" version=\"1\"><fields><field tfid=\"{{BADD9CF9-53E0-4D0C-BCC0-2D784C282F6A}}\" key=\"__updated by\" type=\"text\"><content>sitecore\\admin</content></field><field tfid=\"{{25BED78C-4957-4165-998A-CA1B52F67497}}\" key=\"__created\" type=\"datetime\"><content>{1}</content></field><field tfid=\"{{D9CF14B1-FA16-4BA6-9288-E8A174D4D522}}\" key=\"__updated\" type=\"datetime\"><content>{2}</content></field></fields></version></item>";
        public const string CommentItemXmlTemplate = "<item name=\"Comment{0}\" tid=\"{{70949D4E-35D8-4581-A7A2-52928AA119D5}}\"><version language=\"en\" version=\"1\"><fields><field tfid=\"{{BADD9CF9-53E0-4D0C-BCC0-2D784C282F6A}}\" key=\"__updated by\" type=\"text\"><content>sitecore\\admin</content></field><field tfid=\"{{25BED78C-4957-4165-998A-CA1B52F67497}}\" key=\"__created\" type=\"datetime\"><content>{1}</content></field><field tfid=\"{{D9CF14B1-FA16-4BA6-9288-E8A174D4D522}}\" key=\"__updated\" type=\"datetime\"><content>{2}</content></field></fields></version></item>";
        public const string FolderTemplate = "common/folder";
        public const string CategoryTemplate = "{61FF8D49-90D7-4E59-878D-DF6E03400D3B}";
        public const string CommentTemplate = "{70949D4E-35D8-4581-A7A2-52928AA119D5}";
        public const string EntryTemplate = "{5FA92FF4-4AC2-48E2-92EB-E1E4914677B0}";

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