using Sitecore.Data;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.Modules.WeBlog.Pipelines.ProfanityFilter
{
    public class GetProfanityListFromItem : IProfanityFilterProcessor
    {
        private Database _database = null;

        public string ItemPath { get; set; }

        public GetProfanityListFromItem() : this(null) { }

        public GetProfanityListFromItem(Database database)
        {
            _database = database ?? ContentHelper.GetContentDatabase();
        }

        public void Process(ProfanityFilterArgs args)
        {
            if (args.WordList == null)
            {
                args.WordList = GetProfanityItemContent();
            }
        }

        private IEnumerable<string> GetProfanityItemContent()
        {
            var item = _database.GetItem(ItemPath);
            if (item != null)
            {
                var field = item.Fields[Constants.Fields.WordList];
                if (field != null && !string.IsNullOrEmpty(field.Value))
                {
                    return field.Value.Split('\n').Select(s => s.Trim());
                }
            }
            return new string[0];
        }
    }
}