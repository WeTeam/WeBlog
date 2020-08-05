using Sitecore.Modules.WeBlog.Globalization;

namespace Sitecore.Modules.WeBlog.Pipelines.ValidateComment
{
    /// <summary>
    /// A processor for the weblogValidateComment pipeline which validates the text in the comment is not empty.
    /// </summary>
    public class CommentProvided
    {
        public void Process(ValidateCommentArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.Comment.Text))
            {
                var errorPhrase = Translator.Text(Constants.TranslationPhrases.RequiredField);
                var field = Translator.Text(Constants.TranslationPhrases.Comment);
                var text = string.Format(errorPhrase, field);
                args.Errors.Add(text);
            }
        }
    }
}
