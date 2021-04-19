using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_IT_HELPDESK.Controllers.ObjectManager;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.Models.Extensions;
using Web_IT_HELPDESK.ViewModels;

namespace Web_IT_HELPDESK.Controllers
{
    public class ContactsController : Controller
    {
        ServiceDeskEntities en = new ServiceDeskEntities();
        // GET: Contact
        [CustomAuthorize]
        public ActionResult Index()
        {
            string curr_PlantID = ApplicationUser.Instance.GetPlantID();
            var contactlist = en.Employee_New
                .Join(en.Departments, e => e.Department_ID, d => d.Department_Id, (e, d) => new { e, d })
                .Where(grp => grp.e.Deactive != true && grp.d.Plant_Id == curr_PlantID)
                .Select(grp => grp.e);

            List<PlantViewModel> plants = en.Plants
                .Select(p => new PlantViewModel
                {
                    Plant_Id = p.Plant_Id,
                    Plant_Name = p.Plant_Name
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
        [HttpPost]
        [CustomAuthorize]
        public ActionResult Index(string plantid)
        {
            var contactlist = en.Employee_New
                .Join(en.Departments, e => e.Department_ID, d => d.Department_Id, (e, d) => new { e, d })
                .Where(grp => grp.e.Deactive != true && grp.d.Plant_Id == plantid)
                .Select(grp => grp.e);

            List<PlantViewModel> plants = en.Plants
              .Select(p => new PlantViewModel
              {
                  Plant_Id = p.Plant_Id,
                  Plant_Name = p.Plant_Name
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
        [CustomAuthorize]
        public ActionResult Download(string plantid)
        {
            string plantName = en.Plants.FirstOrDefault(d => d.Plant_Id == plantid).Plant_Name;
            var contactlist = en.Employee_New
                .Join(en.Departments, e => e.Department_ID, d => d.Department_Id, (e, d) => new { e, d })
                .Where(grp => grp.e.Deactive != true && grp.d.Plant_Id == plantid).OrderBy(grp => grp.d.Department_Id)
                .Select(grp => new
                {
                    Emp_CJ = grp.e.Emp_CJ,
                    EmployeeName = grp.e.Employee_Name,
                    Email = grp.e.Email,
                    Phone = grp.e.Phone,
                    Birthday = grp.e.Birthday
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

