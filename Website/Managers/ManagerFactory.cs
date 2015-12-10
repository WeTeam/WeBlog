using System;
using Sitecore.Modules.WeBlog.Diagnostics;
using Sitecore.StringExtensions;

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
                    m_blogManager = CreateInstance<IBlogManager>(Settings.BlogManagerClass, () => { return new BlogManager(); });

                return m_blogManager;
            }
        }

        public static ICategoryManager CategoryManagerInstance
        {
            get
            {
                if (m_categoryManager == null)
                    m_categoryManager = CreateInstance<ICategoryManager>(Settings.CategoryManagerClass, () => { return new CategoryManager(); });

                return m_categoryManager;
            }
        }

        public static ICommentManager CommentManagerInstance
        {
            get
            {
                if (m_commentManager == null)
                    m_commentManager = CreateInstance<ICommentManager>(Settings.CommentManagerClass, () => { return new CommentManager(); });

                return m_commentManager;
            }
        }

        public static IEntryManager EntryManagerInstance
        {
            get
            {
                if (m_entryManager == null)
                    m_entryManager = CreateInstance<IEntryManager>(Settings.EntryManagerClass, () => { return new EntryManager(); });

                return m_entryManager;
            }
        }

        public static ITagManager TagManagerInstance
        {
            get
            {
                if (m_tagManager == null)
                    m_tagManager = CreateInstance<ITagManager>(Settings.TagManagerClass, () => { return new TagManager(); });

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