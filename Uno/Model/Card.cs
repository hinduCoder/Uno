using System;
using System.Linq;

namespace Uno.Model
{
    public class Card
    {
        public CardType Type { get; private set; }
        public CardColor Color { get; internal set; }
        public int Number { get; private set; }

        public Card()
        {
        }

        public Card(CardType type, CardColor color = CardColor.Black, int number = -1)
        {
            Type = type;
            Color = color;
            Number = number;
        }

        public override string ToString()
        {
            var s = String.Empty;
            switch (Type)
            {
                case CardType.Reverse:
                    s = "\x21d7\x21d9";
                    break;
                case CardType.Skip:
                    s = "\x00d8";
                    break;
                case CardType.DrawTwo:
                    s = "+2";
                    break;
                case CardType.Wild:
                    s = "\x2295";
                    break;
                case CardType.WildDrawFour:
                    s = "+4";
                    break;
            }

            return $"{(Type == CardType.Number ? Number.ToString() : s)}";
        }
    }
}