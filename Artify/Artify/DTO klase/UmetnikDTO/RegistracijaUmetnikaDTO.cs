using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Artify.Models;
using System.ComponentModel.DataAnnotations;

namespace Artify.DTO_klase.UmetnikDTO
{
    public class RegistracijaUmetnikaDTO
    {
        // user
        [Required, StringLength(100)]
        public string ImeIPrezime { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(20, MinimumLength = 6)]
        public string Lozinka { get; set; }

        [Required, Compare("Lozinka")]
        public string PotvrdaLozinke { get; set; }

        // artist
        [StringLength(500)]
        public string? Biografija { get; set; }
        public string? Tehnika { get; set; }
        public string? Stil { get; set; }
        public string? Specijalizacija { get; set; }
        public string? Grad { get; set; }
        public string? SlikaUrl { get; set; }
    }
}
