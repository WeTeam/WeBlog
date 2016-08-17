/*
Tagger Widget v1.0
Copyright (C) 2008 Chris Iufer (chris@iufer.com

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>
*/

//--------------------------------------------------------------------------------------------
// TODO: Get a list of tags and put them in a hidden textbox
//<input class="" type="text" name="tags" id="tagnames" runat="server" />
//--------------------------------------------------------------------------------------------

// n = tag


(function($) {

    $.fn.addTag = function(v) {
        var r = v.split(',');
        for (var i in r) {
            n = r[i].replace(/([^a-zA-Z0-9\s\-\_])|^\s|\s$/g, '');
            if (n == '') return false;
            var fn = $(this).data('name');
            var i = $('<input type="hidden" />').attr('name', fn).val(n);
            $('#tagnames').append(n + ', ');
            var t = $('<li />').text(n).addClass('tagName')
				.click(function() {
				    // remove
				    var hidden = $(this).data('hidden');
				    $(hidden).remove();
				    $(this).remove();
				})
				.data('hidden', i);
            var l = $(this).data('list');
            $(l).append(t).append(i);
        }
    };

})(jQuery);

$(document).ready(function() {
    $('.tagger').each(function(i) {
        $(this).data('name', $(this).attr('name'));
        $(this).removeAttr('name');
        var b = $('<button type="button">Add</button>').addClass('tagAdd')
			.click(function() {
			    var tagger = $(this).data('tagger');
			    $(tagger).addTag($(tagger).val());
			    $(tagger).val('');
			    $(tagger).stop();
			})
			.data('tagger', this);
        var l = $('<ul />').addClass('tagList');
        $(this).data('list', l);
        $(this).after(l).after(b);
    })
	.bind('keypress', function(e) {
	    if (13 == e.keyCode) {
	        //console.log(e.keyCode);
	        $(this).addTag($(this).val());
	        $(this).val('');
	        $(this).stop();
	        return false;
	    }
	});
});
