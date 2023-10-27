using Parrucchiere.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Parrucchiere.Controllers
{
    public class RecensioniController : Controller
    {
        ModelDbContext db = new ModelDbContext();
        // GET: Recensioni
        public ActionResult Index()
        {
            var recensioni = db.Recensioni.ToList();

            // Recupera i dati dell'utente per ciascuna recensione
            var recensioniConUtente = recensioni.Select(r => new Review
            {
                Recensione = r,
                Id = r.IdRecensione,
                NomeUtente = db.Utenti.FirstOrDefault(u => u.IdUtente == r.FkUtente)?.Nome,
                Username = db.Utenti.FirstOrDefault(u => u.IdUtente == r.FkUtente)?.Username
            }).ToList();

            return View(recensioniConUtente);
        }


        //Get per la create delle recensioni
        public ActionResult Create()
        {
            return View();
        }


        //Post per la create delle recensioni
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Recensioni r)
        {
            int? userId = Session["UserId"] as int?;
            r.FkUtente = userId.Value;
         
            db.Recensioni.Add(r);
            db.SaveChanges();


            return RedirectToAction("Index");
        }


        //Get per l'edit della recensione
        public ActionResult Edit (int id)
        {
            var r = db.Recensioni.Find(id);
            return View(r);
        }


        //Post per l'edit della recensione
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit (Recensioni r, int id ) 
        {
            var rec = db.Recensioni.Find(id);

            if (rec != null)
            {
                rec.Valutazione = r.Valutazione;
                rec.Testo = r.Testo;
                db.Entry(rec).State= EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(r);
           


        }

        //Get per la delete della recensione che parte direttamente al click
        public ActionResult Delete(int id)
        {
            Recensioni r = db.Recensioni.Find(id);
            db.Recensioni.Remove(r);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}