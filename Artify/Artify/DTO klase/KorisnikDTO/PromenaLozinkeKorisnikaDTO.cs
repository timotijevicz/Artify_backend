using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Artify.Models;
using System.ComponentModel.DataAnnotations;

namespace Artify.DTO_klase.KorisnikDTO
{
    public class PromenaLozinkeKorisnikaDTO
    {
        [Required]
        public string KorisnikId { get; set; }

        [Required]
        public string TrenutnaLozinka { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string NovaLozinka { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        [Compare("NovaLozinka")]
        public string PotvrdaNoveLozinke { get; set; }
    }
}
