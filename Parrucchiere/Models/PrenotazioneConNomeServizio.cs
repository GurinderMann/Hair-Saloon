using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parrucchiere.Models
{
    public class PrenotazioneConNomeServizio
    {
        public Prenotazioni Prenotazione { get; set; }
        public string NomeServizio { get; set; }

        public string Username { get; set; }
    }
}