using System;
using System.Collections.Generic;

namespace Uno.Model
{
    public class CardsAddedEventArgs : EventArgs
    {
        public CardsAddedEventArgs(Player player, params Card[] cards)
        {
            Player = player;
            Cards = cards;
        }

        public IReadOnlyList<Card> Cards { get; set; }

        public Player Player { get; private set; }
    }
}