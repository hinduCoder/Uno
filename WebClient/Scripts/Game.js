"use strict";

$.fn.disabled = function (value) {
    if (value !== undefined) {
        $(this).prop('disabled', value);
        return this;
    } else
        return $(this).prop('disabled');
};

var game = $.connection.gameHub;
game.client.move = function (top) {
    if (top != undefined) {
        var topCard = $('.top-card');
        addColorClass(topCard, top.color);
        topCard.text(top.content);
    }
    $('.btn-pass').disabled(true);
};
game.client.discard = function (index) {
    var card = $('.card').eq(index);
    var topCard = $('.top-card');
    topCard.text(card.text());
    addColorClass(topCard, card.data('color'));
    card.remove();
    $('.card').disabled(true);
}
game.client.activate = function() {
    $('.card').disabled(false);
    $('.btn-draw').disabled(false);
}
game.client.chooseColor = function() {
    $('#choose-color-modal').modal('show');
};
game.client.chosenColor = function(color) {
    addColorClass($('.top-card'), color);
}
game.client.preLastDiscarded = function() {
    $('.btn-uno').disabled(false);
}
game.client.addCards = function(cards) {
    cards.forEach(function(card) {
        addNewCard(card);
    });
}
game.client.wrongCard = function (index, card) {
    alert('Wrong card');    
}
function addNewCard(card) {
    var newButton = $(Mustache.render($('#new-card-template').html(), card));
    $('.cards').append(newButton);
    newButton.click(function () {
        move($(this));
    });
    newButton.disabled($('.card').eq(0).disabled());
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
        $(this).disabled(true);
    });
    $('.modal .btn').click(function() {
        var color = $(this).data('color');
        game.server.chooseColor(color);
        addColorClass($('.top-card'), color);
    });
});
function move(card) {
    game.server.move(card.index());
    
}
function draw() {
    game.server.draw();
    $('.btn-draw').disabled(true);
    $('.btn-pass').disabled(false);
    $('.btn-uno').disable(false);
}
function pass() {
    $('.cards').children().disabled(true);
    game.server.pass();
}
function addColorClass(jq, className) {
    console.log(className);
    jq.removeClass('red');
    jq.removeClass('yellow');
    jq.removeClass('green');
    jq.removeClass('blue');
    jq.removeClass('black');
    jq.addClass(className);
    jq.data('color', className);
}
