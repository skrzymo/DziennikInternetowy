using Dziennik.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dziennik.ViewModels
{
    public class EditNewsModel
    {
        public static List<SelectListItem> getStatuses(int id)
        {
            DziennikEntities db = new DziennikEntities();
            List<SelectListItem> statuses = new List<SelectListItem>();
            var actualStatus = db.Statusy.Find(id);
            var anotherStatus = db.Statusy.Where(k => k.id_statusu != id).FirstOrDefault();

            statuses.Add(new SelectListItem() { Text = actualStatus.status1, Value = actualStatus.id_statusu.ToString() });
            statuses.Add(new SelectListItem() { Text = anotherStatus.status1, Value = anotherStatus.id_statusu.ToString() });

            return statuses;
        }
    }
}