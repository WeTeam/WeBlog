using System;
using System.IO;
using System.Linq;
using System.Web;
using CookComputing.XmlRpc;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Resources.Media;
using Sitecore.Security.Authentication;

namespace Sitecore.Modules.WeBlog
{
    [XmlRpcService(
    Name = "Sitecore WeBlog Module",
    Description = "This is XML-RPC which implements the MetaWeblog API.",
    AutoDocumentation = true)]
    public class MetaBlogApi : XmlRpcService
    {
        #region Helpers
        /// <summary>
        /// Authenticate user
        /// </summary>
        /// <param name="username">UserName</param>
        /// <param name="password">Password</param>
        protected virtual void Authenticate(string username, string password)
        {
            var allowed = AuthenticationManager.Login(username, password);

            if (!allowed)
            {
                throw new System.Security.Authentication.InvalidCredentialException("Invalid credentials. Access denied");
            }
        }

        /// <summary>
        /// Ensure the user has adequate rights to the blog.
        /// </summary>
        /// <param name="blogid">The ID of the blog to check the rights of.</param>
        /// <param name="username">The username of the user to check the rights for.</param>
        protected virtual void CheckUserRights(string blogid, string username)
        {
            var blog = ItemManager.GetItem(blogid, Sitecore.Context.Language, Sitecore.Data.Version.Latest, ContentHelper.GetContentDatabase());
            if (blog != null && !blog.Security.CanWrite(Sitecore.Context.User))
                throw new System.Security.Authentication.InvalidCredentialException("You do not have sufficient user rights");            
        }

        /// <summary>
        /// Gets the categories as string.
        /// </summary>
        /// <param name="postid">The postid.</param>
        /// <param name="rpcstruct">The rpcstruct.</param>
        /// <returns></returns>
        private static string GetCategoriesAsString(string postid, XmlRpcStruct rpcstruct)
        {
            var postItem = ContentHelper.GetContentDatabase().GetItem(postid);
            Item blog = ManagerFactory.BlogManagerInstance.GetCurrentBlog(postItem).SafeGet(x => x.InnerItem);
            var categoryList = ManagerFactory.CategoryManagerInstance.GetCategories(blog);

            if (categoryList.Length != 0)
            {
                string selectedCategories = string.Empty;

                try
                {
                    string[] categories = (string[])rpcstruct["categories"];

                    foreach (string category in categories)
                    {
                        foreach (CategoryItem cat in categoryList)
                        {
                            if (category == cat.Title.Raw)
                            {
                                selectedCategories += cat.ID.ToString();
                            }
                        }
                    }

                    string result = selectedCategories.Replace("}{", "}|{");

                    return result;
                }
                catch
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the categories as string.
        /// </summary>
        /// <param name="BlogID">The blog ID.</param>
        /// <param name="rpcstruct">The rpcstruct.</param>
        /// <returns></returns>
        private static string GetCategoriesAsString(ID BlogID, XmlRpcStruct rpcstruct)
        {
            var blog = ContentHelper.GetContentDatabase().GetItem(BlogID);
            if (blog != null)
            {
                var categoryList = ManagerFactory.CategoryManagerInstance.GetCategories(blog);

                var selectedCategories = string.Empty;

                if (rpcstruct["categories"] != null && ((object[])rpcstruct["categories"]).Count() != 0)
                {
                    var categories = (string[])rpcstruct["categories"];

                    foreach (string category in categories)
                    {
                        foreach (CategoryItem cat in categoryList)
                        {
                            if (category == cat.Title.Raw)
                            {
                                selectedCategories += cat.ID.ToString();
                            }
                        }
                    }

                    var result = selectedCategories.Replace("}{", "}|{");

                    return result;
                }
            }

            return string.Empty;
        }
        #endregion

        #region blogger.getUsersBlogs
        /// <summary>
        /// Returns user's blogs
        /// </summary>
        /// <param name="appKey">Application Key</param>
        /// <param name="username">UserName</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        [XmlRpcMethod("blogger.getUsersBlogs")]
        public XmlRpcStruct[] getUsersBlogs(string appKey, string username, string password)
        {
            int ii = 0;
            Authenticate(username, password);

            var blogList = ManagerFactory.BlogManagerInstance.GetUserBlogs(username);

            //Create structure for blog list
            XmlRpcStruct[] blogs = new XmlRpcStruct[blogList.Length];
            foreach (BlogHomeItem blog in blogList)
            {
                XmlRpcStruct rpcstruct = new XmlRpcStruct();
                rpcstruct.Add("blogid", blog.ID.ToString()); // Blog Id
                rpcstruct.Add("blogName", blog.Title.Raw); // Blog Name
                rpcstruct.Add("url", blog.AbsoluteUrl);
                blogs[ii] = rpcstruct;
                ii++;
            }

            return blogs;
        }
        #endregion

        #region metaWeblog.setTemplate (not implemented)
        ///// <summary>
        ///// Set blog post template
        ///// </summary>
        ///// <param name="appKey">Application Key</param>
        ///// <param name="blogid">Blog Identificator</param>
        ///// <param name="username">UserName</param>
        ///// <param name="password">Password</param>
        ///// <param name="template">Template content</param>
        ///// <param name="templateType">Template Type</param>
        ///// <returns></returns>
        //[XmlRpcMethod("metaWeblog.setTemplate")]
        //public bool setTemplate(string appKey, string blogid, string username, string password, string template, string templateType)
        //{
        //    Authenticate(username, password);
        //    /*
        //        TODO: Add implementation
        //    */
        //    throw new System.NotImplementedException("SetTemplate is not implemented");
        //}
        #endregion

        #region metaWeblog.getCategories
        /// <summary>
        /// Return list of blog category list
        /// </summary>
        /// <param name="blogid">Blog Identificator</param>
        /// <param name="username">UserName</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        [XmlRpcMethod("metaWeblog.getCategories")]
        public XmlRpcStruct[] getCategories(string blogid, string username, string password)
        {
            int ii = 0;
            Authenticate(username, password);

            var blog = ContentHelper.GetContentDatabase().GetItem(blogid);
            if (blog != null)
            {
                var categoryList = ManagerFactory.CategoryManagerInstance.GetCategories(blog);

                XmlRpcStruct[] categories = new XmlRpcStruct[categoryList.Length];

                foreach (CategoryItem category in categoryList)
                {
                    XmlRpcStruct rpcstruct = new XmlRpcStruct();
                    rpcstruct.Add("categoryid", category.ID.ToString()); // Category ID
                    rpcstruct.Add("title", category.Title.Raw); // Category Title
                    rpcstruct.Add("description", "Description is not available"); // Category Description
                    categories[ii] = rpcstruct;
                    ii++;
                }

                return categories;
            }

            return new XmlRpcStruct[0];

        }
        #endregion

        #region metaWeblog.getRecentPosts
        /// <summary>
        /// Returns recent blog posts
        /// </summary>
        /// <param name="blogid">Blog Identificator</param>
        /// <param name="username">UserName</param>
        /// <param name="password">Password</param>
        /// <param name="numberOfPosts">Number of posts to return</param>
        /// <returns></returns>
        [XmlRpcMethod("metaWeblog.getRecentPosts")]
        public XmlRpcStruct[] getRecentPosts(string blogid, string username, string password, int numberOfPosts)
        {
            int ii = 0;
            Authenticate(username, password);

            var blog = ContentHelper.GetContentDatabase().GetItem(blogid);
            if (blog != null)
            {
                var entryList = ManagerFactory.EntryManagerInstance.GetBlogEntries(blog, numberOfPosts, null, null, (DateTime?)null);

                XmlRpcStruct[] posts = new XmlRpcStruct[entryList.Length];

                //Populate structure with post entities
                foreach (EntryItem entry in entryList)
                {
                    XmlRpcStruct rpcstruct = new XmlRpcStruct();
                    rpcstruct.Add("title", entry.Title.Raw);
                    rpcstruct.Add("link", entry.AbsoluteUrl);
                    rpcstruct.Add("description", entry.Content.Text);
                    rpcstruct.Add("pubDate", entry.EntryDate.DateTime);
                    rpcstruct.Add("guid", entry.ID.ToString());
                    rpcstruct.Add("postid", entry.ID.ToString());
                    rpcstruct.Add("keywords", entry.Tags.Raw);
                    rpcstruct.Add("author", entry.InnerItem.Statistics.CreatedBy);
                    posts[ii] = rpcstruct;
                    ii++;
                }
                return posts;
            }

            return new XmlRpcStruct[0];
        }
        #endregion

        #region metaWeblog.getTemplate (not implemented)
        /// <summary>
        /// Return post template
        /// </summary>
        /// <param name="appKey">Application Key</param>
        /// <param name="blogid">Blog Identificator</param>
        /// <param name="username">UserName</param>
        /// <param name="password">Password</param>
        /// <param name="templateType">Template Type</param>
        /// <returns></returns>
        [XmlRpcMethod("metaWeblog.getTemplate")]
        public string getTemplate(string appKey, string blogid, string username, string password, string templateType)
        {
            Authenticate(username, password);

            CheckUserRights(blogid, username);

            // TODO: add implementation with datasource
            string template = @"<HTML>
                          <HEAD>
                            <TITLE><$BlogTitle$>: <$BlogDescription$></TITLE>
                          </HEAD>
                          <BODY >
                            <h1><$BlogTitle$></h1>

                              <!-- Blogger code begins here -->

                              <BLOGGER>
                               <BlogDateHeader>
                                   <b><h4><$BlogDateHeaderDate$>:</h4></b>
                              </BlogDateHeader>
                            
                              <a name='<$BlogItemNumber$>'><$BlogItemBody$></a>
                              <br>
                              <small><$BlogItemAuthor$> 
                              <br>
                              <center>______________________</center>
                              <br>
                              </p>
                              </BLOGGER>
                           '
                          </BODY>
                        </HTML>";
            return template;
        }
        #endregion

        #region metaWeblog.newPost
        /// <summary>
        /// Create new Post
        /// </summary>
        /// <param name="blogid">Blog Identificator</param>
        /// <param name="username">UserName</param>
        /// <param name="password">Password</param>
        /// <param name="rpcstruct">Blog post XML-RPC structure</param>
        /// <param name="publish">TRUE to publish post after creation</param>
        /// <returns></returns>
        [XmlRpcMethod("metaWeblog.newPost")]
        public string newPost(string blogid, string username, string password, XmlRpcStruct rpcstruct, bool publish)
        {
            Authenticate(username, password);
            CheckUserRights(blogid, username);
            
            var entryTitle = rpcstruct["title"].ToString();
            var currentBlog = ContentHelper.GetContentDatabase().GetItem(blogid);

            if (currentBlog != null)
            {
                // test
                var access = Sitecore.Security.AccessControl.AuthorizationManager.GetAccess(currentBlog, Sitecore.Context.User, Sitecore.Security.AccessControl.AccessRight.ItemCreate);
                // end test

                BlogHomeItem blogItem = currentBlog;
                var template = new TemplateID(blogItem.BlogSettings.EntryTemplateID);
                var newItem = ItemManager.AddFromTemplate(entryTitle, template, currentBlog);

                SetItemData(newItem, rpcstruct);

                if (publish)
                    ContentHelper.PublishItemAndRequiredAncestors(newItem.ID);

                return newItem.ID.ToString();
            }
            else
                return string.Empty;
        }


        #endregion

        #region metaWeblog.editPost
        /// <summary>
        /// Edit existing Post
        /// </summary>
        /// <param name="postid">Post Identificator</param>
        /// <param name="username">UserName</param>
        /// <param name="password">Password</param>
        /// <param name="rpcstruct">Blog post XML-RPC structure</param>
        /// <param name="publish">TRUE to publish post after modification</param>
        /// <returns></returns>
        [XmlRpcMethod("metaWeblog.editPost")]
        public bool editPost(string postid, string username, string password, XmlRpcStruct rpcstruct, bool publish)
        {
            Authenticate(username, password);
            CheckUserRights(postid, username);

            var item = ContentHelper.GetContentDatabase().GetItem(new ID(postid));

            if (item != null)
            {
                SetItemData(item, rpcstruct);

                if (publish)
                    ContentHelper.PublishItemAndRequiredAncestors(item.ID);
            }
            else
                return false;

            return true;
        }
        #endregion

        /// <summary>
        /// Sets the item data from an XML RPC struct
        /// </summary>
        /// <param name="item">The item to set the data on</param>
        /// <param name="rpcstruct">The struct to read the data from</param>
        private void SetItemData(Item item, XmlRpcStruct rpcstruct)
        {
            if (item != null)
            {
                var entry = new EntryItem(item);

                entry.BeginEdit();

                if (rpcstruct["title"] != null)
                    entry.Title.Field.Value = rpcstruct["title"].ToString();

                if (rpcstruct["description"] != null)
                    entry.Content.Field.Value = rpcstruct["description"].ToString();

                if (rpcstruct["categories"] != null)
                    entry.Category.Field.Value = GetCategoriesAsString(item.ID.ToString(), rpcstruct);

                if (rpcstruct["dateCreated"] != null)
                {
                    DateTime publishDate = DateTime.MinValue;
                    if (DateTime.TryParse(rpcstruct["dateCreated"].ToString(), out publishDate))
                        entry.InnerItem.Publishing.PublishDate = publishDate;
                }

                entry.EndEdit();
            }
        }

        #region metaWeblog.getPost
        /// <summary>
        /// Return existing post
        /// </summary>
        /// <param name="postid">Post Identificator</param>
        /// <param name="username">UserName</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        [XmlRpcMethod("metaWeblog.getPost")]
        public XmlRpcStruct getPost(string postid, string username, string password)
        {
            Authenticate(username, password);

            CheckUserRights(postid, username);

            var rpcstruct = new XmlRpcStruct();
            var entryItem = ContentHelper.GetContentDatabase().GetItem(postid);
            if (entryItem != null)
            {
                var entry = new EntryItem(entryItem);

                rpcstruct.Add("title", entry.Title.Raw);
                rpcstruct.Add("link", entry.AbsoluteUrl);
                rpcstruct.Add("description", entry.Introduction.Raw);
                rpcstruct.Add("pubDate", entry.InnerItem.Statistics.Created.ToString());
                rpcstruct.Add("guid", entry.ID.ToString());
                rpcstruct.Add("author", entry.InnerItem.Statistics.CreatedBy);
            }

            return rpcstruct;
        }
        #endregion

        #region blogger.deletePost
        /// <summary>
        /// Delete existing post
        /// </summary>
        /// <param name="appKey">Application key</param>
        /// <param name="postid">Post Identificator</param>
        /// <param name="username">UserName</param>
        /// <param name="password">Password</param>
        /// <param name="publish"></param>
        /// <returns></returns>
        [XmlRpcMethod("blogger.deletePost")]
        public bool deletePost(string appKey, string postid, string userName, string password, bool publish)
        {
            Authenticate(userName, password);
            CheckUserRights(postid, userName);

            try
            {
                return ManagerFactory.EntryManagerInstance.DeleteEntry(postid, ContentHelper.GetContentDatabase());
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region metaWeblog.newMediaObject
        /// <summary>
        /// Create new media object associated with post
        /// </summary>
        /// <param name="blogid">Blog Identificator</param>
        /// <param name="username">UserName</param>
        /// <param name="password">Password</param>
        /// <param name="rpcstruct">XML-RPC struct representing media object</param>
        /// <returns>struct with url location of object</returns>
        [XmlRpcMethod("metaWeblog.newMediaObject")]
        public XmlRpcStruct newMediaObject(string blogid, string username, string password, XmlRpcStruct rpcstruct)
        {
            // Check user validation
            Authenticate(username, password);

            var name = rpcstruct["name"].ToString();
            var type = rpcstruct["type"].ToString();
            var media = (byte[])rpcstruct["bits"];
            var blogName = string.Empty;

            var currentBlog = ContentHelper.GetContentDatabase().GetItem(blogid);
            blogName = currentBlog.Name;

            // Get filename
            var fileName = Path.GetFileName(name);
            var imageName = ItemUtil.ProposeValidItemName(Path.GetFileNameWithoutExtension(fileName));

            // Create strem from byte array
            var memStream = new MemoryStream(media);
            var md = new MediaCreatorOptions();
            md.Destination = string.Join("/", new string[]{Constants.Paths.WeBlogMedia, blogName, imageName});
            md.Database = ContentHelper.GetContentDatabase();
            md.AlternateText = imageName;

            // Create mediaitem
            var mediaItem = MediaManager.Creator.CreateFromStream(memStream, fileName, md);

            // Publish mediaitem to web database
            ContentHelper.PublishItemAndRequiredAncestors(mediaItem.ID);

            // Close stream
            memStream.Close();
            memStream.Dispose();

            // Get the mediaitem url and return it
            var rstruct = new XmlRpcStruct();

            var options = new MediaUrlOptions()
            {
                AbsolutePath = false,
                UseItemPath = false
            };

            rstruct.Add("url", MediaManager.GetMediaUrl(mediaItem, options));
            return rstruct;

        }
        #endregion

        [XmlRpcMethod("pingback.ping")]
        public void pingback(string sourceUri, string targetUri)
        {
            try
            {
                string entryId = HttpContext.Current.Request.QueryString["entryId"].ToString();
                var currentID = ID.Parse(entryId);

                var comment = new Model.Comment()
                {
                    AuthorName = "Automatic pingback",
                    Text = string.Format("Pingkback from {0}", sourceUri)
                };
                    
                comment.Fields.Add(Constants.Fields.Website, sourceUri);

                var commentId = ManagerFactory.CommentManagerInstance.AddCommentToEntry(currentID, comment);

                ContentHelper.PublishItemAndRequiredAncestors(commentId);
            }
            catch (Exception)
            {
                throw new XmlRpcFaultException(1, "Invalid sourceUri parameter.");
            }

            // return "Your ping request has been received successfully.";
        }
    }
}