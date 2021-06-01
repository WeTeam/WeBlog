using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Diagnostics;
using Sitecore.StringExtensions;
using System;

namespace Sitecore.Modules.WeBlog.Managers
{
    public static class ManagerFactory
    {
        private static IBlogManager m_blogManager = null;
        private static ICategoryManager m_categoryManager = null;
        private static ICommentManager m_commentManager = null;
        private static IEntryManager m_entryManager = null;
        private static ITagManager m_tagManager = null;

        public static IBlogManager BlogManagerInstance
        {
            get
            {
                if (m_blogManager == null)
                {
                    m_blogManager = CreateInstance<IBlogManager>(WeBlogSettings.Instance.BlogManagerClass, () => {
                        return new BlogManager(null, null, WeBlogSettings.Instance);
                    });
                }

                return m_blogManager;
            }
        }

        public static ICategoryManager CategoryManagerInstance
        {
            get
            {
                if (m_categoryManager == null)
                    m_categoryManager = CreateInstance<ICategoryManager>(WeBlogSettings.Instance.CategoryManagerClass, () => { return new CategoryManager(null, null); });

                return m_categoryManager;
            }
        }

        public static ICommentManager CommentManagerInstance
        {
            get
            {
                if (m_commentManager == null)
                    m_commentManager = CreateInstance<ICommentManager>(WeBlogSettings.Instance.CommentManagerClass, () => { return new CommentManager(null, null, null, null); });

                return m_commentManager;
            }
        }

        public static IEntryManager EntryManagerInstance
        {
            get
            {
                if (m_entryManager == null)
                    m_entryManager = CreateInstance<IEntryManager>(WeBlogSettings.Instance.EntryManagerClass, () => { return new EntryManager(); });

                return m_entryManager;
            }
        }

        public static ITagManager TagManagerInstance
        {
            get
            {
                if (m_tagManager == null)
                    m_tagManager = CreateInstance<ITagManager>(WeBlogSettings.Instance.TagManagerClass, () => { return new TagManager(); });

                return m_tagManager;
            }
        }

        private static T CreateInstance<T>(string typeName, Func<T> fallbackCreation) where T : class
        {
            var type = Type.GetType(typeName, false);
            T instance = null;
            if (type != null)
            {
                try
                {
                    instance = (T)Sitecore.Reflection.ReflectionUtil.CreateObject(type);
                }
                catch(Exception ex)
                {
                    Logger.Error("Failed to create instance of type '{0}' as type '{1}'".FormatWith(type.FullName, typeof(T).FullName), ex, typeof(ManagerFactory));
                }
            }

            if(instance == null)
                instance = fallbackCreation();

            return instance;
        }
    }
}