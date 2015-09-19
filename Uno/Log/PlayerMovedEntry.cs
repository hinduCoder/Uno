using Uno.Model;

namespace Uno.Log
{
    public class PlayerMovedEntry : ILogEntry
    {
        private readonly Player _player;
        private readonly Card _card;

        public string Description => $"Player {_player} have moved with card {_card} {_card.Color}";
        public Player Player => _player;
        public Card Card => _card;

        internal PlayerMovedEntry(Player player, Card card)
        {
            _player = player;
            _card = card;
        }
    }
}