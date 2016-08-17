using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sitecore.Data;

namespace Sitecore.Modules.WeBlog.Pipelines.ProfanityFilter
{
    public class GetProfanityListFromItem : IProfanityFilterProcessor
    {
        public string ItemPath { get; set; }

        public void Process(ProfanityFilterArgs args)
        {
            if (args.WordList == null)
            {
                args.WordList = GetProfanityItemContent();
            }
        }

        private IEnumerable<string> GetProfanityItemContent()
        {
            var item = Context.Database.GetItem(ItemPath);
            if (item != null)
            {
                var field = item.Fields[Constants.Fields.WordList];
                if (field != null && !String.IsNullOrEmpty(field.Value))
                {
                    return field.Value.Split('\n').Select(s => s.Trim());
                }
            }
            return new string[0];
        }
    }
}