define(["sitecore"], function (Sitecore) {
    Sitecore.Commands.EditBlogSettings = {
        canExecute: function (context) {
            var key = "CanExecuteWeBlogCommands";
            if (Sitecore[key] == undefined) {
                Sitecore[key] = context.app.canExecute("ExperienceEditor.WeBlog.CanExecuteWeBlogCommands", context.currentContext);
            }
            return Sitecore[key];
        },
        execute: function (context) {
            Sitecore.ExperienceEditor.PipelinesUtil.generateRequestProcessor("ExperienceEditor.WeBlog.GetCurrentBlogId", function (response1) {
                context.currentContext.itemId = response1.responseValue.value;
                var fields = "Title|Email|Theme|DisplayItemCount|DisplayCommentSidebarCount|Maximum Entry Image Size|Maximum Thumbnail Image Size|Enable RSS|Enable Comments|Show Email Within Comments|EnableLiveWriter|Enable Gravatar|Gravatar Size|Default Gravatar Style|Gravatar Rating|Custom Dictionary Folder";
                context.currentContext.argument = fields;

                Sitecore.ExperienceEditor.PipelinesUtil.generateRequestProcessor("ExperienceEditor.WeBlog.GenerateFieldEditorUrl", function (response) {
                    var dialogUrl = response.responseValue.value;
                    var dialogFeatures = "dialogHeight: 545px;dialogWidth: 640px;";
                    Sitecore.ExperienceEditor.Dialogs.showModalDialog(dialogUrl, '', dialogFeatures, null);
                }).execute(context);
            }).execute(context);
        }
    };
});