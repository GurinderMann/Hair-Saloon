using Parrucchiere.Models;
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
            return View(db.Recensioni.ToList());
        }
    }
}