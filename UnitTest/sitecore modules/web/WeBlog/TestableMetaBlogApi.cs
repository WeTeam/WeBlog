using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Modules.WeBlog.UnitTest.sitecore_modules.web.WeBlog
{
    public class TestableMetaBlogApi : MetaBlogApi
    {
        private readonly Action<string, string> _authenticateFunction = null;

        public TestableMetaBlogApi(Action<string, string> authenticateFunction)
        {
            _authenticateFunction = authenticateFunction;
        }

        protected override void Authenticate(string username, string password)
        {
            _authenticateFunction(username, password);
        }
    }
}
