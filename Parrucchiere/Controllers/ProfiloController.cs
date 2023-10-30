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
           
            int? userId = Session["UserId"] as int?;
            var user = db.Utenti.Where(u => u.IdUtente == userId);




            var prenotazioni = db.Prenotazioni
            .Where(p => p.FkUtente == userId)
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



    }
}