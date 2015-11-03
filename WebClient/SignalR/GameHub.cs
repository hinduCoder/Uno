using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Security;
using Microsoft.AspNet.SignalR;
using Uno;
using Uno.Model;
using WebClient.Exceptions;
using WebClient.Models;
using WebGrease.Css.Extensions;
using Player = WebClient.Models.Player;

namespace WebClient.SignalR
{
    public class GameHub : Hub
    {
        private Lobby _lobby;
        private static bool _finished;
        internal Player CurrentPlayer => _lobby.GetPlayerByName(GetCurrentUserName());

        public GameHub(Lobby lobby)
        {
            _lobby = lobby;
        }

        #region Event Handlers

        private void OnCardsAdded(object sender, CardsAddedEventArgs e)
        {
            var name = e.Player.Name;
            ToClientWithName(name).addCards(e.Cards.Select(SerializeCard));
            ToRoomOfPlayerWithName(name).cardsAdded(name, e.Cards.Count);
        }

        private void GameSessionOnPreLastCardDiscarded(object sender, PreLastCardDiscardedEventArgs e)
        {
            ToClientWithName(e.Player.Name).preLastDiscarded();
        }

        private void OnWildCardDiscarded(object sender, WildCardDiscardedEventArgs e)
        {
            ToClientWithName(e.Player.Name).chooseColor();
        }
#endregion
        private void GameSessionOnGameFinished(object sender, GameFinishedEventArgs e)
        {
            var winner = _lobby.GetPlayerByName(e.Winner.Name);
            Clients.Clients(winner.Room.Players.Select(p => p.ConnectionId).ToList())
                .finish(winner.Room.GameSession.Players.OrderBy(p => p.Score).Select(p => new {player = p.Name, score = p.Score}));
            _finished = true;
        }
        private void NewGameStarted(object sender, EventArgs eventArgs)
        {
            NewGame(sender as GameSession);
        }
        private void NewGame(GameSession gameSession)
        {
            ToRoomOfPlayerWithName(gameSession.Players[0].Name).newGame(SerializeCard(gameSession.DiscardPileTop));
            ToClientWithName(gameSession.CurrentPlayer.Name).activate();
        }

        public virtual void Move(int index)
        {
            var gameSession = CurrentPlayer.Room.GameSession;
            try
            {
                var prevPlayer = gameSession.CurrentPlayer;
                gameSession.Discard(index);
                if (_finished)
                {
                    _finished = false;
                    return;
                }
                var topCard = gameSession.DiscardPileTop;
                ToCurrentPlayerRoom().move(SerializeCard(topCard), prevPlayer.Name, gameSession.CurrentPlayer.Name);
                Clients.Caller.discard(index, gameSession.CurrentPlayer.Name);
                ToClientWithName(gameSession.CurrentPlayer.Name).activate();
            }
            catch (WrongCardException e)
            {
                Clients.Caller.wrongCard(index, SerializeCard(e.Card));
            }
        }

        public virtual void Draw()
        {
            var gameSession = CurrentPlayer.Room.GameSession;
            var currentPlayer = gameSession.CurrentPlayer;
            gameSession.Draw();
            Clients.Caller.draw(SerializeCard(currentPlayer.Cards.Last()));
        }

        public virtual void Pass()
        {
            var room = CurrentPlayer.Room;
            var gameSession = room.GameSession;
            gameSession.Pass();
            ToCurrentPlayerRoom().move();
            ToClientWithName(gameSession.CurrentPlayer.Name).activate();
        }

        public void ChooseColor(string color)
        {
            var chosenColor = (CardColor) Enum.Parse(typeof (CardColor), color, ignoreCase: true);
            CurrentPlayer.Room.GameSession.DiscardPileTop.Color = chosenColor;
            ToCurrentPlayerRoom().chosenColor(color);
        }

        public virtual void Uno()
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

        private dynamic ToRoomOfPlayerWithName(string name)
        {
            return Clients.Clients(_lobby.GetPlayerByName(name).Room.Players.Select(p => p.ConnectionId).ToList());
        }

        private object SerializeCard(Card card) //Refactor!
        {
            String type;
            switch (card.Type)
            {
                case CardType.Number:
                    type = null;
                    break;
                case CardType.DrawTwo:
                    type = "plus-two";
                    break;
                case CardType.WildDrawFour:
                    type = "plus-four";
                    break;
                default:
                    type = card.Type.ToString().ToLower();
                    break;
            }
            return new {color = card.Color.ToString().ToLower(), content = card.ToString(), type};
        }

        private string GetCurrentUserName()
        {
            var cookie = Context.RequestCookies["userid"];
            if (cookie == null)
                return null;
            return FormsAuthentication.Decrypt(cookie.Value).Name;
        }
#region Connection/Disconnection
        public override Task OnDisconnected(bool stopCalled)
        {
            return Task.Run(() =>
            {
                var currentUserName = GetCurrentUserName();
                if (currentUserName == null)
                    return;
                var player = _lobby.GetPlayerByName(currentUserName);
                if (player == null)
                    return;
                player.ConnectionId = null;
                var gameSession = player.Room.GameSession;
            });
        }

        public override Task OnConnected()
        {
            return ConnectTask();
        }

        private Task ConnectTask()
        {
            return Task.Run(() =>
            {
                var currentUserName = GetCurrentUserName();
                if (currentUserName == null)
                    return;
                var player = _lobby.GetPlayerByName(currentUserName);
                if (player == null)
                    return;
                player.ConnectionId =
                    Context.ConnectionId;
                var gameSession = player.Room.GameSession;
                gameSession.WildCardDiscarded.Subscribe(OnWildCardDiscarded);
                gameSession.PreLastCardDiscarded.Subscribe(GameSessionOnPreLastCardDiscarded);
                gameSession.GameFinished.Subscribe(GameSessionOnGameFinished);
                gameSession.NewGameStarted.Subscribe(NewGameStarted);
                gameSession.Players.ForEach(p => p.CardsAdded.Subscribe(OnCardsAdded));
                InitializeClient(player);
            });
        }

        private void InitializeClient(Player player)
        {
            var gameSession = player.Room.GameSession;
            var gamePlayer = gameSession.Players.Single(p => p.Name == player.Name);
            Clients.Client(player.ConnectionId)
                .init(new
                {
                    cards = gamePlayer.Cards.Select(SerializeCard),
                    topCard = SerializeCard(gameSession.DiscardPileTop),
                    otherPlayers = gameSession.Players.Except(new [] { gamePlayer }).Select(p => new {name = p.Name, cardsCount = p.Cards.Count}),
                    deck = gameSession.Game.Cards.Count
                });
            ToClientWithName(gameSession.CurrentPlayer.Name).activate();
        }


        public override Task OnReconnected()
        {
            return ConnectTask();
        }
#endregion
    }
}