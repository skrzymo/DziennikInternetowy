using Dziennik.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dziennik.ViewModels
{
    public class EditClassModel
    {
        public static List<SelectListItem> getFreeTutorsValues()
        {
            DziennikEntities db = new DziennikEntities();
            var classesIds = db.Klasy.Select(k => k.id_wychowawcy).ToArray();
            List<SelectListItem> wychowawcy = db.Nauczyciele.Where(n => !classesIds.Contains(n.id_nauczyciela)).Join(db.Osoby, n => n.id_osoby, o => o.id_osoby,
                                                    (n, o) => new { n, o }).Where(no => no.n.id_osoby == no.o.id_osoby)
                                                    .Select(no => new SelectListItem() { Text = no.o.nazwisko + " " + no.o.imie, Value = no.n.id_nauczyciela.ToString() }).ToList();
            return wychowawcy;
        }

        public static List<SelectListItem> getSubjectTeachers(int id)
        {
            DziennikEntities db = new DziennikEntities();
            List<SelectListItem> teachers = db.Nauczyciele.Where(n => n.id_przedmiotu == id).Select(n => new SelectListItem() { Text = n.Osoba.nazwisko + " " + n.Osoba.imie, Value = n.id_nauczyciela.ToString() }).ToList();
            return teachers;
        }
    }
}