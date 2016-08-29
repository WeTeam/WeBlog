using Sitecore.Data;

namespace Sitecore.Modules.WeBlog
{
    public class Templates
    {
        public struct TemplateMapping
        {
            public static ID ID = ID.Parse("{DF1783C7-8448-42A1-A9AE-86C97A7F5B64}");
            public struct Fields
            {
                public static readonly ID BlogRootTemplate = new ID("{D6269811-BE20-415E-AF48-2BF9F2F2AC38}");
                public static readonly ID BlogEntryTemplate = new ID("{37D3ACEF-F8B8-4E0C-A12F-4BDBD94A8338}");
                public static readonly ID BlogCategoryTemplate = new ID("{B4BDEA2C-CDB6-428A-8DDC-F9178406978E}");
                public static readonly ID BlogCommentTemplate = new ID("{ABCB1890-38CF-4599-865F-3B754B55EA1C}");
            }
        }
    }
}