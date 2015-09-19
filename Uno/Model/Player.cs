using System;
using System.Collections.Generic;
using System.Linq;

namespace Uno.Model
{
    public class Player
    {
        private List<Card> _cards = new List<Card>();
        public string Name { get; private set; }
        private EventHandler<CardsAddedEventArgs> _cardsAdded;
        public event EventHandler<CardsAddedEventArgs> CardsAdded
        {
            add
            {
                if (_cardsAdded == null)
                    _cardsAdded = value;
            }
            remove { _cardsAdded = null; }
        }

        public Player(string name)
        {
            Name = name;
        }

        internal void AddCards(params Card[] cards)
        {
            _cards.AddRange(cards);
            _cardsAdded?.Invoke(this, new CardsAddedEventArgs(this, cards));
        }

        internal void AddCards(IEnumerable<Card> cards)
        {
            AddCards(cards.ToArray());
        }

        internal Card Discard(int index)
        {
            var result = _cards[index];
            _cards.RemoveAt(index);
            return result;
        }

        internal void UndoDiscard(int index, Card card)
        {
            _cards.Insert(index, card);
        }

        public int Score => _cards.Sum(c => c.Score);

        public bool CardsLeft => _cards.Count > 0;

        public IReadOnlyList<Card> Cards => _cards;

        public override string ToString()
        {
            return Name;
        }
    }
}