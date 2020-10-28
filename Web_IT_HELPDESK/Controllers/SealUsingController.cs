using System;
using System.Data;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Web_IT_HELPDESK.Controllers.ObjectManager;

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
            string dept_id = en.Employees.Where(f => (f.EmployeeID == session_emp && f.Plant_Id == v_plant_id)).Select(f => f.Department_Id).SingleOrDefault();
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
            string plant_id = userManager.GetUserPlant(session_emp);
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
                ViewBag.User_name = en.Employees.Where(f => (f.EmployeeID == session_emp.ToString() && f.Plant_Id == plant_id)).Select(f => f.EmployeeName).SingleOrDefault();
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
            string plantid = userManager.GetUserPlant(session_emp);
            var dept = from i in en.Departments where i.Department_Id == seal_using.DepartmentId && i.Plant_Id == plantid select i.Department_Name;
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
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }

            string result, status = "";
            //~~~~~~~~~~~~~~~~~~~~~
            /*if (search.Contains("HR") == true)
            {
                subject = "[Duyệt hỗ trợ] - Phiếu yêu cầu sử dụng con dấu: " + seal_using.Employee_name + " - tạo ngày: " + seal_using.Date;
                result = string.Format("Thông báo! <br /> <br />" +
                                                  "Đã gởi email xác nhận đến trưởng phòng nhân sự để duyệt sử dụng con dấu <br />" +
                                                  "************** Cám ơn đã sử dụng chương trình **************");
                status = "1.1";
            }
            else
            {*/
            subject = "[Duyệt] - Phiếu yêu cầu sử dụng con dấu: " + seal_using.Employee_name + " - tạo ngày: " + seal_using.Date;
            result = string.Format("Thông báo! <br /> <br />" +
                                              "Đã gởi email xác nhận sử dụng con dấu đến trưởng phòng bộ phận: " + dept.Single().ToString() + " <br />" +
                                              "************** Cám ơn đã sử dụng chương trình **************");
            status = "1";
            //}
            body = "Tên người yêu cầu: " + seal_using.Employee_name + "\n" +
                    "          Bộ phận: " + dept.Single().ToString() + "\n" +
                    "     Ngày yêu cầu: " + seal_using.Date + "\n" +
                    "  Ngày ký văn bản: " + seal_using.Date_signature + "\n" +
                    "     Loại văn bản: " + seal_using.Type_document + "\n" +
                    " Nội dung văn bản: " + seal_using.Context + "\n" +
                    "         Nơi nhận: " + seal_using.Place_Recipient + "\n" +
                    " Người ký văn bản: " + seal_using.Name_signature + "\n" +
                    "   Theo đường dẫn: " + "http://52.213.3.168/servicedesk/SealUsing/Edit/" + seal_using.Id + "\n" + "\n" +
                    "Trân trọng!" + "\n" + "\n" + "\n" +

                    "Chương trình gởi mail được bởi IT TEAM: liên hệ Nguyen Thai Binh - IT Software khi cần hỗ trợ";



            inf.email_send("user_email", "pass", seal_using.DepartmentId, subject, body, status, userManager.GetUserPlant(session_emp));
            //~~~~~~~~~~~~~~~~~~~~~
            //return RedirectToAction("Index", "SealUsing");


            return result;
        }



        // resend email to manager
        /*[HttpPost, ActionName("Resend")]
        public String Resend(IEnumerable<string> seal_usings)
        {
            int seal_id = 0;
            foreach (var seal_using_ in seal_usings)
            {
                seal_id = Convert.ToInt32(Session[seal_using_]);
            }
            var seal_using = en.Seal_Using.Where(i => i.Id == seal_id).FirstOrDefault();
            var dept = from i in en.Departments where i.DepartmentId == seal_using.DepartmentId select i.DepartmentName;
            //~~~~~~~~~~~~~~~~~~~~~
            subject = "<<Gấp>> [Cần duyệt] - Phiếu yêu cầu sử dụng con dấu: " + seal_using.Employee_name + " - tạo ngày: " + seal_using.Date;
            body = "Tên người yêu cầu: " + seal_using.Employee_name + "\n" +
                    "          Bộ phận: " + dept.Single().ToString() + "\n" +
                    "     Ngày yêu cầu: " + seal_using.Date + "\n" +
                    "  Ngày ký văn bản: " + seal_using.Date_signature + "\n" +
                    "     Loại văn bản: " + seal_using.Type_document + "\n" +
                    " Nội dung văn bản: " + seal_using.Context + "\n" +
                    "         Nơi nhận: " + seal_using.Place_Recipient + "\n" +
                    " Người ký văn bản: " + seal_using.Name_signature + "\n" +
                    "   Theo đường dẫn: " + "http://52.213.3.168/servicedesk/SealUsing/Edit/" + seal_using.Id + "\n" + "\n" +
                    "Trân trọng!" + "\n" + "\n" + "\n" +
                    "Chương trình gởi mail được bởi IT TEAM: liên hệ Nguyen Thai Binh - IT Software khi cần hỗ trợ";



            inf.email_send("user_email", "pass", seal_using.DepartmentId, subject, body, "1");
            //~~~~~~~~~~~~~~~~~~~~~

            string result = string.Format("Thông báo! <br /> Gởi tin nhắn thành công <br /> Tên người yêu cầu: " + seal_using.Employee_name + " <br />" +
                                          "Bộ phận: " + dept.Single().ToString());

            return result; //RedirectToAction("Index", "SealUsing");
        }*/

        public ActionResult Resend(int? id)
        {
            var seal_using = en.Seal_Using.Where(i => i.Id == id).FirstOrDefault();
            string plantid = userManager.GetUserPlant(session_emp);
            var dept = from i in en.Departments where i.Department_Id == seal_using.DepartmentId && i.Plant_Id == plantid select i.Department_Name;
            //~~~~~~~~~~~~~~~~~~~~~
            subject = "<<Gấp>> [Cần duyệt] - Phiếu yêu cầu sử dụng con dấu: " + seal_using.Employee_name + " - tạo ngày: " + seal_using.Date;
            body = "Tên người yêu cầu: " + seal_using.Employee_name + "\n" +
                    "          Bộ phận: " + dept.Single().ToString() + "\n" +
                    "     Ngày yêu cầu: " + seal_using.Date + "\n" +
                    "  Ngày ký văn bản: " + seal_using.Date_signature + "\n" +
                    "     Loại văn bản: " + seal_using.Type_document + "\n" +
                    " Nội dung văn bản: " + seal_using.Context + "\n" +
                    "         Nơi nhận: " + seal_using.Place_Recipient + "\n" +
                    " Người ký văn bản: " + seal_using.Name_signature + "\n" +
                    "   Theo đường dẫn: " + "http://52.213.3.168/servicedesk/SealUsing/Edit/" + seal_using.Id + "\n" + "\n" +
                    "Trân trọng!" + "\n" + "\n" + "\n" +
                    "Chương trình gởi mail được bởi IT TEAM: liên hệ Nguyen Thai Binh - IT Software khi cần hỗ trợ";



            inf.email_send("user_email", "pass", seal_using.DepartmentId, subject, body, "1", userManager.GetUserPlant(session_emp));
            //~~~~~~~~~~~~~~~~~~~~~

            ViewBag.EmployeeID = seal_using.Employee_name;
            ViewBag.DeptID = dept.Single().ToString();

            return View();
        }



        //
        // GET: /SealUsing/Edit/5
        public UserManager userManager = new UserManager();

        public ActionResult Edit(int? id)
        {
            Seal_Using seal_using = en.Seal_Using.Find(id);
            ViewBag.Department_confirm_date = DateTime.Now;
            ViewBag.DepartmentId = seal_using.DepartmentId;
            ViewBag.Plant = seal_using.Plant;
            ViewBag.DepartmentName = en.Departments.Where(o => o.Department_Id == seal_using.DepartmentId
                                                         && o.Plant_Id == seal_using.Plant).Select(i => i.Department_Name).SingleOrDefault();
            /*get employee name*/
            ViewBag.User_name = en.Employees.Where(f => (f.EmployeeID == session_emp.ToString() && f.Plant_Id == seal_using.DepartmentId.ToString())).Select(f => f.EmployeeName).SingleOrDefault();
            /*get employee name*/
            return View(seal_using);
        }

        //
        // POST: /SealUsing/Edit/5

        [HttpPost]
        public string Edit(Seal_Using seal_using)
        {
            //string plantid = userManager.GetUserPlant(session_emp);
            var dept = from i in en.Departments where i.Department_Id == seal_using.DepartmentId && i.Plant_Id == seal_using.Plant select i.Department_Name;
            string result;

            en.Entry(seal_using).State = System.Data.Entity.EntityState.Modified;
            if (seal_using.Department_confirm == true)
            {
                en.SaveChanges();
                //~~~~~~~~~~~~~~~~~~~~~
                subject = "[Duyệt] - Phiếu yêu cầu sử dụng con dấu: " + seal_using.Employee_name + " - tạo ngày: " + seal_using.Date;
                body = "Tên người yêu cầu: " + seal_using.Employee_name + "\n" +
                        "          Bộ phận: " + dept.Single().ToString() + "\n" +
                        "     Ngày yêu cầu: " + seal_using.Date + "\n" +
                        "  Ngày ký văn bản: " + seal_using.Date_signature + "\n" +
                        "     Loại văn bản: " + seal_using.Type_document + "\n" +
                        " Nội dung văn bản: " + seal_using.Context + "\n" +
                        "         Nơi nhận: " + seal_using.Place_Recipient + "\n" +
                        " Người ký văn bản: " + seal_using.Name_signature + "\n" +
                        "-------------------------------------" + "\n" +
                        "Đã được trưởng phòng duyệt" + "\n" +
                        "   Theo đường dẫn: " + "http://52.213.3.168/servicedesk/SealUsing/Confirm/" + seal_using.Id + "\n" +
                        "Trân trọng!" + "\n" + "\n" + "\n" +

                        "Chương trình gởi mail được bởi IT TEAM: liên hệ Nguyen Thai Binh - IT Software khi cần hỗ trợ";

                //~~~~~~~~~~~~~~~~~~~~~

                confirm_status = "Đồng ý xác nhận";
                inf.email_send("user_email", "pass", seal_using.DepartmentId, subject, body, "2", seal_using.Plant);

                result = string.Format("Thông báo! <br /> Trạng thái được duyệt đã chọn: " + confirm_status + " <br />" +
                                                  "Bộ phận: " + dept.Single().ToString()) + " <br />" +
                                                  "Email xác nhận đã được gởi sang bộ phận quản lý con dấu.";
            }
            else
            {
                subject = "[Kiểm tra chưa duyệt] - Phiếu yêu cầu sử dụng con dấu: " + seal_using.Employee_name + " - tạo ngày: " + seal_using.Date;
                body = "Tên người yêu cầu: " + seal_using.Employee_name + "\n" +
                   "          Bộ phận: " + dept.Single().ToString() + "\n" +
                   "     Ngày yêu cầu: " + seal_using.Date + "\n" +
                   "  Ngày ký văn bản: " + seal_using.Date_signature + "\n" +
                   "     Loại văn bản: " + seal_using.Type_document + "\n" +
                   " Nội dung văn bản: " + seal_using.Context + "\n" +
                   "         Nơi nhận: " + seal_using.Place_Recipient + "\n" +
                   " Người ký văn bản: " + seal_using.Name_signature + "\n" +
                   "   Theo đường dẫn: " + "http://52.213.3.168/servicedesk/SealUsing/Edit/" + seal_using.Id + "\n" + "\n" +
                   "Trân trọng!" + "\n" + "\n" + "\n" +

                   "Chương trình gởi mail được bởi IT TEAM: liên hệ Nguyen Thai Binh - IT Software khi cần hỗ trợ";
                inf.email_send("user_email", "pass", seal_using.DepartmentId, subject, body, "1", userManager.GetUserPlant(session_emp));
                confirm_status = "KHÔNG ĐỒNG Ý XÁC NHẬN";
                result = string.Format("Thông báo! <br /> Trạng thái được duyệt đã chọn: " + confirm_status + " <br />" +
                                                  "Bộ phận: " + dept.Single().ToString() + " <br />" +
                                                  "Email xác nhận không được gởi sang bộ phận quản lý con dấu.");
            }


            return result; ; //RedirectToAction("Index", "SealUsing");
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
            ViewBag.User_name = en.Employees.Where(f => (f.EmployeeID == session_emp.ToString() && f.Plant_Id == seal_using.DepartmentId.ToString())).Select(f => f.EmployeeName).SingleOrDefault();
            /*get employee name*/
            return View(seal_using);
        }

        //
        // POST: /SealUsing/Edit/5

        [HttpPost]
        public ActionResult Confirm(Seal_Using seal_using)
        {
            seal_using.Employee_Seal_keep = DateTime.Now;
            en.Entry(seal_using).State = System.Data.Entity.EntityState.Modified;  // entity 6 thêm System.Data.Entity Entity chứa các function
            en.SaveChanges();
            //~~~~~~~~~~~~~~~~~~~~~
            //~~~~~~~~~~~~~~~~~~~~~
            return RedirectToAction("Index", "SealUsing");
            //return View("Index");
        }

        //
        // GET: /SealUsing/Delete/5

        public ActionResult Delete(int? id)
        {
            Seal_Using seal_using = en.Seal_Using.Find(id);
            ViewBag.Department_confirm_date = DateTime.Now;
            string plant_id = userManager.GetUserPlant(session_emp);
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