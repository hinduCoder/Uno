var game = $.connection.gameHub;
game.client.move = function (top) {
    if (top != undefined) {
        var topCard = $('.top-card');
        topCard.text(top.content);
        topCard.removeClass('red');
        topCard.removeClass('yellow');
        topCard.removeClass('green');
        topCard.removeClass('blue');
        topCard.removeClass('black');
        topCard.addClass(top.color);
    }
    setEnabled($('.cards').children());
    setEnabled($('btn-draw'));
    setDisabled($('.btn-pass'));
};
game.client.draw = function(newCard) {
    $('<button>').text(newCard.content).addClass('card').addClass(newCard.color).appendTo($('.cards'));
};
$.connection.hub.start().done(function() {
    console.log('OK');
    var ress = window.location.pathname.split('/');
    game.server.enter(ress[ress.length - 1]);

    $('.card').on('click', function() {
        game.server.move($(this).index());
        var topCard = $('.top-card');
        topCard.text($(this).text());
        topCard.text(top.content);
        topCard.removeClass('red');
        topCard.removeClass('yellow');
        topCard.removeClass('green');
        topCard.removeClass('blue');
        topCard.removeClass('black');
        topCard.addClass($(this).attr('class').split(/\s+/)[1]);

        $(this).remove();
        $('.card').prop('disabled', true);
    });

    $('.btn-draw').on('click', function() {
        game.server.draw();
        setDisabled($(this));
        setEnabled($('.btn-pass'));
    });
    $('.btn-pass').on('click', function () {
        setDisabled($('.cards').children());
        game.server.pass();
    });
});

function setDisabled(jq) {
    jq.prop('disabled', true);
}
function setEnabled(jq) {
    jq.prop('disabled', false);
}