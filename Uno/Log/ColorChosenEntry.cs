using Uno.Model;

namespace Uno.Log
{
    public class ColorChosenEntry : ILogEntry
    {
        private readonly Player _player;
        private readonly CardColor _color;
        public string Description => $"Player {_player} have chosen {_color} color";

        public Player Player => _player;
        public CardColor Color => _color;

        public ColorChosenEntry(Player player, CardColor color)
        {
            _player = player;
            _color = color;
        }
    }
}