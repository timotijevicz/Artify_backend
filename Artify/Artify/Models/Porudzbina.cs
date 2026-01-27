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

        public decimal UkupnaCena { get; set; }
        public PorudzbinaStatus Status { get; set; } 
        public int KorpaId { get; set; }
        public virtual Korpa Korpa { get; set; } 

        public virtual List<UmetnickoDelo> KupljenaDela { get; set; } = new List<UmetnickoDelo>();

    }

    public enum PorudzbinaStatus
    {
        Obrada,
        Poslato,
        Dostavljeno
    }
}
