var ikrLoader = {
	Show: function () {
		$("body").append("<div id='cover-spin'></div>");
		$('#cover-spin').show(0);
		
	},
	Hide: function () {
		$("body").find(".cover-spin").remove();
		$('#cover-spin').hide(0);
	},

	Show2: function () {
	    $(".randerBody").append("<div id='cover-spin'></div>");
	    $('#cover-spin').show(0);
	},
}