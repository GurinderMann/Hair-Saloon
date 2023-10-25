namespace Parrucchiere.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Recensioni")]
    public partial class Recensioni
    {
        [Key]
        public int IdRecensione { get; set; }

        public string Testo { get; set; }

        public int Valutazione { get; set; }

        public int FkUtente { get; set; }

        public virtual Utenti Utenti { get; set; }
    }
}
