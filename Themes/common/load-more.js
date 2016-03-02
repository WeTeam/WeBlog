jQuery(function(){
	jQuery('.wb-view-more', '.wb-view-more-wrapper').click(function () {
            var viewMore = $(this);
            var loading = viewMore.next(".wb-loading-animation");
            var entries = $("#wb-entry-list ul");
            viewMore.hide();
            loading.show();

            //jQuery.url.setUrl(document.location);
            var params = {
                startIndex: entries.children().length,
                blogAjax: 1
            };
			
			var currentUrl = new Url(document.location);
			
            if (currentUrl.query.tag) {
                params.tag = currentUrl.query.tag;
            }

            //var url = $.url.setUrl(viewMore.attr("href")).attr("path");
			
			var href = new Url(viewMore.attr("href"));
			var url = href.path
			
			//var url = viewMore.attr("href");
            $.get(url, params, function (data) {
                var posts = jQuery(data).find('ul li');
                entries.append(posts);
                loading.hide();
                if (posts.length > 0)
                    viewMore.show();
            });
			
            return false;
        });
});