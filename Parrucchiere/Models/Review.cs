using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parrucchiere.Models
{
    public class Review
    {
        public Recensioni Recensione { get; set; }
        public int Id { get; set; }
        public string Username { get; set; }
        public string NomeUtente { get; set; }

    }
}