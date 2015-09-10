using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace WebClient.Controllers
{
    public class GameController : Controller
    {
        // GET: Game
        public ActionResult Index(int id)
        {
            var cookie = Request.Cookies["userid"];
            if (cookie == null)
                return null;
            var userName = FormsAuthentication.Decrypt(cookie.Value).Name;
            var lobby = Lobby.Instance;
            var currentPlayer = lobby.GetPlayerByName(userName);
            var room = currentPlayer.Room;
            var gameSession = room.CreateGameSession();
            ViewBag.Player = gameSession.Players.Single(p => p.Name == userName);
            return View(gameSession);
        }
    }
}