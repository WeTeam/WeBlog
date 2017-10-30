define(["sitecore"], function (Sitecore) {
    var wordPressImportEndpoint = "/sitecore/api/ssc/Sitecore-Modules-WeBlog-Controllers/WordPressImport/1/";
    var WordPressImport = Sitecore.Definitions.App.extend({
        initialized: function () {
            this.TabManager.showSelectFile(this);
            this.DataSource.add({
                BlogName: "",
                BlogEmail: "",
                ParentId: "{0DE95AE4-41AB-4D01-9EB0-67441B7C2450}",
                DataSourceId: "",
                ImportPosts: true,
                ImportCategories: true,
                ImportComments: true,
                ImportTags: true
            });

            this.Uploader.on("change:totalFiles", this.addedFile, this);
            this.TemplateMappingItem.on("change:selectedItemId", this.templateMappingItemSelected, this);
            this.LanguageSelector.on("change:selectedItemId", this.languageItemSelected, this);
            this.on("upload-info-deleted", this.removedFile, this);
            this.on("upload-fileUploaded", this.uploadedFile, this);
        },

        templateMappingItemSelected: function (that) {
            if (this.TemplateMappingItem.viewModel.selectedItemId() !== "{0B1FD353-3175-43A0-9A81-13B18D99408E}") {
                this.TemplatesMappingButton.viewModel.show();
            } else {
                this.TemplatesMappingButton.viewModel.hide();
            }
        },

        languageItemSelected: function () {
            var selectedItemId = this.LanguageSelector.viewModel.selectedItemId();
            this.updateDatasource(this, { LanguageItemId: selectedItemId });
            if (selectedItemId === "{64C4F646-A3FA-4205-B98E-4DE2C609B60F}") {
                this.btnImport.viewModel.hide();
            } else {
                this.btnImport.viewModel.show();
            }
        },

        updateProgress: function (that) {
            that.ImportStatusProvider.viewModel.getData({
                url: wordPressImportEndpoint + "CheckStatus?jobHandle=" + this.ProgressBar.viewModel.name(),
                parameters: "",
                onSuccess: function (e) {
                    if (e.Code === 200) {
                        that.ProgressBar.set("maxValue", e.Status.Total);
                        that.ProgressBar.set("value", e.Status.Processed);
                        that.ProgressMessage.set("text", e.Message);
                        that.StatusMessage.set("text", e.Status.StatusMessage);
                    }
                    if (e.IsDone === false) {
                        setTimeout(function (that) {
                            that.updateProgress(that);
                        }, 200, that);
                    } else {
                        that.ProgressBar.set("maxValue", 1);
                        that.ProgressBar.set("value", 1);
                        that.StatusMessage.set("text", "");

                        if (e.Code === 200) {
                            that.ProgressMessage.set("text", "Import complete");
                        } else {
                            that.ProgressMessage.set("text", e.Message);
                        }
                    }
                }
            });
        },
        TabManager: {
            showSelectFile: function (that) {
                that.TabManager.hideAllTabs(that);
                that.TabControl1.viewModel.showTab("{BA5DAD8B-207D-4ADE-99AD-2B77B563BCA4}");
            },
            showUpload: function (that) {
                that.TabManager.hideAllTabs(that);
                that.TabControl1.viewModel.showTab("{D4FA7870-D836-4057-8B39-F2B3E2A7A31A}");
            },
            showImportLocation: function (that) {
                that.TabManager.hideAllTabs(that);
                that.TabControl1.viewModel.showTab("{21457E01-670E-4407-86F2-49E3BBD2153F}");
            },
            showImport: function (that) {
                that.TabManager.hideAllTabs(that);
                that.TabControl1.viewModel.showTab("{C5C44663-88A4-4311-A1B1-751FEDE2C1AB}");
            },
            showTemplatesMapping: function (that) {
                that.TabManager.hideAllTabs(that);
                that.TabControl1.viewModel.showTab("{D53FDEBA-4B4B-4CC7-A244-932C1D0A84CF}");
            },
            hideAllTabs: function (that) {
                $.each(that.TabControl1.viewModel.tabs(), function (e, d) {
                    that.TabControl1.viewModel.hideTab(d);
                });
            }
        },
        addedFile: function () {
            this.Uploader.viewModel.upload();
            this.TabManager.showUpload(this);
        },
        acceptLocationButtonClick: function () {
            this.updateDatasource(this, {
                ParentId: this.Location.viewModel.selectedItemId(),
                DatabaseName: this.Location.viewModel.selectedNode().itemUri.databaseUri.databaseName
            });
            this.txtbxBlogName.set("text", this.getData(this).BlogName);
            this.TabManager.showTemplatesMapping(this);
        },
        acceptTemplatesMappingButtonClick: function () {
            this.updateDatasource(this, {
                TemplateMappingItemId: this.TemplateMappingItem.viewModel.selectedItemId(),
            });
            this.TabManager.showImport(this);
        },
        importButtonClick: function () {
            this.updateDatasource(this, {
                BlogName: this.txtbxBlogName.get("text"),
                BlogEmail: this.txtbxBlogEmail.get("text"),
                ImportPosts: this.cbxPosts.viewModel.isChecked(),
                ImportCategories: this.cbxCategories.viewModel.isChecked(),
                ImportComments: this.cbxComments.viewModel.isChecked(),
                ImportTags: this.cbxTags.viewModel.isChecked()
            }),
                $.ajax({
                    url: wordPressImportEndpoint + "ImportItems",
                    type: "PUT",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    context: this,
                    success: function (e) {
                        this.ProgressBar.set("name", e);
                        this.updateProgress(this);
                    },
                    data: JSON.stringify(this.getData(this))
                });
            this.ImportOptionsPanel.viewModel.hide();
            this.ImportProgressPanel.viewModel.show();
        },
        uploadButtonClick: function () {
            var file = this.UploaderInfo.viewModel.files()[0];
            this.updateDatasource(this, { BlogName: file.name() });
            this.Uploader.viewModel.upload();
        },
        removedFile: function () {
            this.TabManager.showSelectFile(this);
        },
        uploadedFile: function (item) {
            this.updateDatasource(this, { DataSourceId: item.itemId });
            this.TabManager.showImportLocation(this);
        },
        getData: function (that) {
            var lastIndex = that.DataSource.viewModel.json().length - 1;
            return that.DataSource.viewModel.json()[lastIndex];
        },
        updateObject: function (obj, diff) {
            for (var key in diff) {
                obj[key] = diff[key];
            }
            return obj;
        },
        updateDatasource: function (that, obj) {
            var data = that.getData(that);
            data = that.updateObject(data, obj);
            that.DataSource.add(data);
        }
    });
    return WordPressImport;
});