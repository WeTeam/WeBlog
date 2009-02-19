using System;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Web;

namespace Sitecore.Modules.Eviblog.Items
{
    public class Entry : CustomItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Entry"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public Entry(Item item)
            : base(item)
        {
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get
            {
                return InnerItem["Title"];
            }
            set
            {
                InnerItem["Title"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the introduction.
        /// </summary>
        /// <value>The introduction.</value>
        public string Introduction
        {
            get
            {
                return InnerItem["Introduction"];
            }
            set
            {
                InnerItem["Introduction"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                return InnerItem["Content"];
            }
            set
            {
                InnerItem["Content"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>The tags.</value>
        public string[] Tags
        {
            get
            {
                return ExtractTags(false);
            }
            set
            {
                InnerItem["Tags"] = FormTags(value);
            }
        }

        public string Categories
        {
            get
            {
                return InnerItem["Category"];
            }
            set
            {
                InnerItem["Category"] = value.ToString();
            }

        }
        
        public string[] CategoriesText
        {
            get
            {
                return ExtractCategories(true);
            }
            set
            {
                InnerItem["Category"] = value.ToString();
            }
        
        }

        public string[] CategoriesID
        {
            get
            {
                return ExtractCategories(false);
            }
            set
            {
                InnerItem["Category"] = value.ToString();
            }

        }

        public string Url
        {
            get
            {
                return "http://" + WebUtil.GetHostName() +LinkManager.GetItemUrl(InnerItem);
            }
            set {}
        }

        /// <summary>
        /// Gets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created
        {
            get
            {
                return InnerItem.Statistics.Created;
            }
        }

        /// <summary>
        /// Forms the tags.
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <returns></returns>
        static string FormTags(string[] tags)
        {
            string tagsString = string.Empty;
            for (int i = 0; i < tags.Length; i++)
            {
                tagsString += tags[i].Trim();
                tagsString += i < (tags.Length - 1) ? "," : string.Empty;
            }
            return tagsString;
        }

        /// <summary>
        /// Extracts the tags.
        /// </summary>
        /// <param name="isEncode">if set to <c>true</c> [is encode].</param>
        /// <returns></returns>
        string[] ExtractTags(bool isEncode)
        {
            string[] tags = InnerItem["Tags"].Trim().Split(',');
            System.Collections.ArrayList result = new System.Collections.ArrayList();
            for (int i = 0; i < tags.Length; i++)
            {
                if (tags[i].Trim() != string.Empty)
                {
                    if (isEncode)
                    {
                        result.Add(System.Web.HttpUtility.HtmlEncode(tags[i]));
                    }
                    else
                    {
                        result.Add(tags[i].Trim());
                    }
                }
            }
            return (string[])result.ToArray(typeof(string));
        }

        string[] ExtractCategories(bool isText)
        {
            string[] categories = InnerItem["Category"].Split('|');
            System.Collections.ArrayList result = new System.Collections.ArrayList();
            for (int i = 0; i < categories.Length; i++)
            {
                if (categories[i].Trim() != string.Empty)
                {
                    if (isText)
                    {
                        Item categorie = Sitecore.Context.Database.GetItem(categories[i]);
                        result.Add(categorie.Name);
                    }
                    else
                    {
                        Item categorie = Sitecore.Context.Database.GetItem(categories[i]);
                        result.Add(categorie.ID.ToString());
                    }
                }
            }
            return (string[])result.ToArray(typeof(string));
        }
    }
}
