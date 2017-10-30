var WeBlog;
(function (WeBlog) {
    var Fields;
    (function (Fields) {
        var TagField = (function () {
            function TagField() {
            }
            TagField.prototype.onTagFieldUpdated = function () {
                var valueId = '#' + jQuery(this).attr('id').replace("_Select", "_Value");
                var selectedValues = jQuery(this).val();
                var valueStr = selectedValues ? selectedValues.join(',') : "";
                jQuery(valueId).val(valueStr);
                event.stopPropagation();
            };
            TagField.prototype.onDropDown = function () {
                jQuery(this).next().find('.chosen-drop').css('position', 'relative');
            };
            TagField.prototype.onHideDropDown = function () {
                jQuery(this).next().find('.chosen-drop').css('position', 'absolute');
            };

            return TagField;
        }());
        Fields.TagField = TagField;
    })(Fields = WeBlog.Fields || (WeBlog.Fields = {}));
})(WeBlog || (WeBlog = {}));


WeBlog.tagField = new WeBlog.Fields.TagField();