define(["sitecore"], function (Sitecore) {
    Sitecore.Commands.NewCategory = {
        canExecute: function (context) {
            var key = "CanExecuteWeBlogCommands";
            if (Sitecore[key] == undefined) {
                Sitecore[key] = context.app.canExecute("ExperienceEditor.WeBlog.CanExecuteWeBlogCommands", context.currentContext);
            }
            return Sitecore[key];
        },
        execute: function (context) {
            Sitecore.ExperienceEditor.Dialogs.prompt("Enter the name of your new category:", "", function (name) {
                if (name == null) {
                    return;
                }
                context.currentContext.argument = name;
                Sitecore.ExperienceEditor.PipelinesUtil.generateRequestProcessor("ExperienceEditor.WeBlog.NewCategory", function (response) {
                }).execute(context);
            });
        }
    };
});