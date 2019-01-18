using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dziennik.Filters
{
    public class Authorize : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["personID"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
                {
                    {"Controller", "Login" },
                    {"Action", "Login" }
                });
            }
            base.OnActionExecuting(filterContext);
        }
    }
}