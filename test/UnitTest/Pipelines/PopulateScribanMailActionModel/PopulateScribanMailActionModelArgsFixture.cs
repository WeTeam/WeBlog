using NUnit.Framework;
using Sitecore.Collections;
using Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel;
using Sitecore.Workflows.Simple;
using System;
using System.Collections.Generic;

namespace Sitecore.Modules.WeBlog.UnitTest.Pipelines.PopulateScribanMailActionModel
{
    [TestFixture]
    public class PopulateScribanMailActionModelArgsFixture
    {
        [Test]
        public void Ctor_WorkflowPipelineArgsIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new PopulateScribanMailActionModelArgs(null);

            // act, assert
            var exception = Assert.Throws<ArgumentNullException>(new TestDelegate(sutAction));
            Assert.That(exception.ParamName, Is.EqualTo("workflowPipelineArgs"));
        }

        [Test]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();

            // act
            var sut = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);

            // assert
            Assert.That(sut.WorkflowPipelineArgs, Is.SameAs(workflowPipelineArgs));
        }

        [Test]
        public void AddModel_KeyIsNull_ThrowsException()
        {
            // arrange
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            var sut = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            Action sutAction = () => sut.AddModel(null, "value");

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(new TestDelegate(sutAction));
            Assert.That(ex.ParamName, Is.EqualTo("key"));
        }

        [TestCaseSource(nameof(InvalidKeysDataSource))]
        public void AddModel_KeyIsInvalid_ThrowsException(string key)
        {
            // arrange
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            var sut = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            Action sutAction = () => sut.AddModel(key, "value");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(new TestDelegate(sutAction));
            Assert.That(ex.ParamName, Is.EqualTo("key"));
        }

        [Test]
        public void AddModel_KeyIsAlreadyPresent_ReturnsLastObject()
        {
            // arrange
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            var sut = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            sut.AddModel("key", "value1");
            sut.AddModel("key", "value2");

            // act
            var result = sut.GetModel("key");

            // assert
            Assert.That(result, Is.EqualTo("value2"));
        }

        [TestCaseSource(nameof(InvalidKeysDataSource))]
        public void ModelContains_KeyIsInvalid_ReturnsFalse(string key)
        {
            // arrange
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            var sut = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);

            // act
            var result = sut.ModelContains(key);

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ModelContains_ObjectNotPresent_ReturnsFalse()
        {
            // arrange
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            var sut = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);

            // act
            var result = sut.ModelContains("other");

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ModelContains_ObjectIsPresent_ReturnsTrue()
        {
            // arrange
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            var sut = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            sut.AddModel("key", "value");

            // act
            var result = sut.ModelContains("key");

            // assert
            Assert.That(result, Is.True);
        }

        [TestCaseSource(nameof(InvalidKeysDataSource))]
        public void GetModelByKey_KeyIsInvalid_ReturnsNull(string key)
        {
            // arrange
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            var sut = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);

            // act
            var result = sut.GetModel(key);

            // assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetModelByKey_ObjectNotPresent_ReturnsNull()
        {
            // arrange
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            var sut = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);

            // act
            var result = sut.GetModel("key");

            // assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetModelByKey_StringObjectIsPresent_ReturnsObject()
        {
            // arrange
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            var sut = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            sut.AddModel("key", "value");

            // act
            var result = sut.GetModel("key");

            // assert
            Assert.That(result, Is.EqualTo("value"));
        }

        [Test]
        public void GetModelByKey_ObjectIsPresent_ReturnsObject()
        {
            // arrange
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            var sut = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            var value = new Tuple<string, int, DateTime>("item1", 42, DateTime.Today);
            sut.AddModel("key", value);

            // act
            var result = sut.GetModel("key");

            // assert
            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void GetModel_NothingSet_ReturnsEmptyDictionary()
        {
            // arrange
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            var sut = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);

            // act
            var result = sut.GetModel();

            // assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetModel_ObjectsSet_ReturnsDictionary()
        {
            // arrange
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            var sut = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            sut.AddModel("key1", "value1");
            sut.AddModel("key2", "value2");

            // act
            var result = sut.GetModel();

            // assert
            Assert.That(result, Is.EquivalentTo(
                new Dictionary<string, object>
                {
                    { "key1", "value1" },
                    { "key2", "value2" }
                }
            ));
        }

        [TestCaseSource(nameof(InvalidKeysDataSource))]
        public void RemoveModel_KeyIsInvalid_ReturnsFalse(string key)
        {
            // arrange
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            var sut = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            sut.AddModel("key", "value");

            // act
            var result = sut.RemoveModel(key);

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void RemoveModel_ObjectIsPresent_ReturnsTrue()
        {
            // arrange
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            var sut = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            sut.AddModel("key", "value");

            // act
            var result = sut.RemoveModel("key");

            // assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void RemoveModel_ObjectIsPresent_RemovesObject()
        {
            // arrange
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            var sut = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);
            sut.AddModel("key", "value");

            // act
            sut.RemoveModel("key");
            var exists = sut.ModelContains("key");

            // assert
            Assert.That(exists, Is.False);
        }

        [Test]
        public void RemoveModel_ObjectNotPresent_ReturnsFalse()
        {
            // arrange
            var workflowPipelineArgs = WorkflowPipelineArgsFactory.CreateWorkflowPipelineArgs();
            var sut = new PopulateScribanMailActionModelArgs(workflowPipelineArgs);

            // act
            var result = sut.RemoveModel("key");

            // assert
            Assert.That(result, Is.False);
        }

        public static IEnumerable<TestCaseData> InvalidKeysDataSource()
        {
            yield return new TestCaseData("");
            yield return new TestCaseData(" ");
            yield return new TestCaseData("\t\n");
        }
    }
}
