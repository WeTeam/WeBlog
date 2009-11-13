using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Web;

namespace Sitecore.Modules.Eviblog.Items
{
    public class Blog : CustomItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Blog"/> class.
        /// </summary>
        /// <param name="item">The blog.</param>
        public Blog(Item item): base(item)
        {
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get
            {
                return InnerItem["Title"];
            }
            set
            {
                InnerItem["Title"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email
        {
            get
            {
                return InnerItem["Email"];
            }
            set
            {
                InnerItem["Email"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the display item count.
        /// </summary>
        /// <value>The display item count.</value>
        public int DisplayItemCount
        {
            get
            {
                return System.Convert.ToInt32(InnerItem["DisplayItemCount"]);
            }
            set
            {
                InnerItem["DisplayItemCount"] = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>The theme.</value>
        public string Theme
        {
            get
            {
                return InnerItem["Theme"];
            }
            set
            {
                InnerItem["Theme"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the display comment sidebar count.
        /// </summary>
        /// <value>The display comment sidebar count.</value>
        public int DisplayCommentSidebarCount
        {
            get
            {
                return System.Convert.ToInt32(InnerItem["DisplayCommentSidebarCount"]);
            }
            set
            {
                InnerItem["DisplayCommentSidebarCount"] = value.ToString();
            }
        }


        /// <summary>
        /// Gets or sets the enable RSS.
        /// </summary>
        /// <value>The enable RSS.</value>
        public bool EnableRSS
        {
            get
            {
                if(InnerItem["Enable RSS"] == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                switch (value)
                {
	                case true:
                        InnerItem["Enable RSS"] = "1";
                        break;
                    case false:
                        InnerItem["Enable RSS"] = "0";
                        break;
                }
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether [show email within comments].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show email within comments]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowEmailWithinComments
        {
            get
            {
                if (InnerItem["Show Email Within Comments"] == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                switch (value)
                {
                    case true:
                        InnerItem["Show Email Within Comments"] = "1";
                        break;
                    case false:
                        InnerItem["Show Email Within Comments"] = "0";
                        break;
                }
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether [enable live writer].
        /// </summary>
        /// <value><c>true</c> if [enable live writer]; otherwise, <c>false</c>.</value>
        public bool EnableLiveWriter
        {
            get
            {
                if (InnerItem["EnableLiveWriter"] == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                switch (value)
                {
                    case true:
                        InnerItem["EnableLiveWriter"] = "1";
                        break;
                    case false:
                        InnerItem["EnableLiveWriter"] = "0";
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url
        {
            get
            {
                return "http://" + WebUtil.GetHostName() + LinkManager.GetItemUrl(InnerItem);
            }
        }
    }
}
