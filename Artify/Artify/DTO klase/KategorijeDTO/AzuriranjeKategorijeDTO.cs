using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Artify.Models;
using System.ComponentModel.DataAnnotations;

namespace Artify.DTO_klase.KategorijeDTO
{
    public class AzuriranjeKategorijeDTO
    {
        [Required]
        public int KategorijaId { get; set; }

        [Required]
        [StringLength(100)]
        public string Naziv { get; set; }
    }
}
