using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web_IT_HELPDESK;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.Models.Extensions;
using Web_IT_HELPDESK.ViewModels;
using Web_IT_HELPDESK.ViewModels.Mailing;

namespace Web_IT_HELPDESK.Controllers
{
    public class MailingController : Controller
    {
        private ServiceDeskEntities en { get; set; }
        private ApplicationUser _appUser { get; set; }
        private string currUserId { get; set; }
        private string currUserDeptId { get; set; }
        private string currUserPlantId { get; set; }

        public MailingController()
        {
            en = new ServiceDeskEntities();
            _appUser = new ApplicationUser();
            currUserId = _appUser.EmployeeID;
            currUserDeptId = _appUser.GetDepartmentID();
            currUserPlantId = _appUser.GetPlantID();
        }

        // GET: Mailing
        public ActionResult Index()
        {
            List<Rule> userRules = new List<Rule>();
            List<Role> userRoles = new List<Role>();
            List<Plant> userPlants = new List<Plant>();
            IEnumerable<MailingIndexViewModel> model = null;
            string controllerName = ControllerContext.RouteData.Values["controller"].ToString();
            bool IsAdmin = _appUser.isAdmin;
            bool IsManager = _appUser.IsManager;

            DateTime fromDate = DateTime.Now;
            DateTime toDate = fromDate.AddMonths(1).AddSeconds(-1);

            var mails = en.Mails.Include(m => m.Employee)
                .Where(m => m.Inactive != true)
                .Where(m => m.SendingDate >= fromDate && m.SendingDate <= toDate)
                .Where(m => m.Employee.Department.Plant_Id == currUserPlantId);

            if (IsAdmin)
            {
                userRules = en.Rules
                    .Join(en.Modules, ru => ru.Module_ID, mo => mo.Module_ID, (ru, mo) => new { ru, mo })
                    .Where(grp => grp.ru.Deactive != true && grp.mo.Module_Name.ToUpper() == controllerName.ToUpper())
                    .Select(grp => grp.ru)
                    .ToList();

                userPlants = en.Plants
                    .Select(p => new Plant { Plant_Id = p.Plant_Id, Plant_Name = p.Plant_Name })
                    .ToList();
            }
            else
            {
                userRoles = _appUser.GetRolesByModuleName(controllerName);
                userRules = _appUser.GetRules(userRoles, controllerName);
                userPlants = _appUser.GetAuthoPlantsByModuleName(controllerName);

                mails = mails.Where(m => m.Employee.Department_ID == currUserDeptId);

                if (!IsManager)
                {
                    mails = mails.Where(m => m.EmployeeId == currUserId);
                }
            }

            ViewBag.userRules = userRules.Select(ru => ru.Rule_Name).ToList();
            ViewBag.userPlants = userPlants;

            model = mails.Select(m => new MailingIndexViewModel
            {
                MailID = m.MailID,
                MailTitle = m.MailTitle,
                FromAddress = m.FromAddress,
                ToAddress = m.ToAddress,
                CcAddress = m.CcAddress,
                Attachment = m.Attachment,
                EmployeeName = m.Employee.Employee_Name,
                SendingDate = m.SendingDate,
                SendingStatus = m.SendingStatus,
                DepartmentName = m.Employee.Department.Department_Name
            });

            return View(model.ToList());
        }

        // POST: Mailing
        [HttpPost]
        public ActionResult Index(DateTime? fromDate, DateTime? toDate, string plants, string keyword)
        {
            List<Rule> userRules = new List<Rule>();
            List<Role> userRoles = new List<Role>();
            List<Plant> userPlants = new List<Plant>();
            IEnumerable<MailingIndexViewModel> model = null;
            string controllerName = ControllerContext.RouteData.Values["controller"].ToString();
            bool IsAdmin = _appUser.isAdmin;
            bool IsManager = _appUser.IsManager;

            var mails = en.Mails.Include(m => m.Employee)
                .Where(m => m.Inactive != true);

            if (fromDate != null && toDate != null)
            {
                mails = mails.Where(m => m.SendingDate >= fromDate && m.SendingDate <= toDate);
            }

            if (IsAdmin)
            {
                userRules = en.Rules
                    .Join(en.Modules, ru => ru.Module_ID, mo => mo.Module_ID, (ru, mo) => new { ru, mo })
                    .Where(grp => grp.ru.Deactive != true && grp.mo.Module_Name.ToUpper() == controllerName.ToUpper())
                    .Select(grp => grp.ru)
                    .ToList();

                userPlants = en.Plants
                    .Select(p => new Plant { Plant_Id = p.Plant_Id, Plant_Name = p.Plant_Name })
                    .ToList();

                if (plants != "all" && plants != null)
                {
                    mails = mails.Where(m => m.Employee.Department.Plant_Id == plants);
                }
            }
            else
            {
                userRoles = _appUser.GetRolesByModuleName(controllerName);
                userRules = _appUser.GetRules(userRoles, controllerName);
                userPlants = _appUser.GetAuthoPlantsByModuleName(controllerName);

                mails = mails.Where(m => m.Employee.Department_ID == currUserDeptId);

                if (!IsManager)
                {
                    mails = mails.Where(m => m.EmployeeId == currUserId);
                }
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                mails = mails.Where(m => m.MailTitle.Contains(keyword));
            }

            ViewBag.userRules = userRules.Select(ru => ru.Rule_Name).ToList();
            ViewBag.userPlants = userPlants;

            model = mails.Select(m => new MailingIndexViewModel
            {
                MailID = m.MailID,
                MailTitle = m.MailTitle,
                FromAddress = m.FromAddress,
                ToAddress = m.ToAddress,
                CcAddress = m.CcAddress,
                Attachment = m.Attachment,
                EmployeeName = m.Employee.Employee_Name,
                SendingDate = m.SendingDate,
                SendingStatus = m.SendingStatus,
                DepartmentName = m.Employee.Department.Department_Name
            });

            return View(model.ToList());
        }

        // GET: Mailing/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mail mail = en.Mails.Find(id);
            if (mail == null)
            {
                return HttpNotFound();
            }
            return View(mail);
        }

        // GET: Mailing/Create
        public ActionResult Create()
        {
            SelectList plants = new SelectList(en.Plants, "Plant_ID", "Plant_Name");
            IEnumerable<DepartmentViewModel> departmentVMs = GetDepartmentVMsByPlantId(currUserPlantId);
            SelectList positions = new SelectList(new List<MailingPositionViewModel>()
            {
                new MailingPositionViewModel()
                {
                    PositionId = 0,
                    PositionName = "Head of Department"
                },
                new MailingPositionViewModel()
                {
                    PositionId = 1,
                    PositionName = "Manager"
                },
                new MailingPositionViewModel()
                {
                    PositionId = 2,
                    PositionName = "Sales"
                }
            }, "PositionId", "PositionName");

            IEnumerable<MailingEmailsViewModel> initialEmails = GetInitialEmails(currUserPlantId, null, null);

            MailingCreateViewModel model = new MailingCreateViewModel()
            {
                Plants = plants,
                DepartmentVMs = departmentVMs,
                Positions = positions,
                PlantId = currUserPlantId,
                DepartmentId = "",
                PositionId = null,
                InitialEmails = initialEmails,
                EmployeeName = "",
                Email = "",
                MailTitle = "",
                MailContent = "",
                FromAddress = "",
                ToAddress = "",
                Attachment = "",
                MailPicture = "",
                EmployeeId = "",
                SenderPW = ""
            };

            return View(model);
        }

        // POST: Mailing/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(MailingCreateViewModel model)
        {
            Mail mail = new Mail();
            if (ModelState.IsValid)
            {
                mail.MailID = Guid.NewGuid();
                en.Mails.Add(mail);
                en.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EmployeeId = new SelectList(en.Employees, "Emp_CJ", "Emp_ID", mail.EmployeeId);
            return View(mail);
        }

        // GET: Mailing/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mail mail = en.Mails.Find(id);
            if (mail == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeId = new SelectList(en.Employees, "Emp_CJ", "Emp_ID", mail.EmployeeId);
            return View(mail);
        }

        // POST: Mailing/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MailID,MailTitle,MailContent,FromAddress,ToAddress,CcAddress,BccAddress,Attachment,MailPicture,EmployeeId,SendingDate,SendingStatus")] Mail mail)
        {
            if (ModelState.IsValid)
            {
                en.Entry(mail).State = EntityState.Modified;
                en.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeId = new SelectList(en.Employees, "Emp_CJ", "Emp_ID", mail.EmployeeId);
            return View(mail);
        }

        // GET: Mailing/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mail mail = en.Mails.Find(id);
            if (mail == null)
            {
                return HttpNotFound();
            }
            return View(mail);
        }

        // POST: Mailing/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Mail mail = en.Mails.Find(id);
            en.Mails.Remove(mail);
            en.SaveChanges();
            return RedirectToAction("Index");
        }

        public PartialViewResult UpdateDepartmentDDLByPlant(string plantID)
        {
            MailingCreateViewModel model = new MailingCreateViewModel();
            model.DepartmentVMs = GetDepartmentVMsByPlantId(plantID);
            return PartialView("_DepartmentPartialView", model);
        }

        public JsonResult LoadEmail(string plantId, string departmentId, int? positionId)
        {

            string jsonResult = JsonConvert.SerializeObject(GetInitialEmails(plantId, departmentId, positionId));

            return Json(new { success = true, data = jsonResult }, JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<DepartmentViewModel> GetDepartmentVMsByPlantId(string plantId)
        {
            IEnumerable<Department> departments = en.Departments.Where(d => d.Deactive != true);
            if (!string.IsNullOrWhiteSpace(plantId))
            {
                departments = departments.Where(d => d.Plant_Id == plantId);
            }

            IEnumerable<DepartmentViewModel> result = departments
                   .Select(d => new DepartmentViewModel()
                   {
                       Department_Id = d.Department_Id,
                       Department_Name = d.Department_Name
                   });

            return result;
        }

        public IEnumerable<MailingEmailsViewModel> GetInitialEmails(string plantId, string departmentId, int? positionId)
        {
            var employeeWithDept = en.Employees
            .Join(en.Departments, e => e.Department_ID, d => d.Department_Id, (e, d) => new { e, d })
            .Where(grp => grp.e.Deactive != true);

            if (!string.IsNullOrEmpty(plantId))
            {
                employeeWithDept = employeeWithDept.Where(grp => grp.d.Plant_Id == plantId);
            }

            if (!string.IsNullOrEmpty(departmentId))
            {
                employeeWithDept = employeeWithDept.Where(grp => grp.d.Department_Id == departmentId);
            }

            if (positionId != null)
            {
                string positionKeyword = "";
                switch (positionId)
                {
                    case 0:
                        positionKeyword = "Head of";
                        break;
                    case 1:
                        positionKeyword = "Manager";
                        break;
                    case 2:
                        positionKeyword = "Sales";
                        break;
                    default:
                        break;
                }

                employeeWithDept = employeeWithDept.Where(grp => grp.e.Position.Contains(positionKeyword));
            }

            IEnumerable<MailingEmailsViewModel> emails = employeeWithDept
                .Select(grp => new MailingEmailsViewModel()
                {
                    EmployeeName = grp.e.Employee_Name,
                    Email = grp.e.Email,
                    Position = grp.e.Position
                });

            return emails;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                en.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
