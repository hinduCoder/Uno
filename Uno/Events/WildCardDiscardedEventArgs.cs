using System;
using Uno.Model;

namespace Uno
{
    public class WildCardDiscardedEventArgs : EventArgs
    {
        public CardColor Color { get; set; }
    }
}