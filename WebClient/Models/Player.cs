using System.IO;
using System.Net.WebSockets;
using System.Security.Principal;

namespace WebClient.Controllers
{
    public class Player 
    {
        public string Name { get; set; }
        public string ConnectionId { get; set; }
        public Room Room { get; set; }
        public Player()
        {
        }

        public Player(string name)
        {
            Name = name;
        }

       
    }

}