using System;

namespace Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel
{
    /// <summary>
    /// A processor for the weBlogPopulateScribanMailActionModel pipeline which adds the current date and time to the model.
    /// </summary>
    public class AddCurrentTimestamp
    {
        /// <summary>
        /// The key used to identify the model.
        /// </summary>
        public const string ModelKey = "time";

        public void Process(PopulateScribanMailActionModelArgs args)
        {
            args.AddModel(ModelKey, DateTime.Now);
        }
    }
}