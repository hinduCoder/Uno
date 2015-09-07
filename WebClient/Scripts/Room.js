
var lobby = $.connection.lobbyHub;
lobby.client.addPlayerToRoom = function (roomIndex, name) {
    $('body').children().eq(roomIndex).find('.players').append('<p>' + name + '</p>');
}
lobby.client.allowStart = function(roomIndex) {
    $('body').children().eq(roomIndex).find('.btn-start').show();
}
$.connection.hub.start().done(function () {
    $('.btn-join').on('click', function () {
        lobby.server.addPlayerToRoom($(this).parent().index());
        $(this).hide();
    });
});
