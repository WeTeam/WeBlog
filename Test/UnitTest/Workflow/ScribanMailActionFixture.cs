using Moq;
using NUnit.Framework;
using Sitecore.Abstractions;
using Sitecore.Analytics.Core;
using Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel;
using Sitecore.Modules.WeBlog.Workflow;
using Sitecore.Pipelines;
using Sitecore.Workflows.Simple;
using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Sitecore.Modules.WeBlog.UnitTest.Workflow
{
    [TestFixture]
    public class ScribanMailActionFixture
    {
        [Test]
        public void Ctor_BaseCorePipelineManagerIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new ScribanMailAction(null, Mock.Of<Func<ISmtpClient>>(), Mock.Of<BaseLog>());

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(new TestDelegate(sutAction));
            Assert.That(ex.ParamName, Is.EqualTo("pipelineManager"));
        }

        [Test]
        public void Ctor_SmtpClientFactoryIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new ScribanMailAction(Mock.Of<BaseCorePipelineManager>(), null, Mock.Of<BaseLog>());

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(new TestDelegate(sutAction));
            Assert.That(ex.ParamName, Is.EqualTo("smtpClientFactory"));
        }

        [Test]
        public void Ctor_LogIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new ScribanMailAction(Mock.Of<BaseCorePipelineManager>(), Mock.Of<Func<ISmtpClient>>(), null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(new TestDelegate(sutAction));
            Assert.That(ex.ParamName, Is.EqualTo("log"));
        }

        [Test]
        public void Process_ToFieldEmpty_LogsError()
        {
            // arrange
            var sutDependencies = CreateSutDependencies();
            var sut = CreateScribanMailAction(sutDependencies);
            var logMock = sutDependencies.LogMock;
            var args = CreateWorkflowPipelineArgs(to: "");

            // act
            sut.Process(args);

            // assert
            logMock.Verify(x => x.Error("Sitecore.Modules.WeBlog.Workflow.ScribanMailAction cannot be invoked with an empty 'To' field.", sut));
        }

        [Test]
        public void Process_FromFieldEmpty_LogsError()
        {
            // arrange
            var sutDependencies = CreateSutDependencies();
            var sut = CreateScribanMailAction(sutDependencies);
            var logMock = sutDependencies.LogMock;
            var args = CreateWorkflowPipelineArgs(from: "");

            // act
            sut.Process(args);

            // assert
            logMock.Verify(x => x.Error("Sitecore.Modules.WeBlog.Workflow.ScribanMailAction cannot be invoked with an empty 'From' field.", sut));
        }

        [Test]
        public void Process_LiteralToField_SendsMailWithFieldValue()
        {
            // arrange
            var sutDependencies = CreateSutDependencies();
            var sut = CreateScribanMailAction(sutDependencies);
            var smtpClientMock = sutDependencies.SmtpClientMock;
            var args = CreateWorkflowPipelineArgs(to: "someone@mail.com");

            // act
            sut.Process(args);

            // assert
            smtpClientMock.Verify(x => x.Send(It.Is<MailMessage>(y => y.To[0].Address == "someone@mail.com")));
        }

        [Test]
        public void Process_TokenInToField_SendsMailWithFieldValue()
        {
            // arrange
            var sutDependencies = CreateSutDependencies(new Dictionary<string, object>
            {
                {"token", "user@mail.com" }
            });

            var sut = CreateScribanMailAction(sutDependencies);
            var smtpClientMock = sutDependencies.SmtpClientMock;
            var args = CreateWorkflowPipelineArgs(to: "{{token}}");

            // act
            sut.Process(args);

            // assert
            smtpClientMock.Verify(x => x.Send(It.Is<MailMessage>(y => y.To[0].Address == "user@mail.com")));
        }

        [Test]
        public void Process_LiteralFromField_SendsMailWithFieldValue()
        {
            // arrange
            var sutDependencies = CreateSutDependencies();
            var sut = CreateScribanMailAction(sutDependencies);
            var smtpClientMock = sutDependencies.SmtpClientMock;
            var args = CreateWorkflowPipelineArgs(from: "someone@mail.com");

            // act
            sut.Process(args);

            // assert
            smtpClientMock.Verify(x => x.Send(It.Is<MailMessage>(y => y.From.Address == "someone@mail.com")));
        }

        [Test]
        public void Process_TokenInFromField_SendsMailWithFieldValue()
        {
            // arrange
            var sutDependencies = CreateSutDependencies(new Dictionary<string, object>
            {
                {"token", "user@mail.com" }
            });

            var sut = CreateScribanMailAction(sutDependencies);
            var smtpClientMock = sutDependencies.SmtpClientMock;
            var args = CreateWorkflowPipelineArgs(from: "{{token}}");

            // act
            sut.Process(args);

            // assert
            smtpClientMock.Verify(x => x.Send(It.Is<MailMessage>(y => y.From.Address == "user@mail.com")));
        }

        [Test]
        public void Process_SubjectFieldEmpty_SendsMailWithEmptyFieldValue()
        {
            // arrange
            var sutDependencies = CreateSutDependencies();
            var sut = CreateScribanMailAction(sutDependencies);
            var smtpClientMock = sutDependencies.SmtpClientMock;
            var args = CreateWorkflowPipelineArgs(subject: "");

            // act
            sut.Process(args);

            // assert
            smtpClientMock.Verify(x => x.Send(It.Is<MailMessage>(y => y.Subject == "")));
        }

        [Test]
        public void Process_LiteralSubjectField_SendsMailWithFieldValue()
        {
            // arrange
            var sutDependencies = CreateSutDependencies();
            var sut = CreateScribanMailAction(sutDependencies);
            var smtpClientMock = sutDependencies.SmtpClientMock;
            var args = CreateWorkflowPipelineArgs(subject: "New Comment Posted");

            // act
            sut.Process(args);

            // assert
            smtpClientMock.Verify(x => x.Send(It.Is<MailMessage>(y => y.Subject == "New Comment Posted")));
        }

        [Test]
        public void Process_TokenInSubjectField_SendsMailWithFieldValue()
        {
            // arrange
            var sutDependencies = CreateSutDependencies(new Dictionary<string, object>
            {
                {"token", "Some User" }
            });

            var sut = CreateScribanMailAction(sutDependencies);
            var smtpClientMock = sutDependencies.SmtpClientMock;
            var args = CreateWorkflowPipelineArgs(subject: "Comment from {{token}}");

            // act
            sut.Process(args);

            // assert
            smtpClientMock.Verify(x => x.Send(It.Is<MailMessage>(y => y.Subject == "Comment from Some User")));
        }

        [Test]
        public void Process_MessageFieldEmpty_SendsMailWithEmptyFieldValue()
        {
            // arrange
            var sutDependencies = CreateSutDependencies();
            var sut = CreateScribanMailAction(sutDependencies);
            var smtpClientMock = sutDependencies.SmtpClientMock;
            var args = CreateWorkflowPipelineArgs(message: "");

            // act
            sut.Process(args);

            // assert
            smtpClientMock.Verify(x => x.Send(It.Is<MailMessage>(y => y.Body == "")));
        }

        [Test]
        public void Process_LiteralMessageField_SendsMailWithFieldValue()
        {
            // arrange
            var sutDependencies = CreateSutDependencies();
            var sut = CreateScribanMailAction(sutDependencies);
            var smtpClientMock = sutDependencies.SmtpClientMock;
            var args = CreateWorkflowPipelineArgs(message: "Something happened");

            // act
            sut.Process(args);

            // assert
            smtpClientMock.Verify(x => x.Send(It.Is<MailMessage>(y => y.Body == "Something happened")));
        }

        [Test]
        public void Process_TokenInMessageField_SendsMailWithFieldValue()
        {
            // arrange
            var sutDependencies = CreateSutDependencies(new Dictionary<string, object>
            {
                {"token", "A comment" }
            });

            var sut = CreateScribanMailAction(sutDependencies);
            var smtpClientMock = sutDependencies.SmtpClientMock;
            var args = CreateWorkflowPipelineArgs(message: "comment: {{token}}");

            // act
            sut.Process(args);

            // assert
            smtpClientMock.Verify(x => x.Send(It.Is<MailMessage>(y => y.Body == "comment: A comment")));
        }

        [Test]
        public void Process_InvalidScribanInField_LogsErrors()
        {
            // arrange
            var sutDependencies = CreateSutDependencies(new Dictionary<string, object>
            {
                {"token", "value" }
            });

            var sut = CreateScribanMailAction(sutDependencies);
            var logMock = sutDependencies.LogMock;
            var args = CreateWorkflowPipelineArgs(message: "{{token}");

            // act
            sut.Process(args);

            // assert
            logMock.Verify(x => x.Error("An error occurred whilst parsing a Scriban template in Sitecore.Modules.WeBlog.Workflow.ScribanMailAction", sut));
        }

        [Test]
        public void Process_AccessInvalidTokenProperty_LogsErrors()
        {
            // arrange
            var sutDependencies = CreateSutDependencies();

            var sut = CreateScribanMailAction(sutDependencies);
            var logMock = sutDependencies.LogMock;
            var args = CreateWorkflowPipelineArgs(message: "{{token.prop}}");

            // act
            sut.Process(args);

            // assert
            logMock.Verify(x => x.Error("An error occurred whilst rendering a Scriban template in Sitecore.Modules.WeBlog.Workflow.ScribanMailAction", It.IsAny<Exception>(), sut));
        }

        private SutDependencies CreateSutDependencies(Dictionary<string, object> model = null)
        {
            var pipelineManagerMock = new Mock<BaseCorePipelineManager>();

            if(model != null)
                pipelineManagerMock
                    .Setup(x => x.Run("weblogPopulateScribanMailActionModel", It.IsAny<PopulateScribanMailActionModelArgs>()))
                    .Callback(new Action<string, PipelineArgs>((y, x) =>
                    {
                        var sargs = x as PopulateScribanMailActionModelArgs;
                        foreach (var entry in model)
                            sargs.AddModel(entry.Key, entry.Value);
                    }));

            return new SutDependencies
            {
                PipelineManagerMock = pipelineManagerMock,
                LogMock = new Mock<BaseLog>(),
                SmtpClientMock = new Mock<ISmtpClient>()
            };
        }

        private ScribanMailAction CreateScribanMailAction(SutDependencies dependencies)
        {
            Func<ISmtpClient> smtpClientFactory = () => dependencies.SmtpClientMock.Object;

            return new ScribanMailAction(
                dependencies.PipelineManagerMock.Object,
                smtpClientFactory,
                dependencies.LogMock.Object
            );
        }

        private WorkflowPipelineArgs CreateWorkflowPipelineArgs(string to = "to@mail.com", string from = "from@mail.com", string subject = "subject", string message = "message")
        {
            var actionItemMock = ItemFactory.CreateItem();
            ItemFactory.SetField(actionItemMock, "to", to);
            ItemFactory.SetField(actionItemMock, "from", from);
            ItemFactory.SetField(actionItemMock, "subject", subject);
            ItemFactory.SetField(actionItemMock, "message", message);

            var dataItemMock = ItemFactory.CreateItem();
            var args = new WorkflowPipelineArgs(dataItemMock.Object, null, null);
            args.ProcessorItem = actionItemMock.Object;

            return args;
        }

        private class SutDependencies
        {   
            public Mock<BaseCorePipelineManager> PipelineManagerMock { get; set; }
            public Mock<ISmtpClient> SmtpClientMock { get; set; }
            public Mock<BaseLog> LogMock { get; set; }
        }
    }
}
