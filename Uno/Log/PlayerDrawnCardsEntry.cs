using Uno.Model;

namespace Uno.Log
{
    public class PlayerDrawnCardsEntry : ILogEntry
    {
        private readonly Player _player;
        private readonly int _count;
        public string Description => $"Player {_player} have drawn {_count} cards";

        public Player Player => _player;
        public int Count => _count;

        public PlayerDrawnCardsEntry(Player player, int count = 1)
        {
            _player = player;
            _count = count;
        }
    }
}