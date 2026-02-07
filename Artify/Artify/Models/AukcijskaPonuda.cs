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
    public class AukcijskaPonuda
    {
        [Key]
        public int AukcijskaPonudaId { get; set; }

        [Required]
        public int UmetnickoDeloId { get; set; }

        [ForeignKey(nameof(UmetnickoDeloId))]
        public UmetnickoDelo UmetnickoDelo { get; set; } = null!;

        // Identity user id (Claim nameidentifier)
        [Required]
        public string KupacId { get; set; } = string.Empty;

        [Column(TypeName = "real")]
        public float? Iznos { get; set; }

        public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;
    }
}
