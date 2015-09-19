using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Security;
using Microsoft.AspNet.SignalR;
using Uno;
using Uno.Model;
using WebClient.Controllers;
using WebClient.Exceptions;
using WebGrease.Css.Extensions;
using Player = WebClient.Controllers.Player;

namespace WebClient.SignalR
{
    public class GameHub : Hub
    {
        private Lobby _lobby = Lobby.Instance;
        private Player CurrentPlayer => _lobby.GetPlayerByName(GetCurrentUserName());

        private void OnCardsAdded(object sender, CardsAddedEventArgs e)
        {
            ToClientWithName(e.Player.Name).addCards(e.Cards.Select(SerializeCard));
        }

        private void GameSessionOnPreLastCardDiscarded(object sender, PreLastCardDiscardedEventArgs e)
        {
            ToClientWithName(e.Player.Name).preLastDiscarded();
        }

        private void OnWildCardDiscarded(object sender, WildCardDiscardedEventArgs e)
        {
            ToClientWithName(e.Player.Name).chooseColor();
        }

        public void Move(int index)
        {
            var room = CurrentPlayer.Room;
            var gameSession = room.GameSession;
            try
            {
                gameSession.Discard(index);
                var topCard = gameSession.DiscardPileTop;
                ToCurrentPlayerRoom().move(new { color = topCard.Color.ToString().ToLower(), content = topCard.ToString() });
                Clients.Caller.discard(index);
                Clients.Client(_lobby.GetPlayerByName(gameSession.CurrentPlayer.Name).ConnectionId).activate();
            }
            catch (WrongCardException e)
            {
                Clients.Caller.wrongCard(index, SerializeCard(e.Card));
            }
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

        public void Uno()
        {
            CurrentPlayer.Room.GameSession.Uno();
        }

        private dynamic ToCurrentPlayerRoom()
        {
            return
                Clients.Clients(
                    CurrentPlayer.Room.Players.Except(new[] {CurrentPlayer}).Select(p => p.ConnectionId).ToList());
        }

        private dynamic ToClientWithName(string name)
        {
            return Clients.Client(_lobby.GetPlayerByName(name).ConnectionId);
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

        public override Task OnDisconnected(bool stopCalled)
        {
            return Task.Run(() =>
            {
                var currentUserName = GetCurrentUserName();
                if (currentUserName == null)
                    return;
                var player = _lobby.GetPlayerByName(currentUserName);
                player.ConnectionId = null;
                var gameSession = player.Room.GameSession;
                gameSession.WildCardDiscarded -= OnWildCardDiscarded;
                gameSession.PreLastCardDiscarded -= GameSessionOnPreLastCardDiscarded;
                gameSession.Players.ForEach(p => p.CardsAdded -= OnCardsAdded);
            });
        }

        public override Task OnConnected()
        {
            return Task.Run(() =>
            {
                var currentUserName = GetCurrentUserName();
                if (currentUserName == null)
                    return;
                var player = _lobby.GetPlayerByName(currentUserName);
                player.ConnectionId =
                    Context.ConnectionId;
                var gameSession = player.Room.GameSession;
                gameSession.WildCardDiscarded += OnWildCardDiscarded;
                gameSession.PreLastCardDiscarded += GameSessionOnPreLastCardDiscarded;
                gameSession.Players.ForEach(p => p.CardsAdded += OnCardsAdded);
            });
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }
    }
}