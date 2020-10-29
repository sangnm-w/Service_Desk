using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web_IT_HELPDESK.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult LogOn()
        {
            return View();
        }
    }
}
