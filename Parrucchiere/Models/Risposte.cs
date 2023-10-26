namespace Parrucchiere.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Risposte")]
    public partial class Risposte
    {
        [Key]
        public int IdRisposta { get; set; }

        [Required]
        public string Risposta { get; set; }

        public int FkUtente { get; set; }

        public int FkDomanda { get; set; }

        public virtual Domande Domande { get; set; }

        public virtual Utenti Utenti { get; set; }
    }
}
