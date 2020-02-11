using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Security.Accounts;

namespace Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel
{
    /// <summary>
    /// A processor for the weBlogPopulateScribanMailActionModel pipeline which adds the entry item to the model.
    /// </summary>
    public class AddEntryModels
    {
        /// <summary>
        /// The key used to identify the entry model.
        /// </summary>
        public const string EntryModelKey = "entry";

        /// <summary>
        /// The key used to identify the entry created by model.
        /// </summary>
        public const string CreatedByModelKey = "entryCreatedBy";

        /// <summary>
        /// The key used to identify the entry updated by model.
        /// </summary>
        public const string UpdatedByModelKey = "entryUpdatedBy";

        public void Process(PopulateScribanMailActionModelArgs args)
        {
            if (args.EntryItem == null)
                return;

            var entryItem = new EntryItem(args.EntryItem);
            args.AddModel(EntryModelKey, entryItem);

            if (!string.IsNullOrEmpty(entryItem.InnerItem.Statistics.CreatedBy))
            {
                var createdByUser = GetUser(entryItem.InnerItem.Statistics.CreatedBy);
                args.AddModel(CreatedByModelKey, createdByUser.Profile);
            }

            if (!string.IsNullOrEmpty(entryItem.InnerItem.Statistics.UpdatedBy))
            {
                var updatedByUser = GetUser(entryItem.InnerItem.Statistics.UpdatedBy);
                args.AddModel(UpdatedByModelKey, updatedByUser.Profile);
            }
        }

        protected virtual User GetUser(string userName)
        {
            return User.FromName(userName, false);
        }
    }
}