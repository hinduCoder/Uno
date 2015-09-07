using System.Collections.Generic;
using System.Linq;

namespace WebClient.Controllers
{
    public class Lobby
    {
        private static Lobby _instance;
        public static Lobby Instance => _instance ?? (_instance = new Lobby());
        private List<Room> _rooms = new List<Room> {new Room()}; 
        private List<Player> _allPlayers = new List<Player>();
        private Lobby()
        {
        }

        public void AddRooms()
        {
            _rooms.Add(new Room());
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
    }
}