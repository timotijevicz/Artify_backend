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
    public class RegistracijaKorisnikaDTO
    {
        [Required]
        [StringLength(100)]
        public string ImeIPrezime { get; set; }


        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string Lozinka { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        [Compare("Lozinka")]
        public string PotvrdaLozinke { get; set; }
    }
}
