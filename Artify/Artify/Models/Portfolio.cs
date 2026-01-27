using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Artify.Models;
using System.ComponentModel.DataAnnotations;
using Artify.DTO_klase.PortfolioDTO;

namespace Artify.Models
{
    public class Portfolio
    {
        public int PortfolioId {  get; set; }

        [Required]
        public string Naziv {  get; set; }  // kom stilu pripada, tako je pravljenja zbirka
        [Required]
        [StringLength(300)]
        public string Opis {  get; set; } 
        public virtual Korisnik Umetnik { get; set; }
        [Required]
        public string UmetnikId { get; set; }

        public virtual List<UmetnickoDelo> UmetnickaDela { get; set; } = new List<UmetnickoDelo>();
    }
}
