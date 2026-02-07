using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Artify.Models;
using System.ComponentModel.DataAnnotations;

namespace Artify.DTO_klase.AukcijskaPonudaDTO
{
    public class DodajPonuduDTO
    {
        [Required]
        public int UmetnickoDeloId { get; set; }

        [Required]
        [Range(0.01, 999999999)]
        public float? Iznos { get; set; }
    }
}
