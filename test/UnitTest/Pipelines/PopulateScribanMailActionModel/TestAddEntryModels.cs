using Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel;
using Sitecore.Security.Accounts;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.PopulateScribanMailActionModel
{
    class TestAddEntryModels : AddEntryModels
    {
        private User _user = null;

        public TestAddEntryModels(User user)
        {
            _user = user;
        }

        protected override User GetUser(string userName)
        {
            return _user;
        }
    }
}
