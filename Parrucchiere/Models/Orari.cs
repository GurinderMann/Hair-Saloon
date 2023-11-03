namespace Parrucchiere.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Orari")]
    public partial class Orari
    {
        [Key]
        public int IdOrario { get; set; }

        public TimeSpan Ora { get; set; }
    }
}
