using Dziennik.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Dziennik.Controllers
{
    public class TeacherController : Controller
    {
        DziennikEntities db = new DziennikEntities();

        // GET: Teacher
        [Filters.AuthorizeTeacher]
        public ActionResult Index()
        {
            int personID = Convert.ToInt32(Session["personID"]);
            var tutors = db.Klasy.Select(k => k.id_wychowawcy).ToArray();
            int teacherID = db.Nauczyciele.Where(n => n.id_osoby == personID).FirstOrDefault().id_nauczyciela;
            if (tutors.Contains(teacherID))
            {
                ViewBag.isTutor = true;
            }
            else
            {
                ViewBag.isTutor = false;
            }

            return View();
        }

        [Filters.AuthorizeTeacher]
        [HttpGet]
        public ActionResult PersonalDetails(int id)
        {
            return View(db.Osoby.Where(o => o.id_osoby == id).FirstOrDefault());
        }

        [Filters.AuthorizeTeacher]
        [HttpGet]
        public ActionResult EditPersonalDetails(int id)
        {
            return View(db.Osoby.Where(o => o.id_osoby == id).FirstOrDefault());
        }

        [Filters.AuthorizeTeacher]
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

        [Filters.AuthorizeTeacher]
        [HttpGet]
        public ActionResult ChooseClass()
        {
            int personID = (int)Session["personID"];
            var teachers = db.Nauczyciele.Where(n => n.id_osoby == personID).ToArray();
            var teachersIds = teachers.Select(t => t.id_nauczyciela).ToArray();
            List<SelectListItem> classes = db.PrzedmiotyWKlasach.Where(p => teachersIds.Contains(p.id_nauczyciela)).Join(db.Klasy, p => p.id_klasy, k => k.id_klasy,
                                           (p, k) => new { p, k }).Where(pk => pk.p.id_klasy == pk.k.id_klasy)
                                                    .Select(pk => new SelectListItem() { Text = pk.k.nazwa, Value = pk.k.id_klasy.ToString() }).ToList();
            ViewBag.classes = classes;
            return View();
        }

        [Filters.AuthorizeTeacher]
        [HttpPost]
        public ActionResult ChooseClass(FormCollection formCollection)
        {
            Session["choosenClassID"] = formCollection["id_klasy"];
            return RedirectToAction("ChooseStudent");
        }

        [Filters.AuthorizeTeacher]
        [HttpGet]
        public ActionResult ChooseStudent()
        {
            int classID = Convert.ToInt32(Session["choosenClassID"]);
            List<SelectListItem> students = db.Uczniowie.Where(u => u.id_klasy == classID).OrderBy(u => u.Osoba.nazwisko).Select(u => new SelectListItem() { Text = u.Osoba.nazwisko + " " + u.Osoba.imie, Value = u.id_ucznia.ToString() }).ToList();
            ViewBag.studentsInClass = students;
            return View();
        }

        [Filters.AuthorizeTeacher]
        [HttpPost]
        public ActionResult ChooseStudent(FormCollection formCollection)
        {
            Session["choosenStudentID"] = formCollection["id_ucznia"];
            return RedirectToAction("AddGrade");
        }

        [Filters.AuthorizeTeacher]
        [HttpGet]
        public ActionResult AddGrade()
        {
            int personID = Convert.ToInt32(Session["personID"]);
            int subjectID = db.Nauczyciele.Where(n => n.id_osoby == personID).FirstOrDefault().id_przedmiotu;
            Session["subjectID"] = subjectID;
            ViewBag.subjectName = db.Przedmioty.Where(p => p.id_przedmiotu == subjectID).FirstOrDefault().nazwa;
            int studentID = Convert.ToInt32(Session["choosenStudentID"]);
            ViewBag.studentName = db.Uczniowie.Where(u => u.id_ucznia == studentID).FirstOrDefault().Osoba.FullName;
            ViewBag.studentClass = db.Uczniowie.Where(u => u.id_ucznia == studentID).FirstOrDefault().Klasa.nazwa;
            return View(db.Oceny.Where(o => o.id_ucznia == studentID && o.id_przedmiotu == subjectID).ToList());
        }

        [Filters.AuthorizeTeacher]
        [HttpPost]
        public ActionResult AddGrade(FormCollection formCollection)
        {
            Ocena ocena = new Ocena();

            int personID = Convert.ToInt32(Session["personID"]);
            int subjectID = db.Nauczyciele.Where(n => n.id_osoby == personID).FirstOrDefault().id_przedmiotu;
            int studentID = Convert.ToInt32(Session["choosenStudentID"]);

            ocena.id_przedmiotu = subjectID;
            ocena.id_ucznia = studentID;
            ocena.data = DateTime.Now;
            ocena.ocena1 = Convert.ToInt32(formCollection["ocena"]);

            db.Oceny.Add(ocena);
            db.SaveChanges();

            return RedirectToAction("AddGrade");
        }

        [Filters.AuthorizeTeacher]
        [HttpGet]
        public ActionResult Messages()
        {
            int personID = Convert.ToInt32(Session["personID"]);
            var teacher = db.Nauczyciele.Where(n => n.id_osoby == personID).FirstOrDefault();
            var tutors = db.Klasy.Select(k => k.id_wychowawcy).ToArray();
            if (tutors.Contains(teacher.id_nauczyciela))
            {
                ViewBag.isTutor = true;
            }
            else
            {
                ViewBag.isTutor = false;
            }

            return View(db.Wiadomosci.Where(w => w.id_nauczyciela == teacher.id_nauczyciela && w.czyNauczyciel == true).OrderBy(w => w.data).ToList());
        }

        [Filters.AuthorizeTeacher]
        [HttpGet]
        public ActionResult MessageDetails(int id)
        {
            return View(db.Wiadomosci.Where(o => o.id_wiadomosci == id).FirstOrDefault());
        }

        [Filters.AuthorizeTeacher]
        [HttpGet]
        public ActionResult CreateResponse(int id)
        {
            return View(db.Wiadomosci.Where(o => o.id_wiadomosci == id).FirstOrDefault());
        }

        [Filters.AuthorizeTeacher]
        [HttpPost]
        public ActionResult CreateResponse(int id, FormCollection formCollection)
        {
            var message = db.Wiadomosci.Where(w => w.id_wiadomosci == id).FirstOrDefault();

            message.odpowiedz = formCollection["odpowiedz"];
            db.Entry(message).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("MessageDetails/" + id);
        }

        [Filters.AuthorizeTeacher]
        public ActionResult DeleteMessage(int id)
        {
            var message = db.Wiadomosci.Where(w => w.id_wiadomosci == id).FirstOrDefault();

            message.czyNauczyciel = false;

            db.Entry(message).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Messages");
        }

        [Filters.AuthorizeTeacher]
        [HttpGet]
        public ActionResult SendNotice()
        {
            int personID = Convert.ToInt32(Session["personID"]);
            var teacher = db.Nauczyciele.Where(n => n.id_osoby == personID).FirstOrDefault();

            ViewBag.className = db.Klasy.Where(k => k.id_wychowawcy == teacher.id_nauczyciela).FirstOrDefault().nazwa;
            return View();
        }

        [Filters.AuthorizeTeacher]
        [HttpPost]
        public ActionResult SendNotice(FormCollection formCollection)
        {
            if (ModelState.IsValid)
            {
                int personID = Convert.ToInt32(Session["personID"]);
                var teacher = db.Nauczyciele.Where(n => n.id_osoby == personID).FirstOrDefault();
                int classID = db.Klasy.Where(k => k.id_wychowawcy == teacher.id_nauczyciela).FirstOrDefault().id_klasy;
                var students = db.Uczniowie.Where(u => u.id_klasy == classID).ToList();
                var parents = students.Select(s => s.Rodzic.Osoba.email).ToList();

                try
                {
                    var senderEmail = new MailAddress("dziennikinternetowyaspmvc@gmail.com", teacher.Osoba.FullName);

                    var password = "BI8378ea";
                    var subject = formCollection["tytul"];
                    var message = formCollection["tresc"];
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };


                    foreach (var parent in parents)
                    {

                        var receiverEmail = new MailAddress(parent);
                        using (var mess = new MailMessage(senderEmail, receiverEmail)
                        {
                            Subject = subject,
                            Body = message
                        })
                        {
                            smtp.Send(mess);
                        }
                    }

                }
                catch (Exception)
                {
                    ViewBag.Error = "Wystąpił błąd z wysłaniem wiadomości e-mail. Spróbuj ponownie.";
                    return View();

                }

                Ogloszenia ogloszenie = new Ogloszenia();
                ogloszenie.tytul = formCollection["tytul"];
                ogloszenie.data_dodania = DateTime.Now;
                ogloszenie.data_waznosci = Convert.ToDateTime(formCollection["dataWaznosci"]);
                ogloszenie.id_statusu = 1;
                ogloszenie.tresc = formCollection["tresc"];
                ogloszenie.id_klasy = classID;

                db.Ogloszenia.Add(ogloszenie);

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View();
        }

        [Filters.AuthorizeTeacher]
        [HttpGet]
        public ActionResult UploadFile()
        {
            return View();
        }

        [Filters.AuthorizeTeacher]
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    int personID = Convert.ToInt32(Session["personID"]);
                    var teacher = db.Nauczyciele.Where(n => n.id_osoby == personID).FirstOrDefault();
                    string path = Path.Combine(Server.MapPath("~/Files/" + teacher.Przedmiot.nazwa), Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    ViewBag.Message = "Dodawanie pliku zakończone pomyślnie";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            }
            else
            {
                ViewBag.Message = "Nie wybrano pliku";
            }

            return View();
        }

        [Filters.AuthorizeTeacher]
        [HttpGet]
        public ActionResult CreateTest()
        {
            return View();
        }

        [Filters.AuthorizeTeacher]
        [HttpPost]
        public ActionResult CreateTest(FormCollection formCollection)
        {
            if(ModelState.IsValid)
            {
                int personID = Convert.ToInt32(Session["personID"]);
                var teacher = db.Nauczyciele.Where(n => n.id_osoby == personID).FirstOrDefault();

                Test test = new Test();

                test.nazwa = formCollection["nazwa"];
                test.ilosc_pytan = Convert.ToInt32(formCollection["iloscPytan"]);
                test.czas_trwania = TimeSpan.Parse(formCollection["czasTrwania"]);
                test.max_punktow = Convert.ToInt32(formCollection["maxPunktow"]);
                test.id_nauczyciela = teacher.id_nauczyciela;
                test.id_przedmiotu = teacher.id_przedmiotu;

                db.Testy.Add(test);

                db.SaveChanges();

                Session["testID"] = test.id_testu;
                Session["testQuestions"] = test.ilosc_pytan;

                return RedirectToAction("CreateQuestions");
            }
            

            return View();
        }

        [Filters.AuthorizeTeacher]
        [HttpGet]
        public ActionResult CreateQuestions()
        {
            int testID = Convert.ToInt32(Session["testID"]);

            ViewBag.testName = db.Testy.Where(t => t.id_testu == testID).FirstOrDefault().nazwa;
            ViewBag.questionsNumber = Convert.ToInt32(Session["testQuestions"]);

            return View();
        }

        [Filters.AuthorizeTeacher]
        [HttpPost]
        public ActionResult CreateQuestions(FormCollection formCollection)
        {

            if(ModelState.IsValid)
            {
                string[] questions = formCollection[1].Split(',');
                string[] points = formCollection[2].Split(',');

                for (int i = 0; i < questions.Length; i++)
                {
                    Pytanie pytanie = new Pytanie();
                    pytanie.id_testu = Convert.ToInt32(Session["testID"]);
                    pytanie.tresc = questions[i];
                    pytanie.pkt_za_odpowiedz = Convert.ToInt32(points[i]);

                    db.Pytania.Add(pytanie);
                    db.SaveChanges();
                }

                return RedirectToAction("TestSummary");
            }

            return View();
        }

        [Filters.AuthorizeTeacher]
        [HttpGet]
        public ActionResult TestSummary()
        {
            int testID = Convert.ToInt32(Session["testID"]);

            return View(db.Testy.Where(t => t.id_testu == testID).FirstOrDefault());
        }

        [Filters.AuthorizeTeacher]
        [HttpGet]
        public ActionResult CreateAnswers(int id)
        {
            ViewBag.questionContent = db.Pytania.Where(p => p.id_pytania == id).FirstOrDefault().tresc;
            Session["questionID"] = id;

            return View(db.Odpowiedzi.Where(o => o.id_pytania == id).ToList());
        }

        [Filters.AuthorizeTeacher]
        [HttpPost]
        public ActionResult CreateAnswers(FormCollection formCollection)
        {
            int questionID = Convert.ToInt32(Session["questionID"]);

            if (ModelState.IsValid)
            {
                Odpowiedz odpowiedz = new Odpowiedz();

                odpowiedz.tresc = formCollection["tresc"];
                odpowiedz.id_pytania = questionID;
                odpowiedz.czyPoprawna = Convert.ToBoolean(formCollection["czyOK"]);

                db.Odpowiedzi.Add(odpowiedz);
                db.SaveChanges();
            }

            return RedirectToAction("CreateAnswers/" + questionID);
        }

    }
}