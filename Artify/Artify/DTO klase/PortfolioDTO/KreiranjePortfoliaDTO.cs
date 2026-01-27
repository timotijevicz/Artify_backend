using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Artify.Models;
using System.ComponentModel.DataAnnotations;

namespace Artify.DTO_klase.PortfolioDTO
{
    public class KreiranjePortfoliaDTO
    {
        [Required]
        [StringLength(100)]
        public string Naziv { get; set; }  

        [Required]
        [StringLength(300)]
        public string Opis { get; set; }  

        [Required]
        public string UmetnikId { get; set; }  // ID umetnika koji kreira portfolio

        public List<int> UmetnickaDelaId { get; set; } = new List<int>();  // Lista ID-eva umetničkih dela koja pripadaju ovom portfolio
    }
}
