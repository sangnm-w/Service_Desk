using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_IT_HELPDESK.Controllers.ObjectManager;

namespace Web_IT_HELPDESK.Controllers
{
    public class DrinkingRequestController : Controller
    {
        //
        // GET: /DrinkingRequest/
        ServiceDeskEntities en = new ServiceDeskEntities();
        private DateTime to_date { get; set; }
        private DateTime from_date { get; set; }
        private string session_emp = System.Web.HttpContext.Current.User.Identity.Name;
        public ActionResult Index()
        {
            string plant_id = userManager.GetUserPlant(session_emp);
            string dept_id = Convert.ToString(GetDept_id(plant_id));

            var DepartmentName = from i in en.Departments where i.Deactive != true select i.Department_Name;
            string deptname = DepartmentName.FirstOrDefault().ToString();
            SelectList deptlist = new SelectList(DepartmentName);
            ViewBag.DepartmentName = deptlist;


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

             if (session_emp != "admin" && session_emp != "D83003" && session_emp != "MK78072" && session_emp != "H88768" && session_emp != "HN91185" && session_emp != "HN92244")
                return View(en.Drinking_Request.Where(i => i.Del != true && i.DepartmentId == dept_id && i.Date >= from_date && i.Date <= to_date));
            else return View(en.Drinking_Request.Where(i => i.Del != true && i.Date >= from_date && i.Date <= to_date));
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(string searchString, string _datetime)
        {
            //ViewBag.DepartmentId = new SelectList(en.Departments, "DepartmentId", "DepartmentName", deparment.DepartmentId);

            //----------------------------------------------------------------------------------------
            /*var DepartmentName = from i in en.Departments where i.Del != true select i.DepartmentName;
            SelectList deptlist = new SelectList(DepartmentName);
            ViewBag.DepartmentName = deptlist;*/
            //----------------------------------------------------------------------------------------

            /*string dept_id = Convert.ToString(GetDept_id());
            var session_emp = Session["employee_id"];
            string a = Convert.ToString(Session["employee_id"]);
            if (a != "admin")
                return View(en.Seal_Using.Where(i => i.Del != true && i.DepartmentId == dept_id));
            else return View(en.Seal_Using.Where(i => i.Del != true));*/


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


            var students = from s in en.Drinking_Request.Where(i => i.Del != true && i.Date >= from_date && i.Date <= to_date)
                           select s;
            string plant_id = userManager.GetUserPlant(session_emp);
            string dept_id = Convert.ToString(GetDept_id(plant_id));

            if (session_emp != "admin" && String.IsNullOrEmpty(searchString))
                students = students.Where(s => s.DepartmentId == dept_id);
            else if (session_emp != "admin" && !String.IsNullOrEmpty(searchString))
                students = students.Where(s => s.DepartmentId == dept_id && (s.DepartmentId.Contains(searchString)
                                   || s.Employee_name.Contains(searchString)));
            else if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.DepartmentId.Contains(searchString)
                                       || s.Employee_name.Contains(searchString));
                //|| Convert.ToString(s.Date).Contains(searchString));
            }
            return View(students);
        }

        //  Get_department_Id
        private string GetDept_id(string v_plant_id)
        {
            //string dept_id = en.Employees.Where(f => (f.Emp_CJ == session_emp && f.Plant_Id == v_plant_id)).Select(f => f.Department_Id).SingleOrDefault();
            string dept_id = en.Employee_New.Where(f => (f.Emp_CJ == session_emp && f.Plant_ID == v_plant_id)).Select(f => f.Department_ID).SingleOrDefault();
            return dept_id;
        }
        //GetPlant_id
        private string GetPlant_id()
        {
            //string plant_id = en.Employees.Where(f => (f.Emp_CJ == session_emp)).Select(f => f.Plant_Id).SingleOrDefault();
            string plant_id = en.Employee_New.Where(f => (f.Emp_CJ == session_emp)).Select(f => f.Plant_ID).SingleOrDefault();
            return plant_id;
        }

        //
        // GET: /DrinkingRequest/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /DrinkingRequest/Create
        public UserManager userManager = new UserManager();
        public ActionResult Create()
        {
            string plant_id = userManager.GetUserPlant(session_emp);
            Drinking_Request driniking_request = new Drinking_Request();
            ViewBag.DepartmentId = GetDept_id(plant_id);
            return View(driniking_request);
        } 

        //
        // POST: /DrinkingRequest/Create

        [HttpPost]
        public string Create(Drinking_Request drinking_request)
        {
            string plant_id = userManager.GetUserPlant(session_emp);
            var dept = from i in en.Departments 
                       where i.Department_Id == drinking_request.DepartmentId
                          && i.Plant_Id == plant_id
                      select i.Department_Name;
            en.Drinking_Request.Add(drinking_request);
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
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }

            string result, status = "";
            //~~~~~~~~~~~~~~~~~~~~~
            /*if (search.Contains("HR") == true)
            {
                subject = "[Duyệt hỗ trợ] - Phiếu yêu cầu sử dụng con dấu: " + seal_using.Employee_name + " - tạo ngày: " + seal_using.Date;
                result = string.Format("Thông báo! <br /> <br />" +
                                                  "Đã gởi email xác nhận đến trưởng phòng nhân sự để duyệt sử dụng con dấu <br />" +
                                                  "************** Thank you!!! **************");
                status = "1.1";
            }
            else
            {*/
            subject = "[Duyệt] - Phiếu yêu cầu sử dụng: " + drinking_request.Employee_name + " - tạo ngày: " + drinking_request.Date;
            result = string.Format("Thông báo! <br /> <br />" +
                                              "Đã gởi email xác nhận sử dụng đến trưởng phòng bộ phận: " + dept.Single().ToString() + " <br />" +
                                              "************** Thank you!!! **************");
            status = "1";
            //}
            body = "Tên người yêu cầu: " + drinking_request.Employee_name + "\n" +
                    "          Bộ phận: " + dept.Single().ToString() + "\n" +
                    "     Ngày yêu cầu: " + drinking_request.Date + "\n" +
                    "         Số lượng: " + drinking_request.Quantity + "\n" +
                    "      Đơn vị tính: " + drinking_request.Unit + "\n" +
                    "          Ghi chú: " + drinking_request.Note + "\n" +
                    "   Theo đường dẫn: " + "http://52.213.3.168/servicedesk/DrinkingRequest/Edit/" + drinking_request.Id + "\n" + "\n" +
                    "Trân trọng!" + "\n" + "\n" + "\n" +

                    "Chương trình gởi mail được bởi IT TEAM: liên hệ Nguyen Thai Binh - IT Software khi cần hỗ trợ";



            inf.email_send("user_email", "pass", drinking_request.DepartmentId, subject, body, status, GetPlant_id());
            //~~~~~~~~~~~~~~~~~~~~~
            //return RedirectToAction("Index", "SealUsing");


            return result;
        }
        
        //
        // GET: /DrinkingRequest/Edit/5

        public ActionResult Edit(int? id)
        {
            Drinking_Request drinking_request = en.Drinking_Request.Find(id);
            ViewBag.Department_confirm_date = DateTime.Now;
            string plant_id = userManager.GetUserPlant(session_emp);
            ViewBag.DepartmentId = GetDept_id(plant_id);
            return View(drinking_request);
        }

        //
        // POST: /DrinkingRequest/Edit/5

        [HttpPost]
        public string Edit(Drinking_Request drinking_request)
        {

            var dept = from i in en.Departments where i.Department_Id == drinking_request.DepartmentId select i.Department_Name;
            string result;

            en.Entry(drinking_request).State = System.Data.Entity   .EntityState.Modified;
            if (drinking_request.Department_confirm == true)
            {
                en.SaveChanges();
                //~~~~~~~~~~~~~~~~~~~~~
                subject = "[Duyệt] - Phiếu yêu cầu sử dụng: " + drinking_request.Employee_name + " - tạo ngày: " + drinking_request.Date;
                body = "Tên người yêu cầu: " + drinking_request.Employee_name + "\n" +
                        "          Bộ phận: " + dept.FirstOrDefault().ToString() + "\n" +
                        "     Ngày yêu cầu: " + drinking_request.Date + "\n" +
                          "         Số lượng: " + drinking_request.Quantity + "\n" +
                    "      Đơn vị tính: " + drinking_request.Unit + "\n" +
                    "          Ghi chú: " + drinking_request.Note + "\n" +
                        "-------------------------------------" + "\n" +
                        "Đã được trưởng phòng duyệt" + "\n" +
                        "   Theo đường dẫn: " + "http://52.213.3.168/servicedesk/DrinkingRequest/Confirm/" + drinking_request.Id + "\n" +
                        "Trân trọng!" + "\n" + "\n" + "\n" +

                        "Chương trình gởi mail được bởi IT TEAM: liên hệ Nguyen Thai Binh - IT Software khi cần hỗ trợ";

                //~~~~~~~~~~~~~~~~~~~~~

                confirm_status = "Đồng ý xác nhận";
                inf.email_send("user_email", "pass", drinking_request.DepartmentId, subject, body, "2", GetPlant_id());

                result = string.Format("Thông báo! <br /> Trạng thái được duyệt đã chọn: " + confirm_status + " <br />" +
                                                  "Bộ phận: " + dept.FirstOrDefault().ToString()) + " <br />" +
                                                  "Email xác nhận đã được gởi sang bộ phận nhân sự phụ trách.";
            }
            else
            {
                subject = "[Kiểm tra chưa duyệt] - Phiếu yêu cầu sử dụng: " + drinking_request.Employee_name + " - tạo ngày: " + drinking_request.Date;
                body = "Tên người yêu cầu: " + drinking_request.Employee_name + "\n" +
                   "          Bộ phận: " + dept.FirstOrDefault().ToString() + "\n" +
                   "     Ngày yêu cầu: " + drinking_request.Date + "\n" +
                   "         Số lượng: " + drinking_request.Quantity + "\n" +
                   "      Đơn vị tính: " + drinking_request.Unit + "\n" +
                   "          Ghi chú: " + drinking_request.Note + "\n" +
                   "   Theo đường dẫn: " + "http://52.213.3.168/servicedesk/DrinkingRequest/Edit/" + drinking_request.Id + "\n" + "\n" +
                   "Trân trọng!" + "\n" + "\n" + "\n" +

                   "Chương trình gởi mail được bởi IT TEAM: liên hệ Nguyen Thai Binh - IT Software khi cần hỗ trợ";
                inf.email_send("user_email", "pass", drinking_request.DepartmentId, subject, body, "1",GetPlant_id());
                confirm_status = "KHÔNG ĐỒNG Ý XÁC NHẬN";
                result = string.Format("Thông báo! <br /> Trạng thái được duyệt đã chọn: " + confirm_status + " <br />" +
                                                  "Bộ phận: " + dept.FirstOrDefault().ToString() + " <br />" +
                                                  "Email xác nhận không được gởi sang bộ phận nhân sự phụ trách.");
            }


            return result; ; //RedirectToAction("Index", "SealUsing");
        }

        public ActionResult Confirm(int? id)
        {
            Drinking_Request drinking_request = en.Drinking_Request.Find(id);
            ViewBag.HR_confirm_date = DateTime.Now;
            string plant_id = userManager.GetUserPlant(session_emp);
            ViewBag.DepartmentId = GetDept_id(plant_id);
            return View(drinking_request);
        }

        //
        // POST: /SealUsing/Edit/5

        [HttpPost]
        public ActionResult Confirm(Drinking_Request drinking_request)
        {
            var dept = from i in en.Departments where i.Department_Id == drinking_request.DepartmentId select i.Department_Name;
            drinking_request.HR_confirm_date = DateTime.Now;
            en.Entry(drinking_request).State = System.Data.Entity.EntityState.Modified;
            en.SaveChanges();
            //~~~~~~~~~~~~~~~~~~~~~
            //~~~~~~~~~~~~~~~~~~~~~
            return RedirectToAction("Index", "DrinkingRequest");
        }

        //
        // GET: /DrinkingRequest/Delete/5
 
        public ActionResult Delete(int? id)
        {
            Drinking_Request drinking_request = en.Drinking_Request.Find(id);
            ViewBag.Department_confirm_date = DateTime.Now;
            string plant_id = userManager.GetUserPlant(session_emp);
            ViewBag.DepartmentId = GetDept_id(plant_id);
            return View(drinking_request);
        }

        //
        // POST: /DrinkingRequest/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int? id)
        {
            try
            {
                Drinking_Request drinking_request = en.Drinking_Request.Find(id);
                if (drinking_request == null)
                    return HttpNotFound();
                else if (drinking_request.Department_confirm == false)
                {
                    drinking_request.Del = true;
                    en.SaveChanges();
                }

            }
            catch
            {
            }
            return RedirectToAction("Index", "DrinkingRequest");
        }


        private string subject { get; set; }
        private string body { get; set; }
        private string confirm_status { get; set; }

        // call class information to send mail
        Information inf = new Information();

        // resend email to manager
        /*[HttpPost, ActionName("Resend")]
        public String Resend(IEnumerable<string> drinking_requests)
        {
            int seal_id = 0;
            foreach (var drinking_request_ in drinking_requests)
            {
                seal_id = Convert.ToInt32(Session[drinking_request_]);
            }
            var drinking_request = en.Drinking_Request.Where(i => i.Id == seal_id).FirstOrDefault();
            var dept = from i in en.Departments where i.DepartmentId == drinking_request.DepartmentId select i.DepartmentName;
            //~~~~~~~~~~~~~~~~~~~~~
            subject = "<<Gấp>> [Cần duyệt] - Phiếu yêu cầu dùng thức uống: " + drinking_request.Employee_name + " - tạo ngày: " + drinking_request.Date;
            body = "Tên người yêu cầu: " + drinking_request.Employee_name + "\n" +
                    "          Bộ phận: " + dept.Single().ToString() + "\n" +
                    "     Ngày yêu cầu: " + drinking_request.Date + "\n" +
                    "         Số lượng: " + drinking_request.Quantity + "\n" +
                    "      Đơn vị tính: " + drinking_request.Unit + "\n" +
                    "          Ghi chú: " + drinking_request.Note + "\n" +
                    "   Theo đường dẫn: " + "http://52.213.3.168/servicedesk/DrinkingRequest/Edit/" + drinking_request.Id + "\n" + "\n" +
                    "Trân trọng!" + "\n" + "\n" + "\n" +
                    "Chương trình gởi mail được bởi IT TEAM: liên hệ Nguyen Thai Binh - IT Software khi cần hỗ trợ";



            inf.email_send("user_email", "pass", drinking_request.DepartmentId, subject, body, "1", GetPlant_id());
            //~~~~~~~~~~~~~~~~~~~~~

            string result = string.Format("Thông báo! <br /> Gởi tin nhắn thành công <br /> Tên người yêu cầu: " + drinking_request.Employee_name + " <br />" +
                                          "Bộ phận: " + dept.Single().ToString());

            return result; //RedirectToAction("Index", "SealUsing");
        }*/

        public ActionResult Resend(int? id)
        {
            var drinking_request = en.Drinking_Request.Where(i => i.Id == id).FirstOrDefault();
            string plantid = userManager.GetUserPlant(session_emp);
            var dept = from i in en.Departments where i.Department_Id == drinking_request.DepartmentId && i.Plant_Id == plantid select i.Department_Name;
            //~~~~~~~~~~~~~~~~~~~~~
            subject = "<<Gấp>> [Cần duyệt] - Phiếu yêu cầu sử dụng: " + drinking_request.Employee_name + " - tạo ngày: " + drinking_request.Date;
            body = "Tên người yêu cầu: " + drinking_request.Employee_name + "\n" +
                    "          Bộ phận: " + dept.Single().ToString() + "\n" +
                    "     Ngày yêu cầu: " + drinking_request.Date + "\n" +
                    "         Số lượng: " + drinking_request.Quantity + "\n" +
                    "      Đơn vị tính: " + drinking_request.Unit + "\n" +
                    "          Ghi chú: " + drinking_request.Note + "\n" +
                    "   Theo đường dẫn: " + "http://52.213.3.168/servicedesk/DrinkingRequest/Edit/" + drinking_request.Id + "\n" + "\n" +
                    "Trân trọng!" + "\n" + "\n" + "\n" +
                    "Chương trình gởi mail được bởi IT TEAM: liên hệ Nguyen Thai Binh - IT Software khi cần hỗ trợ";



            inf.email_send("user_email", "pass", drinking_request.DepartmentId, subject, body, "1", userManager.GetUserPlant(session_emp));
            //~~~~~~~~~~~~~~~~~~~~~

            ViewBag.Emp_CJ = drinking_request.Employee_name;
            ViewBag.DeptID = dept.Single().ToString();

            return View();
        }

    }
}
