﻿using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Web_IT_HELPDESK.Controllers.ObjectManager;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.ViewModels;

namespace Web_IT_HELPDESK.Controllers
{
    public class BIZ_TRIPController : Controller
    {
        // GET: /BIZ_TRIP/
        ServiceDeskEntities en = new ServiceDeskEntities();
        private string session_emp = System.Web.HttpContext.Current.User.Identity.Name;
        private DateTime to_date { get; set; }
        private DateTime from_date { get; set; }
        public UserManager userManager = new UserManager();

        private string GetDept_id(string v_plant_id)
        {
            string dept_id = en.Employees.Where(f => (f.Emp_CJ == session_emp && f.Plant_Id == v_plant_id)).Select(f => f.Department_Id).SingleOrDefault();
            return dept_id;
        }

        public ActionResult biz_trip_index()
        {
            ViewBag.ModalState = "hide";
            ViewBag.Message = "";
            var DepartmentName = from i in en.Departments where i.Deactive != true select i.Department_Name;
            SelectList deptlist = new SelectList(DepartmentName);
            ViewBag.DepartmentName = deptlist;
            string plant_id = userManager.GetUserPlant(session_emp);

            string dept_id = Convert.ToString(GetDept_id(plant_id));
            string dept_name = en.Departments.Where(o => o.Department_Id == dept_id && o.Plant_Id == plant_id).Select(f => f.Department_Name).SingleOrDefault();
            ViewBag.DepartmentNameview = dept_name;

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
            if (session_emp != "")
            {
                /*if (session_emp != "admin" && session_emp != "D83003")
                {
                    var students = en.BIZ_TRIP.Where(i => i.DEL != true 
                                                    && i.DEPT == dept_id 
                                                    && i.PLANT == plant_id 
                                                    && i.DATE >= from_date 
                                                    && i.DATE <= to_date 
                                                    && i.PLANT == plant_id).OrderBy(o => o.NO);
                    return View(students);//.ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    var students2 = en.BIZ_TRIP.Where(i => i.DEL != true
                                                          && i.DATE >= from_date
                                                          && i.DATE <= to_date
                                                          && i.PLANT == plant_id
                                                          ).OrderBy(o => o.NO);
                    return View(students2);//.ToPagedList(pageNumber, pageSize));
                }*/
                var students = en.BIZ_TRIP.Where(i => i.DEL != true
                                                //&& i.DEPT == dept_id
                                                && i.PLANT == plant_id
                                                && i.DATE >= from_date
                                                && i.DATE <= to_date
                                                && i.PLANT == plant_id).OrderBy(o => o.NO);
                return View(students);//.ToPagedList(pageNumber, pageSize));
            }
            else return RedirectToAction("LogOn", "LogOn");
        }

        [Authorize]
        [HttpPost]
        public ActionResult biz_trip_index(string searchString, string _datetime, int? page)
        {
            ViewBag.ModalState = "hide";
            ViewBag.Message = "";
            //http://www.asp.net/mvc/overview/getting-started/getting-started-with-ef-using-mvc/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application
            IFormatProvider culture = new CultureInfo("en-US", true);
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
            string plant_id = userManager.GetUserPlant(session_emp);
            string dept_id = Convert.ToString(GetDept_id(plant_id));
            ViewBag.DepartmentNameview = en.Departments.Where(o => o.Department_Id == dept_id && o.Plant_Id == plant_id).Select(f => f.Department_Name).SingleOrDefault();

            if (session_emp != "")
            {
                var students = from s in en.BIZ_TRIP.Where(i => i.DEL != true && i.DATE >= from_date && i.DATE <= to_date && i.PLANT == plant_id)
                               select s;

                /*if (session_emp != "admin" && String.IsNullOrEmpty(searchString) && session_emp != "D83003")
                    students = students.Where(s => s.DEPT == dept_id && s.PLANT == plant_id);
                else if (session_emp != "admin" && !String.IsNullOrEmpty(searchString) && session_emp != "D83003") // triển khai cho Đồng Nai
                    students = students.Where(s => s.DEPT == dept_id && (s.DEPT.Contains(searchString)
                                       || s.Employee.EmployeeName.Contains(searchString)) && s.PLANT == plant_id);
                else if (!String.IsNullOrEmpty(searchString))
                {
                    students = students.Where(s => (s.DEPT.Contains(searchString)
                                           || s.Employee.EmployeeName.Contains(searchString)) && s.PLANT == plant_id);
                    //|| Convert.ToString(s.Date).Contains(searchString));
                }*/

                if (!String.IsNullOrEmpty(searchString))
                {
                    students = students.Where(s => (s.DEPT.Contains(searchString)
                                           || s.Employee.EmployeeName.Contains(searchString)) && s.PLANT == plant_id);
                    //|| Convert.ToString(s.Date).Contains(searchString));
                }
                return View(students);
            }
            else return RedirectToAction("LogOn", "LogOn");
        }

        private string subject { get; set; }
        private string body { get; set; }
        private string confirm_status { get; set; }
        Information inf = new Information();

        public ActionResult Create()
        {
            string plant_id = userManager.GetUserPlant(session_emp);
            if (session_emp == "")
                return RedirectToAction("LogOn", "LogOn");
            else
            {
                BIZ_TRIP biz_trip = new BIZ_TRIP();
                ViewBag.DEPT = GetDept_id(plant_id);
                string v_dept = GetDept_id(plant_id).ToString();
                ViewBag.DepartmentName = en.Departments.Where(o => o.Department_Id == v_dept && o.Plant_Id == plant_id).Select(f => f.Department_Name).SingleOrDefault();
                /*get employee name*/
                ViewBag.EMPNO = session_emp;
                ViewBag.PLANT = plant_id;
                ViewBag.NAME = en.Employees.Where(f => (f.Emp_CJ == session_emp.ToString() && f.Plant_Id == plant_id)).Select(f => f.EmployeeName).SingleOrDefault();
                ViewBag.Job = en.Employees.Where(f => (f.Emp_CJ == session_emp.ToString() && f.Plant_Id == plant_id)).Select(f => f.Job).SingleOrDefault();
                /*get employee name*/
                return View(biz_trip);
            }
        }

        [HttpPost]
        public ActionResult Create(BIZ_TRIP biz_trip, string p_hour_f, string p_minute_f, string p_hour_t, string p_minute_t)
        {
            string result;
            string linkConfirm = "";
            string curr_PlantID = CurrentUser.Instance.User.Plant_Id;
            string curr_DeptID = CurrentUser.Instance.User.Department_Id;

            string fromdate = biz_trip.FROM_DATE.Value.ToString("dd/MM/yyyy") + " " + p_hour_f + ":" + p_minute_f + ":00,531";
            DateTime v_fromdate = DateTime.ParseExact(fromdate, "dd/MM/yyyy HH:mm:ss,fff",
                                       System.Globalization.CultureInfo.InvariantCulture);

            string todate = biz_trip.TO_DATE.Value.ToString("dd/MM/yyyy") + " " + p_hour_t + ":" + p_minute_t + ":00,531";
            DateTime v_to_date = DateTime.ParseExact(todate, "dd/MM/yyyy HH:mm:ss,fff",
                                       System.Globalization.CultureInfo.InvariantCulture);
            biz_trip.DEPT = curr_DeptID;
            biz_trip.EMPNO = session_emp;
            biz_trip.FROM_DATE = v_fromdate;
            biz_trip.TO_DATE = v_to_date;
            biz_trip.PLANT = curr_PlantID;
            if (biz_trip.USED_EQUIPMENT == false)
                biz_trip.REMARK = "none";
            en.BIZ_TRIP.Add(biz_trip);
            try
            {
                en.SaveChanges();
                string department_name = DepartmentModel.Instance.getDeptName(biz_trip.PLANT, biz_trip.DEPT);
                string deptManagerEmail = DepartmentModel.Instance.getManagerEmail(biz_trip.PLANT, biz_trip.DEPT);
                string currentUserEmail = CurrentUser.Instance.User.Email;
                if (string.Equals(deptManagerEmail.Trim(), currentUserEmail.Trim()))
                {
                    linkConfirm = @"http://52.213.3.168/servicedesk/BIZ_TRIP/bod_confirm/";
                }
                else
                {
                    linkConfirm = @"http://52.213.3.168/servicedesk/BIZ_TRIP/dept_confirm/";
                }

                Employee userRequest = en.Employees.FirstOrDefault(e => e.Emp_CJ == biz_trip.EMPNO);
                bool resultMailing = Biz_TripHelper.Instance.sendBiz_TripEmail(biz_trip, 1, userRequest, linkConfirm);
                if (resultMailing)
                {
                    result = string.Format("Đã gởi email phiếu đăng ký đi công tác đến trưởng phòng bộ phận: " + department_name);
                }
                else
                {
                    result = string.Format("Không gửi được email. Vui lòng kiểm tra lại. Liên hệ hỗ trợ: minhsang.it@cjvina.com");
                }
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);
                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                result = string.Format("Có lỗi khi tạo đăng ký. Liên hệ hỗ trợ: minhsang.it@cjvina.com");
            }

            if (!string.IsNullOrEmpty(result))
            {
                ViewBag.ModalState = "show";
                ViewBag.Message = result;
            }
            else
            {
                ViewBag.ModalState = "hide";
                ViewBag.Message = "";
            }
            return View(biz_trip);
        }


        public ActionResult dept_confirm(Guid? id)
        {
            BIZ_TRIP biz_trip = en.BIZ_TRIP.Find(id);
            //ViewBag.Department_confirm_date = DateTime.Now;
            ViewBag.EMPNO = session_emp;
            ViewBag.DEPT = biz_trip.DEPT;
            ViewBag.PLANT = biz_trip.PLANT;
            ViewBag.DepartmentName = en.Departments.Where(o => o.Department_Id == biz_trip.DEPT
                                                         && o.Plant_Id == biz_trip.PLANT).Select(i => i.Department_Name).SingleOrDefault();
            /*get employee name*/
            ViewBag.User_name = en.Employees.FirstOrDefault(e => e.Emp_CJ == biz_trip.EMPNO).EmployeeName;
            /*get employee name*/


            ViewBag.ModalState = "hide";
            ViewBag.Message = "";
            return View(biz_trip);
        }

        // POST: /BIZ_TRIP/dept_confirm/GUID
        [HttpPost]
        public ActionResult dept_confirm(BIZ_TRIP biz_trip, HttpPostedFileBase image)
        {
            string result = "";
            var dept = from i in en.Departments where i.Department_Id == biz_trip.DEPT && i.Plant_Id == biz_trip.PLANT select i.Department_Name;

            //if (ModelState.IsValid)
            //{
            string plantid = userManager.GetUserPlant(session_emp);
            image = image ?? Request.Files["image"];
            if (image != null && image.ContentLength > 0)
            {
                //en.Entry(recruiment).State = System.Data.Entity.EntityState.Modified;
                if (biz_trip.DEPT_CONFIRM == true)
                {
                    byte[] fileData = new byte[Request.Files["image"].InputStream.Length];
                    Request.Files["image"].InputStream.Read(fileData, 0, Convert.ToInt32(Request.Files["image"].InputStream.Length));
                    System.Data.Linq.Binary binaryFile = new System.Data.Linq.Binary(fileData);
                    biz_trip.PLANT = plantid;
                    biz_trip.DEPT_CONFIRM_IMAGE = binaryFile.ToArray();
                    biz_trip.DEPT_CONFIRM = true;
                    var existingCart = en.BIZ_TRIP.Find(biz_trip.ID);

                    Guid v_ID_num = biz_trip.ID;
                    ///update salary report

                    en.Entry(existingCart).CurrentValues.SetValues(biz_trip);
                    en.SaveChanges();

                    Employee userRequest = en.Employees.FirstOrDefault(e => e.Emp_CJ == biz_trip.EMPNO);
                    bool resultMailing = Biz_TripHelper.Instance.sendBiz_TripEmail(biz_trip, 4, userRequest); // Level 4: HR Manager
                    if (resultMailing)
                    {
                        result = string.Format("Đã đồng ý. Email xác nhận đã được gửi đến trưởng phòng nhân sự");
                    }
                    else
                    {
                        result = string.Format("Không gửi được email. Vui lòng kiểm tra lại. Liên hệ hỗ trợ: minhsang.it@cjvina.com");
                    }
                }
                else
                {
                    Employee userRequest = en.Employees.FirstOrDefault(e => e.Emp_CJ == biz_trip.EMPNO);
                    bool resultMailing = Biz_TripHelper.Instance.sendBiz_TripEmail(biz_trip, 6, userRequest);
                    if (resultMailing)
                    {
                        result = string.Format("Không đồng ý. Đã gửi email phản hồi đến nhân viên");
                    }
                    else
                    {
                        result = string.Format("Không gửi được email. Vui lòng kiểm tra lại. Liên hệ hỗ trợ: minhsang.it@cjvina.com");
                    }
                }
            }
            if (!string.IsNullOrEmpty(result))
            {
                ViewBag.ModalState = "show";
                ViewBag.Message = result;
            }
            else
            {
                ViewBag.ModalState = "hide";
                ViewBag.Message = "";
            }
            return View(biz_trip);
        }

        public ActionResult hr_confirm(Guid? id)
        {
            BIZ_TRIP biz_trip = en.BIZ_TRIP.Find(id);
            //ViewBag.Department_confirm_date = DateTime.Now;
            ViewBag.EMPNO = session_emp;
            ViewBag.DEPT = biz_trip.DEPT;
            ViewBag.PLANT = biz_trip.PLANT;
            ViewBag.DepartmentName = en.Departments.Where(o => o.Department_Id == biz_trip.DEPT
                                                         && o.Plant_Id == biz_trip.PLANT).Select(i => i.Department_Name).SingleOrDefault();
            /*get employee name*/
            ViewBag.User_name = en.Employees.FirstOrDefault(e => e.Emp_CJ == biz_trip.EMPNO).EmployeeName;
            /*get employee name*/

            ViewBag.ModalState = "hide";
            ViewBag.Message = "";
            return View(biz_trip);
        }

        // POST: /BIZ_TRIP/dept_confirm/GUID
        [HttpPost]
        public ActionResult hr_confirm(BIZ_TRIP biz_trip, HttpPostedFileBase image)
        {
            string result = "";

            string plantid = userManager.GetUserPlant(session_emp);
            var dept = from i in en.Departments where i.Department_Id == biz_trip.DEPT && i.Plant_Id == biz_trip.PLANT select i.Department_Name;

            image = image ?? Request.Files["image"];
            if (image != null && image.ContentLength > 0)
            {
                //en.Entry(recruiment).State = System.Data.Entity.EntityState.Modified;
                if (biz_trip.HR_CONFIRM == true)
                {
                    byte[] fileData = new byte[Request.Files["image"].InputStream.Length];
                    Request.Files["image"].InputStream.Read(fileData, 0, Convert.ToInt32(Request.Files["image"].InputStream.Length));
                    System.Data.Linq.Binary binaryFile = new System.Data.Linq.Binary(fileData);
                    biz_trip.HR_CONFIRM_IMAGE = binaryFile.ToArray();
                    biz_trip.HR_CONFIRM = true;
                    biz_trip.PLANT = plantid;
                    var existingCart = en.BIZ_TRIP.Find(biz_trip.ID);
                    biz_trip.DEPT_CONFIRM = existingCart.DEPT_CONFIRM;
                    biz_trip.DEPT_CONFIRM_IMAGE = existingCart.DEPT_CONFIRM_IMAGE;

                    Guid v_ID_num = biz_trip.ID;
                    ///update salary reprot

                    en.Entry(existingCart).CurrentValues.SetValues(biz_trip);
                    en.SaveChanges();

                    Employee userRequest = en.Employees.FirstOrDefault(e => e.Emp_CJ == biz_trip.EMPNO);
                    bool resultMailing = Biz_TripHelper.Instance.sendBiz_TripEmail(biz_trip, 5, userRequest); // Level 5: HR Admin
                    if (resultMailing)
                    {
                        result = string.Format("Đã đồng ý. Email xác nhận đã được gửi đến trưởng phòng nhân sự");
                    }
                    else
                    {
                        result = string.Format("Không gửi được email. Vui lòng kiểm tra lại. Liên hệ hỗ trợ: minhsang.it@cjvina.com");
                    }
                }
                else
                {
                    Employee userRequest = en.Employees.FirstOrDefault(e => e.Emp_CJ == biz_trip.EMPNO);
                    bool resultMailing = Biz_TripHelper.Instance.sendBiz_TripEmail(biz_trip, 6, userRequest);
                    if (resultMailing)
                    {
                        result = string.Format("Không đồng ý. Đã gửi email phản hồi đến nhân viên");
                    }
                    else
                    {
                        result = string.Format("Không gửi được email. Vui lòng kiểm tra lại. Liên hệ hỗ trợ: minhsang.it@cjvina.com");
                    }
                }
            }
            if (!string.IsNullOrEmpty(result))
            {
                ViewBag.ModalState = "show";
                ViewBag.Message = result;
            }
            else
            {
                ViewBag.ModalState = "hide";
                ViewBag.Message = "";
            }
            return View(biz_trip);
        }

        public ActionResult delete_biztrip(Guid v_id)
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

            string plant_id = userManager.GetUserPlant(session_emp);
            string dept_id = Convert.ToString(GetDept_id(plant_id));
            string dept = GetDept_id(System.Web.HttpContext.Current.User.Identity.Name);

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
                                                && o.PLANT == plant_id
                                                && o.DATE >= from_date
                                                && o.DATE <= to_date
                                                && o.PLANT == plant_id).OrderBy(i => i.NO);
            return View("biz_trip_index", biztrip_list);
        }

        public ActionResult Resend(Guid? id)
        {
            var biz_trip = en.BIZ_TRIP.Where(i => i.ID == id).FirstOrDefault();
            string v_plant = userManager.GetUserPlant(session_emp);
            var dept = from i in en.Departments where i.Department_Id == biz_trip.DEPT && i.Plant_Id == v_plant select i.Department_Name;
            //~~~~~~~~~~~~~~~~~~~~~
            subject = "<<Gấp>> [Cần duyệt] - Phiếu yêu cầu sử dụng con dấu: " + biz_trip.NAME + " - tạo ngày: " + biz_trip.DATE;
            body = "     Employee Name: " + biz_trip.NAME + "\n" +
                  "        Department: " + dept.Single().ToString() + "\n" +
                  "              Date: " + biz_trip.DATE + "\n" +
                  "       Description: " + biz_trip.DESCRIPTION + "\n" +
                  "           Vehicle: " + biz_trip.VEHICLE + "\n" +
                  "  Conctact Company: " + biz_trip.CONTACT_COMPANY + "\n" +
                  "Conctact Departent: " + biz_trip.CONTACT_DEPT + "\n" +
                  "    Contact person: " + biz_trip.CONTACT_PERSON + "\n" +
                  "         From date: " + biz_trip.FROM_DATE + "       To date: " + biz_trip.TO_DATE + "\n" +
                  "    Used equipemnt: " + biz_trip.USED_EQUIPMENT.ToString() + "\n" +
                  " Equipemnt remarks: " + biz_trip.REMARK.ToString() + "\n" +
                  "-------------------------------------" + "\n" +
                  "   Follow link to confirm: " + "http://52.213.3.168/servicedesk/BIZ_TRIP/dept_confirm/" + biz_trip.ID + "\n" + "\n" +
                  "Regards!" + "\n" + "\n" + "\n" +

                  "Copy right by IT TEAM: contact Nguyen Thai Binh - IT Software for supporting";



            inf.email_send("user_email", "pass", biz_trip.DEPT, subject, body, "1", userManager.GetUserPlant(session_emp));
            //~~~~~~~~~~~~~~~~~~~~~

            ViewBag.Emp_CJ = biz_trip.NAME;
            ViewBag.DeptID = dept.Single().ToString();

            return View("biz_trip_index");
        }

        //[HttpPost, ActionName("BizTripPrint")]
        public ActionResult BizTripPrint(Guid v_id)
        {
            string reportid = "PDF";
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "Report_BizTrip.rdlc");
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
            catch (Exception ex)
            {

                return View();
            }
        }

    }
}
