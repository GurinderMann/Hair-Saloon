using Parrucchiere.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parrucchiere.Controllers
{
    public class HomeController : Controller
    {
        ModelDbContext db = new ModelDbContext();
       
        public ActionResult Index()
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
      
       
    }
}