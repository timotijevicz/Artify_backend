using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Artify.Models;
using System.ComponentModel.DataAnnotations;

namespace Artify.Models
{
    public class Notifikacija
    {
        public int NotifikacijaId { get; set; }

        [Required]
        public string KorisnikId { get; set; }
        public virtual Korisnik Korisnik { get; set; }

        [Required]
        [StringLength(300)]
        public string Poruka { get; set; }

        public NotifikacijaTip Tip { get; set; }

        //public bool Procitana { get; set; } = false;

        public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;

        // opciono – povezivanje sa entitetima
        public int? PorudzbinaId { get; set; }
        public int? UmetnickoDeloId { get; set; }
    }

    public enum NotifikacijaTip
    {
        Obavestenje,
        Porudzbina,
        Recenzija
    }
}
