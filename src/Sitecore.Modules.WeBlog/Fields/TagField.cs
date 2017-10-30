using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Globalization;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Text;
using DateTime = System.DateTime;

namespace Sitecore.Modules.WeBlog.Fields
{
    public class TagField : Sitecore.Web.UI.HtmlControls.Control, IContentField
    {
        private string _itemid;
        private string _source;

        public string ItemId
        {
            get { return _itemid; }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                _itemid = value;
            }
        }

        public string ItemLanguage
        {
            get { return StringUtil.GetString(ViewState["ItemLanguage"]); }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                ViewState["ItemLanguage"] = value;
            }
        }

        public string Source
        {
            get { return _source; }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                _source = value;
            }
        }

        public string GetValue()
        {
            return Value;
        }

        public void SetValue(string value)
        {
            Assert.ArgumentNotNull(value, "value");
            Value = value;
        }

        protected override void DoRender(HtmlTextWriter output)
        {
            var selectedItems = GetSelectedItems();
            Item current = Sitecore.Context.ContentDatabase.GetItem(ItemId, Language.Parse(ItemLanguage));
            var currentBlog = ManagerFactory.BlogManagerInstance.GetCurrentBlog(current);

            using (new ContextItemSwitcher(current))
            {
                using (new DatabaseSwitcher(current.Database))
                {
                    var sourceItems = GetSourceItems(currentBlog).ToList();
                    RenderMultiList(output, selectedItems, sourceItems);
                }
            }

            string script = "<script type=\"text/javascript\">jQuery('.chosen-select').chosen().change(WeBlog.tagField.onTagFieldUpdated);</script>";
            output.Write(script);

            base.DoRender(output);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            string value = Sitecore.Context.ClientPage.ClientRequest.Form[ID + "_value"];

            if (value != null)
            {
                string viewStateValue = GetViewStateString("Value", string.Empty);

                if (viewStateValue != value)
                {
                    SetModified();
                }

                SetViewStateString("Value", value);
            }
        }

        protected virtual IEnumerable<Tag> GetSelectedItems()
        {
            ListString itemsIds = new ListString(Value, ',');
            return itemsIds.Select(itemId => new Tag(itemId, DateTime.MaxValue, 0)).Where(item => item != null);
        }

        protected virtual IEnumerable<Tag> GetSourceItems(BlogHomeItem currentBlog)
        {
            var sources = ManagerFactory.TagManagerInstance.GetTagsForBlog(currentBlog).ToList();
            sources.Sort((tag, tag1) => tag1.Count - tag.Count);
            var sourceItems = sources.Take(new WeBlogSettings().TagFieldMaxItemCount);
            return sourceItems;
        }

        protected virtual void SetModified()
        {
            Sitecore.Context.ClientPage.Modified = true;
        }

        protected virtual void RenderMultiList(HtmlTextWriter output, IEnumerable<Tag> selectedItems, IEnumerable<Tag> sourceItems)
        {
            output.Write($"<input id=\"{ID}_Value\" class=\"scContentControl\" type=\"hidden\" value=\"{StringUtil.EscapeQuote(Value)}\" />");

            output.Write($"<select data-placeholder=\"{Translator.Text("TAG_FIELD_PLACEHOLDER")}\" id=\"{ID}_Select\" multiple class=\"chosen-select\" {GetControlAttributes()} >");
            var selectedHash = selectedItems.ToDictionary(k => k.Name);
            foreach (var item in selectedHash)
            {
                output.Write($"<option selected value=\"{item.Key}\">{item.Value.Name}</option>");
            }

            var notSelectedItems = sourceItems.Where(s => !selectedHash.ContainsKey(s.Name));
            foreach (var item in notSelectedItems)
            {
                output.Write($"<option value=\"{item.Name}\">{item.Name}</option>");
            }
            output.Write("</select>");

            var selector = $"#{UniqueID}_Select";
            output.Write($"<script type=\"text/javascript\">jQuery('{selector}').chosen({{no_results_text: '{Translator.Text("TAG_FIELD_NO_RESULTS")}'}}); ");
            output.Write($"jQuery('{selector}').on('change', WeBlog.tagField.onTagFieldUpdated); ");
            output.Write($"jQuery('{selector}').on('chosen:showing_dropdown', WeBlog.tagField.onDropDown); ");
            output.Write($"jQuery('{selector}').on('chosen:hiding_dropdown', WeBlog.tagField.onHideDropDown); </script>");
        }
    }
}