using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Security;
using Uno;
using Uno.Model;

namespace WebClient.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var cookie = Request.Cookies["name"];
            if (cookie != null)
            {
                Controllers.Game.AddPlayer(cookie.Value);
                return Redirect("/Room");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Register(RegisterViewModel register)
        {
            Controllers.Game.AddPlayer(register.Name);
            Response.SetCookie(new HttpCookie("name", register.Name) { Expires = DateTime.Now.AddMinutes(30) });
            return Redirect("/Room");
        }

        //public ActionResult Game()
        //{ 
        //    ViewBag.Game = Controllers.Game.Session;
        //    return View("Index1", Controllers.Game.Session?.Players.SingleOrDefault(p => p.Name == Request.Cookies["name"]?.Value));
        //}

        //public ActionResult Move(int? index)
        //{
        //    Controllers.Game.Session.Discard(index.Value);
        //    return Redirect("/");
        //}
    }
}