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
            var url = "/sitecore/shell/default.aspx?xmlcontrol=Prompt";
            var dialogArguments = {
                defaultValue: "Entry",
                header: "Create new entry",
                maxLength: "100",
                maxLengthValidatationText: "The length of the value is too long. Please specify a value of less than 100 characters.",
                message: "Enter the title of your new entry:",
                validation: "^[\\w\\*\\$][\\w\\s\\-\\$]*(\\(\\d{1,}\\)){0,1}$",
                validationText: "'$Input' is not a valid name."
            }
            var features = "dialogWidth:400px;dialogHeight:190px;help:no;scroll:no;resizable:no;maximizable:no;status:no;center:yes;autoIncreaseHeight:yes";

            Sitecore.ExperienceEditor.Dialogs.showModalDialog(url, dialogArguments, features, null, function (name) {
                if (name == null) {
                    response.context.aborted = true;
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