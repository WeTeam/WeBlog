using System.ServiceModel.Activation;
using Sitecore.Data;
using Sitecore.Globalization;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.SecurityModel;

namespace Sitecore.Modules.WeBlog.Services
{
    // Require ASP.NET so we get HttpContext or Sitecore.NVelocity assembly might not work properly
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class CommentService : ICommentService
    {
        public ID SubmitComment(ID EntryId, Model.Comment comment, Language language)
        {
            // Sitecore is not preprocessing the WCF request, so much of the Sitecore context is not set.
            using (new SecurityDisabler())
            {
                var toRet = ManagerFactory.CommentManagerInstance.AddCommentToEntry(EntryId, comment, language);
                return toRet;
            }
        }
    }
}
