using Frankfurter.AnnyPriet.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Frankfurter.AnnyPriet
{
    public class AplicationDbContext : IdentityDbContext
    {
        public AplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TasaDeCambio>()
                .HasOne(t => t.MonedaFrom)
                .WithMany(m => m.MonedasFrom)
                .HasForeignKey(t => t.MonedaFromID)
                .HasPrincipalKey(m => m.ID).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TasaDeCambio>()
                .HasOne(t => t.MonedaTo)
                .WithMany(m => m.MonedasTo)
                .HasForeignKey(t => t.MonedaToID)
                .HasPrincipalKey(m => m.ID).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TasaDeCambio>().Property(p => p.Rate).HasColumnType("float");
            modelBuilder.Entity<TasaDeCambio>().Property(p => p.Amount).HasColumnType("float");

            // nombres para las tablas de Usuario

            modelBuilder.Entity<IdentityUser>().ToTable("Usuarios");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UsuariosClaims");
        }

        public DbSet<TasaDeCambio> TasasDeCambios { get; set; }
        public DbSet<Moneda> Monedas { get; set; }
    }
}
