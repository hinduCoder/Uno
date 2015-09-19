
var lobby = $.connection.lobbyHub;
lobby.client.addPlayerToRoom = function (roomIndex, name) {
    var room = $('.rooms').children().eq(roomIndex);
    room.find('.players').append('<p>' + name + '</p>');
    var placesLeft = room.find('.places-left');
    placesLeft.text(parseInt(placesLeft.text()) - 1);
}
lobby.client.allowStart = function(roomIndex) {
    $('.rooms').children().eq(roomIndex).find('.btn-start').removeClass('disabled');
}
lobby.client.addRoom = function(placesCount) {
    var newRoom = $(Mustache.render($('#room-template').html(), { placesCount: placesCount }));
    $('.rooms').append(newRoom);
    newRoom.find('.btn-join').click(joinHandler);
}
$.connection.hub.start().done(function () {  
    $('.btn-join').click(joinHandler);
    $('#btn-add-room').click(function() {
        var placesCount = $('#input-players-count').val();
        lobby.server.addRoom(placesCount);
    });
});
function joinHandler() {
    lobby.server.addPlayerToRoom($(this).parents('.room').index());
    $('.btn-join').prop('disabled', true);
};
