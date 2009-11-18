using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using CookComputing.XmlRpc;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Modules.Eviblog.Items;
using Sitecore.Modules.Eviblog.Managers;
using Sitecore.Modules.Eviblog.Utilities;
using Sitecore.Resources.Media;
using Sitecore.Security.Accounts;
using Sitecore.Security.Authentication;
using Sitecore.SecurityModel;

namespace Sitecore.Modules.Eviblog.XmlRpc
{
    [XmlRpcService(
    Name = "Sitecore EviBlog Module",
    Description = "This is XML-RPC which implements the MetaWeblog API.",
    AutoDocumentation = true)]
    public class Blogger : XmlRpcService
    {
        #region Helpers
        /// <summary>
        /// Authenticate user
        /// </summary>
        /// <param name="username">UserName</param>
        /// <param name="password">Password</param>
        private static void Authenticate(string username, string password)
        {
            //Check user credentials using form authentication
            bool allowed = new AuthenticationHelper(AuthenticationManager.Provider).ValidateUser(username, password);

            if (allowed == false)
            {
                throw new System.Security.Authentication.InvalidCredentialException("Invalid credentials. Access denied");
            }
        }

        private static void CheckUserRights(string blogid, string username)
        {
            Item blog = ItemManager.GetItem(blogid, Sitecore.Context.Language, Sitecore.Data.Version.Latest, Sitecore.Context.Database);

            Account account = Account.FromName(username, AccountType.User);

            if (blog != null && !blog.Security.CanWrite(account))
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
            Database master = Factory.GetDatabase("master");
            Item blog = BlogManager.GetCurrentBlogItem(new ID(postid), "master");
            List<Category> categoryList = CategoryManager.GetCategories(blog.ID.ToString());

            if (categoryList.Count() != 0)
            {
                string selectedCategories = string.Empty;

                try
                {
                    string[] categories = (string[])rpcstruct["categories"];

                    foreach (string category in categories)
                    {
                        foreach (Category cat in categoryList)
                        {
                            if (category == cat.Title)
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
            Database master = Factory.GetDatabase("master");
            Item blog = BlogManager.GetBlogByID(BlogID);
            List<Category> categoryList = CategoryManager.GetCategories(blog.ID.ToString());

            if (categoryList.Count() != 0)
            {
                string selectedCategories = string.Empty;

                if (rpcstruct["categories"] != null)
                {
                    string[] categories = (string[])rpcstruct["categories"];

                    foreach (string category in categories)
                    {
                        foreach (Category cat in categoryList)
                        {
                            if (category == cat.Title)
                            {
                                selectedCategories += cat.ID.ToString();
                            }
                        }
                    }

                    string result = selectedCategories.Replace("}{", "}|{");

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

            List<Blog> BlogList = BlogManager.GetUserBlogs(username);

            //Create structure for blog list
            XmlRpcStruct[] blogs = new XmlRpcStruct[BlogList.Count];
            foreach (Blog blog in BlogList)
            {
                XmlRpcStruct rpcstruct = new XmlRpcStruct();
                rpcstruct.Add("blogid", blog.ID.ToString()); // Blog Id
                rpcstruct.Add("blogName", blog.Name); // Blog Name
                rpcstruct.Add("url", blog.Url);
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
            
            List<Category> categoryList = CategoryManager.GetCategories(blogid);

            XmlRpcStruct[] categories = new XmlRpcStruct[categoryList.Count];
            
            foreach (Category category in categoryList)
            {
                XmlRpcStruct rpcstruct = new XmlRpcStruct();
                rpcstruct.Add("categoryid", category.ID.ToString()); // Category ID
                rpcstruct.Add("title", category.Title); // Category Title
                rpcstruct.Add("description", "Description is not available"); // Category Description
                categories[ii] = rpcstruct;
                ii++;
            }
            return categories;

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
            
            List<Entry> entryList = EntryManager.GetBlogEntries(new ID(blogid), numberOfPosts);

            XmlRpcStruct[] posts = new XmlRpcStruct[entryList.Count];

            //Populate structure with post entities
            foreach(Entry entry in entryList)
            {
                XmlRpcStruct rpcstruct = new XmlRpcStruct();
                rpcstruct.Add("title", entry.Title);
                rpcstruct.Add("link", entry.Url);
                rpcstruct.Add("description", entry.Text);
                rpcstruct.Add("pubDate", entry.InnerItem.Statistics.Created.ToString());
                rpcstruct.Add("guid", entry.ID.ToString());
                rpcstruct.Add("postid", entry.ID.ToString());
                rpcstruct.Add("keywords", entry.Tags);
                rpcstruct.Add("author", entry.InnerItem.Statistics.CreatedBy);
                posts[ii] = rpcstruct;
                ii++;
            }
            return posts;
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

            SecurityDisabler securitydisabler = new SecurityDisabler();

            string EntryTitle = rpcstruct["title"].ToString();
            string EntryDescription = rpcstruct["description"].ToString();

            Item currentBlog = BlogManager.GetCurrentBlogItem(new ID(blogid), "master");

            TemplateID template = new TemplateID(new ID(Settings.Default.EntryTemplateID));
            Item newItem = ItemManager.AddFromTemplate(EntryTitle, template, currentBlog);

            Entry createdEntry = new Entry(newItem);
            createdEntry.BeginEdit();
            createdEntry.Title = EntryTitle;
            createdEntry.Introduction = EntryDescription;
            createdEntry.Categories = GetCategoriesAsString(currentBlog.ID, rpcstruct);
            createdEntry.EndEdit();

            if(publish == true)
            {
                Publish.PublishItem(createdEntry.ID);
            }

            string id = createdEntry.ID.ToString();

            SecurityEnabler securityenabler = new SecurityEnabler();

            return id;
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

            SecurityDisabler securitydisabler = new SecurityDisabler();

            Database master = Factory.GetDatabase("master");
            Item entry = master.GetItem(new ID(postid));

            entry.Editing.BeginEdit();
            entry.Fields["Title"].Value = rpcstruct["title"].ToString();
            entry.Fields["Introduction"].Value = rpcstruct["description"].ToString();
            entry.Fields["Category"].Value = GetCategoriesAsString(postid, rpcstruct);
            entry.Editing.EndEdit();

            Publish.PublishItem(entry.ID);

            SecurityEnabler securityenabler = new SecurityEnabler();

            return true;
        }
        #endregion
        
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

            Entry entry = EntryManager.GetBlogEntry(new ID(postid));

            XmlRpcStruct rpcstruct = new XmlRpcStruct();
            rpcstruct.Add("title", entry.Title);
            rpcstruct.Add("link", entry.Url);
            rpcstruct.Add("description", entry.Introduction);
            rpcstruct.Add("pubDate", entry.InnerItem.Statistics.Created.ToString());
            rpcstruct.Add("guid", entry.ID.ToString());
            rpcstruct.Add("author", entry.InnerItem.Statistics.CreatedBy);

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
        public bool deletePost(string appKey, string postid, string username, string password, bool publish)
        {
            Authenticate(username, password);

            CheckUserRights(postid, username);

            try
            {
                EntryManager.DeletePost(postid);
            }
            catch
            {
                return false;
            }
            return true;
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
            
            string name = rpcstruct["name"].ToString();
            string type = rpcstruct["type"].ToString();
            byte[] media = (byte[])rpcstruct["bits"];
            string blogName = string.Empty;

            Item currentBlog = BlogManager.GetBlogByID(new ID(blogid));
            blogName = currentBlog.Name;
            // Get filename
            string file = name.Substring(49 + 1);
            // Split filename and extension
            string[] imageName = name.Substring(49 + 1).Split('.'); ;

            // Replace invalid characters which Live Writer puts around image names
            string fileName = file.Replace("[", "");
            fileName = fileName.Replace("]", "");

            // Create strem from byte array
            MemoryStream memStream = new MemoryStream(media);
            MediaCreatorOptions md = new MediaCreatorOptions();
            md.Destination = @"/sitecore/media library/Modules/EviBlog/" + blogName + "/" + imageName[0].ToString();
            md.Database = Sitecore.Configuration.Factory.GetDatabase("master");
            md.AlternateText = fileName;

            // Create mediaitem
            MediaItem mediaItem;
            SecurityDisabler securitydisabler = new SecurityDisabler();
            mediaItem = MediaManager.Creator.CreateFromStream(memStream, fileName, md);
            SecurityEnabler securityenabler = new SecurityEnabler();

            // Publish mediaitem to web database
            Publish.PublishItem(mediaItem);

            // Close stream
            memStream.Close();
            memStream.Dispose();

            // Get the mediaitem url and return it
            XmlRpcStruct rstruct = new XmlRpcStruct();
            rstruct.Add("url", MediaManager.GetMediaUrl(mediaItem));
            return rstruct;

        }
        #endregion

        [XmlRpcMethod("pingback.ping")]
        public void pingback(string sourceUri, string targetUri)
        {
            try
            {
                using (new SecurityDisabler())
                {
                    string entryId = HttpContext.Current.Request.QueryString["entryId"].ToString();

                    Database db = Factory.GetDatabase("master");
                    ID currentID = ID.Parse(entryId);
                    TemplateID templateID = new TemplateID(new ID(Settings.Default.CommentTemplateID));
                    Item currentItem = db.GetItem(currentID);

                    Item newPingbackItem = currentItem.Add("Pingback", templateID);
                    
                    Comment newPingbackComment = new Comment(newPingbackItem);
                    newPingbackComment.BeginEdit();
                    newPingbackComment.CommentText = string.Format("Pingkback from {0}", sourceUri);
                    newPingbackComment.Website = sourceUri;
                    newPingbackComment.UserName = "Automatic pingback";
                    newPingbackComment.EndEdit();

                    Publish.PublishItem(newPingbackItem);
                }
            }
            catch(Exception ex)
            {
                throw new XmlRpcFaultException(1, "Invalid sourceUri parameter.");
            }

           // return "Your ping request has been received successfully.";
        }
    }
}