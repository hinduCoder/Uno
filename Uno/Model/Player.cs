using System;
using System.Collections.Generic;
using System.Linq;

namespace Uno.Model
{
    public class Player
    {
        private readonly List<Card> _cards = new List<Card>();
        public string Name { get; }
        private int _score;

        public readonly OneSubscriberEvent<CardsAddedEventArgs> CardsAdded = new OneSubscriberEvent<CardsAddedEventArgs>();

        public Player(string name)
        {
            Name = name;
        }

        internal void AddCards(params Card[] cards)
        {
            _cards.AddRange(cards);
            CardsAdded.Invoke(this, new CardsAddedEventArgs(this, cards));
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

        internal void Reset()
        {
            _cards.Clear();
        }

        internal void CalculateScore()
        {
            _score += _cards.Sum(c => c.Score);
        }

        public int Score => _score;
        public bool CardsLeft => _cards.Count > 0;
        public IReadOnlyList<Card> Cards => _cards;
        public override string ToString()
        {
            return Name;
        }
    }
}