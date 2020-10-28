using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web_IT_HELPDESK.Controllers
{
    public class EmployeeController : Controller
    {
        //
        // GET: /Employee/

        ServiceDeskEntities en = new ServiceDeskEntities();
        private string session_emp = System.Web.HttpContext.Current.User.Identity.Name;

        public ActionResult EmployeeList()
        {
            var v_employee_list = en.Employees.Where(i => i.Deactive != true && i.EmployeeID != "admin"); //&& (i.EmployeeID == "hr" || i.EmployeeID == "pro" || i.EmployeeID == "qc"));
            return View(v_employee_list);
        }

        //Edit IMPORT MODEL 
        public PartialViewResult EditRoleViewModel(string v_emp_id)
        {
            Session["v_emp_id"] = v_emp_id;
            var v_emp_list = en.Employee_Screen.Where(i => i.Employee.EmployeeID == v_emp_id).OrderBy(i => i.Id);//.SingleOrDefault();
            return PartialView("EditRoleViewModel", v_emp_list);
        }

        //Edit save
        [HttpPost]
        public ActionResult Save(ICollection<int> ints1, ICollection<int> ints2) //IEnumerable<Web_IT_HELPDESK.Employee_Screen> Employee_Screen)
        {
            string emp_id = Convert.ToString(Session["v_emp_id"]);

            // set up all role to false
            var v_employee_screen_list = en.Employee_Screen.Where(i => i.Employee.EmployeeID == emp_id);
            foreach (var item in v_employee_screen_list)
                item.Use = false;
            en.SaveChanges();

            List<int> dups = ints1.Intersect(ints2).ToList();// có tồn tại
            List<int> distincts = ints1.Except(ints2).ToList();// không tồn tại

            foreach (var dup in dups)
            {
                var a = //from i in en.Employee_Screen where i.Employee.EmployeeID == emp_id && i.Screen.ScreenId == int2 select i.Use;
                    en.Employee_Screen.Where(i => i.Employee.EmployeeID == emp_id && i.Screen.ScreenId == dup).FirstOrDefault();
                a.Use = true;
                en.SaveChanges();
            }
            return View("EmployeeList", en.Employees.Where(i => i.Deactive != true && i.EmployeeID != "admin"));
        }


        public ActionResult changepasword(string employeeid)
        {
            //Employee employee = en.Employees.Find(session_emp);
            ViewBag.employeeid = session_emp;
            return View();
        }

         [HttpPost]
        public ActionResult changepasword(string employeeid, string v_newpass)
        {
            Employee employee = en.Employees.Find(session_emp);
            try
            {
                employee.Password = v_newpass;
                en.SaveChanges();
            }
            catch { };
            return View();
        }

    }
}
