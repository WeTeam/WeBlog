jQuery(function(){
	jQuery('.wb-view-more', '.wb-view-more-wrapper').click(function () {
            var viewMore = jQuery(this);
            var loading = viewMore.next(".wb-loading-animation");
			var entryWrapper = jQuery(".wb-entry-list");
            var entries = jQuery("section", entryWrapper);
            viewMore.hide();
            loading.show();

            var params = {
                startIndex: entries.length,
                blogAjax: 1
            };
			
			var currentUrl = new Url(document.location.href);
			
            if (currentUrl.query.tag) {
                params.tag = currentUrl.query.tag;
            }
			
			var href = new Url(viewMore.attr("href"));
			var url = href.path
			
            jQuery.get(url, params, function (data) {
				var posts = jQuery("<div/>").html(data).find(".wb-entry-list section");
				jQuery(".wb-view-more-wrapper").before(posts);
                loading.hide();
                if (posts.length)
                    viewMore.show();
            });
			
            return false;
        });
});