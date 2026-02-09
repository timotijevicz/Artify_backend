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
    public class Porudzbina
    {
        public int PorudzbinaId { get; set; }
        public DateTime DatumKreiranja { get; set; } = DateTime.Now;

        public float CenaUTrenutkuKupovine { get; set; } = 0f;
        public PorudzbinaStatus Status { get; set; }

        public string KorisnikId { get; set; }
        public virtual Korisnik Korisnik { get; set; }
        public bool Arhivirana { get; set; } = false;

        public int UmetnickoDeloId { get; set; }
        public virtual UmetnickoDelo? UmetnickoDelo { get; set; }
    }

    public enum PorudzbinaStatus
    {
        NaCekanju = 0,
        Odobrena = 1,
        Odbijena = 2,
        Placena = 3,
        Otkazana = 4
    }
}
