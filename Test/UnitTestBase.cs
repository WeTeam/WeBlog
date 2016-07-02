using NUnit.Framework;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;

namespace Sitecore.Modules.WeBlog.Test
{
    public class UnitTestBase
    {
        protected Item TestContentRoot = null;

        [TestFixtureSetUp]
        public void SwitchToMaster()
        {
            Sitecore.Context.Database = Database.GetDatabase(Constants.UnitTestDatabase);

            var template = Sitecore.Context.Database.Templates[Constants.FolderTemplate];

            using (new SecurityDisabler())
            {
                TestContentRoot = Sitecore.Context.Database.GetItem(Sitecore.Constants.ContentPath).Add(ID.NewID.ToShortID().ToString(), template);
            }
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
          using (new SecurityDisabler())
          {
              if (TestContentRoot != null)
                  TestContentRoot.Delete();
          }
        }
    }
}