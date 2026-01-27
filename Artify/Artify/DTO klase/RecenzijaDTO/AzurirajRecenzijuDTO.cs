using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Artify.Models;
using System.ComponentModel.DataAnnotations;

namespace Artify.DTO_klase.RecenzijaDTO
{
    public class AzurirajRecenzijuDTO
    {
        [Required]
        public int RecenzijaId { get; set; }

        [Range(1, 5, ErrorMessage = "Ocena mora biti između 1 i 5.")]
        public int? NovaOcena { get; set; }

        [StringLength(200, ErrorMessage = "Komentar ne može imati više od 200 karaktera.")]
        public string NoviKomentar { get; set; }
    }
}
