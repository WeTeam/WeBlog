using Sitecore.Modules.WeBlog.Globalization;

namespace Sitecore.Modules.WeBlog.Pipelines.ValidateComment
{
    /// <summary>
    /// A processor for the weblogValidateComment pipeline which validates the email in the comment is not empty.
    /// </summary>
    public class EmailProvided
    {
        public void Process(ValidateCommentArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.Comment.AuthorEmail))
            {
                var errorPhrase = Translator.Text(Constants.TranslationPhrases.RequiredField);
                var field = Translator.Text(Constants.TranslationPhrases.Email);
                var text = string.Format(errorPhrase, field);
                args.Errors.Add(text);
            }
        }
    }
}
