using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Artify.Models;
using System.ComponentModel.DataAnnotations;


namespace Artify.DTO_klase.NotifikacijaDTO
{
    public class KreirajNotifikacijuDTO
    {
        [Required]
        public string KorisnikId { get; set; }

        [Required]
        [StringLength(300)]
        public string? Poruka { get; set; }

        public NotifikacijaTip Tip { get; set; } = NotifikacijaTip.Obavestenje;

        public int? PorudzbinaId { get; set; }
        public int? UmetnickoDeloId { get; set; }
    }
}
