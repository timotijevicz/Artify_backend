using Artify.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Artify.Data
{
    public class AppDbContext : IdentityDbContext<Korisnik>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSet-ovi za entitete u projektu

        public DbSet<Favoriti> Favoriti { get; set; }
        //public DbSet<Korisnik> Korisnik { get; set; }
        public DbSet<Porudzbina> Porudzbine { get; set; }
        public DbSet<Recenzija> Recenzije { get; set; }
        public DbSet<UmetnickoDelo> UmetnickaDela { get; set; }
        public DbSet<Umetnik> Umetnici { get; set; }
        public DbSet<Notifikacija> Notifikacije { get; set; }
        public DbSet<AukcijskaPonuda> AukcijskePonude { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -- Veze --


            modelBuilder.Entity<Umetnik>()
                .HasMany(u => u.UmetnickaDela)
                .WithOne(d => d.Umetnik)
                .HasForeignKey(d => d.UmetnikId)
                .OnDelete(DeleteBehavior.NoAction); // ⬅ KLJUČNO

            modelBuilder.Entity<Korisnik>()
                .HasMany(k => k.Notifikacije)
                .WithOne(n => n.Korisnik)
                .HasForeignKey(n => n.KorisnikId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Porudzbina>()
                .HasOne(p => p.Korisnik)
                .WithMany()
                .HasForeignKey(p => p.KorisnikId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Porudzbina>()
                .HasOne(p => p.UmetnickoDelo)
                .WithMany()
                .HasForeignKey(p => p.UmetnickoDeloId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Recenzija>()
                .HasOne(r => r.Korisnik)
                .WithMany()
                .HasForeignKey(r => r.KorisnikId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Recenzija>()
                .HasOne(r => r.UmetnickoDelo)
                .WithMany()
                .HasForeignKey(r => r.UmetnickoDeloId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Favoriti>()
                .HasOne(f => f.Korisnik)
                .WithMany()
                .HasForeignKey(f => f.KorisnikId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Favoriti>()
                .HasOne(f => f.UmetnickoDelo)
                .WithMany()
                .HasForeignKey(f => f.UmetnickoDeloId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<AukcijskaPonuda>()
                .HasOne(p => p.UmetnickoDelo)
                .WithMany() // ili .WithMany(d => d.AukcijskePonude) ako dodaš kolekciju u UmetnickoDelo
                .HasForeignKey(p => p.UmetnickoDeloId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AukcijskaPonuda>()
                .HasIndex(p => new { p.UmetnickoDeloId, p.Iznos });

            modelBuilder.Entity<Porudzbina>()
                .HasIndex(p => p.UmetnickoDeloId)
                .IsUnique();


            SeedRoles(modelBuilder);
        }

        
        private void SeedRoles(ModelBuilder modelBuilder)
        {
            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = "1", // Dodavanje fiksnog ID-a za svaku ulogu
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = "2",
                    Name = "Umetnik",
                    NormalizedName = "UMETNIK"
                },
                new IdentityRole
                {
                    Id = "3",
                    Name = "Kupac",
                    NormalizedName = "KUPAC"
                }
            };

            modelBuilder.Entity<IdentityRole>().HasData(roles);
        }
    }
}