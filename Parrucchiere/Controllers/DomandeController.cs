using Parrucchiere.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Parrucchiere.Controllers
{
    public class DomandeController : Controller
    {
        ModelDbContext db = new ModelDbContext();

        // GET: Domande
        public ActionResult Index()
        {
            var domande = db.Domande.ToList();

            // Creo una nuova lista basata sulla mia classe QA
            var domandecomp = new List<QA>();

            foreach (var domanda in domande)
            {
                var utenteDomanda = db.Utenti.FirstOrDefault(u => u.IdUtente == domanda.FkUtente);

                // Creo un oggetto QA per la domanda stessa, anche se non ci sono risposte
                var domandaQA = new QA
                {
                    domande = domanda,
                    Id = domanda.IdDomanda,
                    Nome = (utenteDomanda != null) ? utenteDomanda.Username : null,
                    NomeRisposta = null, // Nessuna risposta associata alla domanda
                    Risposta = null // Nessuna risposta associata alla domanda
                };

                domandecomp.Add(domandaQA);

                // Utilizzo il metodo Select per ottenere tutte le risposte relative alla domanda
                var risposte = domanda.Risposte.Select(r => new QA
                {
                    domande = domanda,
                    Nome = (utenteDomanda != null) ? utenteDomanda.Username : null,
                    NomeRisposta = (r != null) ? (db.Utenti.FirstOrDefault(u => u.IdUtente == r.FkUtente) != null ? db.Utenti.FirstOrDefault(u => u.IdUtente == r.FkUtente).Username : null) : null,
                    Risposta = (r != null) ? r.Risposta : null,
                    IdRisposta = r.IdRisposta
                }).ToList();

                domandecomp.AddRange(risposte);
            }

            return View(domandecomp);
        }



        //Get per la create delle domande
        public ActionResult Create()
        {
            return View();
        }


        //Post per la create delle domande
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create (Domande d)
        {
            var u = User.Identity.Name;

            var user = db.Utenti.Where(us => us.Username == u).FirstOrDefault();
          
            d.FkUtente = user.IdUtente;
            db.Domande.Add(d);
            db.SaveChanges();


            return RedirectToAction("Index");
        }


        //Get per edit delle domande
        public ActionResult Edit(int id) 
        {
            var d =  db.Domande.Find(id);
            return View(d);
        }


        //Post per l'edit delle domande
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit (Domande d, int id) 
        {
            var u = User.Identity.Name;

            var user = db.Utenti.Where(us => us.Username == u).FirstOrDefault();
            d.FkUtente = user.IdUtente;
            var dom = db.Domande.Find(id);

            if(dom != null)
            {
                dom.FkUtente = d.FkUtente;
                dom.Testo = d.Testo;
                db.Entry(dom).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");   
            }

            return RedirectToAction("Index");
        }

        //Get per la delete che parte direttamente al click
        public ActionResult Delete (int id)
        {
            var d = db.Domande.Find(id);

            if (d != null)
            {
                var ris = db.Risposte.Where(r => r.FkDomanda == d.IdDomanda).ToList();

                if (ris != null)
                {
                    foreach (var r in ris)
                    {
                        db.Risposte.Remove(r);
                    }
                }

                db.Domande.Remove(d);
                db.SaveChanges();   
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }


        //Create della risposta associata alla domanda
        public ActionResult Risposta()
        {
            return View();
        }

        //Create della risposta associata alla domanda
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Risposta(Risposte r, int id)
        {
            var u = User.Identity.Name;

            var user = db.Utenti.Where(us => us.Username == u).FirstOrDefault();
            r.FkUtente = user.IdUtente;
            r.FkDomanda = id;
            db.Risposte.Add(r);
            db.SaveChanges();


            return RedirectToAction("Index");
        }



        //Get per edit della risposta
        public ActionResult EditRisposta(int id)
        {
            var d = db.Risposte.Find(id);
            return View(d);
        }


        //Post per l'edit della risposta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRisposta(Risposte r, int id)
        {
            var u = User.Identity.Name;

            var user = db.Utenti.Where(us => us.Username == u).FirstOrDefault();
            r.FkUtente = user.IdUtente;
            var ris = db.Risposte.Find(id);

            if (ris != null)
            {
                ris.FkUtente = r.FkUtente;
                ris.Risposta = r.Risposta;
                db.Entry(ris).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }



        //Delete direttamente al click della risposta
        public ActionResult DeleteRisposta(int id)
        {
            var r = db.Risposte.Find(id);

            if (r != null)
            {
                

                db.Risposte.Remove(r);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }


    }

}