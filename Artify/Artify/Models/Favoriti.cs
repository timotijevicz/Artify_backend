using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Artify.Models;
using System.ComponentModel.DataAnnotations;

namespace Artify.Models
{
    public class Favoriti
    {
        public int FavoritiId { get; set; }

        [Required]
        public string KorisnikId { get; set; }

        public Korisnik Korisnik { get; set; }

        [Required]
        public int UmetnickoDeloId { get; set; }

        public UmetnickoDelo UmetnickoDelo { get; set; }
    }
}
