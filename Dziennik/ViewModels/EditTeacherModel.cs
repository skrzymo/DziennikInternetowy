using Dziennik.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dziennik.ViewModels
{
    public class EditTeacherModel
    {
        public static List<SelectListItem> getAllSubjects(int? id)
        {
            DziennikEntities db = new DziennikEntities();
            List<SelectListItem> subjects = new List<SelectListItem>();
            if (id != null)
            {
                var actualSubject = db.Przedmioty.Find(id);
                var przedmioty = db.Przedmioty.Where(k => k.id_przedmiotu != id);

                subjects.Add(new SelectListItem() { Text = actualSubject.nazwa, Value = actualSubject.id_przedmiotu.ToString() });
                foreach (Przedmiot przedmiot in przedmioty)
                {
                    subjects.Add(new SelectListItem() { Text = przedmiot.nazwa, Value = przedmiot.id_przedmiotu.ToString() });
                }
            }
            else
            {
                foreach (Przedmiot przedmiot in db.Przedmioty)
                {
                    subjects.Add(new SelectListItem() { Text = przedmiot.nazwa, Value = przedmiot.id_przedmiotu.ToString() });
                }
            }


            return subjects;
        }

        public static int? getSubjectId(int id)
        {
            DziennikEntities db = new DziennikEntities();
            var teacherId = db.Nauczyciele.Where(u => u.id_osoby == id).FirstOrDefault();
            return teacherId.id_przedmiotu;
        }

        public static string getSubjectName(int id)
        {
            DziennikEntities db = new DziennikEntities();
            var teacher = db.Nauczyciele.Find(id);
            var klasa = db.Klasy.Where(k => k.id_wychowawcy == teacher.id_nauczyciela).FirstOrDefault();
            
            if(klasa == null)
            {
                return "";
            }

            return "Wychowawca klasy " + klasa.nazwa;
        }
    }
}