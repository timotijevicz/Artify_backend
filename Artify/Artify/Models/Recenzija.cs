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
    public class Recenzija
    {
        public int RecenzijaId { get; set; }
        [Required]
        public string KorisnikId { get; set; }
        [Required]
        public int UmetnickoDeloId { get; set; }
        [Required]
        public int Ocena {  get; set; } // ocena 1-5
        [Required]
        [StringLength(200)]
        public string Komentar {  get; set; }
        [Required]
        public virtual Korisnik? Korisnik { get; set; }
        public virtual UmetnickoDelo UmetnickoDelo { get; set; }
    }
}
