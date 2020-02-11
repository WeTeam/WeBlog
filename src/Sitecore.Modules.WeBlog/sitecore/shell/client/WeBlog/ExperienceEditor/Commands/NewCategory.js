define(["sitecore", "/-/speak/v1/ExperienceEditor/ExperienceEditor.js"], function (Sitecore, ExperienceEditor) {
    Sitecore.Commands.NewCategory = {
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
                defaultValue: "Category",
                header: "Create new category",
                maxLength: "100",
                maxLengthValidatationText: "The length of the value is too long. Please specify a value of less than 100 characters.",
                message: "Enter the name of your new category:",
                validation: "^[\\w\\*\\$][\\w\\s\\-\\$]*(\\(\\d{1,}\\)){0,1}$",
                validationText: "'$Input' is not a valid name."
            }
            var features = "dialogWidth:400px;dialogHeight:190px;help:no;scroll:no;resizable:no;maximizable:no;status:no;center:yes;autoIncreaseHeight:yes";

            ExperienceEditor.Dialogs.showModalDialog(url, dialogArguments, features, null, function (name) {
                if (name == null) {
                    response.context.aborted = true;
                    return;
                }
                context.currentContext.argument = name;
                ExperienceEditor.PipelinesUtil.generateRequestProcessor("ExperienceEditor.WeBlog.NewCategory", function (response) {
                }).execute(context);
            });
        }
    };
});