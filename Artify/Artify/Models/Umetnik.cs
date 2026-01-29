using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Artify.Models;
using System.ComponentModel.DataAnnotations;

namespace Artify.Models
{
    public class Umetnik
    {
        public int UmetnikId { get; set; }  // int PK

        [Required]
        public string KorisnikId { get; set; }  // FK na Korisnik
        public virtual Korisnik Korisnik { get; set; }

        [StringLength(500)]
        public string? Biografija { get; set; }
        public string? Tehnika { get; set; }
        public string? Stil { get; set; }
        public string? Specijalizacija { get; set; }
        public string? SlikaUrl { get; set; }

        public bool IsApproved { get; set; } = false;
        public bool IsAvailable { get; set; } = false;

        public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;

        public virtual ICollection<UmetnickoDelo>? UmetnickaDela { get; set; }
            = new List<UmetnickoDelo>();
    }

}

