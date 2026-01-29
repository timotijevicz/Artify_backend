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
    public class AzurirajUmetnikaDTO
    {
        [Required]
        public int? UmetnikId { get; set; }

        [StringLength(500)]
        public string? Biografija { get; set; }
        public string? Tehnika { get; set; }
        public string? Stil { get; set; }
        public string? Specijalizacija { get; set; }
        public string? SlikaUrl { get; set; }

        public bool? IsApproved { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
