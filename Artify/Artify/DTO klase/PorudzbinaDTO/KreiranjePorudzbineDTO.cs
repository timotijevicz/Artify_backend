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
    public class KreiranjePorudzbineDTO
    {
        [Required]
        public decimal UkupnaCena { get; set; }

        [Required]
        public int KorpaId { get; set; } // ID korpe povezane sa porudžbinom

        public List<int> KupljenaDelaId { get; set; } = new List<int>(); // Lista ID-eva kupljenih umetničkih dela
    }
}
