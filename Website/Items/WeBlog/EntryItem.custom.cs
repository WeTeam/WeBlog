using System;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Security.Accounts;

namespace Sitecore.Modules.WeBlog.Items.WeBlog
{
    public partial class EntryItem
    {
        public string[] TagsSplit
        {
            get
            {
                return ExtractTags(false);
            }
            set
            {
                this.Tags.Field.Value = FormTags(value);
            }
        }

        /// <summary>
        /// Gets the URL of the entry
        /// </summary>
        public string Url
        {
            get
            {
                return LinkManager.GetItemUrl(InnerItem);
            }
        }

        /// <summary>
        /// Gets the absolute URL of the entry including the server
        /// </summary>
        public string AbsoluteUrl
        {
            get
            {
                UrlOptions urlOptions = UrlOptions.DefaultOptions;
                urlOptions.AlwaysIncludeServerUrl = true;
                return LinkManager.GetItemUrl(InnerItem, urlOptions);
            }
        }

        /// <summary>
        /// Gets the count of comments for this blog entry.
        /// </summary>
        public int CommentCount
        {
            get
            {
                return ManagerFactory.CommentManagerInstance.GetCommentsCount(this.ID);
            }
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
                string createdBy = InnerItem[Sitecore.FieldIDs.CreatedBy];
                if (!string.IsNullOrEmpty(createdBy))
                {
                    return User.FromName(createdBy, AccountType.User) as User;
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
                    if (User.Exists(Author.Raw))
                    {
                        var user = User.FromName(Author.Raw, false);
                        if (user != null)
                        {
                            var name = user.LocalName;
                            if (user.Profile != null && !string.IsNullOrEmpty(user.Profile.FullName))
                                name = user.Profile.FullName;

                            return name;
                        }
                    }
                }
                else if (string.IsNullOrEmpty(Author.Raw) || Author.Raw == "$username")
                    return CreatedBy.LocalName;

                return Author.Rendered;
            }
        }

        /// <summary>
        /// Gets the title of the entry or the name if the title is empty
        /// </summary>
        public string DisplayTitle
        {
            get
            {
                if (!string.IsNullOrEmpty(Title.Text))
                    return Title.Text;

                return Name;
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
    }
}