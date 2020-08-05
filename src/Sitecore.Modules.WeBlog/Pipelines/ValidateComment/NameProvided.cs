using Sitecore.Modules.WeBlog.Globalization;

namespace Sitecore.Modules.WeBlog.Pipelines.ValidateComment
{
    /// <summary>
    /// A processor for the weblogValidateComment pipeline which validates the name in the comment is not empty.
    /// </summary>
    public class NameProvided
    {
        public void Process(ValidateCommentArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.Comment.AuthorName))
            {
                var errorPhrase = Translator.Text(Constants.TranslationPhrases.RequiredField);
                var field = Translator.Text(Constants.TranslationPhrases.Name);
                var text = string.Format(errorPhrase, field);
                args.Errors.Add(text);
            }
        }
    }
}
