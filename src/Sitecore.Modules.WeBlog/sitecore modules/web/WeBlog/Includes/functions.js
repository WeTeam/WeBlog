(function ($) {
    function getQueryStringParameter(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name.toLowerCase() + "=([^&#]*)"),
            results = regex.exec(location.search.toLowerCase());
        return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    $(document).ready(function () {
        // infinite scroll
        $('.wb-view-more', '#wb-view-more-wrapper').click(function () {
            var viewMore = $(this);
            var loading = viewMore.next(".wb-loading-animation");
            var entries = $("#wb-entry-list ul");
            viewMore.hide();
            loading.show();

            $.url.setUrl(document.location);
            var params = {
                startIndex: entries.children().length,
                blogAjax: 1
            };

            var startIndex = getQueryStringParameter("startIndex");
            if (startIndex !== "") {
                params.startIndex += parseInt(startIndex, 0);
            }

            if ($.url.param("tag") != null) {
                params.tag = $.url.param("tag");
            }

            if ($.url.param("sort") != null) {
                params.sort = $.url.param("sort");
            }

            var url = $.url.setUrl(viewMore.attr("href")).attr("path");
            $.get(url, params, function (data) {
                var posts = jQuery(data).find('ul li');
                entries.append(posts);
                loading.hide();
                if (posts.length > 0)
                    viewMore.show();
            });
            return false;
        });

        // archive toggle
        $('#wb-archive .wb-year, #wb-archive .wb-month').click(function () {
            var ctl = $(this);
            ctl.next('ul').toggle('200');
            ctl.toggleClass('expanded');
        });
    });
})(jQuery);