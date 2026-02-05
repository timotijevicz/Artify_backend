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

        // FIKSNA CENA
        [Range(0.01, float.MaxValue, ErrorMessage = "Cena mora biti veća od 0.")]
        public float? Cena { get; set; }
        [Required]
        public string Slika { get; set; }

        [Required]
        public string Tehnika { get; set; }

        public string Stil { get; set; }

        public string Dimenzije { get; set; }

        [Required]
        public int UmetnikId { get; set; }
    }
}
