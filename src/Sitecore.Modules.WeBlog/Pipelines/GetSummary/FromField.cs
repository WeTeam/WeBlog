using Sitecore.Web.UI.WebControls;

namespace Sitecore.Modules.WeBlog.Pipelines.GetSummary
{
    public class FromField : GetSummaryProcessorBase
    {
        /// <summary>
        /// Gets or sets the name of the field to extract the summary from
        /// </summary>
        public string FieldName
        {
            get;
            set;
        }

        public FromField()
        {
            FieldName = "Introduction";
        }

        protected override void GetSummary(GetSummaryArgs args)
        {
            var renderer = new FieldRenderer();
            renderer.Item = args.Entry;
            renderer.FieldName = FieldName;

            args.Summary = renderer.Render();
        }
    }
}