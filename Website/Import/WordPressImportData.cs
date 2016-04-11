using Sitecore.Data;
using Sitecore.Services.Core.Model;

namespace Sitecore.Modules.WeBlog.Import
{
    public class WordPressImportData : EntityIdentity
    {
        public string BlogName { get; set; }
        public string BlogEmail { get; set; }
        public ID ParentId { get; set; }
        public string DatabaseName { get; set; }
        public ID DataSourceId { get; set; }
        public bool ImportPosts { get; set; }
        public bool ImportCategories { get; set; }
        public bool ImportComments { get; set; }
        public bool ImportTags { get; set; }
    }
}