jQuery(function(){
	jQuery("a.wb-year").click(function(){
		var target = jQuery(this);
		target.parent().find("ul.wb-month").toggleClass("collapsed");
		target.toggleClass("collapsed");
		target.toggleClass("expanded");
	});
	
	jQuery("a.wb-month").click(function(){
		var target = jQuery(this);
		target.parent().find(".wb-entries").toggleClass("collapsed");
		target.toggleClass("collapsed");
		target.toggleClass("expanded");
	});
});