using Dziennik.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dziennik.ViewModels
{
    public class CreateStudentModel
    {
        public int parentId { get; set; }
        public int classId { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string phone { get; set; }
        public DateTime? birthDate { get; set; }

        public static List<SelectListItem> GetClassesValues()
        {
            DziennikEntities db = new DziennikEntities();

            List<SelectListItem> classes = db.Klasy.OrderBy(k => k.nazwa).Select(s => new SelectListItem() { Text = s.nazwa, Value = s.id_klasy.ToString() }).ToList();

            return classes;
        }

        public static List<SelectListItem> GetParentsValues()
        {
            DziennikEntities db = new DziennikEntities();

            List<SelectListItem> parents = db.Rodzice.OrderBy(r => r.Osoba.nazwisko).Select(r => new SelectListItem() { Text = r.Osoba.nazwisko + " " + r.Osoba.imie, Value = r.id_rodzica.ToString() }).ToList();

            return parents;
        }
    }
}