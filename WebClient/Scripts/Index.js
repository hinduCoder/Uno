$(document).ready(function() {
//    var changeNumberFontSize = function() {
//        var height = $('.card').height();
//        $('.card .content').css('font-size', height / 2);
//        $('.card .content.small').css('font-size', height / 8);
//    }
//    $(window).resize(changeNumberFontSize);
//    changeNumberFontSize();

    $('button').on('click', function(e) {
        window.location.href = "/Home/Move?index=" + $(this).data('index');
    });
});