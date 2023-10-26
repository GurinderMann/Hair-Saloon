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

        //Recupera le prenotazioni passate e future del utente
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


        //Get della create delle prenotazioni
        public ActionResult Create()
        {
            //Dropdwon dei servizi disponibili
            var servizi = db.Servizi.ToList()
              .Select(s => new SelectListItem
              {
                  Value = s.IdServizio.ToString(),
                  Text = s.Tipo + " - " + s.Costo.ToString("C")
              }).ToList();



            ViewBag.TipoOptions = servizi;
            return View();
        }

   
        //Post della create delle prenotazioni
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Prenotazioni a)
        {
            //Dropdwon dei servizi disponibili
            var servizi = db.Servizi.Select(s => new SelectListItem
            {
                Value = s.IdServizio.ToString(),
                Text = s.Tipo
            }).ToList();

            ViewBag.TipoOptions = servizi;

            //Seleziono il servizio dal dropdown
            var servizioSelezionato = db.Servizi.FirstOrDefault(s => s.IdServizio == a.FkServizi);

            //Controllo se il servizio è esistente
            if (servizioSelezionato != null)
            {
              
                a.Fine = a.Data.AddMinutes((double)servizioSelezionato.Durata);
            }

            // Controllo se l'orario è già occupato
            bool orarioOccupato = db.Prenotazioni.Any(app =>
                (app.Data <= a.Fine && app.Data >= a.Data) ||
                (app.Fine >= a.Data && app.Fine <= a.Fine)
            );

            //Controllo che la data di prenotazione non sia già passata
            var oggi = DateTime.Now;
            if (a.Data >= oggi)
            {
                if (orarioOccupato)
                {
                    //Messaggio se l'orario selezionato è occupato
                    ModelState.AddModelError("Data", "L'orario selezionato è già occupato. Scegli un altro orario.");
                    return View(a);
                }

                //Prendo userId e lo assegno a FkUtente
                int? userId = Session["UserId"] as int?;
                a.FkUtente = userId.Value;


                db.Prenotazioni.Add(a);
                db.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            else
            {
                //Se la data selezionata è già passata
                ModelState.AddModelError("Data", "La data inserita non è valida, per favore ricontrolla la data.");
                return View(a);
            }
        }


        // Get per l'edit degli appuntamenti da parte del utente
        public ActionResult Edit(int id)
        {
            //Dropdown dei servizi
            var servizi = db.Servizi.Select(s => new SelectListItem
            {
                Value = s.IdServizio.ToString(),
                Text = s.Tipo
            }).ToList();

            ViewBag.TipoOptions = servizi;
            var p = db.Prenotazioni.Find(id);
            return View(p);
        }


        //Post per l'edit degli appuntamenti da parte del utente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Prenotazioni a, int id)
        {
            //Trovo la prenotazione nel db tramite id
            var p = db.Prenotazioni.Find(id);

            //Verifico se la prenotazione effettivamente esiste 
            if (p != null)
            {
                //Assegno il servizio
                var servizioSelezionato = db.Servizi.FirstOrDefault(s => s.IdServizio == a.FkServizi);

            //Verifco se il servizio esiste 
            if (servizioSelezionato != null)
            {
                //Aggiungo la durata del servizio alla data per ottenere l'orario di fine 
                a.Fine = a.Data.AddMinutes((double)servizioSelezionato.Durata);
            }

            
           
                //Assegnazione valori
                p.Data = a.Data;
                p.FkServizi = a.FkServizi;
                p.Fine = a.Fine;
                db.Entry(p).State = EntityState.Modified;


                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(a);
        }


        //Delete direttamente al click
        public ActionResult Delete(int id)
        {
            Prenotazioni p = db.Prenotazioni.Find(id);
            db.Prenotazioni.Remove(p);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}