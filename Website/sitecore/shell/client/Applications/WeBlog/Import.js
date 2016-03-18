define(["sitecore"], function (Sitecore) {
    var WordPressImport = Sitecore.Definitions.App.extend({
        initialized: function () {
            this.tabManager.showSelectFile(this);
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

            this.UploadButton.on("click", this.uploadButtonClick, this);
            this.btnAcceptLocation.on("click", this.acceptLocationButtonClick, this);
            this.btnImport.on("click", this.importButtonClick, this);
            this.Uploader.on("change:totalFiles", this.addedFile, this);
            this.on("upload-info-deleted", this.removedFile, this);
            this.on("upload-fileUploaded", this.uploadedFile, this);
        },
        updateProgress: function (that) {
            that.GenericDataProvider1.viewModel.getData({
                url: "/sitecore/api/ssc/Sitecore-Modules-WeBlog-Controllers/WordPressImport/1/CheckStatus?jobHandle=" + this.ProgressBar1.viewModel.name(),
                parameters: "",
                onSuccess: function (e) {
                    if (e.Code === 200) {
                        that.ProgressBar1.set("maxValue", e.Status.Total);
                        that.ProgressBar1.set("value", e.Status.Processed);
                        that.ProgressMessage.set("text", e.Message);
                        that.StatusMessage.set("text", e.Status.StatusMessage);
                    }
                    if (e.IsDone === false) {
                        setTimeout(function (that) {
                            console.log("22");
                            that.updateProgress(that);
                        }, 200, that);
                    } else {
                        that.ProgressBar1.set("maxValue", 1);
                        that.ProgressBar1.set("value", 1);
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
        tabManager: {
            showSelectFile: function (that) {
                that.tabManager.hideAllTabs(that);
                that.TabControl1.viewModel.showTab("{BA5DAD8B-207D-4ADE-99AD-2B77B563BCA4}");
            },
            showUpload: function (that) {
                that.tabManager.hideAllTabs(that);
                that.TabControl1.viewModel.showTab("{D4FA7870-D836-4057-8B39-F2B3E2A7A31A}");
            },
            showImportLocation: function (that) {
                that.tabManager.hideAllTabs(that);
                that.TabControl1.viewModel.showTab("{21457E01-670E-4407-86F2-49E3BBD2153F}");
            },
            showImport: function (that) {
                that.tabManager.hideAllTabs(that);
                that.TabControl1.viewModel.showTab("{C5C44663-88A4-4311-A1B1-751FEDE2C1AB}");
            },
            hideAllTabs: function (that) {
                $.each(that.TabControl1.viewModel.tabs(), function (e, d) {
                    that.TabControl1.viewModel.hideTab(d);
                }
                );
            }
        },
        addedFile: function () {
            this.Uploader.viewModel.upload();
            this.tabManager.showUpload(this);
        },
        acceptLocationButtonClick: function () {
            this.updateDatasource(this, { ParentId: this.Location.viewModel.selectedItemId() });
            this.txtbxBlogName.set("text", this.getData(this).BlogName);
            this.tabManager.showImport(this);
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
                url: "/sitecore/api/ssc/Sitecore-Modules-WeBlog-Controllers/WordPressImport/1/ImportItems",
                type: "PUT",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                context: this,
                success: function (e) {
                    this.ProgressBar1.set("name", e);
                    this.updateProgress(this);
                },
                data: JSON.stringify(this.getData(this))
            });
            this.Border1.viewModel.hide();
            this.Border2.viewModel.show();
        },
        uploadButtonClick: function () {
            var file = this.UploaderInfo.viewModel.files()[0];
            this.updateDatasource(this, { BlogName: file.name() });
            this.Uploader.viewModel.upload();
        },
        removedFile: function () {
            this.tabManager.showSelectFile(this);
        },
        uploadedFile: function (item) {
            this.updateDatasource(this, { DataSourceId: item.itemId });
            this.tabManager.showImportLocation(this);
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