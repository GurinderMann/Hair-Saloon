using Parrucchiere.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MimeKit;
using MailKit.Net.Smtp;
namespace Parrucchiere.Controllers
{
    [Authorize]
    public class PrenotazioniController : Controller
    {

        private Email emailService = new Email(); 

        // Metodo per inviare la conferma dell'appuntamento
        private void SendConfirmationEmail(string recipientEmail, Prenotazioni appointment)
        {
            emailService.SendConfirmationEmail(recipientEmail, appointment);
        }

        // Metodo per inviare la modifica dell'appuntamento
        private void SendEditEmail(string recipientEmail, Prenotazioni appointment)
        {
            emailService.SendEditEmail(recipientEmail, appointment);
        }

        // Metodo per inviare il promemoria
        private void SendReminderEmail(string recipientEmail, Prenotazioni appointment)
        {
            emailService.SendReminderEmail(recipientEmail, appointment);
        }

        //Metodo per inviare la cancellazione dell'appuntamento
        private void SendDeleteEmail(string recipientEmail, Prenotazioni appointment)
        {
            emailService.SendDeleteEmail(recipientEmail, appointment);
        }




        ModelDbContext db = new ModelDbContext();

        //Recupera le prenotazioni passate e future del utente
        public ActionResult Index()
        {
            var u = User.Identity.Name;
          
            var user = db.Utenti.Where(us => us.Username == u).FirstOrDefault();
            // Recupera le prenotazioni dell'utente con il nome del servizio associato
            var prenotazioni = db.Prenotazioni
                .Where(a => a.FkUtente == user.IdUtente)
                .Select(p => new PrenotazioneConNomeServizio
                {
                    Prenotazione = p,
                    NomeServizio = p.Servizi.Tipo
                })
                .OrderByDescending(p => p.Prenotazione.Data)
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

            var ferie = db.Ferie.ToList()
                .Select(f => new SelectListItem
                {
                    Text =    f.DataInizio.ToShortDateString() + " - " + f.DataFine.ToShortDateString()
                }).ToList();

            var orario = db.Orari.ToList()
                .Select(o => new SelectListItem
                {
                    Value = o.Ora.ToString(),
                    Text = o.Ora.ToString()
                });
           
            ViewBag.Ferie = ferie;
            ViewBag.Orari = orario;
            ViewBag.TipoOptions = servizi;
            return View();
        }


        //Post della create delle prenotazioni
        [HttpPost]
        [ValidateAntiForgeryToken]
      
        public ActionResult Create(Prenotazioni a)
        {

            // Dropdown dei servizi disponibili
            var servizi = db.Servizi.Select(s => new SelectListItem
            {
                Value = s.IdServizio.ToString(),
                Text = s.Tipo
            }).ToList();

            var ferie = db.Ferie.ToList()
                 .Select(f => new SelectListItem
                 {
                     Text = f.DataInizio.ToShortDateString() + " - " + f.DataFine.ToShortDateString()
                 }).ToList();

            var orario = db.Orari.ToList()
               .Select(o => new SelectListItem
               {
                   Value = o.Ora.ToString(),
                   Text = o.Ora.ToString()
               });

           
            ViewBag.Orari = orario;
            ViewBag.Ferie = ferie;
            ViewBag.TipoOptions = servizi;


            // Seleziono il servizio dal dropdown
            var servizioSelezionato = db.Servizi.FirstOrDefault(s => s.IdServizio == a.FkServizi);

            // Controllo se il servizio è esistente
            if (servizioSelezionato != null)
            {
                a.Fine = a.Data.AddMinutes((double)servizioSelezionato.Durata);

                // Verifico se l'orario di fine supera le 20:00
                TimeSpan orarioMassimo = TimeSpan.Parse("20:00");
                if (a.Fine.TimeOfDay < orarioMassimo)
                {
                    bool orarioOccupato = db.Prenotazioni.Any(app =>
                   (app.Data <= a.Fine && app.Data >= a.Data) ||
                   (app.Fine >= a.Data && app.Fine <= a.Fine));
                    

                    // Recupero le ferie dal database
                    var fer = db.Ferie.ToList();

                    // Confronto la data della prenotazione con le date di inizio e fine delle ferie
                    bool dataInFerie = false;

                    foreach (var f in fer)
                    {
                        if (a.Data >= f.DataInizio && a.Data <= f.DataFine)
                        {
                            dataInFerie = true;
                            break;
                        }
                    }

                    if (dataInFerie)
                    {
                        ModelState.AddModelError("Data", "La data selezionata è durante le ferie. Scegli un'altra data.");
                    }
                    else if (!orarioOccupato)
                    {
                     

                        var u = User.Identity.Name;
                        var user = db.Utenti.Where(us => us.Username == u).FirstOrDefault();
                        a.FkUtente = user.IdUtente;
                       
                        db.Prenotazioni.Add(a);
                        db.SaveChanges();

                        //Invio email di conferma, prendendo come destinatario l'email dello user che sta prenotando
                        SendConfirmationEmail(user.Email, a); 

                  

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        // Messaggio se l'orario selezionato è occupato
                        ModelState.AddModelError("Data", "L'orario selezionato è già occupato. Scegli un altro orario.");
                    }
                }
                else 
                {
                    ModelState.AddModelError("Data", "L'orario selezionato non è valido, perfavore scegli un orario diverso.");
                }

            }

           
          

            return View(a);
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

            var orario = db.Orari.ToList()
             .Select(o => new SelectListItem
             {
                 Value = o.Ora.ToString(),
                 Text = o.Ora.ToString()
             });

            var ferie = db.Ferie.ToList()
                .Select(f => new SelectListItem
                {
                    Text = f.DataInizio.ToShortDateString() + " - " + f.DataFine.ToShortDateString()
                }).ToList();

            ViewBag.Orari = orario;
            ViewBag.Ferie = ferie;
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

                var u = User.Identity.Name;
                var user = db.Utenti.Where(us => us.Username == u).FirstOrDefault();

                //Assegnazione valori
                p.Data = a.Data;
                p.FkServizi = a.FkServizi;
                p.Fine = a.Fine;
                db.Entry(p).State = EntityState.Modified;


                db.SaveChanges();

                //Invio email delle modifiche apportate
                SendEditEmail(user.Email, a);

                return RedirectToAction("Index");
            }
            return View(a);
        }


        //Delete direttamente al click
        public ActionResult Delete(int id)
        {
            var u = User.Identity.Name;
            var user = db.Utenti.Where(us => us.Username == u).FirstOrDefault();

            Prenotazioni p = db.Prenotazioni.Find(id);
            db.Prenotazioni.Remove(p);
            db.SaveChanges();

            //Invio email per la cancellazione del appuntamento
            SendDeleteEmail(user.Email, p);
            return RedirectToAction("Index");
        }

  

    }

}