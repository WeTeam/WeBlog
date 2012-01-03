using System;
using System.Collections.Specialized;
using System.Data;
using System.Reflection;
using NUnit.Core;
using NUnit.Core.Filters;

namespace Codeflood.Testing
{
	public partial class TestRunner : System.Web.UI.Page, EventListener
	{
		#region Member Variables
		DataTable m_results = new DataTable();
		private int m_executedCount = 0;
		private int m_failedCount = 0;
		private TestSuite m_testSuite = null;
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			// Initialise data table to hold test results
			m_results.Columns.Add("test");
			m_results.Columns.Add("result");
            m_results.Columns.Add("time");
			m_results.Columns.Add("message");
			m_results.Columns.Add("class");

			// Initialise controls
			lblResult.Text = "";
			ltlStats.Text = "";

			// Initialise NUnit
			CoreExtensions.Host.InitializeService();

			// Find tests in current assembly
			TestPackage package = new TestPackage(Assembly.GetExecutingAssembly().Location);
			m_testSuite = new TestSuiteBuilder().Build(package);

			if (!IsPostBack)
			{
				// Display category filters
				StringCollection coll = new StringCollection();
				GetCategories((TestSuite)m_testSuite, coll);
				string[] cats = new string[coll.Count];
				coll.CopyTo(cats, 0);
				Array.Sort(cats);
				cblCategories.DataSource = cats;
				cblCategories.DataBind();
			}
		}

		protected void RunClick(object sender, EventArgs args)
		{
			// Determine if any category filters have been selected
			StringCollection categories = new StringCollection();
			for (int i = 0; i < cblCategories.Items.Count; i++)
			{
				if (cblCategories.Items[i].Selected)
					categories.Add(cblCategories.Items[i].Value);
			}

            if (categories.Count == 0)
            {
                for (int i = 0; i < cblCategories.Items.Count; i++)
                {
                    if (!cblCategories.Items[i].Text.ToLower().Contains("performance"))
                        categories.Add(cblCategories.Items[i].Value);
                }
            }

			string[] arCats = new string[categories.Count];
			categories.CopyTo(arCats, 0);

			// Create a category filter
			ITestFilter filter = new CategoryFilter(arCats);
			TestResult result = null;

            result = m_testSuite.Run(this, filter);

			// Bind results to presentation
			gvResults.DataSource = m_results;
			gvResults.DataBind();

			// Display statistics
			ltlStats.Text = string.Format("{0} out of {1} tests run in {2} seconds.", m_executedCount, result.Test.TestCount, result.Time);

			if (m_failedCount > 0)
				ltlStats.Text += string.Format("<br/>{0} {1} failed", m_failedCount, m_failedCount == 1 ? "test" : "tests");

			int skipped = result.Test.TestCount - m_executedCount;
			if (skipped > 0)
				ltlStats.Text += string.Format("<br/>{0} {1} skipped", skipped, skipped == 1 ? "test" : "tests");

			lblResult.Text = "Suite " + (result.IsSuccess ? "Passed" : "Failed");
			if (result.IsSuccess)
				lblResult.CssClass = "passLabel";
			else
				lblResult.CssClass = "failLabel";
		}

		/// <summary>
		/// Find all available categories in the test suite
		/// </summary>
		/// <param name="suite">The test suite to get categories from</param>
		/// <param name="cats">Output collection containing categories found</param>
		private void GetCategories(TestSuite suite, StringCollection cats)
		{
			if (suite.Categories != null)
			{
				for (int i = 0; i < suite.Categories.Count; i++)
					if (!cats.Contains((string)suite.Categories[i]))
						cats.Add((string)suite.Categories[i]);
			}

			for (int i = 0; i < suite.Tests.Count; i++)
				if(((ITest)suite.Tests[i]).IsSuite)
					GetCategories((TestSuite)suite.Tests[i], cats);
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
			DataRow dr = m_results.NewRow();
			dr["test"] = result.Test.TestName;
			dr["message"] = result.Message;
			if (result.IsFailure)
				dr["message"] += result.StackTrace;
			dr["class"] = "notRun";
            dr["time"] = result.Time;

			if (result.IsSuccess && result.Executed)
			{
				dr["result"] = "Pass";
				dr["class"] = "pass";
			}

			if (result.IsFailure && result.Executed)
			{
				dr["result"] = "Fail";
				dr["class"] = "fail";
				m_failedCount++;
			}

			if (result.Executed)
				m_executedCount++;

			m_results.Rows.Add(dr);
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
