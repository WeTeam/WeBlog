﻿define(["sitecore", "/-/speak/v1/ExperienceEditor/Sitecore.ExperienceEditor.js"], function (Sitecore) {
    Sitecore.Commands.EditEntrySettings = {
        canExecute: function (context) {
            return context.app.canExecute("ExperienceEditor.WeBlog.IsEntryItem", context.currentContext);
        },
        execute: function (context) {
            var fields = "Category|Tags|Author|Entry Date|Disable comments";
            context.currentContext.argument = fields;

            Sitecore.ExperienceEditor.PipelinesUtil.generateRequestProcessor("ExperienceEditor.WeBlog.GenerateFieldEditorUrl", function (response) {
                var dialogUrl = response.responseValue.value;
                var dialogFeatures = "dialogHeight: 680px;dialogWidth: 520px;";
                Sitecore.ExperienceEditor.Dialogs.showModalDialog(dialogUrl, '', dialogFeatures, null, function (e) {
                    if (e) {
                        response.context.app.refreshOnItem(response.context.currentContext);
                    }
                });
            }).execute(context);

        }
    };
});