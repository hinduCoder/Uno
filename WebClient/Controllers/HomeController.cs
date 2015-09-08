using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PasswordHash;

namespace WebClient.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var cookie = Request.Cookies["userid"];
            if (cookie != null && (!FormsAuthentication.Decrypt(cookie.Value)?.Expired ?? false))
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
                var user = unoDb.Users.SingleOrDefault(u => u.Username == login.Username);
                if (user == null)
                    return HttpNotFound("No such user"); // TODO: TEMP
                var hashedPasswordString = Encrypt.SHA1(login.Password);
                if (!user.Password.Equals(hashedPasswordString, StringComparison.OrdinalIgnoreCase))
                    return HttpNotFound("Password wrong");

                var ticket = new FormsAuthenticationTicket(login.Username, true, 30);
                var encryptedTicket = FormsAuthentication.Encrypt(ticket);
                Response.SetCookie(new HttpCookie("userid", encryptedTicket) {Expires = DateTime.Now.AddMinutes(30)});
            }
            return Redirect("/Room");
        }

        public ActionResult Register()
        {
            return View("Register");
        }
        [HttpPost]
        public ActionResult SignUp(RegisterViewModel register)
        {
            using (var unoDb = new UnoDb())
            {
                unoDb.Users.Add(new User
                {
                    Email = register.Email,
                    Password = Encrypt.SHA1(register.Password),
                    Username = register.Username
                });
                unoDb.SaveChanges();
            }
            return Redirect("/");
        }
    }
}