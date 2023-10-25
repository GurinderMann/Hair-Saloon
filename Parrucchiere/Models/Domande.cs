namespace Parrucchiere.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Domande")]
    public partial class Domande
    {
        [Key]
        public int IdDomanda { get; set; }

        public string Testo { get; set; }

        public string Risposta { get; set; }

        public int FkUtente { get; set; }

        public virtual Utenti Utenti { get; set; }
    }
}
