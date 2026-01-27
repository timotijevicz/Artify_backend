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
    public class UmetnickoDelo
    {
        public int UmetnickoDeloId { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; } 
        public decimal Cena { get; set; }
        public string Slika { get; set; } 
        public DateTime DatumPostavljanja { get; set; } = DateTime.UtcNow;
        public string Tehnika { get; set; }  // (ulje na platnu, akvarel, itd.)
        public int? KategorijaId { get; set; }
        public virtual Kategorija Kategorija { get; set; }

        public string Stil { get; set; }  //  ( impresionizam, realizam)
        public string Dimenzije {  get; set; }  // (sirina * visina)
        public UmetnickoDeloStatus Status {  get; set; } 

        public string UmetnikId { get; set; }
        public virtual Korisnik Umetnik {  get; set; }
      
    }

    public enum UmetnickoDeloStatus
    {
        Dostupno,
        Prodato,
        Uklonjeno
    }
}
