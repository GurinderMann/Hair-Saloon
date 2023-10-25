namespace Parrucchiere.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Prenotazioni")]
    public partial class Prenotazioni
    {
        [Key]
        public int IdPrenotazione { get; set; }

        public DateTime Data { get; set; }

        public int FkUtente { get; set; }

        public int FkServizi { get; set; }

        public DateTime Fine { get; set; }

        public virtual Servizi Servizi { get; set; }

        public virtual Utenti Utenti { get; set; }
    }
}
