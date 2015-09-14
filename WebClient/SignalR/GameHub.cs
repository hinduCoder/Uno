using System;
using System.Linq;
using System.Web.Security;
using Microsoft.AspNet.SignalR;
using Uno;
using Uno.Model;
using WebClient.Controllers;
using Player = WebClient.Controllers.Player;

namespace WebClient.SignalR
{
    public class GameHub : Hub
    {
        private Lobby _lobby = Lobby.Instance;
        private Player CurrentPlayer => _lobby.GetPlayerByName(GetCurrentUserName());

        public void Enter(int id)
        {
            var currentUserName = GetCurrentUserName();
            if (currentUserName == null)
                return;
            var player = _lobby.GetPlayerByName(currentUserName);
            player.ConnectionId =
                Context.ConnectionId;
            player.Room.GameSession.WildCardDiscarded += OnWildCardDiscarded;
        }

        private void OnWildCardDiscarded(object sender, WildCardDiscardedEventArgs e)
        {
            Clients.Client(_lobby.GetPlayerByName(e.Player.Name).ConnectionId).chooseColor();
        }

        public void Move(int index)
        {
            var room = CurrentPlayer.Room;
            var gameSession = room.GameSession;
            gameSession.Discard(index);
            var topCard = gameSession.DiscardPileTop;
            ToCurrentPlayerRoom().move(new { color = topCard.Color.ToString().ToLower(), content = topCard.ToString() });
            Clients.Client(_lobby.GetPlayerByName(gameSession.CurrentPlayer.Name).ConnectionId).activate();
        }

        public void Draw()
        {
            var room = CurrentPlayer.Room;
            var gameSession = room.GameSession;
            var currentPlayer = gameSession.CurrentPlayer;
            gameSession.Draw();
            Clients.Caller.draw(SerializeCard(currentPlayer.Cards.Last()));
        }

        public void Pass()
        {
            var room = CurrentPlayer.Room;
            var gameSession = room.GameSession;
            gameSession.Pass();
            ToCurrentPlayerRoom().move();
            Clients.Client(_lobby.GetPlayerByName(gameSession.CurrentPlayer.Name).ConnectionId).activate();
        }

        public void ChooseColor(string color)
        {
            var chosenColor = (CardColor) Enum.Parse(typeof (CardColor), color, ignoreCase: true);
            CurrentPlayer.Room.GameSession.DiscardPileTop.Color = chosenColor;
            ToCurrentPlayerRoom().chosenColor(color);
        }

        private dynamic ToCurrentPlayerRoom()
        {
            return
                Clients.Clients(
                    CurrentPlayer.Room.Players.Except(new[] {CurrentPlayer}).Select(p => p.ConnectionId).ToList());
        }

        private object SerializeCard(Card card)
        {
            return new {color = card.Color.ToString().ToLower(), content = card.ToString()};
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