using System;

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
            return $"{(Type == CardType.Number ? Number.ToString() : Type.ToString())} {(Color != CardColor.Black ? Color.ToString() : "")}";
        }
    }
}