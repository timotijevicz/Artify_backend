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
        public DbSet<Kategorija> Kategorija { get; set; }
        public DbSet<Korisnik> Korisnici { get; set; }
        public DbSet<Korpa> Korpe { get; set; }
        public DbSet<Portfolio> Portfolio { get; set; }
        public DbSet<Porudzbina> Porudzbine { get; set; }
        public DbSet<Recenzija> Recenzije { get; set; }
        public DbSet<UmetnickoDelo> UmetnickaDela { get; set; }
       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           

            // Favoriti entitet
            modelBuilder.Entity<Favoriti>()
                .HasOne(f => f.Kupac)
                .WithMany()
                .HasForeignKey(f => f.KupacId)
                .OnDelete(DeleteBehavior.NoAction);  // Zadržavamo korisnika, ali brišemo povezan unikatni zapis

            modelBuilder.Entity<Favoriti>()
                .HasOne(f => f.UmetnickoDelo)
                .WithMany()
                .HasForeignKey(f => f.UmetnickoDeloId)
                .OnDelete(DeleteBehavior.Cascade);  // Brisanje za oba strane, povezani objekat se briše sa 'Favoritima'

            // Kategorija entitet
            modelBuilder.Entity<UmetnickoDelo>()
                .HasOne(ud => ud.Kategorija)
                .WithMany()
                .HasForeignKey(ud => ud.KategorijaId)
                .OnDelete(DeleteBehavior.SetNull);  // Postavljanje null ako je kategorija obrisana

            // Korisnik entitet
            modelBuilder.Entity<Korisnik>()
                .HasMany(k => k.UmetnickaDela)
                .WithOne(ud => ud.Umetnik)
                .HasForeignKey(ud => ud.UmetnikId)
                .OnDelete(DeleteBehavior.Cascade);  // Brisanje umetničkog dela ukoliko korisnik bude obrisan

            // Korpa entitet
            modelBuilder.Entity<Korpa>()
                .HasOne(k => k.Kupac)
                .WithMany()
                .HasForeignKey(k => k.KupacId)
                .OnDelete(DeleteBehavior.NoAction);  // Kroz kupca se može povezivati sa korpom, ali ne briše se automatski

            // Porudzbina entitet
            modelBuilder.Entity<Porudzbina>()
                .HasOne(p => p.Korpa)
                .WithMany()
                .HasForeignKey(p => p.KorpaId)
                .OnDelete(DeleteBehavior.Cascade);  // Porudžbina briše sve stavke iz korpe

            // Recenzija entitet
            modelBuilder.Entity<Recenzija>()
                .HasOne(r => r.Kupac)
                .WithMany()
                .HasForeignKey(r => r.KupacId)
                .OnDelete(DeleteBehavior.NoAction);  // Ne briše korisnika koji je napisao recenziju, čak i kad je uklonjen sa platforme

            modelBuilder.Entity<Recenzija>()
                .HasOne(r => r.UmetnickoDelo)
                .WithMany()
                .HasForeignKey(r => r.UmetnickoDeloId)
                .OnDelete(DeleteBehavior.Restrict);  // Ako je umetničko delo obrisano, recenzija neće biti uklonjena, neće moći da se bavi.

            // Portfolio entitet
            modelBuilder.Entity<Portfolio>()
                .HasOne(p => p.Umetnik)
                .WithMany()
                .HasForeignKey(p => p.UmetnikId)
                .OnDelete(DeleteBehavior.NoAction);  // Ne briše profil umetnika ukoliko se izbriše portfolio

            // Inicijalizacija podataka za uloge
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