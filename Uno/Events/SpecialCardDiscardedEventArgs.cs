using System;
using Uno.Model;

namespace Uno
{
    public class SpecialCardDiscardedEventArgs : EventArgs
    {
        public CardColor? ChoosenColor { get; set; }
        public CardType SpecialCardType { get; private set; }
        public SpecialCardDiscardedEventArgs(CardType type)
        {
            if (type == CardType.Number)
                throw new ArgumentException("Illegial card type");
            SpecialCardType = type;
        }
    }
}