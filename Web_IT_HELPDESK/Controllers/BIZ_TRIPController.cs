using Microsoft.Reporting.WebForms;
using System;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Web_IT_HELPDESK.Controllers.ObjectManager;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.Models.Extensions;

namespace Web_IT_HELPDESK.Controllers
{
    public class BIZ_TRIPController : Controller
    {

        public ServiceDeskEntities en { get; set; }
        public ApplicationUser _appUser { get; set; }
        public string currUserID { get; set; }
        public string currUserDeptId { get; set; }
        public string currUserPlantId { get; set; }
        private DateTime to_date { get; set; }
        private DateTime from_date { get; set; }

        public BIZ_TRIPController()
        {
            en = new ServiceDeskEntities();
            _appUser = new ApplicationUser();
            currUserID = _appUser.EmployeeID;
            currUserDeptId = _appUser.GetDepartmentID();
            currUserPlantId = _appUser.GetPlantID();
        }

        [CustomAuthorize]
        public ActionResult Index()
        {
            ViewBag.ModalState = "false";
            ViewBag.Message = "";
            var departmentNames = DepartmentModel.Instance.getDeptNames();
            SelectList deptlist = new SelectList(departmentNames);
            ViewBag.DepartmentName = deptlist;

            string currUserDeptId = _appUser.GetDepartmentID();
            string currUserPlantId = _appUser.GetPlantID();

            from_date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            to_date = from_date.AddMonths(1).AddSeconds(-1);


            bool currUserIsManager = _appUser.isAdmin;

            string deptIdHrAdmin = currUserPlantId + "S0002";
            bool currUserIsHrAdmin = en.Departments
                .FirstOrDefault(d => d.Plant_Id == currUserPlantId
                                  && d.Department_Id == deptIdHrAdmin
                                  && d.Manager_Id == currUserID) != null ? true : false;

            var bizz = en.BIZ_TRIP.Where(i => i.DEL != true
                                           && i.DEPT == currUserDeptId
                                           && i.DATE >= from_date
                                           && i.DATE <= to_date
                                           && i.PLANT == currUserPlantId);

            bool isResend = true;

            if (currUserIsManager == false && currUserIsHrAdmin == false)
            {
                bizz = bizz.Where(i => i.EMPNO == currUserID);
                isResend = false;
            }

            ViewBag.IsResend = isResend;
            return View(bizz);
        }
        [HttpPost]
        [CustomAuthorize]
        public ActionResult Index(string searchString, string _datetime, int? page)
        {
            ViewBag.ModalState = "false";
            ViewBag.Message = "";

            from_date = DateTime.ParseExact("01/" + _datetime, "dd/MM/yyyy", null);
            to_date = from_date.AddMonths(1).AddSeconds(-1);

            ViewBag.DepartmentNameview = _appUser.GetDepartmentName();

            bool currUserIsManager = _appUser.isAdmin;

            string deptIdHrAdmin = currUserPlantId + "S0002";
            bool currUserIsHrAdmin = en.Departments
                .FirstOrDefault(d => d.Plant_Id == currUserPlantId
                                  && d.Department_Id == deptIdHrAdmin
                                  && d.Manager_Id == currUserID) != null ? true : false;

            var bizz = en.BIZ_TRIP.Where(i => i.DEL != true
                                           && i.DEPT == currUserDeptId
                                           && i.DATE >= from_date
                                           && i.DATE <= to_date
                                           && i.PLANT == currUserPlantId);

            bool isResend = true;

            if (currUserIsManager == false && currUserIsHrAdmin == false)
            {
                bizz = bizz.Where(i => i.EMPNO == currUserID);
                isResend = false;
            }

            ViewBag.IsResend = isResend;

            if (!String.IsNullOrEmpty(searchString))
            {
                bizz = bizz.Where(s => (s.DEPT.Trim().ToUpper().Contains(searchString.Trim().ToUpper())
                                         || s.Employee.EmployeeName.Trim().ToUpper().Contains(searchString.Trim().ToUpper())) && s.PLANT == currUserPlantId);
            }
            return View(bizz);
        }
        [CustomAuthorize]
        public ActionResult Create()
        {
            BIZ_TRIP biz_trip = new BIZ_TRIP();
            ViewBag.DepartmentId = currUserDeptId;
            ViewBag.DepartmentName = _appUser.GetDepartmentName();

            ViewBag.EMPNO = currUserID;
            ViewBag.NAME = _appUser.EmployeeName;
            ViewBag.Position = en.Employee_New.Find(currUserID).Position.ToString();

            ViewBag.ModalState = "false";
            ViewBag.Message = "";
            return View(biz_trip);
        }

        [HttpPost]
        [CustomAuthorize]
        public ActionResult Create(BIZ_TRIP biz_trip, string p_hour_f, string p_minute_f, string p_hour_t, string p_minute_t)
        {
            string result;
            string linkConfirm = "";

            string fromdate = biz_trip.FROM_DATE.Value.ToString("dd/MM/yyyy") + " " + p_hour_f + ":" + p_minute_f + ":00,531";
            DateTime v_fromdate = DateTime.ParseExact(fromdate, "dd/MM/yyyy HH:mm:ss,fff", CultureInfo.InvariantCulture);

            string todate = biz_trip.TO_DATE.Value.ToString("dd/MM/yyyy") + " " + p_hour_t + ":" + p_minute_t + ":00,531";
            DateTime v_to_date = DateTime.ParseExact(todate, "dd/MM/yyyy HH:mm:ss,fff", CultureInfo.InvariantCulture);
            biz_trip.DEPT = currUserDeptId;
            biz_trip.EMPNO = currUserID;
            biz_trip.FROM_DATE = v_fromdate;
            biz_trip.TO_DATE = v_to_date;
            biz_trip.PLANT = currUserPlantId;
            if (biz_trip.USED_EQUIPMENT == false)
                biz_trip.REMARK = "none";
            en.BIZ_TRIP.Add(biz_trip);
            try
            {
                en.SaveChanges();
                string dept_name = DepartmentModel.Instance.getDeptNameByDeptId(biz_trip.DEPT);
                bool currUserIsManager = _appUser.IsManager;

                Employee_New userRequest = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == biz_trip.EMPNO);
                string domainName = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                if (currUserIsManager)
                {
                    linkConfirm = domainName + @"/servicedesk/BIZ_TRIP/bod_confirm/";
                    bool resultMailing = Biz_TripHelper.Instance.sendBiz_TripEmail(biz_trip, 3, userRequest, linkConfirm); // Level 3: BOD
                    if (resultMailing)
                    {
                        result = string.Format("Email to confirm the Business Trip Registration has been sent to BOD: " + dept_name);
                    }
                    else
                    {
                        result = string.Format("Can't send confirm email to BOD. Please contact for support: minhsang.it@cjvina.com");
                    }
                }
                else
                {
                    linkConfirm = domainName + @"/servicedesk/BIZ_TRIP/dept_confirm/";
                    bool resultMailing = Biz_TripHelper.Instance.sendBiz_TripEmail(biz_trip, 1, userRequest, linkConfirm); // Level 1: Department Manager
                    if (resultMailing)
                    {
                        result = string.Format("Email to confirm the Business Trip Registration has been sent to Department Manager: " + dept_name);
                    }
                    else
                    {
                        result = string.Format("Can't send confirm email to Department Manager. Please contact for support: minhsang.it@cjvina.com");
                    }
                }
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);
                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                result = string.Format("Can't create your Business Trip Registration. Please contact for support: minhsang.it@cjvina.com");
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
            return View(biz_trip);
        }
        [CustomAuthorize]
        public ActionResult Details(Guid? id)
        {
            BIZ_TRIP biz_trip = en.BIZ_TRIP.Find(id);
            ViewBag.EMPNO = currUserID;
            ViewBag.DepartmentId = biz_trip.DEPT;
            ViewBag.PLANT = biz_trip.PLANT;
            ViewBag.DepartmentName = en.Departments.Where(o => o.Department_Id == biz_trip.DEPT
                                                         && o.Plant_Id == biz_trip.PLANT).Select(i => i.Department_Name).SingleOrDefault();

            ViewBag.User_name = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == biz_trip.EMPNO).Employee_Name;

            bool currUserIsManager = _appUser.IsManager;

            ViewBag.IsManager = currUserIsManager;

            ViewBag.ModalState = "false";
            ViewBag.Message = "";
            return View(biz_trip);
        }
        public ActionResult dept_confirm(Guid? id)
        {
            BIZ_TRIP biz_trip = en.BIZ_TRIP.Find(id);
            //ViewBag.Department_confirm_date = DateTime.Now;
            ViewBag.EMPNO = currUserID;
            ViewBag.DepartmentId = biz_trip.DEPT;
            ViewBag.PLANT = biz_trip.PLANT;
            ViewBag.DepartmentName = en.Departments.Where(o => o.Department_Id == biz_trip.DEPT
                                                         && o.Plant_Id == biz_trip.PLANT).Select(i => i.Department_Name).SingleOrDefault();
            ViewBag.User_name = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == biz_trip.EMPNO).Employee_Name;

            ViewBag.ModalState = "false";
            ViewBag.Message = "";
            return View(biz_trip);
        }
        [HttpPost]
        public ActionResult dept_confirm(BIZ_TRIP biz_trip, HttpPostedFileBase signatureimage, bool approved)
        {
            string result = "";
            var dept = from i in en.Departments where i.Department_Id == biz_trip.DEPT && i.Plant_Id == biz_trip.PLANT select i.Department_Name;

            biz_trip.DEPT_CONFIRM = approved;
            var existingCart = en.BIZ_TRIP.Find(biz_trip.ID);
            if (biz_trip.DEPT_CONFIRM == true)
            {
                existingCart.DEPT_CONFIRM = biz_trip.DEPT_CONFIRM;
                existingCart.DEPT_CONFIRM_DATE = biz_trip.DEPT_CONFIRM_DATE;
                existingCart.DEPT_CONFIRM_IMAGE = biz_trip.DEPT_CONFIRM_IMAGE;

                signatureimage = signatureimage ?? Request.Files["signatureimage"];
                if (signatureimage != null && signatureimage.ContentLength > 0)
                {
                    byte[] fileData = new byte[Request.Files["signatureimage"].InputStream.Length];
                    Request.Files["signatureimage"].InputStream.Read(fileData, 0, Convert.ToInt32(Request.Files["signatureimage"].InputStream.Length));
                    System.Data.Linq.Binary binaryFile = new System.Data.Linq.Binary(fileData);

                    existingCart.DEPT_CONFIRM_IMAGE_NAME = Request.Files["signatureimage"].FileName;
                    existingCart.DEPT_CONFIRM_IMAGE = binaryFile.ToArray();
                }
                else
                {
                    ModelState.AddModelError("DEPT_CONFIRM_IMAGE", "Please select signature");
                    existingCart.DEPT_CONFIRM_DATE = null;
                    ViewBag.ModalState = "false";
                    ViewBag.Message = "";
                    return View(existingCart);
                }

                en.Entry(existingCart).State = System.Data.Entity.EntityState.Modified;
                en.SaveChanges();

                Employee_New userRequest = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == existingCart.EMPNO);
                bool resultMailing = Biz_TripHelper.Instance.sendBiz_TripEmail(existingCart, 4, userRequest); // Level 4: HR Manager
                if (resultMailing)
                {
                    result = string.Format("Approved! Email to confirm the Business Trip Registration has been sent to HR Manager");
                }
                else
                {
                    result = string.Format("Can't send confirm email to HR Manager. Please contact for support: minhsang.it@cjvina.com");
                }
            }
            else
            {
                existingCart.DEPT_CONFIRM = biz_trip.DEPT_CONFIRM;
                existingCart.DEPT_CONFIRM_DATE = biz_trip.DEPT_CONFIRM_DATE;
                en.Entry(existingCart).State = System.Data.Entity.EntityState.Modified;
                en.SaveChanges();

                Employee_New userRequest = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == existingCart.EMPNO);
                //Employee userRequest = en.Employees.FirstOrDefault(e => e.Emp_CJ == existingCart.EMPNO);
                bool resultMailing = Biz_TripHelper.Instance.sendBiz_TripEmail(existingCart, 8, userRequest); // Level 8: Return NOT APPROVED
                if (resultMailing)
                {
                    result = string.Format("Not approved! Feedback mail has been sent to employee");
                }
                else
                {
                    result = string.Format("Can't send feedback email to employee. Please contact for support: minhsang.it@cjvina.com");
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
            return View(existingCart);
        }
        public ActionResult bod_confirm(Guid? id)
        {
            BIZ_TRIP biz_trip = en.BIZ_TRIP.Find(id);
            //ViewBag.Department_confirm_date = DateTime.Now;
            ViewBag.EMPNO = currUserID;
            ViewBag.DepartmentId = biz_trip.DEPT;
            ViewBag.PLANT = biz_trip.PLANT;
            ViewBag.DepartmentName = en.Departments.Where(o => o.Department_Id == biz_trip.DEPT
                                                         && o.Plant_Id == biz_trip.PLANT).Select(i => i.Department_Name).SingleOrDefault();
            ViewBag.User_name = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == biz_trip.EMPNO).Employee_Name;

            ViewBag.ModalState = "false";
            ViewBag.Message = "";
            return View(biz_trip);
        }
        [HttpPost]
        public ActionResult bod_confirm(BIZ_TRIP biz_trip, HttpPostedFileBase BODsignatureimage, bool BODapproved)
        {
            string result = "";
            var dept = from i in en.Departments where i.Department_Id == biz_trip.DEPT && i.Plant_Id == biz_trip.PLANT select i.Department_Name;

            biz_trip.BOD_CONFIRM = BODapproved;
            var existingCart = en.BIZ_TRIP.Find(biz_trip.ID);
            if (biz_trip.BOD_CONFIRM == true)
            {
                existingCart.BOD_CONFIRM = biz_trip.BOD_CONFIRM;
                existingCart.BOD_CONFIRM_DATE = biz_trip.BOD_CONFIRM_DATE;
                existingCart.BOD_CONFIRM_IMAGE = biz_trip.BOD_CONFIRM_IMAGE;

                BODsignatureimage = BODsignatureimage ?? Request.Files["BODsignatureimage"];
                if (BODsignatureimage != null && BODsignatureimage.ContentLength > 0)
                {
                    byte[] fileData = new byte[Request.Files["BODsignatureimage"].InputStream.Length];
                    Request.Files["BODsignatureimage"].InputStream.Read(fileData, 0, Convert.ToInt32(Request.Files["BODsignatureimage"].InputStream.Length));
                    System.Data.Linq.Binary binaryFile = new System.Data.Linq.Binary(fileData);

                    existingCart.BOD_CONFIRM_IMAGE_NAME = Request.Files["BODsignatureimage"].FileName;
                    existingCart.BOD_CONFIRM_IMAGE = binaryFile.ToArray();
                }
                else
                {
                    ModelState.AddModelError("BOD_CONFIRM_IMAGE", "Please select signature");
                    existingCart.BOD_CONFIRM_DATE = null;
                    ViewBag.ModalState = "false";
                    ViewBag.Message = "";
                    return View(existingCart);
                }

                en.Entry(existingCart).State = System.Data.Entity.EntityState.Modified;
                en.SaveChanges();

                Employee_New userRequest = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == existingCart.EMPNO);
                bool resultMailing = Biz_TripHelper.Instance.sendBiz_TripEmail(existingCart, 4, userRequest); // Level 4: HR Manager
                if (resultMailing)
                {
                    result = string.Format("Approved! Email to confirm the Business Trip Registration has been sent to HR Manager");
                }
                else
                {
                    result = string.Format("Can't send confirm email to HR Manager. Please contact for support: minhsang.it@cjvina.com");
                }
            }
            else
            {
                existingCart.BOD_CONFIRM = biz_trip.BOD_CONFIRM;
                existingCart.BOD_CONFIRM_DATE = biz_trip.BOD_CONFIRM_DATE;
                en.Entry(existingCart).State = System.Data.Entity.EntityState.Modified;
                en.SaveChanges();

                Employee_New userRequest = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == existingCart.EMPNO);
                bool resultMailing = Biz_TripHelper.Instance.sendBiz_TripEmail(existingCart, 8, userRequest); // Level 8: Return NOT APPROVED
                if (resultMailing)
                {
                    result = string.Format("Not approved! Feedback mail has been sent to employee");
                }
                else
                {
                    result = string.Format("Can't send feedback email to employee. Please contact for support: minhsang.it@cjvina.com");
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
            return View(existingCart);
        }
        public ActionResult hr_confirm(Guid? id)
        {
            BIZ_TRIP biz_trip = en.BIZ_TRIP.Find(id);
            //ViewBag.Department_confirm_date = DateTime.Now;
            ViewBag.EMPNO = currUserID;
            ViewBag.DepartmentId = biz_trip.DEPT;
            ViewBag.PLANT = biz_trip.PLANT;
            ViewBag.DepartmentName = en.Departments.Where(o => o.Department_Id == biz_trip.DEPT
                                                         && o.Plant_Id == biz_trip.PLANT).Select(i => i.Department_Name).SingleOrDefault();
            ViewBag.User_name = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == biz_trip.EMPNO).Employee_Name;

            ViewBag.ModalState = "false";
            ViewBag.Message = "";
            return View(biz_trip);
        }
        [HttpPost]
        public ActionResult hr_confirm(BIZ_TRIP biz_trip, HttpPostedFileBase HRsignatureimage, bool HRapproved)
        {
            string result = "";
            var dept = from i in en.Departments where i.Department_Id == biz_trip.DEPT && i.Plant_Id == biz_trip.PLANT select i.Department_Name;
            biz_trip.HR_CONFIRM = HRapproved;
            var existingCart = en.BIZ_TRIP.Find(biz_trip.ID);
            if (biz_trip.HR_CONFIRM == true)
            {
                existingCart.HR_CONFIRM = biz_trip.HR_CONFIRM;
                existingCart.HR_CONFIRM_DATE = biz_trip.HR_CONFIRM_DATE;
                existingCart.HR_CONFIRM_IMAGE = biz_trip.HR_CONFIRM_IMAGE;

                HRsignatureimage = HRsignatureimage ?? Request.Files["HRsignatureimage"];
                if (HRsignatureimage != null && HRsignatureimage.ContentLength > 0)
                {
                    byte[] fileData = new byte[Request.Files["HRsignatureimage"].InputStream.Length];
                    Request.Files["HRsignatureimage"].InputStream.Read(fileData, 0, Convert.ToInt32(Request.Files["HRsignatureimage"].InputStream.Length));
                    System.Data.Linq.Binary binaryFile = new System.Data.Linq.Binary(fileData);

                    existingCart.HR_CONFIRM_IMAGE_NAME = Request.Files["HRsignatureimage"].FileName;
                    existingCart.HR_CONFIRM_IMAGE = binaryFile.ToArray();
                }
                else
                {
                    ModelState.AddModelError("HR_CONFIRM_IMAGE", "Please select signature");
                    existingCart.HR_CONFIRM_DATE = null;
                    ViewBag.ModalState = "false";
                    ViewBag.Message = "";
                    return View(existingCart);
                }

                en.Entry(existingCart).State = System.Data.Entity.EntityState.Modified;
                en.SaveChanges();

                Employee_New userRequest = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == existingCart.EMPNO);
                bool resultMailing = Biz_TripHelper.Instance.sendBiz_TripEmail(existingCart, 5, userRequest); // Level 5: HR Admin
                if (resultMailing)
                {
                    result = string.Format("Approved. Email to confirm the business trip registration has been sent to HR Admin");
                }
                else
                {
                    result = string.Format("Can't send confirm email to HR Admin. Please contact for support: minhsang.it@cjvina.com");
                }
            }
            else
            {
                existingCart.HR_CONFIRM = biz_trip.HR_CONFIRM;
                existingCart.HR_CONFIRM_DATE = biz_trip.HR_CONFIRM_DATE;
                en.Entry(existingCart).State = System.Data.Entity.EntityState.Modified;
                en.SaveChanges();

                Employee_New userRequest = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == existingCart.EMPNO);
                bool resultMailing = Biz_TripHelper.Instance.sendBiz_TripEmail(existingCart, 8, userRequest); // Level 8: Return NOT APPROVED
                if (resultMailing)
                {
                    result = string.Format("Not approved! Feedback mail has been sent to employee");
                }
                else
                {
                    result = string.Format("Can't send feedback email to employee. Please contact for support: minhsang.it@cjvina.com");
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
            return View(existingCart);
        }
        public ActionResult hr_admin(Guid? id)
        {
            BIZ_TRIP biz_trip = en.BIZ_TRIP.Find(id);
            ViewBag.EMPNO = currUserID;
            ViewBag.DepartmentId = biz_trip.DEPT;
            ViewBag.PLANT = biz_trip.PLANT;
            ViewBag.DepartmentName = en.Departments.Where(o => o.Department_Id == biz_trip.DEPT
                                                         && o.Plant_Id == biz_trip.PLANT).Select(i => i.Department_Name).SingleOrDefault();
            ViewBag.User_name = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == biz_trip.EMPNO).Employee_Name;

            ViewBag.ModalState = "false";
            ViewBag.Message = "";
            return View(biz_trip);
        }
        [HttpPost]
        public ActionResult hr_admin(BIZ_TRIP biz_trip)
        {
            string result = "";

            var existingCart = en.BIZ_TRIP.Find(biz_trip.ID);
            existingCart.Vehicle_Infor = biz_trip.Vehicle_Infor;

            en.Entry(existingCart).State = System.Data.Entity.EntityState.Modified;
            en.SaveChanges();

            Employee_New userRequest = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == existingCart.EMPNO);
            bool resultMailing = Biz_TripHelper.Instance.sendBiz_TripEmail(existingCart, 7, userRequest); // Level 7: Return APPROVED
            if (resultMailing)
            {
                result = string.Format("Feedback mail has been sent to employee");
            }
            else
            {
                result = string.Format("Can't send feedback email to employee. Please contact for support: minhsang.it@cjvina.com");
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
            return View(biz_trip);
        }
        [CustomAuthorize]
        public ActionResult Delete(Guid v_id)
        {
            BIZ_TRIP bip_trip_del = en.BIZ_TRIP.Find(v_id);
            if (bip_trip_del == null)
            {
                return HttpNotFound();
            }
            else
            {
                try
                {
                    bip_trip_del.DEL = true;
                    en.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage));
                    throw new DbEntityValidationException(errorMessages);
                }
            }

            IFormatProvider culture = new CultureInfo("en-US", true);
            string _datetime = DateTime.Now.ToString("MM/yyyy");
            from_date = DateTime.ParseExact("01/" + _datetime, "dd/MM/yyyy", culture);
            string v_mm = _datetime.Substring(0, 2);

            switch (v_mm)
            {
                case "01":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "02":
                    to_date = DateTime.ParseExact("28/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "03":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "04":
                    to_date = DateTime.ParseExact("30/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "05":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "06":
                    to_date = DateTime.ParseExact("30/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "07":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "08":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "09":
                    to_date = DateTime.ParseExact("30/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "10":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "11":
                    to_date = DateTime.ParseExact("30/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "12":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
            }

            // https://www.nuget.org/packages/PagedList.Mvc/3.0.0 --Install-Package PagedList.Mvc -Version 3.0.0

            //int pageSize = 20;
            //int pageNumber = (page ?? 1);
            var biztrip_list = en.BIZ_TRIP.Where(o => o.DEL != true
                                                && o.DEPT == currUserDeptId
                                                && o.DATE >= from_date
                                                && o.DATE <= to_date
                                                && o.PLANT == currUserPlantId).OrderBy(i => i.NO);
            return View("Index", biztrip_list);
        }
        [CustomAuthorize]
        public ActionResult Resend(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var biz_trip = en.BIZ_TRIP.FirstOrDefault(s => s.ID == id);

            if (biz_trip == null)
            {
                return HttpNotFound();
            }

            string result = "";
            string linkConfirm = "";

            string dept_name = DepartmentModel.Instance.getDeptNameByDeptId(biz_trip.DEPT);
            bool IsManager = en.Departments.FirstOrDefault(d => d.Plant_Id == biz_trip.PLANT && d.Department_Id == biz_trip.DEPT && d.Manager_Id == biz_trip.EMPNO) != null ? true : false;

            //Employee userRequest = en.Employees.FirstOrDefault(e => e.Emp_CJ == biz_trip.EMPNO);
            Employee_New userRequest = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == biz_trip.EMPNO);
            string domainName = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            if (IsManager)
            {
                linkConfirm = domainName + @"/servicedesk/BIZ_TRIP/bod_confirm/";
                bool resultMailing = Biz_TripHelper.Instance.sendBiz_TripEmail(biz_trip, 3, userRequest, linkConfirm); // Level 3: BOD
                if (resultMailing)
                {
                    result = string.Format("Email to confirm the Business Trip Registration has been sent to BOD: " + dept_name);
                }
                else
                {
                    result = string.Format("Can't send confirm email to BOD. Please contact for support: minhsang.it@cjvina.com");
                }
            }
            else
            {
                linkConfirm = domainName + @"/servicedesk/BIZ_TRIP/dept_confirm/";
                bool resultMailing = Biz_TripHelper.Instance.sendBiz_TripEmail(biz_trip, 1, userRequest, linkConfirm); // Level 1: Department Manager
                if (resultMailing)
                {
                    result = string.Format("Email to confirm the Business Trip Registration has been sent to Department Manager: " + dept_name);
                }
                else
                {
                    result = string.Format("Can't send confirm email to Department Manager. Please contact for support: minhsang.it@cjvina.com");
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

            DateTime now = DateTime.Now;
            from_date = new DateTime(now.Year, now.Month, 1);
            to_date = from_date.AddMonths(1).AddSeconds(-1);

            var bizz = en.BIZ_TRIP.Where(i => i.DEL != true
                                           && i.DEPT == currUserDeptId
                                           && i.DATE >= from_date
                                           && i.DATE <= to_date
                                           && i.PLANT == currUserPlantId);

            ViewBag.IsResend = true;

            return View("Index", bizz);
        }
        public ActionResult BizTripPrint(Guid v_id)
        {
            string reportid = "PDF";
            LocalReport lr = new LocalReport();
            string path = Path.Combine(HostingEnvironment.MapPath("~/Report"), "Report_BizTrip.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("Index");
            }

            try
            {
                GridView gv = new GridView();
                var list = from i in en.BIZ_TRIP
                           where i.ID == v_id
                           select i;

                var lstBizTrip = en.BIZ_TRIP.Where(b => b.ID == v_id).Select(b => new Biz_TripViewModel
                {
                    EMPNO = b.EMPNO,
                    NAME = b.NAME,
                    DEPT = b.DEPT,
                    DESCRIPTION = b.DESCRIPTION,
                    VEHICLE = b.VEHICLE,
                    CONTACT_COMPANY = b.CONTACT_COMPANY,
                    CONTACT_PERSON = b.CONTACT_PERSON,
                    CONTACT_DEPT = b.CONTACT_DEPT,
                    DATE = b.DATE,
                    FROM_DATE = b.FROM_DATE,
                    TO_DATE = b.TO_DATE,
                    USED_EQUIPMENT = b.USED_EQUIPMENT,
                    REMARK = b.REMARK,
                    PLANT = b.PLANT,
                    DEPT_CONFIRM_IMAGE = b.DEPT_CONFIRM_IMAGE,
                    HR_CONFIRM_IMAGE = b.HR_CONFIRM_IMAGE,
                    POSITION = b.Employee.Job,
                    DEPTNAME = en.Departments.FirstOrDefault(d => d.Plant_Id == b.PLANT && d.Department_Id == b.DEPT).Department_Name
                });

                ReportDataSource rd = new ReportDataSource("DataSetBizTrip", lstBizTrip);
                lr.DataSources.Add(rd);
                string reportType = reportid;
                string mimeType;
                string encoding;
                string fileNameExtension;

                string deviceInfo =

                "<DeviceInfo>" +
                "  <OutputFormat>" + reportid + "</OutputFormat>" +
                "  <PageWidth>8.3in</PageWidth>" +
                "  <PageHeight>11in</PageHeight>" +
                "  <MarginTop>0.2in</MarginTop>" +
                "  <MarginLeft>0.2in</MarginLeft>" +
                "  <MarginRight>0.2in</MarginRight>" +
                "  <MarginBottom>0.2in</MarginBottom>" +
                "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;

                renderedBytes = lr.Render(
                    reportType,
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);


                return File(renderedBytes, mimeType);

            }
            catch (Exception)
            {

                return View();
            }
        }

    }
}
