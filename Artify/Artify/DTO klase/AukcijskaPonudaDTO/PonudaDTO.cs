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
    public class PonudaDTO
    {
        public int PonudaId { get; set; }
        public int UmetnickoDeloId { get; set; }
        public string KupacId { get; set; } = "";
        public float? Iznos { get; set; }
        public DateTime DatumKreiranja { get; set; }

        // ✅ NOVO
        public string? KupacIme { get; set; }
    }
}
