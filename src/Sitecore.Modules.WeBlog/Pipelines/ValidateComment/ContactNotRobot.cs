using Sitecore.Analytics;
using Sitecore.Analytics.Core;
using Sitecore.Modules.WeBlog.Globalization;

namespace Sitecore.Modules.WeBlog.Pipelines.ValidateComment
{
    /// <summary>
    /// A processor for the weblogValidateComment pipeline which validates the user is not a robot.
    /// </summary>
    public class ContactNotRobot
    {
        public void Process(ValidateCommentArgs args)
        {
            if (ContactClassification.IsRobot(Tracker.Current.Contact.System.Classification))
            {
                var text = Translator.Text(Constants.TranslationPhrases.ErrorOccurredTryAgain);
                args.Errors.Add(text);
            }
        }
    }
}
