jQuery(function() {
    var sortOptions = jQuery('.wb-tag-sorting li a');
    var sortDirection = {
        lastused: -1,
        alphabetic: 1,
        entrycount: -1
    }
    var sortTags = function(selectedSort) {
        var tags = jQuery('.wb-tag-cloud .wb-entries a');
        if (tags.length === 0) {
            return;
        }
        if (selectedSort.hasClass('wb-tag-sort-current')) {
            return;
        }
        var container = tags.first().parent();
        var sortOption = selectedSort.data('tag-sort');
        var direction = sortDirection[sortOption] ? sortDirection[sortOption] : 1;
        tags.sort(function(a, b) {
            a = jQuery(a).data(sortOption);
            b = jQuery(b).data(sortOption);
            return a > b ? direction : a < b ? (direction * -1) : 0;
        }).each(function() {
            container.append(this);
        });
        sortOptions.removeClass('wb-tag-sort-current');
        selectedSort.addClass('wb-tag-sort-current');
    }
    sortOptions.click(function() {
        sortTags(jQuery(this));
        return false;
    });
    var defaultSort = sortOptions.first();
    if (defaultSort) {
        sortTags(defaultSort);
    }
    if (sortOptions.length < 2) {
        jQuery('.wb-tag-sorting').hide();
    }
});