function ToggleVisibility(elementId) {
    switch(document.getElementById(elementId).style.display)
    {
    case "block":
      document.getElementById(elementId).style.display = "none";
      break;    
    case "none":
      document.getElementById(elementId).style.display = "block";
      break;
    default:
        document.getElementById(elementId).style.display = "none";
    }
}

jQuery = jQuery.noConflict();
jQuery(function () {
    blogViewMore();
});

function blogViewMore() {
    jQuery(".viewMore").click(function () {
        var viewMore = jQuery(this);
        var loading = viewMore.next(".loadingAnimation");
        viewMore.remove();
        loading.show();
        jQuery.url.setUrl(document.location);
        var params = {
            startIndex: jQuery(".entry").size(),
            blogAjax: 1
        }
        if (jQuery.url.param("tag") != null) {
            params.tag = jQuery.url.param("tag");
        }
        var url = jQuery.url.setUrl(jQuery(this).attr("href")).attr("path");
        jQuery.get(url, params, function (data) {
            loading.parent().after(data);
            loading.parent().remove();
            blogViewMore();
        });
        return false;
    });
}