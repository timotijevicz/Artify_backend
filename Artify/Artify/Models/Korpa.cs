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
    public class Korpa
    {
        [Key]
        public int KorpaId { get; set; }
        public virtual Korisnik Kupac { get; set; } = null!;
        [Required]
        public string KupacId { get; set; }

        public ICollection<Porudzbina> Porudzbine { get; set; } 
    }
}
