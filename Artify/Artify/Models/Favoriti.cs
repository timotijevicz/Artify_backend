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
        [Key]
        public int FavoritiId { get; set; }
        [Required]
        public string KupacId { get; set; }
        [Required]
        public int UmetnickoDeloId { get; set; }

        public virtual Korisnik Kupac { get; set; }
        public virtual UmetnickoDelo UmetnickoDelo { get; set; }
    }
}
