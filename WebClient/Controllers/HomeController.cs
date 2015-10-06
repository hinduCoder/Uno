using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
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
                return RedirectToAction("Index", "Room");
            }
            return View();

        }
        [System.Web.Mvc.HttpPost]
        public ActionResult LogIn(LoginViewModel login)
        { 
            using (var unoDb = new UnoDb())
            {
                var user = unoDb.Users.SingleOrDefault(u => u.Username == login.Username);
                if (user == null)
                    return Error("No such user");// HttpNotFound("No such user"); // TODO: TEMP
                var hashedPasswordString = Encrypt.SHA1(login.Password);
                if (!user.Password.Equals(hashedPasswordString, StringComparison.OrdinalIgnoreCase))
                    return Error("Password wrong");

                var ticket = new FormsAuthenticationTicket(login.Username, true, 30);
                var encryptedTicket = FormsAuthentication.Encrypt(ticket);
                Response.SetCookie(new HttpCookie("userid", encryptedTicket) {Expires = DateTime.Now.AddDays(1)});
            }
            return RedirectToAction("Index", "Room");
        }

        public ActionResult Register()
        {
            return View("Register");
        }
        [System.Web.Mvc.HttpPost]
        public ActionResult SignUp(RegisterViewModel register)
        {
            using (var unoDb = new UnoDb())
            {
                unoDb.Users.Add(new User
                {
                    //Email = register.Email,
                    Password = Encrypt.SHA1(register.Password),
                    Username = register.Username
                });
                unoDb.SaveChanges();
            }
            return LogIn(new LoginViewModel {Username = register.Username, Password = register.Password});
        }
        [System.Web.Mvc.HttpGet]
        public ActionResult IsUsernameFree([FromUri] string username)
        {
            using (var unoDb = new UnoDb())
            {
                var all = unoDb.Users.All(u => u.Username != username);
                return Json(new {response = all}, JsonRequestBehavior.AllowGet);
            }
        }

        private ActionResult Error(string error)
        {
            return View("Error", (object)error);
        }
    }
}