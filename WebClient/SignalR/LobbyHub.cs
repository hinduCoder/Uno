using System.Diagnostics;
using System.Linq;
using System.Net.Cache;
using System.Threading.Tasks;
using System.Web.Security;
using dotless.Core.Parser.Infrastructure;
using Microsoft.AspNet.SignalR;
using WebClient.Controllers;
using WebClient.Models;

namespace WebClient.SignalR
{
    public class LobbyHub : Hub
    {
        private readonly Lobby _lobby;

        public LobbyHub(Lobby lobby)
        {
            _lobby = lobby;
        }

        public void AddPlayerToRoom(int roomIndex)
        {
            var player = _lobby.GetPlayerByName(GetCurrentUserName());
            var room = _lobby.Rooms[roomIndex];
            room.AddPlayer(player);
            
            Clients.All.addPlayerToRoom(roomIndex, GetCurrentUserName());
            if (room.CanStart)
                Clients.Clients(room.Players.Select(p => p.ConnectionId).ToList()).allowStart(roomIndex);
        }

        public void AddRoom(int playersCount)
        {
            _lobby.AddRoom(playersCount);
            Clients.All.addRoom(playersCount);
        }

        public override async Task OnConnected()
        {
            await Task.Run(() =>
            {
                    var newPlayerName = GetCurrentUserName();
                if (newPlayerName == null)
                    return;
                var newPlayer = new Player(newPlayerName) {ConnectionId = Context.ConnectionId};
                _lobby.AddPlayer(newPlayer);
            });
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            await Task.Run(() =>
            {
                Debug.WriteLine(stopCalled);
                //var name = Context.RequestCookies["name"]?.Value;
                //Lobby.Instance.RemovePlayer(name);
            });
        }

        public override Task OnReconnected()
        {
            return OnConnected();
        }

        private string GetCurrentUserName()
        {
            var cookie = Context.RequestCookies["userid"];
            if (cookie == null)
                return null;
            return FormsAuthentication.Decrypt(cookie.Value).Name;
        }
    }
}