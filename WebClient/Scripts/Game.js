var game = $.connection.gameHub;
game.client.move = function (top) {
    if (top != undefined) {
        var topCard = $('.top-card');
        addColorClass(topCard, top.color);
        topCard.text(top.content);
    }
    setDisabled($('.btn-pass'));
};
game.client.activate = function() {
    setEnabled($('.cards').children());
    setEnabled($('.btn-draw'));
}
game.client.chooseColor = function() {
    $('#choose-color-modal').modal('show');
};
game.client.chosenColor = function(color) {
    addColorClass($('.top-card'), color);
}
game.client.preLastDiscarded = function() {
    setEnabled($('.btn-uno'));
}
game.client.addCards = function(cards) {
    cards.forEach(function(card) {
        var newButton = Mustache.render($('#new-card-template').html(), card);
        $('.cards').append(newButton);
        $('.card').click(function () {
            move($(this));
        });
        $('.card').prop('disabled', $('.card').eq(0).prop('disabled'));
    });
}
$.connection.hub.start().done(function() {

    $('.card').click(function () {
        move($(this));
    });

    $('.btn-draw').click(function () {
        draw();
    });
    $('.btn-pass').click(function () {
        pass();
    });
    $('.btn-uno').click(function() {
        game.server.uno();
        setDisabled($(this));
    });
    $('.modal .btn').click(function() {
        var color = $(this).data('color');
        game.server.chooseColor(color);
        addColorClass($('.top-card'), color);
    });
});
function move(card) {
    game.server.move(card.index());
    var topCard = $('.top-card');
    topCard.text(card.text());
    addColorClass(topCard, card.data('color'));
    card.remove();
    setDisabled($('.card'));
}
function draw() {
    game.server.draw();
    setDisabled($('.btn-draw'));
    setEnabled($('.btn-pass'));
}
function pass() {
    setDisabled($('.cards').children());
    game.server.pass();
}
function addColorClass(jq, className) {
    jq.removeClass('red');
    jq.removeClass('yellow');
    jq.removeClass('green');
    jq.removeClass('blue');
    jq.removeClass('black');
    jq.addClass(className);
    jq.data('color', className);
}
function setDisabled(jq) {
    jq.prop('disabled', true);
}
function setEnabled(jq) {
    jq.prop('disabled', false);
}