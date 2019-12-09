$(".navbar-toggler").on('click', function () {
    if ($('.fixed-nav').hasClass('nav-scroll')) {
        $('.fixed-nav').removeClass('nav-scroll');
    }
    else {
        $('.fixed-nav').addClass('nav-scroll');
    }
});