using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parrucchiere.Models
{
    public class QA
    {
        public Domande domande { get; set; }
        public string Nome { get; set; }
        public int Id { get; set; }
        public string Username { get; set; }
        public string NomeRisposta { get; set; }
        public string Risposta { get; set; }
        public int IdRisposta { get; set; }
    }
}