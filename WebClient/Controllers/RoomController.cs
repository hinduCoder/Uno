using System;
using System.Collections;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
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
            if (Request.Cookies["userid"] == null)
                return Redirect("/");
            ViewBag.RoomContainer = Lobby.Instance;
            ViewBag.CanJoin = !Lobby.Instance.Rooms.Any(r => r.Players.Any(p => p.Name == Request.Cookies["name"]?.Value));
            return View();
        }

        public ActionResult Start(int id)
        {
            return Redirect($"/Game/Index/{id}");
        }
    }

}