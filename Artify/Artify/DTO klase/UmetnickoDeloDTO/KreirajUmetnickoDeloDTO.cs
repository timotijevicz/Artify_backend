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
    public class KreirajUmetnickoDeloDTO
    {
        [Required]
        public string Naziv { get; set; }

        [StringLength(500)]
        public string Opis { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cena mora biti veća od 0.")]
        public decimal Cena { get; set; }

        public string Slika { get; set; } // URL slike umetničkog dela

        [Required]
        public string Tehnika { get; set; } // Tehnika umetničkog dela

        public int? KategorijaId { get; set; } // ID kategorije umetničkog dela

        public string Stil { get; set; } // Stil (impresionizam, realizam)

        public string Dimenzije { get; set; } // Dimenzije (širina * visina)

        [Required]
        public string UmetnikId { get; set; } // ID umetnika koji je kreirao delo
    }
}
