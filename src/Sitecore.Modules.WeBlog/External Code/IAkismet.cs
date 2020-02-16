/* Author:      Joel Thoms (http://joel.net)
 * Copyright:   2006 Joel Thoms (http://joel.net)
 * About:       Akismet (http://akismet.com) .Net 2.0 API allow you to check
 *              Akismet's spam database to verify your comments and prevent spam.
 * 
 * Note:        Do not launch 'DEBUG' code on your site.  Only build 'RELEASE' for your site.  Debug code contains
 *              Console.WriteLine's, which are not desireable on a website.
*/
namespace Joel.Net
{
    public interface IAkismet
    {
        void Init(string apiKey, string blog, string userAgent);
        bool CommentCheck(AkismetComment comment);
        void SubmitHam(AkismetComment comment);
        void SubmitSpam(AkismetComment comment);
        bool VerifyKey();
    }
}