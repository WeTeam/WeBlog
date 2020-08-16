using Moq;
using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.Data.Templates;
using System.Collections.Generic;

namespace Sitecore.Modules.WeBlog.UnitTest
{
    internal static class TemplateFactory
    {
        public static BaseTemplateManager CreateTemplateManager(params ID[] templateIds)
        {
            var templates = new List<Template>();
            var templateCollection = new TemplateCollection();

            foreach (var id in templateIds)
            {
                var builder = new Template.Builder($"test-{id}", id, templateCollection);
                var template = builder.Template;
                templates.Add(template);
            }

            return CreateTemplateManager(templates.ToArray());
        }

        public static BaseTemplateManager CreateTemplateManager(Template[] templates)
        {
            var templateManagerMock = new Mock<BaseTemplateManager>();

            foreach (var template in templates)
            {
                templateManagerMock.Setup(x => x.GetTemplate(template.ID, It.IsAny<Database>())).Returns(template);
            }

            return templateManagerMock.Object;
        }

        public static Template CreateTemplate(ID templateId, ID baseTemplateId, TemplateCollection templateCollection)
        {
            var builder = new Template.Builder($"test-{templateId}", templateId, templateCollection);

            if(baseTemplateId != (ID)null)
                builder.SetBaseIDs(baseTemplateId.ToString());

            var template = builder.Template;
            templateCollection.Add(template);
            return template;
        }
    }
}
