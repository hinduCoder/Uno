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
using WebGrease.Css.Extensions;

namespace WebClient.Controllers
{
    public class RoomController : Controller
    {
        // GET: Room
        public ActionResult Index()
        {
            var cookie = Request.Cookies["userid"];
            if (cookie == null)
                return Redirect("/");
            var lobby = Lobby.Instance;
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
            //return Redirect($"/Game/Index/{id}");
            return Redirect($"/Game/");
        }
    }

}