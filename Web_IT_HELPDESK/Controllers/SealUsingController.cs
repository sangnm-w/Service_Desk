using System;
using System.Data;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Web_IT_HELPDESK.Controllers.ObjectManager;
using Web_IT_HELPDESK.Models;

namespace Web_IT_HELPDESK.Controllers
{
    public class SealUsingController : Controller
    {
        //
        // GET: /SealUsing/
        ServiceDeskEntities en = new ServiceDeskEntities();
        private string session_emp = System.Web.HttpContext.Current.User.Identity.Name;

        public ActionResult Index()
        {
            var DepartmentName = from i in en.Departments where i.Deactive != true select i.Department_Name;
            SelectList deptlist = new SelectList(DepartmentName);
            ViewBag.DepartmentName = deptlist;
            string plant_id = CurrentUser.Instance.User.Plant_Id;

            string dept_id = Convert.ToString(GetDept_id(plant_id));
            string dept_name = en.Departments.Where(o => o.Department_Id == dept_id && o.Plant_Id == plant_id).Select(f => f.Department_Name).SingleOrDefault();
            ViewBag.DepartmentNameview = dept_name;

            IFormatProvider culture = new CultureInfo("en-US", true);
            string _datetime = DateTime.Now.ToString("MM/yyyy");
            from_date = DateTime.ParseExact("01/" + _datetime, "dd/MM/yyyy", culture);
            to_date = from_date.AddMonths(1).AddSeconds(-1);

            if (session_emp != "")
            {
                if (session_emp != "admin" && session_emp != "D83003" && session_emp != "MK78072" && session_emp != "H88768" && session_emp != "HN91185" && session_emp != "HN92244")
                {
                    var students = en.Seal_Using.Where(i => i.Del != true
                                                    && i.DepartmentId == dept_id
                                                    && i.Plant == plant_id
                                                    && i.Date >= from_date
                                                    && i.Date <= to_date
                                                    && i.Plant == plant_id
                                                    ).OrderByDescending(o => o.Id);
                    return View(students);//.ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    var students2 = en.Seal_Using.Where(i => i.Del != true
                                                          && i.Date >= from_date
                                                          && i.Date <= to_date
                                                          && i.Plant == plant_id
                                                          ).OrderByDescending(o => o.Id);
                    return View(students2);//.ToPagedList(pageNumber, pageSize));
                }
            }
            else return RedirectToAction("LogOn", "LogOn");

        }


        private DateTime to_date { get; set; }
        private DateTime from_date { get; set; }
        [Authorize]
        [HttpPost]
        public ActionResult Index(string searchString, string _datetime, int? page)
        {
            //http://www.asp.net/mvc/overview/getting-started/getting-started-with-ef-using-mvc/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application
            IFormatProvider culture = new CultureInfo("en-US", true);
            from_date = DateTime.ParseExact("01/" + _datetime, "dd/MM/yyyy", culture);
            to_date = from_date.AddMonths(1).AddSeconds(-1);

            string plant_id = CurrentUser.Instance.User.Plant_Id;
            string dept_id = Convert.ToString(GetDept_id(plant_id));
            ViewBag.DepartmentNameview = en.Departments.Where(o => o.Department_Id == dept_id && o.Plant_Id == plant_id).Select(f => f.Department_Name).SingleOrDefault();

            if (session_emp != "")
            {
                var students = from s in en.Seal_Using.Where(i => i.Del != true && i.Date >= from_date && i.Date <= to_date && i.Plant == plant_id).OrderByDescending(o => o.Id)
                               select s;

                if (session_emp != "admin" && String.IsNullOrEmpty(searchString) && session_emp != "D83003" && session_emp != "V78157"
                                   || session_emp != "MK78072" || session_emp != "H88768" || session_emp != "HN91185" || session_emp != "HN92244")
                    students = students.Where(s => s.DepartmentId == dept_id && s.Plant == plant_id).OrderByDescending(i => i.Id);
                else if (session_emp != "admin" && !String.IsNullOrEmpty(searchString) && session_emp != "D83003" && session_emp != "V78157" // triển khai cho Đồng Nai
                                               || session_emp != "MK78072" || session_emp != "H88768" || session_emp != "HN91185" || session_emp != "HN92244") // triem khai tat ca chi nhanh
                    students = students.Where(s => s.DepartmentId == dept_id && (s.DepartmentId.Contains(searchString)
                                       || s.Employee_name.Contains(searchString)) && s.Plant == plant_id).OrderByDescending(i => i.Id);
                else if (!String.IsNullOrEmpty(searchString))
                {
                    students = students.Where(s => (s.DepartmentId.Contains(searchString)
                                           || s.Employee_name.Contains(searchString)) && s.Plant == plant_id).OrderByDescending(i => i.Id);
                    //|| Convert.ToString(s.Date).Contains(searchString));
                }
                return View(students);
            }
            else return RedirectToAction("LogOn", "LogOn");
        }




        private string GetDept_id(string v_plant_id)
        {
            string dept_id = en.Employees.Where(f => (f.Emp_CJ == session_emp && f.Plant_Id == v_plant_id)).Select(f => f.Department_Id).SingleOrDefault();
            return dept_id;
        }

        //
        // GET: /SealUsing/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /SealUsing/Create

        public ActionResult Create()
        {
            string plant_id = CurrentUser.Instance.User.Plant_Id;
            if (session_emp == "")
                return RedirectToAction("LogOn", "LogOn");
            else
            {
                Seal_Using seal_using = new Seal_Using();
                ViewBag.DepartmentId = GetDept_id(plant_id);
                string v_dept = GetDept_id(plant_id).ToString();
                ViewBag.DepartmentName = en.Departments.Where(o => o.Department_Id == v_dept && o.Plant_Id == plant_id).Select(f => f.Department_Name).SingleOrDefault();
                /*get employee name*/

                ViewBag.Plant = plant_id;
                ViewBag.User_name = en.Employees.Where(f => (f.Emp_CJ == session_emp.ToString() && f.Plant_Id == plant_id)).Select(f => f.EmployeeName).SingleOrDefault();
                /*get employee name*/
                return View(seal_using);
            }
        }

        //
        // POST: /SealUsing/Create

        private string subject { get; set; }
        private string body { get; set; }
        private string confirm_status { get; set; }

        [HttpPost]
        public string Create(Seal_Using seal_using)
        {
            string result = "";
            string plantid = CurrentUser.Instance.User.Plant_Id;
            var dept = from i in en.Departments where i.Department_Id == seal_using.DepartmentId && i.Plant_Id == plantid select i.Department_Name;
            seal_using.Employee_ID = CurrentUser.Instance.User.Emp_CJ;
            en.Seal_Using.Add(seal_using);

            try
            {
                en.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);
                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                result = string.Format("Thông báo! <br /> <br />" +
                                              "Có lỗi khi tạo yêu cầu. Liên hệ hỗ trợ: minhsang.it@cjvina.com <br />" +
                                              "************** Cám ơn đã sử dụng chương trình **************");
                return result;
            }

            Employee userRequest = en.Employees.FirstOrDefault(e => e.Emp_CJ == seal_using.Employee_ID);
            bool resultMailing = SealUsingHelper.Instance.sendSealUsingEmail(seal_using, 1, userRequest);

            if (resultMailing)
            {
                result = string.Format("Thông báo! <br /> <br />" +
                                              "Đã gởi email xác nhận đến trưởng phòng nhân sự để duyệt sử dụng con dấu <br />" +
                                              "************** Cám ơn đã sử dụng chương trình **************");
            }
            else
            {
                result = string.Format("Thông báo! <br /> <br />" +
                                              "Có lỗi khi gửi mail. Liên hệ hỗ trợ: minhsang.it@cjvina.com <br />" +
                                              "************** Cám ơn đã sử dụng chương trình **************");
            }

            return result;
        }

        public string Resend(int? id)
        {
            var seal_using = en.Seal_Using.Where(i => i.Id == id).FirstOrDefault();
            string result = "";

            Employee userRequest = en.Employees.FirstOrDefault(e => e.Emp_CJ == seal_using.Employee_ID);
            bool resultMailing = SealUsingHelper.Instance.sendSealUsingEmail(seal_using, 2, userRequest);
            if (resultMailing)
            {
                result = string.Format("Thông báo! <br /> <br />" +
                                              "Đã gởi lại email xác nhận đến trưởng phòng nhân sự để duyệt sử dụng con dấu <br />" +
                                              "************** Cám ơn đã sử dụng chương trình **************");
            }
            else
            {
                result = string.Format("Thông báo! <br /> <br />" +
                                              "Có lỗi khi gửi mail. Liên hệ hỗ trợ: minhsang.it@cjvina.com <br />" +
                                              "************** Cám ơn đã sử dụng chương trình **************");
            }
            return result;
        }

        public ActionResult Edit(int? id)
        {
            Seal_Using seal_using = en.Seal_Using.Find(id);
            ViewBag.Department_confirm_date = DateTime.Now;
            ViewBag.DepartmentId = seal_using.DepartmentId;
            ViewBag.Plant = seal_using.Plant;
            ViewBag.DepartmentName = en.Departments.Where(o => o.Department_Id == seal_using.DepartmentId
                                                         && o.Plant_Id == seal_using.Plant).Select(i => i.Department_Name).SingleOrDefault();
            /*get employee name*/
            ViewBag.User_name = en.Employees.Where(f => (f.Emp_CJ == session_emp.ToString() && f.Plant_Id == seal_using.DepartmentId.ToString())).Select(f => f.EmployeeName).SingleOrDefault();
            /*get employee name*/
            return View(seal_using);
        }

        //
        // POST: /SealUsing/Edit/5

        [HttpPost]
        public string Edit(Seal_Using seal_using)
        {
            string result;
            string departmentName = DepartmentModel.Instance.getDeptName(seal_using.Plant, seal_using.DepartmentId);

            Employee userRequest = en.Employees.FirstOrDefault(e => e.Emp_CJ == seal_using.Employee_ID);
            if (seal_using.Department_confirm == true)
            {
                en.Entry(seal_using).State = System.Data.Entity.EntityState.Modified;
                en.SaveChanges();

                bool resultMailing = SealUsingHelper.Instance.sendSealUsingEmail(seal_using, 5, userRequest);
                if (resultMailing)
                {
                    result = string.Format("Thông báo! <br /> Trạng thái được duyệt đã chọn: Đồng ý xác nhận <br />" +
                                                      "Bộ phận: " + departmentName + " <br />" +
                                                      "Email xác nhận đã được gởi sang bộ phận quản lý con dấu.");
                }
                else
                {
                    result = string.Format("Thông báo! <br /> <br />" +
                                                  "Có lỗi khi gửi mail. Liên hệ hỗ trợ: minhsang.it@cjvina.com <br />" +
                                                  "************** Cám ơn đã sử dụng chương trình **************");
                }
            }
            else
            {
                bool resultMailing = SealUsingHelper.Instance.sendSealUsingEmail(seal_using, 6, userRequest);
                if (resultMailing)
                {
                    result = string.Format("Thông báo! <br /> Trạng thái được duyệt đã chọn: KHÔNG ĐỒNG Ý XÁC NHẬN <br />" +
                                                  "Bộ phận: " + departmentName + " <br />" +
                                                  "Email xác nhận không được gởi sang bộ phận quản lý con dấu.");
                }
                else
                {
                    result = string.Format("Thông báo! <br /> <br />" +
                                                  "Có lỗi khi gửi mail. Liên hệ hỗ trợ: minhsang.it@cjvina.com <br />" +
                                                  "************** Cám ơn đã sử dụng chương trình **************");
                }
            }
            return result;
        }

        public ActionResult Confirm(int? id)
        {
            Seal_Using seal_using = en.Seal_Using.Find(id);
            ViewBag.Employee_Seal_keep = DateTime.Now;
            ViewBag.DepartmentId = ViewBag.DepartmentId = seal_using.DepartmentId;
            ViewBag.Plant = seal_using.Plant;
            ViewBag.DepartmentName = en.Departments.Where(o => o.Department_Id == seal_using.DepartmentId
                                                         && o.Plant_Id == seal_using.Plant).Select(i => i.Department_Name).SingleOrDefault();
            /*get employee name*/
            ViewBag.User_name = en.Employees.Where(f => (f.Emp_CJ == session_emp.ToString() && f.Plant_Id == seal_using.DepartmentId.ToString())).Select(f => f.EmployeeName).SingleOrDefault();
            /*get employee name*/
            return View(seal_using);
        }

        [HttpPost]
        public ActionResult Confirm(Seal_Using seal_using)
        {
            seal_using.Employee_Seal_keep = DateTime.Now;
            en.Entry(seal_using).State = System.Data.Entity.EntityState.Modified;
            en.SaveChanges();
            return RedirectToAction("Index", "SealUsing");
        }

        //
        // GET: /SealUsing/Delete/5

        public ActionResult Delete(int? id)
        {
            Seal_Using seal_using = en.Seal_Using.Find(id);
            ViewBag.Department_confirm_date = DateTime.Now;
            string plant_id = CurrentUser.Instance.User.Plant_Id;
            ViewBag.DepartmentId = GetDept_id(plant_id);
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

        // call class information to send mail
        Information inf = new Information();
    }
}