using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Security;
using PasswordHash;
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
                var hashedPasswordString = Encrypt.SHA1(login.Password);
                if (!user.Password.Equals(hashedPasswordString, StringComparison.OrdinalIgnoreCase))
                    return HttpNotFound("Password wrong");

                Response.SetCookie(new HttpCookie("userid", user.Id.ToString()) {Expires = DateTime.Now.AddMinutes(10)});
            }
            return Redirect("/Room");
        }
        
    }
}