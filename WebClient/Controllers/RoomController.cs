using System;
using System.Collections;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.WebSockets;
using Newtonsoft.Json;
using WebClient.Models;
using WebGrease.Css.Extensions;

namespace WebClient.Controllers
{
    public class RoomController : Controller
    {
        // GET: Room
        public ActionResult Index(Lobby lobby)
        {
            var cookie = Request.Cookies["userid"];
            if (cookie == null)
                return Redirect("/");
            ViewBag.RoomContainer = lobby;
            var username = FormsAuthentication.Decrypt(cookie.Value).Name;
            var currentUser = lobby.GetPlayerByName(username);
            var room = currentUser?.Room;
            ViewBag.CanJoin = room == null || !room.IsFull;
            ViewBag.CurrentUser = currentUser;
            return View();
        }

        public ActionResult Start()
        {
            return RedirectToRoute(new {controller = "Game"});
        }
    }

}