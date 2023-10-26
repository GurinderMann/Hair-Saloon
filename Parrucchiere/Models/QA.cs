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

        public string NomeRisposta { get; set; }
        public string Risposta { get; set; }
    }
}