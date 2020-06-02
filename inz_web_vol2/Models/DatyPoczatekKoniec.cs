using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace inz_web_vol2.Models
{
    public class DatyPoczatekKoniec
    {
        public DateTime Poczatek { get; set; }
        public DateTime Koniec { get; set; }
        public DatyPoczatekKoniec() { }

        public DatyPoczatekKoniec(DateTime poczatek, DateTime koniec)
        {
            this.Poczatek = poczatek;
            this.Koniec = koniec;
        }

    }
}