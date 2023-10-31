using Parrucchiere.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parrucchiere.Controllers
{
    
    public class AdminController : Controller
    {
        ModelDbContext db = new ModelDbContext();
        // GET: Admin

       
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Prenotazioni(DateTime data)
        {
            var app = db.Prenotazioni
                .Where(p => DbFunctions.TruncateTime(p.Data) == data.Date)
                .ToList();

            var risultati = new List<object>();
            decimal totaleIncassato = 0; // Inizializza il totale a zero.

            foreach (var a in app)
            {
                var utente = db.Utenti
                    .Where(u => u.IdUtente == a.FkUtente)
                    .FirstOrDefault();
                var s = db.Servizi
                    .Where(t => t.IdServizio == a.FkServizi)
                    .FirstOrDefault();

                if (utente != null)
                {
                    var result = new
                    {
                        AppuntamentoId = a.IdPrenotazione,
                        DataAppuntamento = a.Data.ToString("HH:mm"),
                        Tipo = s.Tipo,
                        Costo = s.Costo,
                        NomeUtente = utente.Nome
                    };

                    risultati.Add(result);

                    
                    totaleIncassato += s.Costo;
                }
            }

           
            var totaleIncassatoResult = new
            {
                TotaleIncassato = totaleIncassato
            };

            risultati.Add(totaleIncassatoResult);

            return Json(risultati, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Lista()
        {



              var prenotazioni = db.Prenotazioni
             .Select(p => new PrenotazioneConNomeServizio
             {
                 Prenotazione = p,
                 NomeServizio = p.Servizi.Tipo,
                 Username = p.Utenti.Nome
             })
             .OrderByDescending(p => p.Prenotazione.Data) 
             .ToList();

            return View(prenotazioni);
        }

        public ActionResult Edit(int id)
        {
            var servizi = db.Servizi.Select(s => new SelectListItem
            {
                Value = s.IdServizio.ToString(),
                Text = s.Tipo
            }).ToList();

            ViewBag.TipoOptions = servizi;
            var p= db.Prenotazioni.Find(id);
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Prenotazioni a, int id)
        {
            var p = db.Prenotazioni.Find(id);
            var servizioSelezionato = db.Servizi.FirstOrDefault(s => s.IdServizio == a.FkServizi);
            if (servizioSelezionato != null)
            {

                a.Fine = a.Data.AddMinutes((double)servizioSelezionato.Durata);
            }
            if (p != null)
            {
                p.Data = a.Data;
                p.FkServizi = a.FkServizi;
                p.Fine = a.Fine;
                db.Entry(p).State = EntityState.Modified;


                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(a);
        }

        public ActionResult Delete(int id)
        {
            Prenotazioni p = db.Prenotazioni.Find(id);
            db.Prenotazioni.Remove(p);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult ListaUtenti()
        {
            return View(db.Utenti.ToList());
        }
    }
}