define(["sitecore", "/-/speak/v1/ExperienceEditor/ExperienceEditor.js"], function (Sitecore, ExperienceEditor) {
    Sitecore.Commands.NewEntry = {
        canExecute: function (context) {
            var key = "CanExecuteWeBlogCommands";
            if (Sitecore[key] == undefined) {
                Sitecore[key] = context.app.canExecute("ExperienceEditor.WeBlog.CanExecuteWeBlogCommands", context.currentContext);
            }
            return Sitecore[key];
        },
        execute: function (context) {
            ExperienceEditor.Dialogs.prompt("Enter the title of your new entry:", "", function (name) {
                if (name == null) {
                    return;
                }
                context.currentContext.argument = name;
                ExperienceEditor.PipelinesUtil.generateRequestProcessor("ExperienceEditor.WeBlog.NewEntry", function (response) {
                    response.context.app.refreshOnItem(response.context.currentContext);
                }).execute(context);
            });
        }
    };
});
