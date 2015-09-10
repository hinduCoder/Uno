using System;
using System.Collections.Generic;
using System.Linq;
using Uno;
using WebClient.Controllers;

namespace WebClient
{
    public class Room
    {
        readonly List<Player> _players = new List<Player>();
        
        public void AddPlayer(Player player)
        {
            _players.Add(player);
            player.Room = this;
            if (_players.Count == PlayerCount)
                CanStart = true;
        }

        public void RemovePlayer(Player player)
        {
            _players.Remove(player);
            player.Room = null;
            if (_players.Count < PlayerCount)
                CanStart = false;
        }

        public GameSession CreateGameSession()
        {
            if (GameSession == null)
            {
                GameSession = new GameSession(_players.Select(p => p.Name).ToArray());
                GameSession.Deal();
            }
            return GameSession;
        }

        public IReadOnlyList<Player> Players => _players;

        public bool CanStart { get; private set; }

        public int PlayerCount { get; set; } = 2;

        public GameSession GameSession { get; set; }
    }
}