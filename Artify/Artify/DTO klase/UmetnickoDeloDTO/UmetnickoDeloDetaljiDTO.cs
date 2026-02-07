using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Artify.Models;
using System.ComponentModel.DataAnnotations;

namespace Artify.DTO_klase.UmetnickoDeloDTO
{
    public class UmetnickoDeloDetaljiDTO
    {
        public int UmetnickoDeloId { get; set; }
        public string? Naziv { get; set; }
        public string? Opis { get; set; }
        public string? Tehnika { get; set; }
        public string? Stil { get; set; }
        public string? Dimenzije { get; set; }

        public decimal? Cena { get; set; }
        public bool NaAukciji { get; set; }
        public float? PocetnaCenaAukcije { get; set; }
        public float? TrenutnaCenaAukcije { get; set; }
        public DateTime? AukcijaPocinje { get; set; }
        public DateTime? AukcijaZavrsava { get; set; }

        // Cloudinary url (ili lokalni put)
        public string? SlikaUrl { get; set; }

        public int UmetnikId { get; set; }
        public string UmetnikImePrezime { get; set; } = "Nepoznati umetnik";

        public string Status { get; set; } = "Dostupno";
        public DateTime DatumPostavljanja { get; set; }
    }
}
