using Dziennik.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dziennik.ViewModels
{
    public class EditStudentModel
    {
        public static List<SelectListItem> getAllClasses(int? id)
        {
            DziennikEntities db = new DziennikEntities();
            List<SelectListItem> classes = new List<SelectListItem>();
            if (id != null)
            {
                var actualClass = db.Klasy.Find(id);
                var klasy = db.Klasy.Where(k => k.id_klasy != id);
                
                classes.Add(new SelectListItem() { Text = actualClass.nazwa, Value = actualClass.id_klasy.ToString() });
                foreach (Klasa klasa in klasy)
                {
                    classes.Add(new SelectListItem() { Text = klasa.nazwa, Value = klasa.id_klasy.ToString() });
                }
            }
            else
            {
                foreach (Klasa klasa in db.Klasy)
                {
                    classes.Add(new SelectListItem() { Text = klasa.nazwa, Value = klasa.id_klasy.ToString() });
                }
            }
            
            
            return classes;
        }

        public static int? getClassId(int id)
        {
            DziennikEntities db = new DziennikEntities();
            var studentId = db.Uczniowie.Where(u => u.id_osoby == id).FirstOrDefault();
            return studentId.id_klasy;
        }
    }
}