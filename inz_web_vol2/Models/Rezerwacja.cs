using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace inz_web_vol2.Models
{
    public class Rezerwacja
    {
        [Required]
        public string Imie { get; set; }
        [Required]
        public string Nazwisko { get; set; }
        [Required]
        public string Numer { get; set; }
        public int Posilki { get; set; }    
        public string Opis { get; set; }
    }
}