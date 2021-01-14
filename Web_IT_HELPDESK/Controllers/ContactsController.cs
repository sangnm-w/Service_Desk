using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_IT_HELPDESK.Controllers.ObjectManager;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.ViewModels;

namespace Web_IT_HELPDESK.Controllers
{
    public class ContactsController : Controller
    {
        ServiceDeskEntities en = new ServiceDeskEntities();
        // GET: Contact
        [Authorize]
        public ActionResult Index()
        {
            string curr_PlantID = CurrentUser.Instance.User.Plant_Id;
            var contactlist = en.Employees.Where(e => e.Deactive != true && e.Plant_Id == curr_PlantID);

            List<PlantViewModel> plants = en.Departments
                .Select(d => new PlantViewModel
                {
                    Plant_Id = d.Plant_Id,
                    Plant_Name = d.Plant_Name
                }).Distinct().ToList();
            ViewBag.plants = plants;

            List<DepartmentViewModel> departments = en.Departments
                .Where(d => d.Plant_Id == curr_PlantID)
                .Select(d => new DepartmentViewModel
                {
                    Department_Id = d.Department_Id,
                    Department_Name = d.Department_Name
                }).Distinct().ToList();
            ViewBag.departments = departments;

            ViewBag.curr_PlantID = curr_PlantID;
            return View(contactlist);
        }

        // POST: Contact
        [Authorize]
        [HttpPost]
        public ActionResult Index(string plantid)
        {
            var contactlist = en.Employees
                .Where(e => e.Deactive != true && e.Plant_Id == plantid);

            List<PlantViewModel> plants = en.Departments
                .Select(d => new PlantViewModel
                {
                    Plant_Id = d.Plant_Id,
                    Plant_Name = d.Plant_Name
                }).Distinct().ToList();
            ViewBag.plants = plants;

            List<DepartmentViewModel> departments = en.Departments
                .Where(d => d.Plant_Id == plantid)
                .Select(d => new DepartmentViewModel
                {
                    Department_Id = d.Department_Id,
                    Department_Name = d.Department_Name
                }).Distinct().ToList();
            ViewBag.departments = departments;

            ViewBag.curr_PlantID = plantid;

            return View(contactlist);
        }

        public ActionResult Download(string plantid)
        {
            string plantName = en.Departments.FirstOrDefault(d => d.Plant_Id == plantid).Plant_Name;
            var contactlist = en.Employees
               .Where(e => e.Deactive != true && e.Plant_Id == plantid)
               .Select(c => new
               {
                   Emp_CJ = c.Emp_CJ,
                   EmployeeName = c.EmployeeName,
                   Email = c.Email,
                   Phone = c.Phone,
                   Birthday = c.Birthday
               }).ToList();

            //Col need format date
            List<int> colsDate = new List<int>()
                {
                    5
                };

            var stream = ExcelHelper.Instance.CreateExcelFile(null, contactlist, ExcelTitle.Instance.Contacts(), colsDate);
            var buffer = stream as MemoryStream;
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", "attachment; filename=Contacts_" + plantName + ".xlsx");
            Response.BinaryWrite(buffer.ToArray());
            Response.Flush();
            Response.End();

            return View(contactlist);
        }
    }
}

