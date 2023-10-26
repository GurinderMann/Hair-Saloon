using Parrucchiere.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parrucchiere.Controllers
{
    [Authorize]
    public class PrenotazioniController : Controller
    {
     
        ModelDbContext db = new ModelDbContext();
        public ActionResult Index()
        {
            int? userId = Session["UserId"] as int?;

            // Recupera le prenotazioni dell'utente con il nome del servizio associato
            var prenotazioni = db.Prenotazioni
                .Where(a => a.FkUtente == userId)
                .Select(p => new PrenotazioneConNomeServizio
                {
                    Prenotazione = p,
                    NomeServizio = p.Servizi.Tipo
                })
                .ToList();

            return View(prenotazioni);
        }

        public ActionResult Create()
        {
            var servizi = db.Servizi.ToList()
              .Select(s => new SelectListItem
              {
                  Value = s.IdServizio.ToString(),
                  Text = s.Tipo + " - " + s.Costo.ToString("C")
              }).ToList();



            ViewBag.TipoOptions = servizi;
            return View();
        }

   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Prenotazioni a)
        {
            var servizi = db.Servizi.Select(s => new SelectListItem
            {
                Value = s.IdServizio.ToString(),
                Text = s.Tipo
            }).ToList();

            ViewBag.TipoOptions = servizi;

          
            var servizioSelezionato = db.Servizi.FirstOrDefault(s => s.IdServizio == a.FkServizi);

            if (servizioSelezionato != null)
            {
              
                a.Fine = a.Data.AddMinutes((double)servizioSelezionato.Durata);
            }

            // Controlla se l'orario è già occupato
            bool orarioOccupato = db.Prenotazioni.Any(app =>
                (app.Data <= a.Fine && app.Data >= a.Data) ||
                (app.Fine >= a.Data && app.Fine <= a.Fine)
            );

            var oggi = DateTime.Now;
            if (a.Data >= oggi)
            {
                if (orarioOccupato)
                {
                    ModelState.AddModelError("Data", "L'orario selezionato è già occupato. Scegli un altro orario.");
                    return View(a);
                }

                int? userId = Session["UserId"] as int?;
                a.FkUtente = userId.Value;
                db.Prenotazioni.Add(a);
                db.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("Data", "La data inserita non è valida, per favore ricontrolla la data.");
                return View(a);
            }
        }


        public ActionResult Edit(int id)
        {
            var servizi = db.Servizi.Select(s => new SelectListItem
            {
                Value = s.IdServizio.ToString(),
                Text = s.Tipo
            }).ToList();

            ViewBag.TipoOptions = servizi;
            var p = db.Prenotazioni.Find(id);
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


    }
}