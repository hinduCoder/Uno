using System.Collections.Generic;

namespace Uno.Model
{
    public class Player
    {
        private List<Card> _cards = new List<Card>();
        public string Name { get; private set; }
        public Player(string name)
        {
            Name = name;
        }

        internal void AddCards(params Card[] cards)
        {
            _cards.AddRange(cards);
        }

        internal void AddCards(IEnumerable<Card> cards)
        {
            _cards.AddRange(cards);
        }

        internal Card Discard(int index)
        {
            var result = _cards[index];
            _cards.RemoveAt(index);
            return result;
        }
        public bool CardsLeft => _cards.Count > 0;
        public IReadOnlyList<Card> Cards
        {
            get { return _cards; }
        }
    }
}