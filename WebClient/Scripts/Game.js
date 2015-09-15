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
game.client.draw = function(newCard) {
    
};
game.client.chooseColor = function() {
    var color = prompt("Choose color");
    game.server.chooseColor(color);
    addColorClass($('.top-card'), color);
};
game.client.chosenColor = function(color) {
    addColorClass($('.top-card'), color);
}
game.client.preLastDiscarded = function() {
    setEnabled($('.btn-uno'));
}
game.client.addCards = function(cards) {
    cards.forEach(function(card) {
        var newButton = $('<button>');
        newButton.text(card.content).addClass('card').addClass('btn').addClass('btn-default').addClass(card.color).appendTo($('.cards'));
        newButton.on('click', function () {
            move($(this));
        });
    });
}
$.connection.hub.start().done(function() {
    console.log('OK');
    var ress = window.location.pathname.split('/');
    game.server.enter(ress[ress.length - 1]);

    $('.card').on('click', function() {
        move($(this));
    });

    $('.btn-draw').on('click', function() {
        draw();
    });
    $('.btn-pass').on('click', function () {
        pass();
    });
    $('.btn-uno').on('click', function() {
        game.server.uno();
        setDisabled($(this));
    });
});

function move(card) {
    game.server.move(card.index());
    var topCard = $('.top-card');
    topCard.text(card.text());
    addColorClass(topCard, card.data('color'));// $(this).attr('class').split(/\s+/)[1]);
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