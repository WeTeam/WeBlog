using System.Web.Mvc;
using Recaptcha;
using Sitecore.Modules.WeBlog.Captcha;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class BlogSubmitCommentController : BlogBaseController
    {
        protected ISubmitCommentCore SubmitCommentCore { get; set; }

        public BlogSubmitCommentController() : this(null) { }

        public BlogSubmitCommentController(ISubmitCommentCore submitCommentCore)
        {
            SubmitCommentCore = submitCommentCore ?? new SubmitCommentCore();
        }

        public ActionResult Index()
        {
            var currentBlog = Managers.ManagerFactory.BlogManagerInstance.GetCurrentBlog(CurrentEntry);
            if (!currentBlog.EnableComments.Checked || CurrentEntry.DisableComments.Checked)
            {
                return null;
            }
            HandleSubmitRedirect();
            return View("~/Views/WeBlog/SubmitComment.cshtml", new SubmitCommentRenderingModel());
        }

        [HttpPost]
        [RecaptchaControlMvc.CaptchaValidator]
        [CaptchaValidator]
        public ActionResult Index(SubmitCommentRenderingModel model, bool captchaValid, string captchaErrorMessage)
        {
            if (ModelState.IsValid && captchaValid)
            {
                var comment = BuildComment(model);
                var submissionResult = SubmitCommentCore.Submit(comment);
                if (submissionResult.IsNull)
                {
                    Session["Submitted"] = false;
                }
                else
                {
                    Session["Submitted"] = true;
                }
                return Redirect(HttpContext.Request.Url.AbsoluteUri);
            }
            ValidateModelState(model);
            if (!captchaValid)
            {
                ModelState.AddModelError("", "The text you typed does not match the text in the image.");
            }
            TempData["ScrollTo"] = true;
            return View("~/Views/WeBlog/SubmitComment.cshtml");
        }

        protected virtual void HandleSubmitRedirect()
        {
            if (Session["Submitted"] != null)
            {
                TempData["Submitted"] = Session["Submitted"];
                Session.Remove("Submitted");
            }
        }

        protected virtual Comment BuildComment(SubmitCommentRenderingModel model)
        {
            var comment = new Comment
            {
                AuthorName = model.UserName,
                Text = model.Comment,
                AuthorEmail = model.Email
            };
            comment.Fields.Add(Constants.Fields.Website, model.Website);
            comment.Fields.Add(Constants.Fields.IpAddress, HttpContext.Request.UserHostAddress);
            return comment;
        }

        protected virtual void ValidateModelState(SubmitCommentRenderingModel model)
        {
            if (model.UserName == null)
            {
                ModelState.AddModelError("", "UserName");
            }
            if (model.Email == null)
            {
                ModelState.AddModelError("", "Email");
            }
            if (model.Comment == null)
            {
                ModelState.AddModelError("", "Comment");
            }
        }
    }
}