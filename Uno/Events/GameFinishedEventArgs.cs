using System;
using Uno.Model;

namespace Uno
{
    public class GameFinishedEventArgs : EventArgs
    {
        public Player Winner { get; private set; }
        public GameFinishedEventArgs(Player winner)
        {
            Winner = winner;
        }
    }
}