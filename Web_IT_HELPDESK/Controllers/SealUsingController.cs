using System;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Web_IT_HELPDESK.Controllers.ObjectManager;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.Models.Extensions;
using Web_IT_HELPDESK.ViewModels;

namespace Web_IT_HELPDESK.Controllers
{
    public class SealUsingController : Controller
    {
        public ServiceDeskEntities en { get; set; }
        public ApplicationUser _appUser { get; set; }

        public SealUsingController()
        {
            en = new ServiceDeskEntities();
            _appUser = new ApplicationUser();
        }

        // GET: /SealUsing/
        [CustomAuthorize]
        public ActionResult Index()
        {
            string curr_PlantID = _appUser.GetPlantID();
            string curr_DeptID = _appUser.GetDepartmentID();

            DateTime now = DateTime.Now;
            DateTime from_date = new DateTime(now.Year, now.Month, 1);
            DateTime to_date = from_date.AddMonths(1).AddSeconds(-1);

            bool currUserIsManager = _appUser.IsManager;
            string deptIdHrSealManager = curr_PlantID + "S0003";
            bool currUserIsHrSealManager = en.Departments
                .FirstOrDefault(d => d.Plant_Id == curr_PlantID
                && d.Department_Id == deptIdHrSealManager
                && d.Manager_Id == _appUser.EmployeeID) != null ? true : false;

            var suVM = en.Seal_Using
                .Where(s => s.Del != true
                        && s.Date >= from_date
                        && s.Date <= to_date
                      )
                .Join(en.Departments,
                      s => s.DepartmentId,
                      d => d.Department_Id,
                      (s, d) => new SealUsingViewModel.IndexSealUsing()
                      {
                          SealUsing = s,
                          DeptName = d.Department_Name
                      })
                .ToList();

            bool isResend = true;

            if (currUserIsHrSealManager != true)
            {
                suVM = suVM.Where(s => s.SealUsing.DepartmentId == curr_DeptID).ToList();
                isResend = false;
            }

            ViewBag.IsResend = isResend;
            ViewBag.ModalState = "false";
            ViewBag.Message = "";
            return View(suVM);
        }

        [HttpPost]
        [CustomAuthorize]
        public ActionResult Index(string searchString, string _datetime, int? page)
        {
            string curr_PlantID = _appUser.GetPlantID();
            string curr_DeptID = _appUser.GetDepartmentID();

            DateTime from_date = DateTime.ParseExact("01/" + _datetime, "dd/MM/yyyy", null);
            DateTime to_date = from_date.AddMonths(1).AddSeconds(-1);

            var sealusings = en.Seal_Using
                .Where(s => s.Del != true
                        && s.Date >= from_date
                        && s.Date <= to_date
                ).ToList();
            var suVM = sealusings
              .Join(en.Departments,
                    s => s.DepartmentId,
                    d => d.Department_Id,
                    (s, d) => new SealUsingViewModel.IndexSealUsing()
                    {
                        SealUsing = s,
                        DeptName = d.Department_Name
                    })
              .ToList();

            bool currUserIsManager = _appUser.IsManager;
            string deptIdHRSealManager = curr_PlantID + "S0003";
            bool currUserIsHRSealManager = en.Departments
                .FirstOrDefault(d => d.Plant_Id == curr_PlantID
                && d.Department_Id == deptIdHRSealManager
                && d.Manager_Id == _appUser.EmployeeID) != null ? true : false;

            bool isResend = true;

            if (currUserIsManager != true && currUserIsHRSealManager != true)
            {
                isResend = false;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                suVM = suVM.Where(s => (s.DeptName.Trim().ToUpper().Contains(searchString.Trim().ToUpper())
                || s.SealUsing.Employee_name.Trim().ToUpper().Contains(searchString.Trim().ToUpper()))).ToList();
            }

            ViewBag.IsResend = isResend;
            ViewBag.ModalState = "false";
            ViewBag.Message = "";
            return View(suVM);
        }

        // GET: /SealUsing/Details/5
        [CustomAuthorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var seal_Using = en.Seal_Using.FirstOrDefault(s => s.Id == id);

            if (seal_Using == null)
            {
                return HttpNotFound();
            }

            SealUsingViewModel.DetailsSealUsing dsuVM = new SealUsingViewModel.DetailsSealUsing(seal_Using);

            return View(dsuVM);
        }

        // GET: /SealUsing/Create
        [CustomAuthorize]
        public ActionResult Create()
        {
            SealUsingViewModel.CreateSealUsing csuVM = new SealUsingViewModel.CreateSealUsing();

            ViewBag.ModalState = "false";
            ViewBag.Message = "";
            return View(csuVM);
        }

        // POST: /SealUsing/Create
        [HttpPost]
        [CustomAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SealUsingViewModel.CreateSealUsing csuVM)
        {
            string result = "";
            if (ModelState.IsValid)
            {
                Seal_Using su = new Seal_Using();
                su = csuVM.CreateSealUsing_To_SealUsing(su);

                try
                {
                    en.Seal_Using.Add(su);
                    en.SaveChanges();
                    //Employee userRequest = en.Employees.FirstOrDefault(e => e.Emp_CJ == su.Employee_ID);
                    Employee_New userRequest = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == su.Employee_ID);
                    bool resultMailing = SealUsingHelper.Instance.sendSealUsingEmail(su, 1, userRequest); // Level 1: Department Manager 
                    if (resultMailing)
                    {
                        result = string.Format("Email to confirm The Seal Using Registration has been sent to Department Manager");
                    }
                    else
                    {
                        result = string.Format("Can't send confirm email to Department Manager. Please contact for support: minhsang.it@cjvina.com");
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);
                    var fullErrorMessage = string.Join("; ", errorMessages);
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                    result = string.Format("Can't create your Seal Using Registration. Please contact for support: minhsang.it@cjvina.com");
                }
            }
            if (!string.IsNullOrEmpty(result))
            {
                ViewBag.ModalState = "true";
                ViewBag.Message = result;
            }
            else
            {
                ViewBag.ModalState = "false";
                ViewBag.Message = "";
            }
            return View(csuVM);
        }

        [CustomAuthorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var seal_Using = en.Seal_Using.FirstOrDefault(s => s.Id == id);

            if (seal_Using == null)
            {
                return HttpNotFound();
            }

            SealUsingViewModel.EditSealUsing esuVM = new SealUsingViewModel.EditSealUsing(seal_Using);

            ViewBag.ModalState = "false";
            ViewBag.Message = "";
            return View(esuVM);
        }

        //POST: /SealUsing/Edit/5
        [HttpPost]
        public ActionResult Edit(SealUsingViewModel.EditSealUsing model, bool approved)
        {
            string result = "";
            var sealUsing = en.Seal_Using.FirstOrDefault(s => s.Id == model.Id);
            if (sealUsing != default(Seal_Using))
            {
                sealUsing = model.EditSealUsing_To_SealUsing(sealUsing);
                sealUsing.Department_confirm = approved;
            }

            string deptName = DepartmentModel.Instance.getDeptNameByDeptId(sealUsing.DepartmentId);
            if (ModelState.IsValid)
            {
                //Employee userRequest = en.Employees.FirstOrDefault(e => e.Emp_CJ == sealUsing.Employee_ID);
                Employee_New userRequest = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == sealUsing.Employee_ID);

                try
                {
                    en.Entry(sealUsing).State = System.Data.Entity.EntityState.Modified;
                    en.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);
                    var fullErrorMessage = string.Join("; ", errorMessages);
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                    result = string.Format("An error occurred while confirming registration. Please contact for support: minhsang.it@cjvina.com");
                    ViewBag.ModalState = "true";
                    ViewBag.Message = result;
                    return View(model);
                }

                if (sealUsing.Department_confirm == true)
                {
                    bool resultMailing = SealUsingHelper.Instance.sendSealUsingEmail(sealUsing, 6, userRequest); // Level 6: HR Seal Using
                    if (resultMailing)
                    {
                        result = string.Format("Approved! Email to confirm The Seal Using Registration has been sent to HR Seal Using Manager.");
                    }
                    else
                    {
                        result = string.Format("Can't send confirm email to HR Seal Using Manager. Please contact for support: minhsang.it@cjvina.com");
                    }
                }
                else
                {
                    bool resultMailing = SealUsingHelper.Instance.sendSealUsingEmail(sealUsing, 8, userRequest); // Level 8: Return NOT APPROVED
                    if (resultMailing)
                    {
                        result = string.Format("Not approved! Feedback mail has been sent to employee.");
                    }
                    else
                    {
                        result = string.Format("Can't send feedback email to employee. Please contact for support: minhsang.it@cjvina.com");
                    }
                }
            }
            if (!string.IsNullOrEmpty(result))
            {
                ViewBag.ModalState = "true";
                ViewBag.Message = result;
            }
            else
            {
                ViewBag.ModalState = "false";
                ViewBag.Message = "";
            }
            return View(model);
        }

        public ActionResult Confirm(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var seal_Using = en.Seal_Using.FirstOrDefault(s => s.Id == id);

            if (seal_Using == null)
            {
                return HttpNotFound();
            }

            SealUsingViewModel.ConfirmSealUsing esuVM = new SealUsingViewModel.ConfirmSealUsing(seal_Using);

            ViewBag.ModalState = "false";
            ViewBag.Message = "";
            return View(esuVM);
        }

        [HttpPost]
        public ActionResult Confirm(SealUsingViewModel.ConfirmSealUsing model, bool HRapproved)
        {
            string result = "";
            var sealUsing = en.Seal_Using.FirstOrDefault(s => s.Id == model.Id);
            if (sealUsing != default(Seal_Using))
            {
                sealUsing = model.ConfirmSealUsing_To_SealUsing(sealUsing);
                sealUsing.Employee_Seal_keep_confrim = HRapproved;
            }

            string deptName = DepartmentModel.Instance.getDeptNameByDeptId(sealUsing.DepartmentId);
            if (ModelState.IsValid)
            {
                //Employee userRequest = en.Employees.FirstOrDefault(e => e.Emp_CJ == sealUsing.Employee_ID);
                Employee_New userRequest = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == sealUsing.Employee_ID);

                try
                {
                    en.Entry(sealUsing).State = System.Data.Entity.EntityState.Modified;
                    en.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);
                    var fullErrorMessage = string.Join("; ", errorMessages);
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                    result = string.Format("An error occurred while confirming registration. Please contact for support: minhsang.it@cjvina.com");
                    ViewBag.ModalState = "true";
                    ViewBag.Message = result;
                    return View(model);
                }

                if (sealUsing.Employee_Seal_keep_confrim == true)
                {
                    bool resultMailing = SealUsingHelper.Instance.sendSealUsingEmail(sealUsing, 7, userRequest); // Level 7: Return APPROVED
                    if (resultMailing)
                    {
                        result = string.Format("Approved! Feedback mail has been sent to employee.");
                    }
                    else
                    {
                        result = string.Format("Can't send feedback email to employee. Please contact for support: minhsang.it@cjvina.com");
                    }
                }
                else
                {
                    bool resultMailing = SealUsingHelper.Instance.sendSealUsingEmail(sealUsing, 8, userRequest); // Level 8: Return NOT APPROVED
                    if (resultMailing)
                    {
                        result = string.Format("Not approved! Feedback mail has been sent to employee.");
                    }
                    else
                    {
                        result = string.Format("Can't send feedback email to employee. Please contact for support: minhsang.it@cjvina.com");
                    }
                }
            }
            if (!string.IsNullOrEmpty(result))
            {
                ViewBag.ModalState = "true";
                ViewBag.Message = result;
            }
            else
            {
                ViewBag.ModalState = "false";
                ViewBag.Message = "";
            }
            return View(model);
        }

        [CustomAuthorize]
        public ActionResult Resend(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var seal_Using = en.Seal_Using.FirstOrDefault(s => s.Id == id);

            if (seal_Using == null)
            {
                return HttpNotFound();
            }

            SealUsingViewModel.EditSealUsing esuVM = new SealUsingViewModel.EditSealUsing(seal_Using);

            string result = "";

            //Employee userRequest = en.Employees.FirstOrDefault(e => e.Emp_CJ == seal_Using.Employee_ID);
            Employee_New userRequest = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == seal_Using.Employee_ID);
            bool resultMailing = SealUsingHelper.Instance.sendSealUsingEmail(seal_Using, 2, userRequest); // Level 2: Resend
            if (resultMailing)
            {
                result = string.Format("Email to confirm The Seal Using Registration has been sent to Department Manager");
            }
            else
            {
                result = string.Format("Can't send confirm email to Department Manager. Please contact for support: minhsang.it@cjvina.com");
            }

            string curr_PlantID = _appUser.GetPlantID();
            string curr_DeptID = _appUser.GetDepartmentID();

            DateTime from_date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime to_date = from_date.AddMonths(1).AddSeconds(-1);

            ViewBag.IsResend = true;

            var suVM = en.Seal_Using
                .Where(s => s.Del != true
                        && s.DepartmentId == curr_DeptID
                        && s.Date >= from_date
                        && s.Date <= to_date
                      )
                .Join(en.Departments,
                      s => s.DepartmentId,
                      d => d.Department_Id,
                      (s, d) => new SealUsingViewModel.IndexSealUsing()
                      {
                          SealUsing = s,
                          DeptName = d.Department_Name
                      }
                      )
                .ToList();

            if (!string.IsNullOrEmpty(result))
            {
                ViewBag.ModalState = "true";
                ViewBag.Message = result;
            }
            else
            {
                ViewBag.ModalState = "false";
                ViewBag.Message = "";
            }

            return View("Index", suVM);
        }
        //
        // GET: /SealUsing/Delete/5

        public ActionResult Delete(int? id)
        {
            Seal_Using seal_using = en.Seal_Using.Find(id);
            ViewBag.Department_confirm_date = DateTime.Now;
            string plant_id = _appUser.GetPlantID();
            ViewBag.DepartmentId = _appUser.GetDepartmentID();
            return View(seal_using);
        }

        //
        // POST: /SealUsing/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int? id)
        {
            try
            {
                Seal_Using seal_using = en.Seal_Using.Find(id);
                if (seal_using == null)
                    return HttpNotFound();
                else if (seal_using.Department_confirm == false)
                {
                    seal_using.Del = true;
                    en.SaveChanges();
                }
            }
            catch
            {
            }
            return RedirectToAction("Index", "SealUsing");
        }
    }
}