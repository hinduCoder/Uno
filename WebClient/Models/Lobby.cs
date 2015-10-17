using System.Collections.Generic;
using System.Linq;

namespace WebClient.Models
{
    public interface ILobby
    {
        void AddRoom(int playersCount = 2);
        void AddPlayer(Player player);
        List<Room> Rooms { get; }
        List<Player> AllPlayers { get; }
        void RemovePlayer(string name);
        Player GetPlayerByName(string name);
    }

    public class Lobby : ILobby
    {
        private List<Room> _rooms = new List<Room> {new Room()}; 
        private List<Player> _allPlayers = new List<Player>();
        public Lobby()
        {
        }

        public void AddRoom(int playersCount = 2)
        {
            _rooms.Add(new Room { MaxPlayersCount = playersCount});
        }

        public void AddPlayer(Player player)
        {
            if (_allPlayers.All(p => p.Name != player.Name))
                _allPlayers.Add(player);
        }

        public List<Room> Rooms => _rooms;
        public List<Player> AllPlayers => _allPlayers;

        public void RemovePlayer(string name)
        {
            var player = AllPlayers.Find(p => p.Name == name);
            if (player == null)
                return;
            
            AllPlayers.Remove(player);
            _rooms.ForEach(r => r.RemovePlayer(player));
        }

        public Player GetPlayerByName(string name)
        {
            return AllPlayers.SingleOrDefault(p => p.Name == name);
        }
    }
}