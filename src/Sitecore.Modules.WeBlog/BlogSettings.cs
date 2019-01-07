using System.Linq;
using Sitecore.Data;
using Sitecore.Modules.WeBlog.Configuration;

namespace Sitecore.Modules.WeBlog
{
    public class BlogSettings
    {
        protected ID _categoryTemplateId = ID.Null;
        protected ID _entryTemplateId = ID.Null;
        protected ID _commentTemplateId = ID.Null;

        protected IWeBlogSettings Settings { get; }

        public BlogSettings(IWeBlogSettings settings)
        {
            Settings = settings;
        }

        public ID CategoryTemplateID
        {
            get
            {
                if (_categoryTemplateId == ID.Null)
                    return Settings.CategoryTemplateIds.First();

                return _categoryTemplateId;
            }
            set
            {
                _categoryTemplateId = value;
            }
        }

        public ID EntryTemplateID
        {
            get
            {
                if (_entryTemplateId == ID.Null)
                    return Settings.EntryTemplateIds.First();
                
                return _entryTemplateId;
            }
            set
            {
                _entryTemplateId = value;
            }
        }

        public ID CommentTemplateID
        {
            get
            {
                if (_commentTemplateId == ID.Null)
                    return Settings.CommentTemplateIds.First();
                
                return _commentTemplateId;
            }

            set
            {
                _commentTemplateId = value;
            }
        }
    }
}