using Dziennik.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dziennik.Controllers
{
    public class ParentController : Controller
    {
        DziennikEntities db = new DziennikEntities();

        // GET: Parent
        [Filters.AuthorizeParent]
        public ActionResult Index()
        {
            int personID = Convert.ToInt32(Session["personID"]);
            var parent = db.Rodzice.Where(r => r.id_osoby == personID).FirstOrDefault();
            var childrens = db.Uczniowie.Where(u => u.id_rodzica == parent.id_rodzica);
            var childrenClassID = childrens.Select(c => c.id_klasy).ToArray();
            ViewBag.children = childrens.ToList();
            return View(db.Ogloszenia.Where(o => childrenClassID.Contains(o.id_klasy)).ToList());
        }

        [Filters.AuthorizeParent]
        [HttpGet]
        public ActionResult PersonalDetails(int id)
        {
            return View(db.Osoby.Where(o => o.id_osoby == id).FirstOrDefault());
        }

        [Filters.AuthorizeParent]
        [HttpGet]
        public ActionResult EditPersonalDetails(int id)
        {
            return View(db.Osoby.Where(o => o.id_osoby == id).FirstOrDefault());
        }

        [Filters.AuthorizeParent]
        [HttpPost]
        public ActionResult EditPersonalDetails(int id, Osoba osoba)
        {
            if (ModelState.IsValid)
            {
                var person = db.Osoby.AsNoTracking().Where(o => o.id_osoby == id).FirstOrDefault();

                osoba.data_urodzenia = person.data_urodzenia;
                osoba.data_hasla = person.data_hasla;
                osoba.uprawnienia = person.uprawnienia;
                osoba.email = person.email;

                db.Entry(osoba).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("PersonalDetails/" + id);
            }

            return View();
        }

        [Filters.AuthorizeParent]
        [HttpGet]
        public ActionResult ChooseSubject()
        {
            int personID = Convert.ToInt32(Session["personID"]);
            var parent = db.Rodzice.Where(p => p.id_osoby == personID).FirstOrDefault();
            var student = db.Uczniowie.Where(u => u.id_rodzica == parent.id_rodzica).FirstOrDefault();
            Session["studentID"] = student.id_ucznia;
            List<SelectListItem> subjects = db.Klasy.Where(k => k.id_klasy == student.id_klasy).Join(db.PrzedmiotyWKlasach, k => k.id_klasy, p => p.id_klasy,
                            (k, p) => new { k, p }).Where(kp => kp.k.id_klasy == kp.p.id_klasy).Select(kp => new SelectListItem() { Text = kp.p.Przedmiot.nazwa, Value = kp.p.id_przedmiotu.ToString() }).ToList();

            ViewBag.studentSubjects = subjects;

            return View();    
        }

        [Filters.AuthorizeParent]
        [HttpPost]
        public ActionResult ChooseSubject(FormCollection formCollection)
        {
            Session["choosenSubject"] = formCollection["id_przedmiotu"];

            return RedirectToAction("GradesReview");
        }

        [Filters.AuthorizeParent]
        [HttpGet]
        public ActionResult GradesReview()
        {
            int studentID = Convert.ToInt32(Session["studentID"]);
            int subjectID = Convert.ToInt32(Session["choosenSubject"]);

            ViewBag.subjectName = db.Przedmioty.Find(subjectID).nazwa;
            ViewBag.studentName = db.Uczniowie.Find(studentID).Osoba.FullName;
            ViewBag.studentClass = db.Uczniowie.Find(studentID).Klasa.nazwa;

            DateTime from = Convert.ToDateTime(Session["dateFrom"]);
            DateTime to = Convert.ToDateTime(Session["dateTo"]);

            if(from == DateTime.MinValue || to == DateTime.MinValue)
            {
                return View(db.Oceny.Where(o => o.id_przedmiotu == subjectID && o.id_ucznia == studentID).ToList());
            }

            Session["dateFrom"] = DateTime.MinValue;
            Session["dateTo"] = DateTime.MinValue;

            if (from > to)
            {
                ModelState.AddModelError("dateRange", "Data 'od' nie może być późniejsza niż 'do'!");
            }

            return View(db.Oceny.Where(o => o.id_przedmiotu == subjectID && o.id_ucznia == studentID && o.data <= to && o.data >= from).ToList());

        }

        [Filters.AuthorizeParent]
        [HttpPost]
        public ActionResult GradesReview(FormCollection formCollection)
        {
            Session["dateFrom"] = formCollection["dataOd"];
            Session["dateTo"] = formCollection["dataDo"];

            return RedirectToAction("GradesReview");
        }

        [Filters.AuthorizeParent]
        [HttpGet]
        public ActionResult Messages()
        {
            int personID = Convert.ToInt32(Session["personID"]);
            var parent = db.Rodzice.Where(r => r.id_osoby == personID).FirstOrDefault();
            ViewBag.children = db.Uczniowie.Where(u => u.id_rodzica == parent.id_rodzica).ToList();

            return View(db.Wiadomosci.Where(w => w.id_rodzica == parent.id_rodzica && w.czyRodzic == true).OrderBy(w => w.data).ToList());
        }

        [Filters.AuthorizeParent]
        [HttpGet]
        public ActionResult CreateMessage()
        {
            List<SelectListItem> teachers = db.Nauczyciele.OrderBy(n => n.Osoba.nazwisko).OrderBy(w => w.Osoba.imie)
                                            .Select(n => new SelectListItem() { Text = n.Osoba.nazwisko + " " + n.Osoba.imie, Value = n.id_nauczyciela.ToString() }).ToList();

            ViewBag.teachers = teachers;
            return View();
        }

        [Filters.AuthorizeParent]
        [HttpPost]
        public ActionResult CreateMessage(FormCollection formCollection)
        {
            if(ModelState.IsValid)
            {
                int personID = Convert.ToInt32(Session["personID"]);
                var parent = db.Rodzice.Where(r => r.id_osoby == personID).FirstOrDefault();

                Wiadomosc wiadomosc = new Wiadomosc();

                wiadomosc.tytul = formCollection["tytul"];
                wiadomosc.tresc = formCollection["tresc"];
                wiadomosc.id_rodzica = parent.id_rodzica;
                wiadomosc.id_nauczyciela = Convert.ToInt32(formCollection["id_nauczyciela"]);
                wiadomosc.data = DateTime.Now;
                wiadomosc.czyNauczyciel = true;
                wiadomosc.czyRodzic = true;

                db.Wiadomosci.Add(wiadomosc);
                db.SaveChanges();

                return RedirectToAction("Messages");

            }


            return View();
        }

        [Filters.AuthorizeParent]
        [HttpGet]
        public ActionResult MessageDetails(int id)
        {
            return View(db.Wiadomosci.Where(o => o.id_wiadomosci == id).FirstOrDefault());
        }

        [Filters.AuthorizeParent]
        public ActionResult DeleteMessage(int id)
        {
            var message = db.Wiadomosci.Where(w => w.id_wiadomosci == id).FirstOrDefault();

            message.czyRodzic = false;

            db.Entry(message).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Messages");
        }
    }
}