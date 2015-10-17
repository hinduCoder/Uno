using System.Collections.Generic;
using System.Linq;
using Uno.Log;
using Uno.Model;
using WebClient.Exceptions;
namespace Uno
{
    public class GameSession
    {
        List<Player> _players = new List<Player>();
        Model.Uno _uno = new Model.Uno();
        int _currentPlayerIndex;
        bool _reverse;
        bool _unoSaid;
        Log.Log _log = new Log.Log();
#region Events

        public readonly OneSubscriberEvent<WildCardDiscardedEventArgs> WildCardDiscarded = new OneSubscriberEvent<WildCardDiscardedEventArgs>();
        public readonly OneSubscriberEvent<GameFinishedEventArgs> GameFinished = new OneSubscriberEvent<GameFinishedEventArgs>();
        public readonly OneSubscriberEvent<PreLastCardDiscardedEventArgs> PreLastCardDiscarded = new OneSubscriberEvent<PreLastCardDiscardedEventArgs>();
        public readonly OneSubscriberEvent NewGameStarted = new OneSubscriberEvent();
#endregion
        public GameSession(params string[] players)
        {
            _players.AddRange(players.Select(p => new Player(p)));
            _uno.SpecialCardDiscarded += SpecialCardDiscardedHandler;
        }
        private void SpecialCardDiscardedHandler(object sender, SpecialCardDiscardedEventArgs e)
        {
            CardColor chosenColor = CardColor.Black;
            if (e.SpecialCardType == CardType.Wild || e.SpecialCardType == CardType.WildDrawFour)
            {
                var wildCardDiscardedEventArgs = new WildCardDiscardedEventArgs(CurrentPlayer);
                WildCardDiscarded.Invoke(this, wildCardDiscardedEventArgs);
                chosenColor = wildCardDiscardedEventArgs.Color;
            }
            switch (e.SpecialCardType)
            {
                case CardType.Reverse: _reverse = !_reverse; break;
                case CardType.DrawTwo:
                {
                    NextPlayer();
                    CurrentPlayer.AddCards(_uno.DrawCards(2));
                    _log.AddEntry(new PlayerDrawnCardsEntry(CurrentPlayer, 2));
                    break;
                }
                case CardType.Skip: NextPlayer(); break;
                case CardType.Wild: e.ChoosenColor = chosenColor; break;
                case CardType.WildDrawFour:
                {
                    e.ChoosenColor = chosenColor;
                    NextPlayer();
                    CurrentPlayer.AddCards(_uno.DrawCards(4));
                    _log.AddEntry(new PlayerDrawnCardsEntry(CurrentPlayer, 4));
                    break;
                }
            }
        }
        public void Deal()
        {
            foreach (var player in _players)
            {
                player.AddCards(_uno.DrawCards(7).ToArray());
            }
            _uno.Start();
        }
        public void Discard(int index)
        {
            Card cardToDiscard = null;
            try
            {
                cardToDiscard = CurrentPlayer.Discard(index);
                _uno.Discard(cardToDiscard);
            }
            catch (WrongCardException)
            {
                CurrentPlayer.UndoDiscard(index, cardToDiscard);
                throw;
            }
            if (CurrentPlayer.Cards.Count == 2)
                PreLastCardDiscarded.Invoke(this, new PreLastCardDiscardedEventArgs(CurrentPlayer));
            _log.AddEntry(new PlayerMovedEntry(CurrentPlayer, cardToDiscard));
            NextPlayer();
            _unoSaid = false;
        }

        public void Pass()
        {
            NextPlayer();
        }
        public void Uno()
        {
            _unoSaid = true;
        }
        public void Draw()
        {
            CurrentPlayer.AddCards(_uno.DrawCard());
            _log.AddEntry(new PlayerDrawnCardsEntry(CurrentPlayer));
        }

        private void NextPlayer()
        {
            if (!CurrentPlayer.CardsLeft)
            {
                FinishGame();
                return;
            }
            if (CurrentPlayer.Cards.Count == 1 && !_unoSaid)
            {
                CurrentPlayer.AddCards(_uno.DrawCards(2));
                _log.AddEntry(new PlayerDrawnCardsEntry(CurrentPlayer, 2));
            }

            _currentPlayerIndex += _reverse ? -1 : 1;
            if (_currentPlayerIndex < 0)
                _currentPlayerIndex += _players.Count;
            _currentPlayerIndex %= _players.Count;
        }

        private void FinishGame()
        {
            _players.ForEach(p => p.CalculateScore());
            GameFinished.Invoke(this, new GameFinishedEventArgs(CurrentPlayer));
            _uno.Reset();
            _players.ForEach(p => p.Reset());
            Deal();
            NewGameStarted.Invoke(this);
        }

        #region Properties

        public Player CurrentPlayer => _players[_currentPlayerIndex];
        public Model.Uno Game => _uno;
        public IReadOnlyList<Player> Players => _players;
        public Card DiscardPileTop => Game.DiscardPileTop;
        public Log.Log Log => _log;

        #endregion

    }  
}