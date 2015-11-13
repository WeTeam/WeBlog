using System;
using System.Linq;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Items.Custom;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Security.Accounts;

namespace Sitecore.Modules.WeBlog.Items.WeBlog
{
    public class EntryItem : CustomItem
    {
        public EntryItem(Item innerItem) : base(innerItem) { }

        public static implicit operator EntryItem(Item innerItem)
        {
            return innerItem != null ? new EntryItem(innerItem) : null;
        }

        public static implicit operator Item(EntryItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        public CustomMultiListField Category
        {
            get { return new CustomMultiListField(InnerItem, InnerItem.Fields["Category"]); }
        }


        public CustomCheckboxField DisableComments
        {
            get { return new CustomCheckboxField(InnerItem, InnerItem.Fields["Disable Comments"]); }
        }


        public CustomTextField Title
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["Title"]); }
        }


        public CustomTextField Introduction
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["Introduction"]); }
        }


        public CustomTextField Tags
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["Tags"]); }
        }


        public CustomTextField Content
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["Content"]); }
        }

        public CustomTextField Author
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["Author"]); }
        }

        public CustomDateField EntryDate
        {
            get { return new CustomDateField(InnerItem, InnerItem.Fields["Entry Date"]); }
        }

        public CustomImageField Image
        {
            get { return new CustomImageField(InnerItem, InnerItem.Fields["Image"]); }
        }


        public CustomImageField ThumbnailImage
        {
            get { return new CustomImageField(InnerItem, InnerItem.Fields["Thumbnail Image"]); }
        }

        public string[] TagsSplit
        {
            get { return ExtractTags(false); }
            set { Tags.Field.Value = FormTags(value); }
        }

        /// <summary>
        /// Gets the URL of the entry
        /// </summary>
        public string Url
        {
            get { return LinkManager.GetItemUrl(InnerItem); }
        }

        /// <summary>
        /// Gets the absolute URL of the entry including the server
        /// </summary>
        public string AbsoluteUrl
        {
            get
            {
                var urlOptions = UrlOptions.DefaultOptions;
                urlOptions.AlwaysIncludeServerUrl = true;
                return LinkManager.GetItemUrl(InnerItem, urlOptions);
            }
        }

        /// <summary>
        /// Gets the count of comments for this blog entry.
        /// </summary>
        public int CommentCount
        {
            get { return ManagerFactory.CommentManagerInstance.GetCommentsCount(ID); }
        }

        /// <summary>
        /// Gets the creation date of the blog entry item.
        /// </summary>
        public DateTime Created
        {
            get
            {
                return EntryDate.DateTime != DateTime.MinValue ? EntryDate.DateTime : InnerItem.Statistics.Created;
            }
        }

        public User CreatedBy
        {
            get
            {
                //go direct to field since ItemStatistics ignores standard values
                var createdBy = InnerItem[FieldIDs.CreatedBy];
                if (!string.IsNullOrEmpty(createdBy))
                {
                    return Account.FromName(createdBy, AccountType.User) as User;
                }
                return null;
            }
        }

        public string AuthorName
        {
            get
            {
                if (Author.Raw.Contains("\\"))
                {
                    // if we find the user, return the local name
                    if (!User.Exists(Author.Raw)) return Author.Rendered;

                    var user = User.FromName(Author.Raw, false);

                    if (user == null) return Author.Rendered;

                    var name = user.LocalName;

                    if (user.Profile != null && !string.IsNullOrEmpty(user.Profile.FullName))
                        name = user.Profile.FullName;

                    return name;
                }

                if (string.IsNullOrEmpty(Author.Raw) || Author.Raw == "$currentuser") return CreatedBy.LocalName;

                return Author.Rendered;
            }
        }

        /// <summary>
        /// Gets the title of the entry or the name if the title is empty
        /// </summary>
        public string DisplayTitle
        {
            get { return !string.IsNullOrEmpty(Title.Text) ? Title.Text : Name; }
        }

        /// <summary>
        /// Forms the tags.
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <returns></returns>
        static string FormTags(string[] tags)
        {
            var tagsString = string.Empty;
            for (var i = 0; i < tags.Length; i++)
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
            var result = new System.Collections.ArrayList();
            foreach (var tag in tags.Where(tag => tag.Trim() != string.Empty))
            {
                result.Add(isEncode ? System.Web.HttpUtility.HtmlEncode(tag) : tag.Trim());
            }
            return (string[])result.ToArray(typeof(string));
        }


        public bool DoesFieldRequireWrapping(string fieldName)
        {
            return InnerItem.DoesFieldRequireWrapping(fieldName);
        }
    }
}