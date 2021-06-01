using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Web_IT_HELPDESK.Models;

namespace Web_IT_HELPDESK.Controllers
{
    public class AccountController : Controller
    {
        ServiceDeskEntities en = new ServiceDeskEntities();

        // GET: /LogOn/
        public ActionResult LogOn()
        {
            return View();
        }

        // POST: /LogOn/
        [HttpPost]
        public ActionResult LogOn(Employee emp, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                Employee logonUser = en.Employees.FirstOrDefault(e => e.Emp_CJ == emp.Emp_CJ && e.Password == emp.Password);

                if (logonUser != null)
                {
                    string curr_deptId = logonUser.Department_ID;
                    string curr_plantId = en.Departments.Find(curr_deptId).Plant_Id;

                    FormsAuthentication.SetAuthCookie(emp.Emp_CJ, false);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        Session["employee_id"] = emp.Emp_CJ;
                        Session["plant_id"] = curr_plantId;
                        Session["dept_id"] = curr_deptId;
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        Session["employee_id"] = emp.Emp_CJ;
                        Session["plant_id"] = curr_plantId;
                        Session["dept_id"] = curr_deptId;
                        string emailcookies = System.Web.HttpContext.Current.User.Identity.Name;
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("LogonF", "The Id or Password is invalid!");
                }
            }
            return View(emp);
        }

        // log out
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Logon", "Logon");
        }
    }
}