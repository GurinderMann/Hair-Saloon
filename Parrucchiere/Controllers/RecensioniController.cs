﻿using Parrucchiere.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
                NomeUtente = db.Utenti.FirstOrDefault(u => u.IdUtente == r.FkUtente)?.Nome
            }).ToList();

            return View(recensioniConUtente);
        }


    }
}