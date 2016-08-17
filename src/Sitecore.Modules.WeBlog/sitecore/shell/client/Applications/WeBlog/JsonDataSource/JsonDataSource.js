define(["jquery", "sitecore"], function ($, Sitecore) {
    "use strict";

    var model = Sitecore.Definitions.Models.ComponentModel.extend({
        initialize: function () {
            this._super();
            this.set("json", null);
        },
        add: function (data) {
            var json = this.get("json");
            if (json === null) {
                json = new Array();
            }

            var newArray = new Array(json.length + 1);
            for (var i = json.length - 1; i >= 0; i--) {
                newArray[i + 1] = json[i];
            }
            newArray[0] = data;
            this.set("json", newArray);
        }
    });

    var view = Sitecore.Definitions.Views.ComponentView.extend({
        initialize: function () {
            this._super();
            this.model.set("json", null);
        }
    });

    Sitecore.Factories.createComponent("JsonDataSource", model, view, "script[type='text/x-sitecore-jsondatasource']");
});