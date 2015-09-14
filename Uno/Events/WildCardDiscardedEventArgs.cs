using System;
using Uno.Model;

namespace Uno
{
    public class WildCardDiscardedEventArgs : EventArgs
    {
        public WildCardDiscardedEventArgs(Player player)
        {
            Player = player;
        }

        public Player Player { get; private set; }

        public CardColor Color { get; set; }
    }
}