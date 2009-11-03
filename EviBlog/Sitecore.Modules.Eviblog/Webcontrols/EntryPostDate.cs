using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Web.UI.WebControls;
using Sitecore.Xml.Xsl;
using Sitecore.Web;
using Sitecore.Collections;
using Sitecore.Data.Items;
using System.Web.UI;

namespace Sitecore.Modules.Eviblog.Webcontrols
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
