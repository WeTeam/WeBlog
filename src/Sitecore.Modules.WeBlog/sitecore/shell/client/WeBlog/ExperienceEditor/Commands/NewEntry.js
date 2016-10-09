define(["sitecore", "/-/speak/v1/ExperienceEditor/Sitecore.ExperienceEditor.js"], function (Sitecore) {
    Sitecore.Commands.NewEntry = {
        canExecute: function (context) {
            var key = "CanExecuteWeBlogCommands";
            if (Sitecore[key] == undefined) {
                Sitecore[key] = context.app.canExecute("ExperienceEditor.WeBlog.CanExecuteWeBlogCommands", context.currentContext);
            }
            return Sitecore[key];
        },
        execute: function (context) {
            Sitecore.ExperienceEditor.Dialogs.prompt("Enter the title of your new entry:", "", function (name) {
                if (name == null) {
                    return;
                }
                context.currentContext.argument = name;
                Sitecore.ExperienceEditor.PipelinesUtil.generateRequestProcessor("ExperienceEditor.WeBlog.NewEntry", function (response) {
                    var itemId = !response.responseValue.value ? null : response.responseValue.value.itemId;
                    if (itemId == null || itemId.length <= 0) {
                        response.context.aborted = true;
                        return;
                    }

                    response.context.currentContext.itemId = itemId;
                    response.context.app.refreshOnItem(response.context.currentContext);
                }).execute(context);
            });
        }
    };
});