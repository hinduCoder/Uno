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

        public string Id { get; private set; }

        public Room()
        {
            Id = DateTime.Now.Ticks.ToString();
        }

        public void AddPlayer(Player player)
        {
            _players.Add(player);
            player.Room = this;
            if (_players.Count == MaxPlayersCount)
                CanStart = true;
        }

        public void RemovePlayer(Player player)
        {
            _players.Remove(player);
            player.Room = null;
            if (_players.Count < MaxPlayersCount)
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

        public int MaxPlayersCount { get; set; } = 2;

        public bool IsFull => _players.Count == MaxPlayersCount;

        public GameSession GameSession { get; set; }
    }
}