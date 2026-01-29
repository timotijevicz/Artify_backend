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
    public class UmetnickoDelo
    {
        public int UmetnickoDeloId { get; set; }

        [Required]
        public string Naziv { get; set; }

        [Required]
        public string Opis { get; set; }

        // FIKSNA CENA (nije aukcija)
        public float? Cena { get; set; }

        [Required]
        public string Slika { get; set; }

        // AUKCIJA
        public float? PocetnaCenaAukcije { get; set; }
        public float? TrenutnaCenaAukcije { get; set; }

        public DateTime? AukcijaPocinje { get; set; }
        public DateTime? AukcijaZavrsava { get; set; }

        public DateTime DatumPostavljanja { get; set; } = DateTime.UtcNow;

        [Required]
        public string Tehnika { get; set; }

        [Required]
        public string Stil { get; set; }

        [Required]
        public string Dimenzije { get; set; }

        public UmetnickoDeloStatus Status { get; set; } = UmetnickoDeloStatus.Dostupno;

        public bool NaAukciji { get; set; } = false;

        [Required]
        public int UmetnikId { get; set; }
        public virtual Umetnik Umetnik { get; set; }
    }

    public enum UmetnickoDeloStatus
    {
        Dostupno = 0,
        Prodato = 1,
        Uklonjeno = 2
    }
}
