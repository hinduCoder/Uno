var game = $.connection.gameHub;
$.extend(game.client, {
    move: function(top) {
        if (top != undefined) {
            setTopCard(top);
        }
        $('.btn-pass').disabled(true);
    },
    discard: function(index) {
        var card = $('.card').eq(index);
        var topCard = $('.top-card');
        topCard.text(card.text());
        addColorClass(topCard, card.data('color'));
        card.remove();
        $('.card').disabled(true);
    },
    activate: function() {
        $('.card').disabled(false);
        $('.btn-draw').disabled(false);
    },
    chooseColor: function() {
        $('#choose-color-modal').modal('show');
    },
    chosenColor: function(color) {
        addColorClass($('.top-card'), color);
    },
    preLastDiscarded: function() {
        $('.btn-uno').disabled(false);
    },
    addCards: function(cards) {
        cards.forEach(function (card) { addNewCard(card)});
    },
    wrongCard: function() {
        $('#wrong-card-alert').addClass('in');
        setTimeout(function () { $('#wrong-card-alert').removeClass('in')}, 1000);
    },
    finish: function (scores) {
        $('.card').remove();
        $('#score-modal .modal-body').html(Mustache.render($('#score-table-template').html(), { table: scores }));
        $('#score-modal').modal('show');
        
    },
    win: function() {
        $('#congrats-alert').addClass('in');
    },
    log: function(desc) {
        $('.list-group').prepend(Mustache.render($('#log-item-template').html(), { description: desc }));
    },
    newGame: function(top) {
        setTopCard(top);
    }
});
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
    $('.btn-uno').disabled(false);
}
function pass() {
    $('.cards').children().disabled(true);
    game.server.pass();
}

function setTopCard(card) {
    var topCard = $('.top-card');
    addColorClass(topCard, card.color);
    topCard.text(card.content);
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
$.fn.disabled = function (value) {
    if (value !== undefined) {
        $(this).prop('disabled', value);
        return this;
    } else
        return $(this).prop('disabled');
};