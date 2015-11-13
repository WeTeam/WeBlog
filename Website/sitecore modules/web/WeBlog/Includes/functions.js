(function ($) {
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
            if ($.url.param("tag") != null) {
                params.tag = $.url.param("tag");
            }

            var url = $.url.setUrl(viewMore.attr("href")).attr("path");
            $.get(url, params, function (data) {
                var posts = jQuery(data).find('ul li');
                entries.append(posts);
                loading.hide();
                if (posts.length>0)
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