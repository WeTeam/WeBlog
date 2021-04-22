using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sitecore.Modules.WeBlog.Model
{
    /// <summary>
    /// Represents an abstract comment
    /// </summary>
    [DataContract]
    public class Comment
    {
        /// <summary>
        /// Gets or sets the name of the author
        /// </summary>
        [DataMember]
        public string AuthorName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the author's email address
        /// </summary>
        [DataMember]
        public string AuthorEmail
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text of the comment
        /// </summary>
        [DataMember]
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a collection of additional fields for this comment submission
        /// </summary>
        [DataMember]
        public Dictionary<string,string> Fields
        {
            get;
            set;
        }

        public Comment()
        {
            Fields = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets a hash for this comment
        /// </summary>
        /// <returns>A hash of the comment</returns>
        public virtual string GetHash()
        {
            var data = AuthorName + "-" + "-" + AuthorEmail + "-" + Text;
            return Crypto.GetMD5Hash(data);
        }
    }
}
