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
    public class Korisnik : IdentityUser
    {
        public string? ImeIPrezime { get; set; }

        // Token potreban samo za reset lozinke
        public string? TokenZaResetovanjeLozinke { get; set; }
        public DateTime? VremeIstekaTokenaZaReset { get; set; }

        public DateTime DatumRegistracije {  get; set; }
        public virtual ICollection<UmetnickoDelo> UmetnickaDela { get; set; } = new List<UmetnickoDelo>();
    }
}
