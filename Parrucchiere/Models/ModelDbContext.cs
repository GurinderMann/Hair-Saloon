using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Parrucchiere.Models
{
    public partial class ModelDbContext : DbContext
    {
        public ModelDbContext()
            : base("name=ModelDbContext")
        {
        }

        public virtual DbSet<Domande> Domande { get; set; }
        public virtual DbSet<Prenotazioni> Prenotazioni { get; set; }
        public virtual DbSet<Recensioni> Recensioni { get; set; }
        public virtual DbSet<Risposte> Risposte { get; set; }
        public virtual DbSet<Servizi> Servizi { get; set; }
        public virtual DbSet<Utenti> Utenti { get; set; }
        public virtual DbSet<Ferie> Ferie { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domande>()
                .HasMany(e => e.Risposte)
                .WithRequired(e => e.Domande)
                .HasForeignKey(e => e.FkDomanda)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Servizi>()
                .Property(e => e.Costo)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Servizi>()
                .HasMany(e => e.Prenotazioni)
                .WithRequired(e => e.Servizi)
                .HasForeignKey(e => e.FkServizi)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Utenti>()
                .HasMany(e => e.Domande)
                .WithRequired(e => e.Utenti)
                .HasForeignKey(e => e.FkUtente)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Utenti>()
                .HasMany(e => e.Prenotazioni)
                .WithRequired(e => e.Utenti)
                .HasForeignKey(e => e.FkUtente)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Utenti>()
                .HasMany(e => e.Recensioni)
                .WithRequired(e => e.Utenti)
                .HasForeignKey(e => e.FkUtente)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Utenti>()
                .HasMany(e => e.Risposte)
                .WithRequired(e => e.Utenti)
                .HasForeignKey(e => e.FkUtente)
                .WillCascadeOnDelete(false);
        }
    }
}
