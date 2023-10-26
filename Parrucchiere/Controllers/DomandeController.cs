using Parrucchiere.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parrucchiere.Controllers
{
    public class DomandeController : Controller
    {
        ModelDbContext db = new ModelDbContext();

        // GET: Domande
        public ActionResult Index()
        {
            var domande = db.Domande.ToList();
            domande.GroupBy(d => d.IdDomanda);
            var domandecomp = new List<QA>();

            foreach (var domanda in domande)
            {
                
                var utenteDomanda = db.Utenti.FirstOrDefault(u => u.IdUtente == domanda.FkUtente);

                // Utilizza il metodo Select per ottenere tutte le risposte relative alla domanda
                var risposte = domanda.Risposte.Select(r => new QA
                {
                    domande = domanda,
                    Nome = (utenteDomanda != null) ? utenteDomanda.Nome : null,
                    NomeRisposta = (r != null) ? (db.Utenti.FirstOrDefault(u => u.IdUtente == r.FkUtente) != null ? db.Utenti.FirstOrDefault(u => u.IdUtente == r.FkUtente).Nome : null) : null,
                    Risposta = (r != null) ? r.Risposta : null
                }).ToList();

                domandecomp.AddRange(risposte);
            }

            return View(domandecomp);
        }





    }

}