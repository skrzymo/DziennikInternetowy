using Dziennik.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dziennik.ViewModels
{
    public class CreateTeacherModel
    {
        public int subjectId { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string phone { get; set; }
        public DateTime? birthDate { get; set; }

        public static List<SelectListItem> GetSubjectsValues()
        {
            DziennikEntities db = new DziennikEntities();

            List<SelectListItem> subjects = db.Przedmioty.OrderBy(s => s.nazwa).Select(s => new SelectListItem() { Text = s.nazwa, Value = s.id_przedmiotu.ToString() }).ToList();
           
            return subjects;
        }

    }
}