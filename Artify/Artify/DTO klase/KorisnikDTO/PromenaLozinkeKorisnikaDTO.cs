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
        [System.ComponentModel.DataAnnotations.Required]
        public string TrenutnaLozinka { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(20, MinimumLength = 6)]
        public string NovaLozinka { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.Compare("NovaLozinka")]
        public string PotvrdaNoveLozinke { get; set; } = string.Empty;
    }
}
