using System.Linq;
using System.Web.Security;
using Microsoft.AspNet.SignalR;
using Uno.Model;
using WebClient.Controllers;

namespace WebClient.SignalR
{
    public class GameHub : Hub
    {
        private Lobby _lobby = Lobby.Instance;

        public void Enter(int id)
        {
            var currentUserName = GetCurrentUserName();
            if (currentUserName == null)
                return;
            _lobby.GetPlayerByName(currentUserName).ConnectionId =
                Context.ConnectionId;
            
        }

        public void Move(int index)
        {
            var lobby = _lobby;
            var currentPlayer = lobby.GetPlayerByName(GetCurrentUserName());
            var room = currentPlayer.Room;
            var gameSession = room.GameSession;
            gameSession.Discard(index);
            var topCard = gameSession.Game.DiscardPileTop;
            Clients.AllExcept(currentPlayer.ConnectionId).move(SerializeCard(topCard));
        }

        private string SerializeCard(Card card)
        {
            return $"{(card.Type == CardType.Number ? card.Number.ToString() : card.Type.ToString())} {card.Color}";
        }

        public void Draw()
        {
            var lobby = _lobby;
            var currentUser = lobby.GetPlayerByName(GetCurrentUserName());
            var room = currentUser.Room;
            var gameSession = room.GameSession;
            var currentPlayer = gameSession.CurrentPlayer;
            gameSession.Draw();
            Clients.Caller.draw(SerializeCard(currentPlayer.Cards.Last()));
        }


        public void Pass()
        {
            var lobby = _lobby;
            var currentPlayer = lobby.GetPlayerByName(GetCurrentUserName());
            var room = currentPlayer.Room;
            var gameSession = room.GameSession;
            gameSession.Pass();
            Clients.AllExcept(currentPlayer.ConnectionId).move();
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