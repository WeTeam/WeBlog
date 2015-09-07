using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Text;
using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;

namespace Sitecore.Modules.WeBlog.ExperienceEditor
{
    public class GenerateFieldEditorUrl : PipelineProcessorRequest<ItemContext>
    {
        public string GenerateUrl()
        {
            var fieldList = CreateFieldDescriptors(RequestContext.Argument);
            var fieldeditorOption = new FieldEditorOptions(fieldList);
            fieldeditorOption.SaveItem = true;
            return fieldeditorOption.ToUrlString().ToString();
        }

        private List<FieldDescriptor> CreateFieldDescriptors(string fields)
        {
            var fieldList = new List<FieldDescriptor>();
            var fieldString = new ListString(fields);
            foreach (string field in new ListString(fieldString))
                fieldList.Add(new FieldDescriptor(RequestContext.Item, field));
            return fieldList;
        }

        public override PipelineProcessorResponseValue ProcessRequest()
        {
            return new PipelineProcessorResponseValue
            {
                Value = GenerateUrl()
            };
        }
    }
}