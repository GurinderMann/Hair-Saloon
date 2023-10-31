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
        [Column(Order = 0)]
        public int IdFerie { get; set; }

        [Key]
        [Column(Order = 1, TypeName = "date")]
        public DateTime DataInizio { get; set; }

        [Key]
        [Column(Order = 2, TypeName = "date")]
        public DateTime DataFine { get; set; }
    }
}
