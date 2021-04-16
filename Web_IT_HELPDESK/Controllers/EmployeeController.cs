using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.ViewModels;

namespace Web_IT_HELPDESK.Controllers
{
    public class EmployeeController : Controller
    {
        ServiceDeskEntities en = new ServiceDeskEntities();
        private string session_emp = System.Web.HttpContext.Current.User.Identity.Name;

        public ActionResult EmployeeList()
        {
            var v_employee_list = en.Employee_New.Where(i => i.Deactive != true && i.Emp_CJ != "admin");
            return View(v_employee_list);
        }

        ////Edit IMPORT MODEL 
        //public PartialViewResult EditRoleViewModel(string v_emp_id)
        //{
        //    Session["v_emp_id"] = v_emp_id;
        //    var v_emp_list = en.Employee_Screen.Where(i => i.Employee.Emp_CJ == v_emp_id).OrderBy(i => i.Id);//.SingleOrDefault();
        //    return PartialView("EditRoleViewModel", v_emp_list);
        //}

        ////Edit save
        //[HttpPost]
        //public ActionResult Save(ICollection<int> ints1, ICollection<int> ints2) //IEnumerable<Web_IT_HELPDESK.Employee_Screen> Employee_Screen)
        //{
        //    string emp_id = Convert.ToString(Session["v_emp_id"]);

        //    // set up all role to false
        //    foreach (var item in v_employee_screen_list)
        //        item.Use = false;
        //    en.SaveChanges();

        //    List<int> dups = ints1.Intersect(ints2).ToList();// có tồn tại
        //    List<int> distincts = ints1.Except(ints2).ToList();// không tồn tại

        //    foreach (var dup in dups)
        //    {
        //        var a = //from i in en.Employee_Screen where i.Employee.Emp_CJ == emp_id && i.Screen.ScreenId == int2 select i.Use;
        //            en.Employee_Screen.Where(i => i.Employee.Emp_CJ == emp_id && i.Screen.ScreenId == dup).FirstOrDefault();
        //        a.Use = true;
        //        en.SaveChanges();
        //    }
        //    //return View("EmployeeList", en.Employees.Where(i => i.Deactive != true && i.Emp_CJ != "admin"));
        //    return View("EmployeeList", en.Employee_New.Where(i => i.Deactive != true && i.Emp_CJ != "admin"));
        //}

        [Authorize]
        public ActionResult ChangePasword(string employeeid)
        {
            ViewBag.Emp_CJ = session_emp;
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult ChangePasword(ChangePasswordViewModel cpVM)
        {
            if (!string.Equals(cpVM.NewPsw, cpVM.NewPsw_Confirm))
            {
                ModelState.AddModelError("", "New Password and Confirm New Password does not match.");
                return View(cpVM);
            }
            if (ModelState.IsValid)
            {
                string curr_Psw = CurrentUser.Instance.User.Password;
                if (cpVM.OldPsw == curr_Psw)
                {
                    string curr_EmpID = CurrentUser.Instance.User.Emp_CJ;
                    //Employee emp = en.Employees.FirstOrDefault(e => e.Emp_CJ == curr_EmpID && e.Password == cpVM.OldPsw);
                    Employee_New emp = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == curr_EmpID && e.Password == cpVM.OldPsw);
                    emp.Password = cpVM.NewPsw_Confirm;
                    en.Entry(emp).State = System.Data.Entity.EntityState.Modified;
                    en.SaveChanges();
                }
            }
            return View(cpVM);
        }

    }
}
