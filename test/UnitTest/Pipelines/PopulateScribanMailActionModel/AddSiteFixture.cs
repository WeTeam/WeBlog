using NUnit.Framework;
using Sitecore.Collections;
using Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel;
using Sitecore.Sites;
using Sitecore.Web;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.PopulateScribanMailActionModel
{
    [TestFixture]
    public class AddSiteFixture
    {
        [Test]
        public void Process_WhenCalled_AddsSite()
        {
            // arrange
            var siteInfo = new SiteInfo(new StringDictionary());
            var site = new SiteContext(siteInfo);

            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            var args = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            var sut = new AddSite(site);

            // act
            sut.Process(args);

            // assert
            var value = args.GetModel(AddSite.ModelKey);
            Assert.That(value, Is.EqualTo(site));
        }
    }
}
