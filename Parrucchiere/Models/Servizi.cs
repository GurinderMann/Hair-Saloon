namespace Parrucchiere.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Servizi")]
    public partial class Servizi
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Servizi()
        {
            Prenotazioni = new HashSet<Prenotazioni>();
        }

        [Key]
        public int IdServizio { get; set; }

        [Required]
        [StringLength(50)]
        public string Tipo { get; set; }

        [Column(TypeName = "money")]
        public decimal Costo { get; set; }

        public int? Durata { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Prenotazioni> Prenotazioni { get; set; }
    }
}
