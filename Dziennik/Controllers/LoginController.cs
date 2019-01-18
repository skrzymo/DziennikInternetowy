using Dziennik.Models;
using Dziennik.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dziennik.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            if (Session["AdminIsLoggedIn"] != null)
            {
                return RedirectToAction("Index", "Admin");
            } else if (Session["TeacherIsLoggedIn"] != null)
            {
                return RedirectToAction("Index", "Teacher");
            } else if (Session["StudentIsLoggedIn"] != null)
            {
                return RedirectToAction("Index", "Student");
            } else if (Session["ParentIsLoggedIn"] != null)
            {
                return RedirectToAction("Index", "Parent");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Login(Osoba osoba)
        {

            using (DziennikEntities db = new DziennikEntities())
            {
                var userDetails = db.Osoby.Where(x => x.email == osoba.email && x.haslo == osoba.haslo).FirstOrDefault();
                if(userDetails == null)
                {
                    osoba.LoginErrorMessage = "Zły email lub hasło!";
                    return View("Login", osoba);
                } else
                {
                    Session["personID"] = userDetails.id_osoby;

                    DateTime now = DateTime.Now;
                    TimeSpan result = now - userDetails.data_hasla;
                    string days = result.TotalDays.ToString();
                    string[] numbers = days.Split(',');
                    int daysNumber = Int32.Parse(numbers[0]);

                    if (userDetails.uprawnienia == "admin")
                    {
                        Session["AdminIsLoggedIn"] = true;
                        return RedirectToAction("Index", "Admin");
                    } else if(userDetails.uprawnienia == "nauczyciel")
                    {
                        Session["TeacherIsLoggedIn"] = true;
                        
                        if (daysNumber > 90)
                        {
                            return RedirectToAction("ChangePassword", "Login");
                        }
                        return RedirectToAction("Index", "Teacher");
                    } else if (userDetails.uprawnienia == "uczen")
                    {
                        Session["StudentIsLoggedIn"] = true;
                        if (daysNumber > 90)
                        {
                            return RedirectToAction("ChangePassword", "Login");
                        }
                        return RedirectToAction("Index", "Student");
                    } else if (userDetails.uprawnienia == "rodzic")
                    {
                        Session["ParentIsLoggedIn"] = true;
                        if (daysNumber > 90)
                        {
                            return RedirectToAction("ChangePassword", "Login");
                        }
                        return RedirectToAction("Index", "Parent");
                    } else
                    {
                        osoba.LoginErrorMessage = "Złe uprawnienia, skontaktuj się z administratorem";
                        return View();
                    }
                }
            }
        }

        public ActionResult Logout()
        {
            Session["AdminIsLoggedIn"] = null;
            Session["TeacherIsLoggedIn"] = null;
            Session["StudentIsLoggedIn"] = null;
            Session["ParentIsLoggedIn"] = null;
            Session["PersonID"] = null;
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Filters.Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Filters.Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel changePassword)
        {
            if(ModelState.IsValid)
            {
                DziennikEntities db = new DziennikEntities();

                int personID = (int)Session["personID"];
                var person = db.Osoby.Find(personID);

                if(person.haslo == changePassword.oldPassword)
                {
                    person.haslo = changePassword.newPassword;
                    person.data_hasla = DateTime.Now;

                    db.SaveChanges();

                    Logout();

                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    ModelState.AddModelError("oldPassword", "Podane stare hasło jest nieprawidłowe");
                }
            }
             
            return View();
        }
    }
}