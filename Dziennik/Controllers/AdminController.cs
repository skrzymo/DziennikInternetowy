using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dziennik.Models;

namespace Dziennik.Controllers
{
    public class AdminController : Controller
    {
        DziennikEntities db = new DziennikEntities();

        // GET: Admin
        [Filters.AuthorizeAdmin]
        public ActionResult Index()
        {
            return View();
        }

        [Filters.AuthorizeAdmin]
        public ActionResult Classes()
        {
            return View(db.Klasy.ToList());
        }

        [Filters.AuthorizeAdmin]
        [HttpGet]
        public ActionResult CreateClass()
        {
            return View();   
        }

        [Filters.AuthorizeAdmin]
        [HttpPost]
        public ActionResult CreateClass(FormCollection formCollection)
        {
            Klasa klasa = new Klasa();
            klasa.nazwa = formCollection["nazwa"];
            klasa.id_wychowawcy = Int32.Parse(formCollection["tutorId"]);

            db.Klasy.Add(klasa);

            db.SaveChanges();

            return RedirectToAction("Classes");
        }

        [Filters.AuthorizeAdmin]
        public ActionResult Teachers()
        {
            return View(db.Nauczyciele.OrderBy(n => n.Osoba.nazwisko).ToList());
        }

        [Filters.AuthorizeAdmin]
        [HttpGet]
        public ActionResult CreateTeacher()
        {
            return View();
        }

        [Filters.AuthorizeAdmin]
        [HttpPost]
        public ActionResult CreateTeacher(FormCollection formCollection)
        {
            Osoba osoba = new Osoba();
            osoba.imie = formCollection["imie"];
            osoba.nazwisko = formCollection["nazwisko"];
            osoba.adres = formCollection["adres"];
            osoba.email = formCollection["email"];
            osoba.haslo = formCollection["haslo"];
            osoba.telefon = formCollection["telefon"];
            osoba.data_urodzenia = Convert.ToDateTime(formCollection["dataUrodzenia"]);
            DateTime dataHasla = DateTime.Now;
            osoba.data_hasla = dataHasla;
            osoba.uprawnienia = "nauczyciel";

            db.Osoby.Add(osoba);

            db.SaveChanges();

            int lastPersonId = osoba.id_osoby;

            Nauczyciel nauczyciel = new Nauczyciel();
            nauczyciel.id_osoby = lastPersonId;
            nauczyciel.id_przedmiotu = Int32.Parse(formCollection["subjectId"]);

            db.Nauczyciele.Add(nauczyciel);

            db.SaveChanges();

            return RedirectToAction("Teachers");
        }

        [Filters.AuthorizeAdmin]
        public ActionResult Students()
        {
            return View(db.Uczniowie.OrderBy(n => n.Klasa.nazwa).OrderBy(n => n.Osoba.nazwisko).ToList());
        }

        [Filters.AuthorizeAdmin]
        [HttpGet]
        public ActionResult CreateStudent()
        {
            return View();
        }

        [Filters.AuthorizeAdmin]
        [HttpPost]
        public ActionResult CreateStudent(FormCollection formCollection)
        {
            Osoba osoba = new Osoba();
            osoba.imie = formCollection["imie"];
            osoba.nazwisko = formCollection["nazwisko"];
            osoba.adres = formCollection["adres"];
            osoba.email = formCollection["email"];
            osoba.haslo = formCollection["haslo"];
            osoba.telefon = formCollection["telefon"];
            osoba.data_urodzenia = Convert.ToDateTime(formCollection["dataUrodzenia"]);
            DateTime dataHasla = DateTime.Now;
            osoba.data_hasla = dataHasla;
            osoba.uprawnienia = "uczen";

            db.Osoby.Add(osoba);

            db.SaveChanges();

            int lastPersonId = osoba.id_osoby;

            Uczen uczen = new Uczen();
            uczen.id_osoby = lastPersonId;
            uczen.id_rodzica = Int32.Parse(formCollection["parentId"]);
            uczen.id_klasy = Int32.Parse(formCollection["classId"]);

            db.Uczniowie.Add(uczen);

            db.SaveChanges();

            return RedirectToAction("Students");
        }

        [Filters.AuthorizeAdmin]
        public ActionResult Subjects()
        {
            return View(db.Przedmioty.OrderBy(p => p.nazwa).ToList());
        }

        [Filters.AuthorizeAdmin]
        [HttpGet]
        public ActionResult CreateSubject()
        {
            return View();
        }

        [Filters.AuthorizeAdmin]
        [HttpPost]
        public ActionResult CreateSubject(FormCollection formCollection)
        {
            Przedmiot przedmiot = new Przedmiot();
            przedmiot.nazwa = formCollection["nazwa"];
            przedmiot.tresc_ksztalcenia = formCollection["trescKsztalcenia"];

            db.Przedmioty.Add(przedmiot);

            db.SaveChanges();

            return RedirectToAction("Subjects");
        }

        [Filters.AuthorizeAdmin]
        public ActionResult Parents()
        {
            return View(db.Rodzice.OrderBy(r => r.Osoba.nazwisko).ToList());
        }

        [Filters.AuthorizeAdmin]
        [HttpGet]
        public ActionResult CreateParent()
        {
            return View();
        }

        [Filters.AuthorizeAdmin]
        [HttpPost]
        public ActionResult CreateParent(FormCollection formCollection)
        {
            Osoba osoba = new Osoba();
            osoba.imie = formCollection["imie"];
            osoba.nazwisko = formCollection["nazwisko"];
            osoba.adres = formCollection["adres"];
            osoba.email = formCollection["email"];
            osoba.haslo = formCollection["haslo"];
            osoba.telefon = formCollection["telefon"];
            osoba.data_urodzenia = Convert.ToDateTime(formCollection["dataUrodzenia"]);
            DateTime dataHasla = DateTime.Now;
            osoba.data_hasla = dataHasla;
            osoba.uprawnienia = "rodzic";

            db.Osoby.Add(osoba);

            db.SaveChanges();

            int lastPersonId = osoba.id_osoby;

            Rodzic rodzic = new Rodzic();
            rodzic.id_osoby = lastPersonId;

            db.Rodzice.Add(rodzic);

            db.SaveChanges();

            return RedirectToAction("Parents");
        }

        [Filters.AuthorizeAdmin]
        public ActionResult News()
        {
            return View(db.Ogloszenia.OrderBy(o => o.Status.status1).OrderByDescending(o => o.data_dodania).ToList());
        }

        [Filters.AuthorizeAdmin]
        [HttpGet]
        public ActionResult CreateNews()
        {
            return View();
        }

        [Filters.AuthorizeAdmin]
        [HttpPost]
        public ActionResult CreateNews(FormCollection formCollection)
        {
            Ogloszenia ogloszenie = new Ogloszenia();
            ogloszenie.tytul = formCollection["tytul"];
            DateTime dataDodania = DateTime.Now;
            ogloszenie.data_dodania = dataDodania;
            ogloszenie.data_waznosci = Convert.ToDateTime(formCollection["dataWaznosci"]);
            ogloszenie.id_statusu = 1;
            ogloszenie.tresc = formCollection["tresc"];


            db.Ogloszenia.Add(ogloszenie);

            db.SaveChanges();

            return RedirectToAction("News");
        }

        [Filters.AuthorizeAdmin]
        [HttpGet]
        public ActionResult EditClass(int id)
        {
            Session["classID"] = id;
            return View(db.Klasy.Where(k => k.id_klasy == id).FirstOrDefault());
        }

        [Filters.AuthorizeAdmin]
        [HttpPost]
        [Filters.MultipleButton(Name = "action", Argument = "EditClass")]
        public ActionResult EditClass(int id, Klasa klasa)
        {
            if(ModelState.IsValid)
            {
                var classId = db.Klasy.AsNoTracking().Where(k => k.id_klasy == id).ToArray();
                if (klasa.id_wychowawcy == null)
                {
                    klasa.id_wychowawcy = classId.Select(k => k.id_wychowawcy).Single();
                }

                db.Entry(klasa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Classes");
            }

            return View();
        }

        [Filters.AuthorizeAdmin]
        [HttpPost]
        [Filters.MultipleButton(Name = "action", Argument = "EditStudentsInClass")]
        public ActionResult EditStudentsInClass(int id, FormCollection formCollection)
        {
            if(formCollection["checkStudents"] == null)
            {
                return RedirectToAction("EditClass/" + id);
            }
            string[] ids = formCollection["checkStudents"].Split(new char[] { ',' });
            foreach(string idd in ids)
            {
                var student = db.Uczniowie.Find(int.Parse(idd));
                student.id_klasy = null;
                db.SaveChanges();
            }

            return RedirectToAction("EditClass/" + id);
        }

        [Filters.AuthorizeAdmin]
        [HttpPost]
        [Filters.MultipleButton(Name = "action", Argument = "EditSubjectsInClass")]
        public ActionResult EditSubjectsInClass(int id, FormCollection formCollection)
        {
            if (formCollection["checkSubjects"] == null)
            {
                return RedirectToAction("EditClass/" + id);
            }
            string[] ids = formCollection["checkSubjects"].Split(new char[] { ',' });
            foreach (string idd in ids)
            {
                int subjectId = Int32.Parse(idd);
                var subject = db.PrzedmiotyWKlasach.Where(p => p.id_klasy == id && p.id_przedmiotu == subjectId).FirstOrDefault();
                db.PrzedmiotyWKlasach.Remove(subject);
                db.SaveChanges();
            }

            return RedirectToAction("EditClass/" + id);
        }

        [Filters.AuthorizeAdmin]
        [HttpGet]
        public ActionResult AddStudentsToClass()
        {
            int classID = Convert.ToInt32(Session["classID"]);
            ViewBag.className = db.Klasy.Find(classID).nazwa;
            return View(db.Uczniowie.Where(u => u.id_klasy == null).ToList());
        }

        [Filters.AuthorizeAdmin]
        [HttpPost]
        public ActionResult AddStudentsToClass(FormCollection formCollection)
        {
            if(formCollection["checkStudents"] == null)
            {
                return RedirectToAction("EditClass/" + Session["classID"]);
            }
            string[] ids = formCollection["checkStudents"].Split(new char[] { ',' });
            foreach(string id in ids)
            {
                var student = db.Uczniowie.Find(int.Parse(id));
                student.id_klasy = (int)Session["classID"];
                db.SaveChanges();
            }
            return RedirectToAction("EditClass/" + Session["classID"]);
        }

        [Filters.AuthorizeAdmin]
        [HttpGet]
        public ActionResult AddSubjectsToClass()
        {
            int classID = Convert.ToInt32(Session["classID"]);
            ViewBag.className = db.Klasy.Find(classID).nazwa;
            var subjectsIds = db.PrzedmiotyWKlasach.Where(p => p.id_klasy == classID).Select(p => p.id_przedmiotu).ToArray();
            return View(db.Przedmioty.Where(p => !subjectsIds.Contains(p.id_przedmiotu)).ToList());
        }

        [Filters.AuthorizeAdmin]
        [HttpPost]
        public ActionResult AddSubjectsToClass(FormCollection formCollection)
        {
            if (formCollection["checkSubjects"] == null)
            {
                return RedirectToAction("EditClass/" + Session["classID"]);
            }
            string[] ids = formCollection["checkSubjects"].Split(new char[] { ',' });
            string[] teachersIds = formCollection["id_nauczyciela"].Split(new char[] { ',' });
            int counter = 0;
            foreach (string id in ids)
            {
                PrzedmiotyWKlasie przedmioty = new PrzedmiotyWKlasie();
                przedmioty.id_przedmiotu = Int32.Parse(id);
                przedmioty.id_klasy = (int)Session["classID"];
                przedmioty.id_nauczyciela = Int32.Parse(teachersIds[counter]);
                counter++;

                db.PrzedmiotyWKlasach.Add(przedmioty);
                db.SaveChanges();
            }
            return RedirectToAction("EditClass/" + Session["classID"]);
        }

        [Filters.AuthorizeAdmin]
        [HttpGet]
        public ActionResult ClassDetails(int id)
        {
            return View(db.Klasy.Where(k => k.id_klasy == id).FirstOrDefault());
        }

        [Filters.AuthorizeAdmin]
        public ActionResult DeleteClass(int id)
        {
            db.Klasy.Remove(db.Klasy.Where(k => k.id_klasy == id).FirstOrDefault());
            db.SaveChanges();
            return RedirectToAction("Classes");
        }

        [Filters.AuthorizeAdmin]
        [HttpGet]
        public ActionResult EditSubject(int id)
        {
            return View(db.Przedmioty.Where(n => n.id_przedmiotu == id).FirstOrDefault());
        }

        [Filters.AuthorizeAdmin]
        [HttpPost]
        public ActionResult EditSubject(int id, Przedmiot przedmiot)
        {
            if (ModelState.IsValid)
            {
                db.Entry(przedmiot).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Subjects");
            }

            return View();
        }

        [Filters.AuthorizeAdmin]
        public ActionResult DeleteSubject(int id)
        {
            db.Przedmioty.Remove(db.Przedmioty.Where(k => k.id_przedmiotu == id).FirstOrDefault());
            db.SaveChanges();
            return RedirectToAction("Subjects");
        }

        [Filters.AuthorizeAdmin]
        [HttpGet]
        public ActionResult SubjectDetails(int id)
        {
            return View(db.Przedmioty.Where(k => k.id_przedmiotu == id).FirstOrDefault());
        }

        [Filters.AuthorizeAdmin]
        [HttpGet]
        public ActionResult EditStudent(int id)
        {
            var student = db.Uczniowie.Where(u => u.id_ucznia == id).FirstOrDefault();
            int personId = student.id_osoby;
            return View(db.Osoby.Where(o => o.id_osoby == personId).FirstOrDefault());
        }

        [Filters.AuthorizeAdmin]
        [HttpPost]
        public ActionResult EditStudent(int id, Osoba osoba, FormCollection formCollection)
        {
            if (ModelState.IsValid)
            {
                var student = db.Uczniowie.AsNoTracking().Where(u => u.id_ucznia == id).FirstOrDefault();
                int personId = student.id_osoby;
                int parentId = student.id_rodzica;

                var person = db.Osoby.AsNoTracking().Where(o => o.id_osoby == personId).FirstOrDefault();

                osoba.data_urodzenia = person.data_urodzenia;
                osoba.data_hasla = person.data_hasla;
                osoba.uprawnienia = person.uprawnienia;

                db.Entry(osoba).State = EntityState.Modified;
                db.SaveChanges();

                Uczen uczen = new Uczen();
                string clas = formCollection["id_osoby"].ToString();
                string[] ids = clas.Split(',');
                int classId = Int32.Parse(ids[ids.Length - 1]);

                uczen.id_ucznia = id;
                uczen.id_osoby = personId;
                uczen.id_rodzica = parentId;
                uczen.id_klasy = classId;

                db.Entry(uczen).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Students");

            }

            return View();
        }

        [Filters.AuthorizeAdmin]
        [HttpGet]
        public ActionResult StudentDetails(int id)
        {
            return View(db.Uczniowie.Where(u => u.id_ucznia == id).FirstOrDefault());
        }

        [Filters.AuthorizeAdmin]
        public ActionResult DeleteStudent(int id)
        {
            var student = db.Uczniowie.Where(u => u.id_ucznia == id).FirstOrDefault();
            int personId = student.id_osoby;
            db.Uczniowie.Remove(student);
            db.Osoby.Remove(db.Osoby.Where(o => o.id_osoby == personId).FirstOrDefault());
            db.SaveChanges();
            return RedirectToAction("Students");
        }

        [Filters.AuthorizeAdmin]
        [HttpGet]
        public ActionResult EditParent(int id)
        {
            var parent = db.Rodzice.Where(u => u.id_rodzica == id).FirstOrDefault();
            int personId = parent.id_osoby;
            return View(db.Osoby.Where(o => o.id_osoby == personId).FirstOrDefault());
        }

        [Filters.AuthorizeAdmin]
        [HttpPost]
        public ActionResult EditParent(int id, Osoba osoba)
        {
            if (ModelState.IsValid)
            {

                var student = db.Uczniowie.AsNoTracking().Where(u => u.id_ucznia == id).FirstOrDefault();
                int personId = student.id_osoby;

                var person = db.Osoby.AsNoTracking().Where(o => o.id_osoby == personId).FirstOrDefault();

                osoba.data_urodzenia = person.data_urodzenia;
                osoba.data_hasla = person.data_hasla;
                osoba.uprawnienia = person.uprawnienia;

                db.Entry(osoba).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Parents");
            }

            return View();
        }

        [Filters.AuthorizeAdmin]
        [HttpGet]
        public ActionResult ParentDetails(int id)
        {
            return View(db.Rodzice.Where(u => u.id_rodzica == id).FirstOrDefault());
        }

        [Filters.AuthorizeAdmin]
        public ActionResult DeleteParent(int id)
        {
            var parent = db.Rodzice.Where(u => u.id_rodzica == id).FirstOrDefault();
            int personId = parent.id_osoby;
            db.Rodzice.Remove(parent);
            db.Osoby.Remove(db.Osoby.Where(o => o.id_osoby == personId).FirstOrDefault());
            db.SaveChanges();
            return RedirectToAction("Parents");
        }

        [Filters.AuthorizeAdmin]
        [HttpGet]
        public ActionResult EditTeacher(int id)
        {
            var teacher = db.Nauczyciele.Where(u => u.id_nauczyciela == id).FirstOrDefault();
            int personId = teacher.id_osoby;
            return View(db.Osoby.Where(o => o.id_osoby == personId).FirstOrDefault());
        }

        [Filters.AuthorizeAdmin]
        [HttpPost]
        public ActionResult EditTeacher(int id, Osoba osoba, FormCollection formCollection)
        {
            if (ModelState.IsValid)
            {
                var teacher = db.Nauczyciele.AsNoTracking().Where(u => u.id_nauczyciela == id).FirstOrDefault();
                int personId = teacher.id_osoby;

                var person = db.Osoby.AsNoTracking().Where(o => o.id_osoby == personId).FirstOrDefault();

                osoba.data_urodzenia = person.data_urodzenia;
                osoba.data_hasla = person.data_hasla;
                osoba.uprawnienia = person.uprawnienia;

                db.Entry(osoba).State = EntityState.Modified;
                db.SaveChanges();

                Nauczyciel nauczyciel = new Nauczyciel();
                string subject = formCollection["id_osoby"].ToString();
                string[] ids = subject.Split(',');
                int subjectId = Int32.Parse(ids[ids.Length - 1]);

                nauczyciel.id_nauczyciela = id;
                nauczyciel.id_osoby = personId;
                nauczyciel.id_przedmiotu = subjectId;

                db.Entry(nauczyciel).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Teachers");
            }

            return View();
        }

        [Filters.AuthorizeAdmin]
        [HttpGet]
        public ActionResult TeacherDetails(int id)
        {
            return View(db.Nauczyciele.Where(u => u.id_nauczyciela == id).FirstOrDefault());
        }

        [Filters.AuthorizeAdmin]
        public ActionResult DeleteTeacher(int id)
        {
            var teacher = db.Nauczyciele.Where(u => u.id_nauczyciela == id).FirstOrDefault();
            int personId = teacher.id_osoby;
            db.Nauczyciele.Remove(teacher);
            db.Osoby.Remove(db.Osoby.Where(o => o.id_osoby == personId).FirstOrDefault());
            db.SaveChanges();
            return RedirectToAction("Teachers");
        }

        [Filters.AuthorizeAdmin]
        [HttpGet]
        public ActionResult EditNews(int id)
        {
            return View(db.Ogloszenia.Where(o => o.id_ogloszenia == id).FirstOrDefault());
        }

        [Filters.AuthorizeAdmin]
        [HttpPost]
        public ActionResult EditNews(int id, Ogloszenia ogloszenie)
        {
            if (ModelState.IsValid)
            {
                var news = db.Ogloszenia.AsNoTracking().Where(u => u.id_ogloszenia == id).FirstOrDefault();

                ogloszenie.data_dodania = news.data_dodania;

                db.Entry(ogloszenie).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("News");
            }

            return View();
        }

        [HttpGet]
        public ActionResult NewsDetails(int id)
        {
            return View(db.Ogloszenia.Where(o => o.id_ogloszenia == id).FirstOrDefault());
        }

        [Filters.AuthorizeAdmin]
        public ActionResult DeleteNews(int id)
        {
            db.Ogloszenia.Remove(db.Ogloszenia.Where(o => o.id_ogloszenia == id).FirstOrDefault());
            db.SaveChanges();
            return RedirectToAction("News");
        }

    }
}