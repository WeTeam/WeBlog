using Sitecore.StringExtensions;

namespace Sitecore.Modules.WeBlog.Pipelines.GetSummary
{
    public class Wrap : IGetSummaryProcessor
    {
        /// <summary>
        /// Gets or sets whether the summary should be wrapped only when required (tag doesn't already exist) or always
        /// </summary>
        public bool OnlyWhenRequired
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tag to wrap the summary with
        /// </summary>
        public string WrappingTag
        {
            get;
            set;
        }

        public Wrap()
        {
            OnlyWhenRequired = true;
            WrappingTag = "p";
        }

        public void Process(GetSummaryArgs args)
        {
            // This processor is to compensate for messy markup. It shouldn't be required when editing (and it can break EE)
#if !FEATURE_EXPERIENCE_EDITOR
            if (Context.PageMode.IsPageEditor)
#else
            if (Context.PageMode.IsExperienceEditor)
#endif
                return;

            if (!OnlyWhenRequired || !args.Summary.StartsWith("<" + WrappingTag + ">"))
                args.Summary = "<{0}>{1}</{0}>".FormatWith(WrappingTag, args.Summary);
        }
    }
}