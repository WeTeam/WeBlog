jQuery(function(){
	jQuery("a.wb-year").click(function(){
		jQuery(this).parent().find(".wb-month").toggleClass("collapsed");
	});
	
	jQuery("a.wb-month").click(function(){
		jQuery(this).parent().find(".wb-entries").toggleClass("collapsed");
	});
});