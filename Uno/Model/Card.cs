using System;

namespace Uno.Model
{
    public class Card
    {
        public CardType Type { get; private set; }
        public CardColor Color { get;  set; }
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
            return Type == CardType.Number ? Number.ToString() : String.Empty;
        }
        
        public int Score
        {
            get
            {
                switch (Type)
                {
                    
                    case CardType.Reverse:
                    case CardType.Skip:
                    case CardType.DrawTwo:
                        return 20;
                    case CardType.Wild:
                    case CardType.WildDrawFour:
                        return 50;
                    default:
                        return Number;
                }
            }
        }
    }
}