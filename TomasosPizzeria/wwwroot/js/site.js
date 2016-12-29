
//Sticky navigation 
var mn = $(".main-nav");
var m = $(".main");
mns = "main-nav-scrolled";
mmt = "main-margin-top";
hdr = $('.header').height();


$(window).scroll(function () {
    if ($(this).scrollTop() > hdr) {
        mn.addClass(mns);
        m.addClass(mmt);
    } else {
        mn.removeClass(mns);
        m.removeClass(mmt);
    }
});

//Flimmering message on successful shoppingcart change

var $successDiv = $("#success-div");
//var backgroundInterval = setInterval(function() {
//        $successDiv.toggleClass("flimmer");
//    },
//    100);

if ($('#success-div').length) {
    setInterval(function () {
        $successDiv.toggleClass("flimmer");
    },
 500);
    setTimeout(function () {
        $successDiv.remove();
    },
        3000);
}


