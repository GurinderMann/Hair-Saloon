namespace Parrucchiere.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Ferie")]
    public partial class Ferie
    {
        [Key]
        public int IdFerie { get; set; }

        [Column(TypeName = "date")]
        public DateTime DataInizio { get; set; }

        [Column(TypeName = "date")]
        public DateTime DataFine { get; set; }
    }
}
