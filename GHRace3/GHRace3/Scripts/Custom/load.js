
function LoadClick() {
    //$('#clickme').click(function () {
    //    $('#book').animate({
    //        opacity: 0.25,
    //        left: '+=50',
    //        height: 'toggle'
    //    }, 5000, function () {
    //        // Animation complete.
    //    });
    //});
    $("#progress-bar").animate({
        width: '100%'
    }, 10000, function () {
        //complete
    });

}

//$("#race-results").ready(function () {
//    if ($(this).hidden) {
//        $("#url-form").slideUp(300, function () { });
//    }
    
//});

$("#minimize").click(function () {
    $("#url-form").slideUp(300, function () { });
});

$("#maximize").click(function () {
    $("#url-form").slideDown(300, function () { });
});

$(document).ready(function () {
    $("#load-button").click(LoadClick);
});
