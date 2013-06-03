using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Diagnostics;

namespace Sitecore.Modules.WeBlog.Pipelines.CreateComment
{
    public class AddHash : ICreateCommentProcessor
    {
        /// <summary>
        /// Gets or sets the number of comment hashes to store in the cookie
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// Gets or sets the expiration age of the cookie in minutes
        /// </summary>
        public int CookieExpirationAge { get; set; }

        public AddHash()
        {
            CommentCount = 10;
            CookieExpirationAge = 10080;
        }

        public void Process(CreateCommentArgs args)
        {
            Assert.IsNotNull(args.Comment, "Comment DTO cannot be null");

            if (HttpContext.Current != null)
            {
                var cookie = HttpContext.Current.Request.Cookies[Constants.CookieName];
                if (cookie == null)
                    cookie = new HttpCookie(Constants.CookieName);

                var hashes = new List<string>();

                if (cookie.Value != null)
                    hashes.AddRange(cookie.Value.Split(new char[] { ';' }));

                hashes.Add(args.Comment.GetHash());

                if (hashes.Count > CommentCount)
                    hashes.RemoveRange(0, hashes.Count - CommentCount);

                cookie.Value = string.Join(";", hashes);
                cookie.Expires = DateTime.Now.AddMinutes(CookieExpirationAge);

                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
    }
}