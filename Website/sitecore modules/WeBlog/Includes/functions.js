jQuery = jQuery.noConflict();
jQuery(function () {
    blogViewMore();
});

function ToggleVisibility(elementId, caller) {
    var el = jQuery("#" + elementId);
    el.toggle('200');
    if (caller != null)
        jQuery(caller).toggleClass('expanded');
}

function blogViewMore() {
    jQuery(".wb-view-more").live("click", function () {
        var viewMore = jQuery(this);
        var loading = viewMore.next(".wb-loading-animation");
        viewMore.hide();
        loading.show();
        jQuery.url.setUrl(document.location);
        var params = {
            startIndex: jQuery(".wb-entry-list ul").children().length,
            blogAjax: 1
        }
        if (jQuery.url.param("tag") != null) {
            params.tag = jQuery.url.param("tag");
        }
        var url = jQuery.url.setUrl(jQuery(this).attr("href")).attr("path");
        jQuery.get(url, params, function (data) {
            var posts = jQuery(data).find('ul li');
            loading.parent().parent().find('ul').append(posts);
            loading.hide();
            viewMore.show();
        });
        return false;
    });
}