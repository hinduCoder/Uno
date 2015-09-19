using System;
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
        private EventHandler<WildCardDiscardedEventArgs> _wildCardEvent;
        public event EventHandler<WildCardDiscardedEventArgs> WildCardDiscarded
        {
            add
            {
                if (_wildCardEvent == null)
                    _wildCardEvent = value;
            }
            remove { _wildCardEvent = null; }
        }
        EventHandler<GameFinishedEventArgs> _gameFinishedEvent;
        public event EventHandler<GameFinishedEventArgs> GameFinished
        {
            add
            {
                if (_gameFinishedEvent == null)
                    _gameFinishedEvent = value;
            }
            remove { _gameFinishedEvent = null; }
        }

        private EventHandler<PreLastCardDiscardedEventArgs> _preLastCardDiscarded;
        public event EventHandler<PreLastCardDiscardedEventArgs> PreLastCardDiscarded
        {
            add
            {
                if (_preLastCardDiscarded == null)
                    _preLastCardDiscarded = value;
            }
            remove { _preLastCardDiscarded = null; }
        }
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
                _wildCardEvent?.Invoke(this, wildCardDiscardedEventArgs);
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
            catch (WrongCardException e)
            {
                CurrentPlayer.UndoDiscard(index, cardToDiscard);
                throw;
            }
            if (CurrentPlayer.Cards.Count == 2)
                _preLastCardDiscarded?.Invoke(this, new PreLastCardDiscardedEventArgs(CurrentPlayer));
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
        public void NextPlayer()
        {
            if (!CurrentPlayer.CardsLeft)
            {
                _gameFinishedEvent?.Invoke(this, new GameFinishedEventArgs(CurrentPlayer));
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
        public Player CurrentPlayer => _players[_currentPlayerIndex];
        public Model.Uno Game => _uno;

        public IReadOnlyList<Player> Players => _players;
        public Card DiscardPileTop => Game.DiscardPileTop;
        public Log.Log Log => _log;
    }  
}