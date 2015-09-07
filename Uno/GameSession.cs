using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Model;

namespace Uno
{
    public class GameSession
    {
        List<Player> _players = new List<Player>();
        Model.Uno _uno = new Model.Uno();
        int _currentPlayerIndex;
        bool _reverse;
        bool _unoSaid;

        public event EventHandler<WildCardDiscardedEventArgs> WildCardDiscarded;
        public event EventHandler<GameFinishedEventArgs> GameFinished;
        
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
                var wildCardDiscardedEventArgs = new WildCardDiscardedEventArgs();
                WildCardDiscarded?.Invoke(this, wildCardDiscardedEventArgs);
                chosenColor = wildCardDiscardedEventArgs.Color;
            }
            switch (e.SpecialCardType)
            {
                case CardType.Reverse: _reverse = !_reverse; break;
                case CardType.DrawTwo:
                {
                    NextPlayer();
                    CurrentPlayer.AddCards(_uno.DrawCards(2));
                    break;
                }
                case CardType.Skip: NextPlayer(); break;
                case CardType.Wild: e.ChoosenColor = chosenColor; break;
                case CardType.WildDrawFour:
                {
                    e.ChoosenColor = chosenColor;
                    NextPlayer();
                    CurrentPlayer.AddCards(_uno.DrawCards(4));
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
            _uno.Discard(CurrentPlayer.Discard(index));
            NextPlayer();
            _unoSaid = false;
        }

        public void Discard(Card card)
        {
            Discard(CurrentPlayer.Cards.Single(c => c == card));
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
        }
        public void NextPlayer()
        {
            if (!CurrentPlayer.CardsLeft)
            {
                GameFinished?.Invoke(this, new GameFinishedEventArgs(CurrentPlayer));
                return;
            }
            if (CurrentPlayer.Cards.Count == 1 && !_unoSaid)
                CurrentPlayer.AddCards(_uno.DrawCards(2));
            
            _currentPlayerIndex += _reverse ? -1 : 1;
            if (_currentPlayerIndex < 0)
                _currentPlayerIndex += _players.Count;
            _currentPlayerIndex %= _players.Count;
        }
        public Player CurrentPlayer => _players[_currentPlayerIndex];
        public Model.Uno Game => _uno;

        public IReadOnlyList<Player> Players => _players;
    }
}