using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using WebClient.Models;

namespace WebClient.Controllers
{
    public class GameController : Controller
    {
        // GET: Game
        public ActionResult Index(Lobby lobby)
        {
            var cookie = Request.Cookies["userid"];
            if (cookie == null)
                return Redirect("/");
            var userName = FormsAuthentication.Decrypt(cookie.Value).Name;
            var currentPlayer = lobby.GetPlayerByName(userName);
            var room = currentPlayer?.Room;
            if (room == null)
                return RedirectToRoute(new {controller="Home"});
            var gameSession = room.CreateGameSession();
            ViewBag.Player =  gameSession.Players.Single(p => p.Name == userName);

            Response.CacheControl = "no-cache";
            return View(gameSession);
        }
    }
}