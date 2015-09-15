using System;
using Uno.Model;

namespace Uno
{
    public class PreLastCardDiscardedEventArgs : EventArgs
    {
        public PreLastCardDiscardedEventArgs(Player player)
        {
            Player = player;
        }

        public Player Player { get; private set; }
    }
}