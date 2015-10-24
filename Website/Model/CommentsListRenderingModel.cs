using System;
using Sitecore.Modules.WeBlog.Items.WeBlog;

namespace Sitecore.Modules.WeBlog.Model
{
    public class CommentsListRenderingModel
    {
        public CommentItem[] Comments { get; set; }
        public bool EnableGravatar { get; set; }
        public int GravatarSizeNumeric { get; set; }
        public bool ShowEmailWithinComments { get; set; }
        public Func<string,string> GetGravatarUrl { get; set; }
    }
}