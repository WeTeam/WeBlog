using System;
using Joel.Net;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Items.Custom;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Items.WeBlog
{
    public partial class CommentItem : CustomItem
    {
        public static readonly string TemplateId = "{70949D4E-35D8-4581-A7A2-52928AA119D5}";

        public CommentItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator CommentItem(Item innerItem)
        {
            return innerItem != null ? new CommentItem(innerItem) : null;
        }

        public static implicit operator Item(CommentItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        public CustomTextField Name
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["Name"]); }
        }


        public CustomTextField Email
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["Email"]); }
        }


        public CustomTextField Comment
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["Comment"]); }
        }


        public CustomTextField Website
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["Website"]); }
        }


        public CustomTextField IpAddress
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["IP Address"]); }
        }

        /// <summary>
        /// Gets the creation date of the comment item.
        /// </summary>
        public DateTime Created
        {
            get
            {
#if FEATURE_UTC_DATE
                return DateUtil.ToServerTime(InnerItem.Statistics.Created);
#else
              return InnerItem.Statistics.Created;
#endif
            }
        }

        /// <summary>
        /// For use with NVelocity token in email template
        /// </summary>
        public string AuthorName
        {
            get { return Name.Raw; }
        }

        /// <summary>
        /// Convert the comment to an AkismetComment for submission to Akismet
        /// </summary>
        /// <param name="comment">The comment to convert</param>
        /// <returns>An Akismet comment</returns>
        public static implicit operator AkismetComment(CommentItem comment)
        {
            var url = string.Empty;
            var blog = ManagerFactory.BlogManagerInstance.GetCurrentBlog();
            if (blog != null)
                url = blog.Url;
            else
                url = Context.Site.HostName;

            var akismetComment = new AkismetComment();
            akismetComment.Blog = url;
            akismetComment.UserIp = comment.IpAddress.Text;
            akismetComment.UserAgent = ""; // TODO
            akismetComment.CommentContent = comment.Comment.Text;
            akismetComment.CommentType = "comment";
            akismetComment.CommentAuthor = comment.AuthorName;
            akismetComment.CommentAuthorEmail = comment.Email.Text;
            akismetComment.CommentAuthorUrl = comment.Website.Text;

            return akismetComment;
        }

    }
}