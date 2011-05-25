using System.Web.UI;
using Sitecore.Data.Items;

namespace Sitecore.Modules.Blog.WebControls
{
    public class EntryPostDate : System.Web.UI.WebControls.WebControl
    {
        public Item Item { get; set; }
        private Item currentEntry;

        protected override void CreateChildControls()
        {
            if (Item == null)
                 currentEntry = Sitecore.Context.Item;
            else
                currentEntry = Item;

            Controls.Add(new LiteralControl(currentEntry.Statistics.Created.ToString("dddd, MMMM d, yyyy")));
            
            base.CreateChildControls();
        }
    }
}
