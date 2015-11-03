using System;
using System.Collections.Generic;
using System.Linq;
using WebClient.Exceptions;

namespace Uno.Model
{
    public class Uno
    {
        readonly List<Card> _cards = new List<Card>(108);
        readonly List<Card> _discardPile = new List<Card>();
        readonly Random _random = new Random();

        public Uno()
        {
            Reset();
        }

        internal void Reset()
        {
            _discardPile.Clear();
            _cards.Clear();
            Generate();
            Shuffle();
        }

        internal void Start()
        {
            if (_cards[0].Type == CardType.WildDrawFour)
            {
                var card = _cards[0];
                _cards.RemoveAt(0);
                _cards.Insert(_random.Next(1, _cards.Count), card);
            }
            Discard(DrawCard());
        }

        internal List<Card> DrawCards(int count)
        {
            List<Card> result;
            if (_cards.Count < count)
            {
                result = _cards.ToList();
                _cards.Clear();
                _cards.AddRange(_discardPile);
                foreach (var card in _cards)
                {
                    if (card.Type == CardType.Wild || card.Type == CardType.WildDrawFour)
                        card.Color = CardColor.Black;
                } //Temp fix
                Shuffle();
            }
            else
            {
                result = _cards.Take(count).ToList();
                _cards.RemoveRange(0, count);
            }
            return result;
        }

        internal Card DrawCard() => DrawCards(1)[0];

        public bool CanDiscard(Card card)
        {
            if (DiscardPileTop == null)
                return true;
            return card.Number == DiscardPileTop.Number
                || card.Type != CardType.Number && card.Type == DiscardPileTop.Type
                || card.Color == DiscardPileTop.Color
                || card.Color == CardColor.Black;
        }
        public event EventHandler<SpecialCardDiscardedEventArgs> SpecialCardDiscarded;

        internal void Discard(Card card)
        {
            if (!CanDiscard(card))
                throw new WrongCardException() { Card = card };
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
            for (int i = _cards.Count - 1; i >= 1; i--)
            {
                var j = _random.Next(i + 1);
                var temp = _cards[j];
                _cards[j] = _cards[i];
                _cards[i] = temp;
            }
        }

        public IReadOnlyList<Card> Cards => _cards;
        internal Card DiscardPileTop => _discardPile.Count == 0 ? null : _discardPile[_discardPile.Count - 1];
    }
}