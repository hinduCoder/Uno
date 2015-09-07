using System;
using System.Collections.Generic;
using System.Linq;

namespace Uno.Model
{
    public class Uno
    {
        readonly List<Card> _cards = new List<Card>(108);
        readonly List<Card> _discardPile = new List<Card>();
        public Uno()
        {
            Generate();
            Shuffle();
        }
        public void Start()
        {
            Discard(DrawCard());
        }
        public List<Card> DrawCards(int count)
        {
            var result = _cards.Where((e, i) => i >= _cards.Count - count).ToList();
            _cards.RemoveRange(_cards.Count - count, count);
            return result;
        }
        public Card DrawCard() => DrawCards(1)[0];

        public bool CanDiscard(Card card)
        {
            if (DiscardPileTop == null)
                return true;
            return card.Number == DiscardPileTop.Number
                || card.Type == DiscardPileTop.Type
                || card.Color == DiscardPileTop.Color
                || card.Color == CardColor.Black;
        }
        public event EventHandler<SpecialCardDiscardedEventArgs> SpecialCardDiscarded;

        internal void Discard(Card card)
        {
            if (!CanDiscard(card))
                throw new ArgumentException("This card cannot be discarded");
            _discardPile.Add(card);
            if (card.Type != CardType.Number)
                if (SpecialCardDiscarded != null)
                {
                    var eventArgs = new SpecialCardDiscardedEventArgs(card.Type);
                    SpecialCardDiscarded(this, eventArgs);
                    if (DiscardPileTop.Color == CardColor.Black)
                        DiscardPileTop.Color = eventArgs.ChoosenColor.Value;
                }
        }
        private void Generate()
        {
            foreach (CardColor color in Enum.GetValues(typeof(CardColor)))
            {
                if (color == CardColor.Black)
                    continue;
                for (int number = 0; number <= 9; number++)
                {
                    for (int i = 0; i < (number == 0 ? 1 : 2); i++)
                    {
                        _cards.Add(new Card(CardType.Number, color, number));
                    }
                }
                for (int i = 0; i < 2; i++)
                {
                    _cards.Add(new Card(CardType.DrawTwo, color));
                    _cards.Add(new Card(CardType.Reverse, color));
                    _cards.Add(new Card(CardType.Skip, color));
                }

            }
            for (int i = 0; i < 4; i++)
            {
                _cards.Add(new Card(CardType.Wild));
                _cards.Add(new Card(CardType.WildDrawFour));
            }

        }
        private void Shuffle()
        {
            var random = new Random();
            for (int i = _cards.Count - 1; i >= 1; i--)
            {
                var j = random.Next(i + 1);
                var temp = _cards[j];
                _cards[j] = _cards[i];
                _cards[i] = temp;
            }
        }

        public IReadOnlyList<Card> Cards => _cards;
        public Card DiscardPileTop => _discardPile.Count == 0 ? null : _discardPile[_discardPile.Count - 1];
    }
}