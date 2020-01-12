using Sitecore.Security.Accounts;

namespace Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel
{
    /// <summary>
    /// A processor for the weBlogPopulateScribanMailActionModel pipeline which adds the current user to the model.
    /// </summary>
    public class AddUser
    {
        /// <summary>
        /// The key used to identify the model.
        /// </summary>
        public const string ModelKey = "user";

        private User _user = null;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        public AddUser()
        {
        }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="user">The user to add to the model.</param>
        public AddUser(User user)
        {
            _user = user;
        }

        public void Process(PopulateScribanMailActionModelArgs args)
        {
            args.AddModel(ModelKey, _user ?? Context.User);
        }
    }
}