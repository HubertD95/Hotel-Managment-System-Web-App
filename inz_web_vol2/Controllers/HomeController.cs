using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;
using inz_web_vol2.Models;

namespace inz_web_vol2.Controllers
{
    
    public class HomeController : Controller
    {
        int Nr_pok;
        string mainconn = "Data Source=localhost;Initial Catalog=inzynierka;User Id=root;password=''";
        List<MySqlClass> list = new List<MySqlClass>();
        List<MySqlClass> WolnePokoje = new List<MySqlClass>();
        public List<Daty> daty { get; set; } = new List<Daty>();
        DateTime pDate, kDate;

        string Data_poczatek, Data_koniec;

        public ActionResult Index(string error)
        {
            ViewBag.Error = error;
            return View();
        }

        public ActionResult Pokoje(string datepicker, string datepickerr)
        {
            
            Data_poczatek = FormatujDate(datepicker);
            Data_koniec = FormatujDate(datepickerr);
            //if (SprawdzDaty(datepicker, datepickerr))
            //{
                ViewBag.data_poczatek = Data_poczatek;
                ViewBag.data_koniec = Data_koniec;
                pDate = Convert.ToDateTime(Data_poczatek);
                kDate = Convert.ToDateTime(Data_koniec);
            if (pDate < kDate)
            {
                PobierzPokoje();
                PobierzRezerwacje();
                return View(WolnePokoje);
            }
            else
            {
                ViewBag.Error = "Podano złe daty";
                return RedirectToAction("Index");
            }

                
            //}
            //else
            //{
            //    return RedirectToAction("Index", new { error = ViewBag.Error });
            //}
        }

        public ActionResult Rezerwuj(int id, string data1, string data2, string error, string imie, string nazwisko, string nrtel, string opis)
        {
            ViewBag.Opis = opis;
            ViewBag.Imie = imie;
            ViewBag.Nazwisko = nazwisko;
            ViewBag.Numer = nrtel;
            ViewBag.Error = error;
            ViewBag.data_poczatek = data1;
            ViewBag.data_koniec = data2;
            ViewBag.Nr_pok = id;
            return View();
        }

        public ActionResult DodajRezerwacje(string imie, string nazwisko, string nrtel, string Select, string data1, string data2, int nr_pok, string opis)
        {
            if(SprawdzPola(imie, nazwisko, nrtel)) { return RedirectToAction("Rezerwuj", new { id = nr_pok, data1, data2, error = ViewBag.Error, imie, nazwisko, nrtel, opis }); }
            
            MySqlConnection mysql = new MySqlConnection(mainconn);
            string query = "INSERT INTO rezerwacje (Imie, Nazwisko, Nr_pokoju, Potwierdzone, Data_poczatek, Data_koniec, Posilki, Opis) " +
                "VALUES('" + imie.ToString() + "', '" + nazwisko.ToString() + "', '" + nr_pok + "', '0', '" + data1.ToString() + "', '" + data2.ToString() + "', '" + Convert.ToInt32(Select) + "', '" + opis.ToString() + "'); ";
            MySqlCommand comm = new MySqlCommand(query);
            comm.Connection = mysql;
            try
            {
                mysql.Open();
            }
            catch (Exception e)
            {

            }
            comm.ExecuteNonQuery();
            mysql.Close();

            return RedirectToAction("Index");
        }




        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        private bool SprawdzDaty(string data1, string data2)
        {
            try
            {
                DateTime myDate1 = DateTime.ParseExact(data1, "dd-MM-yyyy",
                                           System.Globalization.CultureInfo.InvariantCulture);
                DateTime myDate2 = DateTime.ParseExact(data2, "dd-MM-yyyy",
                                           System.Globalization.CultureInfo.InvariantCulture);

                if (myDate1 > myDate2)
                {
                    ViewBag.Error = "Podano nieodpowiednie daty";
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                ViewBag.Error = "Zły format dat (dd-mm-yyyy lub d-m-yyyy)";
                return false;
            }
        }

        private string FormatujDate (string datain)
        {
            bool flaga1 = true, flaga2 = true, flaga3 = true;
            string day = null, month = null, year = null, nday = "0", nmonth = "0", data = null, split = "-", data_poczatek = null, data_koniec = null;
            data_poczatek = datain.ToString();

            //try
            //{
                foreach (char c in data_poczatek)
                {
                    if (c != '-')
                    {
                        if (flaga1) { day += c; }
                        else if (flaga2) { month += c; }
                        else if (flaga3) { year += c; }

                    }
                    else if (flaga1) { flaga1 = !flaga1; }
                    else if (flaga2) { flaga2 = !flaga2; }
                    else if (flaga3) { flaga3 = !flaga3; }
                }

                if (day.Length == 1) { nday += day; }
                else { nday = day; }
                if (month.Length == 1) { nmonth += month; }
                else { nmonth = month; }
                data += year += split += nmonth += split += nday;
            //}
            //catch (Exception e)
            //{
            //    ViewBag.Error = "Zły format dat (dd-mm-yyyy lub d-m-yyyy)";
            //    DoIndex();
            //}
            return data;
        }
        private RedirectToRouteResult DoIndex()
        {
            return Index(ViewBag.Error);
        }
        private void PobierzPokoje()
        {
            MySqlConnection mysql = new MySqlConnection(mainconn);
            string query = "SELECT * FROM pokoje";
            MySqlCommand comm = new MySqlCommand(query);
            comm.Connection = mysql;
            try
            {
                mysql.Open();
            }
            catch (Exception e)
            {

            }
            MySqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new MySqlClass
                {
                    Nr_pok = Convert.ToInt32(dr["Nr_pok"]),
                    Czy_zajety = Convert.ToInt32(dr["Czy_zajety"]),
                    Opis = dr["Opis"].ToString(),
                    Typ = dr["Typ"].ToString(),
                    Cena = Convert.ToInt32(dr["Cena"])
                });
            }
            mysql.Close();
        }

        private void PobierzRezerwacje()
        {
            bool flag = false, flagaprzeciecie = false;

            MySqlConnection mysql = new MySqlConnection(mainconn);
            string query = "SELECT * FROM rezerwacje";
            MySqlCommand comm = new MySqlCommand(query);
            comm.Connection = mysql;
            try
            {
                mysql.Open();
            }
            catch (Exception e)
            {

            }
            MySqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                foreach (var x in daty)
                {
                    if (x.Nr_pok == Convert.ToInt32(dr["Nr_pokoju"]))
                    {
                        flag = true;
                        x.datypoczatekkoniec.Add(new DatyPoczatekKoniec(Convert.ToDateTime(dr["Data_poczatek"]), Convert.ToDateTime(dr["Data_koniec"])));
                    }
                }
                if (flag == false)
                {
                    daty.Add(new Daty(Convert.ToInt32(dr["Nr_pokoju"]), Convert.ToDateTime(dr["Data_poczatek"]), Convert.ToDateTime(dr["Data_koniec"])));
                }
            }
            mysql.Close();

            foreach (MySqlClass x in list)
            {
                foreach (Daty d in daty)
                {
                    if (x.Nr_pok == d.Nr_pok)
                    {
                        foreach (var dpk in d.datypoczatekkoniec)
                        {
                            if (dpk.Poczatek >= Convert.ToDateTime(Data_poczatek) && dpk.Poczatek <= Convert.ToDateTime(Data_koniec)) { flagaprzeciecie = true; }
                            if (dpk.Koniec >= Convert.ToDateTime(Data_poczatek) && dpk.Poczatek <= Convert.ToDateTime(Data_koniec)) { flagaprzeciecie = true; }
                        }
                    }
                }

                if (flagaprzeciecie == false) { WolnePokoje.Add(x); }
                flagaprzeciecie = false;
            }
        }

        private bool SprawdzPola(string imie, string nazwisko, string nrtel)
        {
            if (imie.Length == 0)
            {
                ViewBag.Error = "Pole IMIE nie może być puste";
                return true;
            }
            if (nazwisko.Length == 0)
            {
                ViewBag.Error = "Pole NAZWISKO nie może być puste";
                return true;
            }
            if (nrtel.Length == 0)
            {
                ViewBag.Error = "Pole NUMER TELEFONU nie może być puste";
                return true;
            }
            if (nrtel.Length < 9 || nrtel.Length > 11)
            {
                ViewBag.Error = "Zły numer telefonu";
                return true;
            }
            foreach (Char c in imie)
            {
                if (Char.IsNumber(c)) { ViewBag.Error = "Pole IMIE nie powinno zawierać cyfr"; return true; }

            }
            foreach (Char c in nazwisko)
            {
                if (Char.IsNumber(c)) { ViewBag.Error = "Pole NAZWISKO nie powinno zawierać cyfr"; return true; }
            }
            foreach (Char c in nrtel)
            {
                if (Char.IsLetter(c)) { ViewBag.Error = "Pole NUMER TELEFONU nie powinno zawierać liter"; return true; }
            }

            return false;
        }

        //public class Daty
        //{
        //    public int Nr_pok { get; set; }
        //    public List<DatyPoczatekKoniec> datypoczatekkoniec { get; set; }
        //    public Daty() { }

        //    public Daty(int nr_pok, DateTime poczatek, DateTime koniec)
        //    {
        //        datypoczatekkoniec = new List<DatyPoczatekKoniec>();
        //        this.Nr_pok = nr_pok;
        //        datypoczatekkoniec.Add(new DatyPoczatekKoniec(poczatek, koniec));
        //    }
        //}

        //public class DatyPoczatekKoniec
        //{
        //    public DateTime Poczatek { get; set; }
        //    public DateTime Koniec { get; set; }
        //    public DatyPoczatekKoniec() { }

        //    public DatyPoczatekKoniec(DateTime poczatek, DateTime koniec)
        //    {
        //        this.Poczatek = poczatek;
        //        this.Koniec = koniec;
        //    }
        //}
    }
}