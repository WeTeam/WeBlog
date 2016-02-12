jQuery(function(){
	jQuery(".wb-year").click(function(){
		jQuery(this).parent().find(".wb-month").toggle();
	});
	
	jQuery(".wb-month").click(function(){
		jQuery(this).parent().find(".wb-entries").toggle();
	});
});