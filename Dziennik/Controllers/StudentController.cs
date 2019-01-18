using Dziennik.Models;
using Dziennik.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dziennik.Controllers
{
    public class StudentController : Controller
    {
        DziennikEntities db = new DziennikEntities();

        // GET: Student
        [Filters.AuthorizeStudent]
        public ActionResult Index()
        {
            return View();
        }

        [Filters.AuthorizeStudent]
        [HttpGet]
        public ActionResult PersonalDetails(int id)
        {
            return View(db.Osoby.Where(o => o.id_osoby == id).FirstOrDefault());
        }

        [Filters.AuthorizeStudent]
        [HttpGet]
        public ActionResult EditPersonalDetails(int id)
        {
            return View(db.Osoby.Where(o => o.id_osoby == id).FirstOrDefault());
        }

        [Filters.AuthorizeStudent]
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

        [Filters.AuthorizeStudent]
        [HttpGet]
        public ActionResult ChooseSubject(string id)
        {
            if(id == "files")
            {
                Session["nextStep"] = "files";
            }
            else if(id == "grades")
            {
                Session["nextStep"] = "grades";
            }

            int personID = Convert.ToInt32(Session["personID"]);
            var student = db.Uczniowie.Where(u => u.id_osoby == personID).FirstOrDefault();
            Session["studentID"] = student.id_ucznia;
            List<SelectListItem> subjects = db.Klasy.Where(k => k.id_klasy == student.id_klasy).Join(db.PrzedmiotyWKlasach, k => k.id_klasy, p => p.id_klasy,
                            (k, p) => new { k, p }).Where(kp => kp.k.id_klasy == kp.p.id_klasy).Select(kp => new SelectListItem() { Text = kp.p.Przedmiot.nazwa, Value = kp.p.id_przedmiotu.ToString() }).ToList();

            ViewBag.studentSubjects = subjects;

            return View();
        }

        [Filters.AuthorizeStudent]
        [HttpPost]
        public ActionResult ChooseSubject(FormCollection formCollection)
        {
            Session["choosenSubject"] = formCollection["id_przedmiotu"];

            if(Session["nextStep"].ToString() == "files")
            {
                return RedirectToAction("ExploreFiles");
            }

            return RedirectToAction("GradesReview");
        }

        [Filters.AuthorizeStudent]
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

            if (from == DateTime.MinValue || to == DateTime.MinValue)
            {
                return View(db.Oceny.Where(o => o.id_przedmiotu == subjectID && o.id_ucznia == studentID).ToList());
            }

            Session["dateFrom"] = DateTime.MinValue;
            Session["dateTo"] = DateTime.MinValue;

            if(from > to)
            {
                ModelState.AddModelError("dateRange", "Data 'od' nie może być późniejsza niż 'do'!");
            }

            return View(db.Oceny.Where(o => o.id_przedmiotu == subjectID && o.id_ucznia == studentID && o.data <= to && o.data >= from).ToList());

        }

        [Filters.AuthorizeStudent]
        [HttpPost]
        public ActionResult GradesReview(FormCollection formCollection)
        {
            Session["dateFrom"] = formCollection["dataOd"];
            Session["dateTo"] = formCollection["dataDo"];

            return RedirectToAction("GradesReview");
        }

        [Filters.AuthorizeStudent]
        [HttpPost]
        public ActionResult ChooseSubjectFiles(FormCollection formCollection)
        {
            Session["choosenSubject"] = formCollection["id_przedmiotu"];

            return RedirectToAction("Explore Files");
        }

        [Filters.AuthorizeStudent]
        [HttpGet]
        public ActionResult ExploreFiles()
        {
            int subjectID = Convert.ToInt32(Session["choosenSubject"]);
            var subjectName = db.Przedmioty.Find(subjectID).nazwa;

            ViewBag.subjectName = subjectName;

            string realPath = Server.MapPath("~/Files/" + subjectName);

            if(Directory.Exists(realPath))
            {
                List<FileModel> fileListModel = new List<FileModel>();

                IEnumerable<string> fileList = Directory.EnumerateFiles(realPath);

                foreach (string file in fileList)
                {
                    FileInfo f = new FileInfo(file);

                    FileModel fileModel = new FileModel();

                    if(f.Extension.ToLower() != "aspx" && f.Extension.ToLower() != "asp")
                    {
                        fileModel.FileName = Path.GetFileName(file);
                        fileModel.FileAccessed = f.LastAccessTime;
                        fileModel.FileSizeText = (f.Length < 1024) ? f.Length.ToString() + " B" : f.Length / 1024 + " KB";

                        fileListModel.Add(fileModel);
                    }
                }

                ExplorerModel explorerModel = new ExplorerModel(fileListModel);

                return View(explorerModel);
            }
            else
            {
                return Content("Nie można otworzyć zasobu");
            }

        }

        [Filters.AuthorizeStudent]
        [HttpGet]
        public ActionResult Tests()
        {
            int personID = Convert.ToInt32(Session["personID"]);

            var student = db.Uczniowie.Where(u => u.id_osoby == personID).FirstOrDefault();
            var studentClass = student.Klasa.id_klasy;
            var studentSubjects = db.PrzedmiotyWKlasach.Where(p => p.id_klasy == studentClass).Select(p => p.id_przedmiotu).ToArray();
            var studentGrades = db.Oceny.Where(o => o.id_ucznia == student.id_ucznia && o.id_testu != null).Select(o => o.id_testu).ToArray();
            var testsNotDone = db.Testy.Where(t => !studentGrades.Contains(t.id_testu)).ToArray();

            List<Test> tests = new List<Test>();
            foreach(var item in testsNotDone)
            {
                if(studentSubjects.Contains(item.id_przedmiotu))
                {
                    tests.Add(item);
                }
            }

            return View(tests);
        }

        //[Filters.AuthorizeStudent]
        [HttpGet]
        public ActionResult DoTest(int id)
        {
            Session["testID"] = id;
            ViewBag.testName = db.Testy.Find(id).nazwa;
            ViewBag.testTime = db.Testy.Find(id).czas_trwania.TotalSeconds;
            return View(db.Pytania.Where(p => p.id_testu == id).ToList());
        }

        [Filters.AuthorizeStudent]
        [HttpPost]
        public ActionResult DoTest(FormCollection formCollection)
        {
            if(formCollection != null)
            {
                double points = 0.00;
                int testID = Convert.ToInt32(Session["testID"]);
                int questionNumber = db.Pytania.Where(p => p.id_testu == testID).Count();
                int maxPoints = db.Testy.Find(testID).max_punktow;
                int personID = db.Osoby.Find(Convert.ToInt32(Session["personID"])).id_osoby;
                var student = db.Uczniowie.Where(u => u.id_osoby == personID).FirstOrDefault();

                Ocena ocena = new Ocena();

                ocena.id_przedmiotu = db.Testy.Find(testID).id_przedmiotu;
                ocena.id_ucznia = student.id_ucznia;
                ocena.data = DateTime.Now;
                ocena.id_testu = testID;

                if(formCollection.AllKeys.Count() == 0)
                {
                    points = 0;
                } else
                {
                    for (int i = 1; i <= questionNumber; i++)
                    {
                        if(formCollection["inlineRadioOptions" + i] == null)
                        {
                            points += 0;
                        }
                        else
                        {
                            var answer = db.Odpowiedzi.Find(int.Parse(formCollection["inlineRadioOptions" + i]));
                            if (answer.czyPoprawna)
                            {
                                points += answer.Pytanie.pkt_za_odpowiedz;
                            }
                        }
                    }
                }

                double score = (points / maxPoints) * 100;

                if (score < 30)
                {
                    ocena.ocena1 = 1;
                }
                else if(score >= 30 && score < 45)
                {
                    ocena.ocena1 = 2;
                }
                else if (score >= 45 && score < 60)
                {
                    ocena.ocena1 = 3;
                }
                else if (score >= 60 && score < 75)
                {
                    ocena.ocena1 = 4;
                }
                else
                {
                    ocena.ocena1 = 5;
                }

                Session["testGrade"] = ocena;
                Session["testPoints"] = points;

                db.Oceny.Add(ocena);
                db.SaveChanges();

                return RedirectToAction("TestSummary");

            }

            return RedirectToAction("Tests");
        }

        [Filters.AuthorizeStudent]
        [HttpGet]
        public ActionResult TestSummary()
        {
            Ocena ocena = (Ocena)Session["testGrade"];
            ViewBag.testPoints = Session["testPoints"];
            return View(db.Oceny.Where(o => o.id_oceny == ocena.id_oceny).FirstOrDefault());
        }
    }
}