using Parrucchiere.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Parrucchiere.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        ModelDbContext db = new ModelDbContext();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult login()
        {
            return View();
        }

        

        [HttpPost]
        public ActionResult Login(Utenti u)
        {
            var user = db.Utenti.FirstOrDefault(
                usr => usr.Username == u.Username && usr.Password == u.Password);

            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(user.Username, false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("Login", "Credenziali non valide. Riprova.");
                return View();
            }
        }


        public ActionResult Registrati()
        {
            return View();
        }
        [HttpPost]

        
        public ActionResult Registrati([Bind(Exclude = "Role")] Utenti u, string ConfermaPassword)
        {
            var utenti = db.Utenti.ToList();

            if (u.Password != ConfermaPassword)
            {
                ModelState.AddModelError("ConfermaPassword", "Le password non corrispondono.");
            }

            foreach (var utente in utenti)
            {
                if (u.Username == utente.Username)
                {
                    ModelState.AddModelError("", "Username già in uso. Scegliere un altro username.");
                }
            }

            // Rimuovi il campo ConfermaPassword dal modello se non vuoi salvarlo nel database
            ModelState.Remove("ConfermaPassword");

            if (ModelState.IsValid)
            {
                u.Role = "User";
                db.Utenti.Add(u);
                db.SaveChanges();
                return RedirectToAction("Login", "Login");
            }

            return View(u);
        }


        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }
    }
}