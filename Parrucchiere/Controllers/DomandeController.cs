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

                // Crea un oggetto QA per la domanda stessa, anche se non ci sono risposte
                var domandaQA = new QA
                {
                    domande = domanda,
                    Id = domanda.IdDomanda,
                    Nome = (utenteDomanda != null) ? utenteDomanda.Username : null,
                    NomeRisposta = null, // Nessuna risposta associata alla domanda
                    Risposta = null // Nessuna risposta associata alla domanda
                };

                domandecomp.Add(domandaQA);

                // Utilizza il metodo Select per ottenere tutte le risposte relative alla domanda
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
            int? userId = Session["UserId"] as int?;
            d.FkUtente = userId.Value;
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
            int? userId = Session["UserId"] as int?;
            d.FkUtente = userId.Value;
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

        public ActionResult Risposta()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Risposta(Risposte r, int id)
        {
            int? userId = Session["UserId"] as int?;
            r.FkUtente = userId.Value;
            r.FkDomanda = id;
            db.Risposte.Add(r);
            db.SaveChanges();


            return RedirectToAction("Index");
        }

        //Get per edit delle domande
        public ActionResult EditRisposta(int id)
        {
            var d = db.Risposte.Find(id);
            return View(d);
        }


        //Post per l'edit delle domande
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRisposta(Risposte r, int id)
        {
            int? userId = Session["UserId"] as int?;
            r.FkUtente = userId.Value;
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

        //Get per la delete che parte direttamente al click
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