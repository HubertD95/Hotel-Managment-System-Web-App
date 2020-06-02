using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace inz_web_vol2.Models
{
    public class Daty
    {

        public int Nr_pok { get; set; }
        public List<DatyPoczatekKoniec> datypoczatekkoniec { get; set; }
        public Daty() { }

        public Daty(int nr_pok, DateTime poczatek, DateTime koniec)
        {
            datypoczatekkoniec = new List<DatyPoczatekKoniec>();
            this.Nr_pok = nr_pok;
            datypoczatekkoniec.Add(new DatyPoczatekKoniec(poczatek, koniec));
        }

    }
}