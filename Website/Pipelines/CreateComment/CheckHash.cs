using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Diagnostics;

namespace Sitecore.Modules.WeBlog.Pipelines.CreateComment
{
    public class CheckHash : ICreateCommentProcessor
    {
        public void Process(CreateCommentArgs args)
        {
            Assert.IsNotNull(args.Comment, "Comment DTO cannot be null");

            if (HttpContext.Current != null)
            {
                // Get cookie
                var cookie = HttpContext.Current.Request.Cookies[Constants.CookieName];
                if (cookie != null)
                {
                    // get the most recent comment hashes
                    var hashes = cookie.Value.Split(new char[] {';'});
                    var currentHash = args.Comment.GetHash();

                    if(hashes.Contains(currentHash))
                        args.AbortPipeline();
                }
            }
        }
    }
}