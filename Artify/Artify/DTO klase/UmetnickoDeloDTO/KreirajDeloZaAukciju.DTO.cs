using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Artify.Models;
using System.ComponentModel.DataAnnotations;

namespace Artify.DTO_klase.UmetnickoDeloDTO
{
    public class KreirajDeloZaAukcijuDTO
    {
        [Required]
        public string Naziv { get; set; }

        [Required]
        public string Opis { get; set; }

        public float PocetnaCenaAukcije { get; set; }

        public DateTime AukcijaZavrsava { get; set; }

        public string Slika { get; set; }

        [Required]
        public string Tehnika { get; set; }

        public string Stil { get; set; }
        public string Dimenzije { get; set; }

        [Required]
        public int UmetnikId { get; set; }
    }
}
