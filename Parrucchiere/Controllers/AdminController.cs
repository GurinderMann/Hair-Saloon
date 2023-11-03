using Parrucchiere.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parrucchiere.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        ModelDbContext db = new ModelDbContext();
        

       //Calendario con ricerca appuntamenti per data
        public ActionResult Index()
        {
            return View();
        }

        //Json result per visualizzare le prenotazioni tramite chiamata AJAX
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


        //Lista delle prenotazioni passate e future
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


        //Edit della prenotazione
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


        //Edit della prenotazione
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


        //Delete direttamente al click
        public ActionResult Delete(int id)
        {
            Prenotazioni p = db.Prenotazioni.Find(id);
            db.Prenotazioni.Remove(p);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        //Lista di tutti gli utenti registrati
        public ActionResult ListaUtenti()
        {
            return View(db.Utenti.ToList());
        }

        //Edit del ruolo degli utenti registrati
        public ActionResult EditUtenti(int id)
        {
            var u = db.Utenti.Find(id); 
            return View(u);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUtenti(int id, Utenti u)
        {
            var user = db.Utenti.Find(id);
            if (user != null) 
            {
                user.Role = u.Role;
                db.Entry(user).State = EntityState.Modified;


                db.SaveChanges();
                return RedirectToAction("ListaUtenti");

            }
            return View(u);
        }



        //Lista delle ferie
        public ActionResult ferie() 
        {
            return View(db.Ferie.ToList());
        }
            
        //Post per creare le ferie fatto con una chiamata AJAX
        [HttpPost]
        public ActionResult CreaFerie(Ferie f, DateTime DataInizio, DateTime DataFine)
        {
            if (f != null)
            {
                f.DataInizio = DataInizio;
                f.DataFine = DataFine;
                db.Ferie.Add(f);
                db.SaveChanges();
                return RedirectToAction("ferie");
            }
            return View();
        }
            
        //Post per modificare le ferie fatto con una chiamata AJAX
        [HttpPost]
        public ActionResult EditFerie(int IdFerie, DateTime DataInizio, DateTime DataFine)
        {
           var f =  db.Ferie.Find(IdFerie);

            if (f != null)
            {
                f.DataInizio = DataInizio;
                f.DataFine = DataFine;
                db.Entry(f).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ferie");
            };

            return View();

        }

        //Delete direttamente al click
        public ActionResult EliminaFeria(int id)
        {
            Ferie f = db.Ferie.Find(id); 
            db.Ferie.Remove(f);
            db.SaveChanges();
            return RedirectToAction("ferie");
        }


        //Lista di tutti i servizi disponibili
        public ActionResult ListaServizi() 
        { 
            return View(db.Servizi.ToList());
        }

        //Post per creare un nuovo servizio, fatto con una chiamata AJAX
        [HttpPost]
        public ActionResult CreaServizio(Servizi s, string Tipo, int Costo, int Durata) 
        {
            if ( s != null)
            {
                s.Tipo = Tipo;
                s.Costo = Costo;
                s.Durata = Durata;
                db.Servizi.Add(s);
                db.SaveChanges();
                return RedirectToAction("ListaServizi");

            }
            return View();

        } 

        //Post per modificare un servizio, fatto con una chiamata AJAX
        [HttpPost]
        public ActionResult EditServizio( int idSrv,  string Tipo, int Costo, int Durata) 
        {
            var s = db.Servizi.Find(idSrv);

            if (s != null)
            {
                s.Tipo = Tipo;
                s.Costo = Costo;
                s.Durata= Durata;

                db.Entry(s).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ListaServizi");
            };

            return View();
        }

        //Delete direttamente al click
        public ActionResult EliminaServizio(int id) 
        {
            Servizi s = db.Servizi.Find(id);
            db.Servizi.Remove(s);
            db.SaveChanges();
            return RedirectToAction("ListaServizi");
        }



    }
}