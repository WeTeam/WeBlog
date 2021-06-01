using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using CookComputing.XmlRpc;
using Sitecore.Abstractions;
using Sitecore.DependencyInjection;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Extensions.StringExtensions;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Search;
using Sitecore.Resources.Media;
using Sitecore.Security.Authentication;
using Microsoft.Extensions.DependencyInjection;

#if FEATURE_URL_BUILDERS
using Sitecore.Links.UrlBuilders;
#else
using Sitecore.Links;
#endif

namespace Sitecore.Modules.WeBlog
{
    [XmlRpcService(
    Name = "Sitecore WeBlog Module",
    Description = "This is XML-RPC which implements the MetaWeblog API.",
    AutoDocumentation = true)]
    public class MetaBlogApi : XmlRpcService
    {
        /// <summary>
        /// Gets the <see cref="IBlogManager"/> instance used to access blog structures.
        /// </summary>
        protected IBlogManager BlogManager { get; set; }

        /// <summary>
        /// Gets the <see cref="ICategoryManager"/> instance used to access categories.
        /// </summary>
        protected ICategoryManager CategoryManager { get; set; }

        /// <summary>
        /// Gets the <see cref="IEntryManager"/> instance used to access blog entries.
        /// </summary>
        protected IEntryManager EntryManager { get; set; }

        /// <summary>
        /// The settings to use.
        /// </summary>
        protected IWeBlogSettings Settings { get; set; }

        /// <summary>
        /// Gets the <see cref="IBlogSettingsResolver"/> used to resolve the settings for a given blog item.
        /// </summary>
        protected IBlogSettingsResolver BlogSettingsResolver { get; }

        /// <summary>
        /// The <see cref="BaseMediaManager"/> to use for media operations.
        /// </summary>
        protected BaseMediaManager MediaManager { get; set; }

        /// <summary>
        /// The <see cref="BaseLinkManager"/> to use to generate item links.
        /// </summary>
        protected BaseLinkManager LinkManager { get; set; }

        public MetaBlogApi()
            : this(null, null, null, null, null, null, null)
        {
        }

        public MetaBlogApi(
            IBlogManager blogManager,
            ICategoryManager categoryManager,
            IEntryManager entryManager,
            IWeBlogSettings settings,
            BaseMediaManager mediaManager,
            BaseLinkManager linkManager,
            IBlogSettingsResolver blogSettingsResolver)
        {
            BlogManager = blogManager ?? ServiceLocator.ServiceProvider.GetRequiredService<IBlogManager>();
            CategoryManager = categoryManager ?? ServiceLocator.ServiceProvider.GetRequiredService<ICategoryManager>();
            EntryManager = entryManager ?? ServiceLocator.ServiceProvider.GetRequiredService<IEntryManager>();
            Settings = settings ?? ServiceLocator.ServiceProvider.GetRequiredService<IWeBlogSettings>();
            MediaManager = mediaManager ?? ServiceLocator.ServiceProvider.GetRequiredService<BaseMediaManager>();
            LinkManager = linkManager ?? ServiceLocator.ServiceProvider.GetRequiredService<BaseLinkManager>();
            BlogSettingsResolver = blogSettingsResolver ?? ServiceLocator.ServiceProvider.GetRequiredService<IBlogSettingsResolver>();
        }

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
            Authenticate(username, password);

            var blogList = BlogManager.GetUserBlogs(username);

            //Create structure for blog list
            var blogs = new List<XmlRpcStruct>();

#if FEATURE_URL_BUILDERS
            var urlOptions = new ItemUrlBuilderOptions();
#else
            var urlOptions = UrlOptions.DefaultOptions;
#endif

            urlOptions.AlwaysIncludeServerUrl = true;

            foreach (var blog in blogList)
            {
                var url = LinkManager.GetItemUrl(blog, urlOptions);

                var rpcstruct = new XmlRpcStruct
                {
                    {"blogid", blog.ID.ToString()}, // Blog Id
                    {"blogName", blog.Title.Raw}, // Blog Name
                    {"url", url}
                };

                blogs.Add(rpcstruct);
            }

            return blogs.ToArray();
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
            Authenticate(username, password);

            var blog = GetBlog(blogid);
            if (blog != null)
            {
                var categoryList = CategoryManager.GetCategories(blog);

                var categories = new List<XmlRpcStruct>();

                foreach (var category in categoryList)
                {
                    var rpcstruct = new XmlRpcStruct
                    {
                        {"categoryid", category.ID.ToString()}, // Category ID
                        {"title", category.Title.Raw}, // Category Title
                        {"description", "Description is not available"} // Category Description
                    };

                    categories.Add(rpcstruct);
                }

                return categories.ToArray();
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
            Authenticate(username, password);

            var blog = GetBlog(blogid);
            if (blog != null)
            {
                var criteria = new EntryCriteria
                {
                    PageNumber = 1,
                    PageSize = numberOfPosts
                };

                var entryList = EntryManager.GetBlogEntries(blog, criteria, ListOrder.Descending).Results.ToArray();

                var posts = new List<XmlRpcStruct>();

                //Populate structure with post entities
                foreach (var entry in entryList)
                {
                    var item = Database.GetItem(entry.Uri);
                    if(item == null)
                        continue;

                    var entryItem = new EntryItem(item);

                    var rpcstruct = new XmlRpcStruct
                    {
                        {"title", entryItem.Title.Raw},
                        {"link", GetItemAbsoluteUrl(entryItem)},
                        {"description", entryItem.Content.Text},
                        {"pubDate", entryItem.EntryDate.DateTime},
                        {"guid", entryItem.ID.ToString()},
                        {"postid", entryItem.ID.ToString()},
                        {"keywords", entryItem.Tags.Raw},
                        {"author", entryItem.InnerItem.Statistics.CreatedBy}
                    };

                    posts.Add(rpcstruct);
                }

                return posts.ToArray();
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

            CheckUserRights(blogid);

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
            CheckUserRights(blogid);

            var entryTitleRaw = rpcstruct["title"];
            if (entryTitleRaw == null)
                throw new ArgumentException("'title' must be provided");

            var entryTitle = entryTitleRaw.ToString();
            var currentBlog = GetContentDatabase().GetItem(blogid);

            if (currentBlog != null)
            {
                var blogItem = new BlogHomeItem(currentBlog);
                var settings = BlogSettingsResolver.Resolve(blogItem);
                var template = new TemplateID(settings.EntryTemplateID);
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
            CheckUserRights(postid);

            var item = GetContentDatabase().GetItem(new ID(postid));

            if (item != null)
            {
                SetItemData(item, rpcstruct);

                if (publish)
                    ContentHelper.PublishItemAndRequiredAncestors(item.ID);

                return true;
            }
            
            return false;
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
                    entry.Category.Field.Value = GetCategoriesAsString(item, rpcstruct);

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
            CheckUserRights(postid);

            var rpcstruct = new XmlRpcStruct();
            var entryItem = GetContentDatabase().GetItem(postid);
            if (entryItem != null)
            {
                var entry = new EntryItem(entryItem);

                rpcstruct.Add("title", entry.Title.Raw);
                rpcstruct.Add("link", GetItemAbsoluteUrl(entry));
                rpcstruct.Add("description", entry.Content.Raw);
                rpcstruct.Add("pubDate", entry.EntryDate.DateTime);
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
            CheckUserRights(postid);

            try
            {
                return EntryManager.DeleteEntry(postid, GetContentDatabase());
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
            var media = (byte[])rpcstruct["bits"];
            var blogName = string.Empty;

            var currentBlog = GetContentDatabase().GetItem(blogid);
            blogName = currentBlog.Name;

            // Get filename
            var fileName = Path.GetFileName(name);
            var imageName = ItemUtil.ProposeValidItemName(Path.GetFileNameWithoutExtension(fileName));

            // Create strem from byte array
            var memStream = new MemoryStream(media);
            var md = new MediaCreatorOptions();
            md.Destination = string.Join("/", new string[]{Constants.Paths.WeBlogMedia, blogName, imageName});
            md.Database = GetContentDatabase();
            md.AlternateText = imageName;

            // Check access rights
            CheckUserCreateRights(md.Destination);

            // Create mediaitem
            var mediaItem = MediaManager.Creator.CreateFromStream(memStream, fileName, md);

            // Close stream
            memStream.Close();
            memStream.Dispose();

            // Publish mediaitem to web database
            ContentHelper.PublishItemAndRequiredAncestors(mediaItem.ID);

            // Get the mediaitem url and return it
            var rstruct = new XmlRpcStruct();

#if FEATURE_URL_BUILDERS
            var options = new MediaUrlBuilderOptions()
#else
            var options = new MediaUrlOptions()
#endif
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

        /// <summary>
        /// Gets the database where content is authored
        /// </summary>
        /// <returns>The contenet database</returns>
        protected virtual Database GetContentDatabase()
        {
            return ContentHelper.GetContentDatabase();
        }

        /// <summary>
        /// Gets the blog given by the provided ID
        /// </summary>
        /// <param name="blogId">The ID of the blog to get</param>
        /// <returns>The blog item</returns>
        protected virtual BlogHomeItem GetBlog(string blogId)
        {
            var db = GetContentDatabase();
            return db.GetItem(blogId);
        }

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
        protected virtual void CheckUserRights(string blogid)
        {
            var blog = ItemManager.GetItem(blogid, Sitecore.Context.Language, Sitecore.Data.Version.Latest, GetContentDatabase());
            if (blog != null && !blog.Security.CanWrite(Sitecore.Context.User))
                throw new System.Security.Authentication.InvalidCredentialException("You do not have sufficient user rights");
        }

        /// <summary>
        /// Ensure the context user has create rights to the item given by path.
        /// </summary>
        /// <param name="path">The path of the item to check the rights on.</param>
        /// <remarks>If the item in the path doesn't exist, the ancestors will be checked until an item is found to exist.</remarks>
        protected virtual void CheckUserCreateRights(string path)
        {
            var item = ItemManager.GetItem(path, Sitecore.Context.Language, Sitecore.Data.Version.Latest, GetContentDatabase());

            if (item == null)
            {
                // Item didn't exist, try parent
                var lastPathSeparatorIndex = path.LastIndexOf("/");
                if (lastPathSeparatorIndex >= 0)
                {
                    var parentPath = path.Left(lastPathSeparatorIndex);
                    CheckUserCreateRights(parentPath);
                }
            }

            if (item != null && !item.Security.CanCreate(Sitecore.Context.User))
                throw new System.Security.Authentication.InvalidCredentialException("You do not have sufficient user rights");
        }

        /// <summary>
        /// Gets the categories as string.
        /// </summary>
        /// <param name="postid">The postid.</param>
        /// <param name="rpcstruct">The rpcstruct.</param>
        /// <returns></returns>
        protected virtual string GetCategoriesAsString(Item postItem, XmlRpcStruct rpcstruct)
        {
            var blog = BlogManager.GetCurrentBlog(postItem)?.InnerItem;
            var categoryList = CategoryManager.GetCategories(blog);

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
        /// Gets the absolute URL of the item including the server.
        /// </summary>
        /// <param name="item">The item to get the URL for.</param>
        /// <returns>The item URL.</returns>
        protected string GetItemAbsoluteUrl(Item item)
        {
#if FEATURE_URL_BUILDERS
            var urlOptions = new ItemUrlBuilderOptions();
#else
            var urlOptions = UrlOptions.DefaultOptions;
#endif

            urlOptions.AlwaysIncludeServerUrl = true;

            return LinkManager.GetItemUrl(item, urlOptions);
        }
    }
}