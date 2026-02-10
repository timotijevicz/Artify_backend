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
    public class PromenaEmailKorisnikaDTO
    {
        [Required]
        [EmailAddress]
        public string NoviEmail { get; set; } = string.Empty;

        // radi sigurnosti traži lozinku
        [Required]
        public string Lozinka { get; set; } = string.Empty;
    }
}
