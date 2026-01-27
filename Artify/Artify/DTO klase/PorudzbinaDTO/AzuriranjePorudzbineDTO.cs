using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Artify.Models;
using System.ComponentModel.DataAnnotations;


namespace Artify.DTO_klase.PorudzbinaDTO
{
    public class AzuriranjePorudzbineDTO
    {
        [Required]
        public int PorudzbinaId { get; set; } // ID porudžbine koja se menja
        [Required]
        public decimal? NovaUkupnaCena { get; set; } // Opcionalno ažuriranje cene
        [Required]
        public PorudzbinaStatus? NoviStatus { get; set; } // Opcionalno ažuriranje statusa porudžbine
        public List<int>? NovaKupljenaDelaId { get; set; } // Opcionalno ažuriranje umetničkih dela
    }
}
