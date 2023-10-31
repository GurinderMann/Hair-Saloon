using Parrucchiere.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Services.Description;
using System.Data.Entity;

namespace Parrucchiere.Controllers
{
    public class ProfiloController : Controller
    {
        ModelDbContext db = new ModelDbContext();
        // GET: Profilo
        public ActionResult Index()
        {

            var u = User.Identity.Name;

            var utente = db.Utenti.Where(us => us.Username == u).FirstOrDefault();
            var user = db.Utenti.Where(ur => ur.IdUtente == utente.IdUtente);




            var prenotazioni = db.Prenotazioni
            .Where(p => p.FkUtente == utente.IdUtente)
            .OrderByDescending(p => p.Data)
            .Take(5)
            .Select(p => new SelectListItem
            {
                Text = p.Data.ToString()
            })
            .ToList();
            ViewBag.TipoOptions = prenotazioni;
            return View(user);
        }

        public ActionResult Edit(int id)
        {
            var u = db.Utenti.Find(id);
            return View(u);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit (Utenti u, int id) 
        {
            var user = db.Utenti.Find(id);
            if (user != null) 
            {
                user.Nome = u.Nome;
                user.Email = u.Email;
                user.Cognome = u.Cognome;
                user.Telefono = u.Telefono;
                user.Username = u.Username;
                db.Entry(user).State =EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");   
            }
            return View(u);
        }

        public ActionResult Delete(int id )
        {
            var user = db.Utenti.Find(id);
            
            if (user != null) 
            {
                var p = db.Prenotazioni.Where(pr => pr.FkUtente == user.IdUtente).ToList();
                if (p.Count > 0)
                {
                    foreach (var item in p)
                    {
                        db.Prenotazioni.Remove(item);
                    }
                    db.Utenti.Remove(user);
                    db.SaveChanges() ;
                    return RedirectToAction("Index");
                }
                return View();
            }

            return View();
        }

        public ActionResult EditPass(int id)
        {
            var user = db.Utenti.Find(id);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPass(int id, string currentPassword, string newPassword, string confirmPassword)
        {
            var user = db.Utenti.Find(id);

            if (user != null)
            {
                // Check if the current password matches the user's current password
                if (user.Password == currentPassword)
                {
                    // Check if the new password and confirm password match
                    if (newPassword == confirmPassword)
                    {
                        // Update the user's password
                        user.Password = newPassword;
                        db.Entry(user).State = EntityState.Modified;
                        db.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("ConfirmPassword", "New password and confirm password must match.");
                    }
                }
                else
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                }
            }

            return View(user);
        }


    }
}