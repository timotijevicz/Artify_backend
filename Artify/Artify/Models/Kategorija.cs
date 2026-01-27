using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Artify.Models;

namespace Artify.Models
{
    public class Kategorija
    {
        public int KategorijaId { get; set; }

        [Required]
        [StringLength(100)]
        public string Naziv { get; set; } 
    }
}
