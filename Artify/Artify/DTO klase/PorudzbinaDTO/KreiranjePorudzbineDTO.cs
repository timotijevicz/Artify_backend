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
        public int UmetnickoDeloId { get; set; }

        [Required]
        public string KorisnikId { get; set; }
    }
}
