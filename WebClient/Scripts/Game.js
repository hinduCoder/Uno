var game = $.connection.gameHub;
game.client.move = function (top) {
    if (top != undefined)
        $('.top-card').text(top);
    setEnabled($('.card'));
    setEnabled($('btn-draw'));
    setDisabled($('.btn-pass'));
};
game.client.draw = function(newCard) {
    $('<button>').text(newCard).appendTo($('.cards'));
};
$.connection.hub.start().done(function() {
    console.log('OK');
    var ress = window.location.pathname.split('/');
    game.server.enter(ress[ress.length - 1]);

    $('.card').on('click', function() {
        game.server.move($(this).index());
        $('.top-card').text($(this).text());
        $(this).remove();
        $('.card').prop('disabled', true);
    });

    $('.btn-draw').on('click', function() {
        game.server.draw();
        setDisabled($(this));
        setEnabled($('.btn-pass'));
    });
    $('.btn-pass').on('click', function () {
        setDisabled($('.card'));
        game.server.pass();
    });
});

function setDisabled(jq) {
    jq.prop('disabled', true);
}
function setEnabled(jq) {
    jq.prop('disabled', false);
}