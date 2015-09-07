using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Uno;

namespace WebClient.Controllers
{
    public class GameController : Controller
    {
        // GET: Game
        public ActionResult Index(int id)
        {
            var gameSession = Lobby.Instance.Rooms[id].CreateGameSession();
            ViewBag.Player = gameSession.Players.Single(p => p.Name == Request.Cookies["name"].Value);
            return View(gameSession);
        }
    }
}