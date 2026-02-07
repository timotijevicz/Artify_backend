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
    public class AzuriranjeUmetnickogDelaDTO
    {
        [Required]
        public int UmetnickoDeloId { get; set; } // ID umetničkog dela za ažuriranje

        public string? Naziv { get; set; }

        [StringLength(500)]
        public string? Opis { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Cena mora biti veća od 0.")]
        public float? Cena { get; set; }
        [Required]
        public string? Slika { get; set; }

        public string? Tehnika { get; set; }

        public string? Stil { get; set; }

        public string? Dimenzije { get; set; } 

        public UmetnickoDeloStatus? Status { get; set; }
    }
}
