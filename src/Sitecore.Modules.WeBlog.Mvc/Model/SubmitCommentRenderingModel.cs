using System.ComponentModel.DataAnnotations;

namespace Sitecore.Modules.WeBlog.Mvc.Model
{
    public class SubmitCommentRenderingModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Comment { get; set; }
        [Required]
        public string Email { get; set; }
        public string Website { get; set; }
    }
}