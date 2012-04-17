using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;

namespace Sitecore.Modules.WeBlog
{
    public class BlogSettings
    {
        protected ID _categoryTemplateID = ID.Null;
        protected ID _entryTemplateID = ID.Null;
        protected ID _commentTemplateID = ID.Null;

        public ID CategoryTemplateID
        {
            get
            {
                if (_categoryTemplateID == ID.Null)
                {
                    return Settings.CategoryTemplateID;
                }
                return _categoryTemplateID;
            }
            set
            {
                _categoryTemplateID = value;
            }
        }

        public ID EntryTemplateID
        {
            get
            {
                if (_entryTemplateID == ID.Null)
                {
                    return Settings.EntryTemplateID;
                }
                return _entryTemplateID;
            }
            set
            {
                _entryTemplateID = value;
            }
        }

        public ID CommentTemplateID
        {
            get
            {
                if (_commentTemplateID == ID.Null)
                {
                    return Settings.CommentTemplateID;
                }
                return _commentTemplateID;
            }
            set
            {
                _commentTemplateID = value;
            }
        }
    }
}