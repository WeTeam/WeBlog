using NUnit.Core;
using NUnit.Core.Filters;
using NUnit.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;

namespace Codeflood.Testing
{
    public partial class TestRunner : System.Web.UI.Page, EventListener
    {
        #region Member Variables
        DataTable _results = new DataTable();
        private int _executedCount = 0;
        private int _failedCount = 0;
        private TestPackage _testPackage = null;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            // Initialise data table to hold test results
            _results.Columns.Add("test");
            _results.Columns.Add("result");
            _results.Columns.Add("time");
            _results.Columns.Add("message");
            _results.Columns.Add("class");

            // Initialise controls
            lblResult.Text = "";
            ltlStats.Text = "";

            // Initialise NUnit
            CoreExtensions.Host.InitializeService();

            // Find tests in current assembly
            _testPackage = new TestPackage(Assembly.GetExecutingAssembly().Location);

            if (!IsPostBack)
            {
                var testSuite = new TestSuiteBuilder().Build(_testPackage);

                LoadCategories(testSuite);
                LoadTestMethodNames(testSuite);
            }
        }

        protected void LoadTestMethodNames(ITest test)
        {
            var names = new List<string>();
            FindTestMethodNames(test, names);

            cblMethods.DataSource = names;
            cblMethods.DataBind();
        }

        protected void LoadCategories(ITest test)
        {
            var categoryManager = new CategoryManager();
            categoryManager.AddAllCategories(test);

            cblCategories.DataSource = (from string cat in categoryManager.Categories select cat).OrderBy(x => x);
            cblCategories.DataBind();
        }

        protected void RunClick(object sender, EventArgs args)
        {
            var filter = ConstructFilter();

            var runner = new SimpleTestRunner();
            runner.Load(_testPackage);

            var result = runner.Run(this, filter, true, LoggingThreshold.All);

            // Bind results to presentation
            gvResults.DataSource = _results;
            gvResults.DataBind();

            // Display statistics
            ltlStats.Text = string.Format("{0} out of {1} tests run in {2} seconds.", _executedCount, result.Test.TestCount, result.Time);

            if (_failedCount > 0)
                ltlStats.Text += string.Format("<br/>{0} {1} failed", _failedCount, _failedCount == 1 ? "test" : "tests");

            var skipped = result.Test.TestCount - _executedCount;
            if (skipped > 0)
                ltlStats.Text += string.Format("<br/>{0} {1} skipped", skipped, skipped == 1 ? "test" : "tests");

            lblResult.Text = "Suite " + (result.IsSuccess ? "Passed" : "Failed");
            if (result.IsSuccess)
                lblResult.CssClass = "passLabel";
            else
                lblResult.CssClass = "failLabel";
        }

        protected ITestFilter ConstructFilter()
        {
            var categories = (from ListItem item in cblCategories.Items
                where item.Selected
                select item.Value).ToArray();

            var methodNames = (from ListItem item in cblMethods.Items
                where item.Selected
                select item.Value).ToArray();

            if (!categories.Any() && !methodNames.Any())
                return TestFilter.Empty;

            var categoryFilter = new CategoryFilter(categories);
            var methodFilter = new SimpleNameFilter(methodNames);

            return new OrFilter(categoryFilter, methodFilter);
        }

        protected void FindTestMethodNames(ITest test, List<string> list)
        {
            foreach (ITest t in test.Tests)
            {
                if (t is NUnitTestMethod)
                {
                    list.Add(t.TestName.FullName);
                }

                if (t.Tests != null)
                {
                    FindTestMethodNames(t, list);
                }
            }
        }

        #region EventListener Members

        public void RunFinished(Exception exception)
        {
        }

        public void RunFinished(TestResult result)
        {
        }

        public void RunStarted(string name, int testCount)
        {
        }

        public void SuiteFinished(TestResult result)
        {
        }

        public void SuiteStarted(TestName testName)
        {
        }

        public void TestFinished(TestResult result)
        {
            // Put results into data table
            var dr = _results.NewRow();
            dr["test"] = result.Test.TestName;
            dr["class"] = "notRun";
            dr["time"] = result.Time;

            var message = result.Message;
            if (result.IsFailure)
                message += "\r\n" + result.StackTrace;

            if (!string.IsNullOrEmpty(message))
                message = message.Replace("\r\n", "<br/>");

            dr["message"] = message;

            if (result.IsSuccess && result.Executed)
            {
                dr["result"] = "Pass";
                dr["class"] = "pass";
            }

            if (result.IsFailure && result.Executed)
            {
                dr["result"] = "Fail";
                dr["class"] = "fail";
                _failedCount++;
            }

            if (result.Executed)
                _executedCount++;

            _results.Rows.Add(dr);
        }

        public void TestOutput(TestOutput testOutput)
        {
        }

        public void TestStarted(TestName testName)
        {
        }

        public void UnhandledException(Exception exception)
        {
        }

        #endregion
    }
}
