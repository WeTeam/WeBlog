using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NUnit.Framework;
using Sitecore.Data;

namespace Sitecore.Modules.WeBlog.Test
{
    public class UnitTestBase
    {
        [TestFixtureSetUp]
        public void SwitchToMaster()
        {
            Sitecore.Context.Database = Database.GetDatabase(Constants.UnitTestDatabase);
        }

    }
}