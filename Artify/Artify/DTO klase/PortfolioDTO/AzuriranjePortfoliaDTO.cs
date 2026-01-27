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
    public class AzuriranjePortfoliaDTO
    {
        [Required]
        public int PortfolioId { get; set; }  // ID portfolia koji se ažurira

        [Required]
        [StringLength(100)]
        public string Naziv { get; set; }  
        [Required]
        [StringLength(300)]
        public string Opis { get; set; }  

        public List<int> UmetnickaDelaId { get; set; } = new List<int>();  // Lista novih umetničkih dela koja će biti dodeljena portfoliu
    }
}
