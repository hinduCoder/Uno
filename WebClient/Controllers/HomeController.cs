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
using Uno = Uno.Model.Uno;

namespace WebClient.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var cookie = Request.Cookies["userid"];
            if (cookie != null)
            {
                return Redirect("/Room");
            }
            return View();

        }
        [HttpPost]
        public ActionResult LogIn(LoginViewModel login)
        {
            using (var unoDb = new UnoDb())
            {
                var user = unoDb.User.SingleOrDefault(u => u.Username == login.Username);

                if (user == null)
                    return HttpNotFound("No such user"); // TODO: TEMP
                if (user.Password != login.Password)
                    return HttpNotFound("Password wrong"); //TODO: hash

                Response.SetCookie(new HttpCookie("userid", user.Id.ToString()) {Expires = DateTime.Now.AddMinutes(10)});
            }
            return Redirect("/Room");
        }
        
    }
}