using System.Diagnostics;
using System.Linq;
using System.Net.Cache;
using System.Threading.Tasks;
using System.Web.Security;
using dotless.Core.Parser.Infrastructure;
using Microsoft.AspNet.SignalR;
using WebClient.Controllers;

namespace WebClient.SignalR
{
    public class LobbyHub : Hub
    {
        public void AddPlayerToRoom(int roomIndex)
        {
            var lobby = Lobby.Instance;
            var playerName = GetCurrentUserName();
            var player = lobby.AllPlayers.Find(p => p.Name == playerName);
            Groups.Add(player.ConnectionId, $"Room{roomIndex}");
            var room = lobby.Rooms[roomIndex];
            room.AddPlayer(player);
            
            Clients.All.addPlayerToRoom(roomIndex, playerName);
            if (room.CanStart)
                Clients.Group($"Room{roomIndex}").allowStart(roomIndex);
            //Clients.Clients(room.Players.Select(p => p.ConnectionId).ToList()).allowStart(roomIndex);
        }

        public override async Task OnConnected()
        {
            await Task.Run(() =>
            {
                var newPlayerName = GetCurrentUserName();
                if (newPlayerName == null)
                    return;
                var newPlayer = new Player(newPlayerName) {ConnectionId = Context.ConnectionId};
                var lobby = Lobby.Instance;
                lobby.AddPlayer(newPlayer);
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