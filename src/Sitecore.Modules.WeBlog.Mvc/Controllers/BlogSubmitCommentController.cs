using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Modules.WeBlog.Mvc.Components.Parameters;
using Sitecore.Modules.WeBlog.Mvc.Model;
using System.Web.Mvc;

namespace Sitecore.Modules.WeBlog.Mvc.Controllers
{
    public class BlogSubmitCommentController : BlogBaseController
    {
        /// <summary>
        /// Gets the <see cref="ISubmitCommentCore"/> to use to submit a comment.
        /// </summary>
        protected ISubmitCommentCore SubmitCommentCore { get;  }

        /// <summary>
        /// Gets the <see cref="IValidateCommentCore"/> to use to validate submitted comments.
        /// </summary>
        protected IValidateCommentCore ValidateCommentCore { get; }

        public BlogSubmitCommentController(ISubmitCommentCore submitCommentCore, IValidateCommentCore validateCommentCore)
        {
            SubmitCommentCore = submitCommentCore;
            ValidateCommentCore = validateCommentCore;
            new RenderingParameterHelper<Controller>(this, true);
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
        public ActionResult Index(SubmitCommentRenderingModel model)
        {
            var comment = BuildComment(model);
            var result = ValidateCommentCore.Validate(comment, HttpContext.Request.Form);

            if (ModelState.IsValid && result.Success)
            {
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

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
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
    }
}