using Dziennik.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DziennikInternetowy.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["AdminIsLoggedIn"] != null)
            {
                return RedirectToAction("Index", "Admin");
            }
            DziennikEntities db = new DziennikEntities();

            return View(db.Ogloszenia.Where(o => o.id_statusu == 1 && o.id_klasy == null).ToList());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}