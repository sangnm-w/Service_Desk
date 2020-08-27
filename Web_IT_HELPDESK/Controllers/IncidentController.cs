using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Web_IT_HELPDESK.Controllers.ObjectManager;

namespace Web_IT_HELPDESK.Controllers
{
    public class IncidentController : Controller
    {
        //
        // GET: /Incident/
        //db_context_entity en = new db_context_entity();
        Web_IT_HELPDESKEntities en = new Web_IT_HELPDESKEntities();
        private string session_emp = System.Web.HttpContext.Current.User.Identity.Name;


        public ActionResult Index()
        {
            // Kiểm tra thông tin user không được phân quyền nhưng lưu địa chỉ vào lậu, check session user và màn hình đang sử dụng
            UserManager userManager = new UserManager();
            string plantid = userManager.GetUserPlant(session_emp);
            string dept_id = en.Employees.Where(f => (f.EmployeeID == session_emp.ToString() && f.Plant == plantid)).Select(f => f.DepatmentId).SingleOrDefault();

            string admin_role = en.Employees.Where(e => e.EmployeeID == session_emp.ToString() && e.Plant == plantid).Select(e => e.administrator).SingleOrDefault();
            ViewBag.IsAdmin = Convert.ToBoolean(Convert.ToInt32(admin_role));

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

            if (session_emp != null)
            {
                IEnumerable<Incident> inc = null;
                inc = en.Incidents.Where(i => i.Del != true
                                            && i.datetime >= from_date
                                            && i.datetime <= to_date);
                if (admin_role == "0")
                {
                    inc = inc.Where(i => i.Del != true
                    && i.User_create == session_emp
                    && i.Plant == plantid);
                }

                return View(inc.OrderByDescending(i => i.Code));
            }
            else return RedirectToAction("LogOn", "LogOn");
        }

        private DateTime to_date { get; set; }
        private DateTime from_date { get; set; }

        [Authorize]
        [HttpPost]
        public ActionResult Index(string solved, string _datetime, string plants)
        {
            IFormatProvider culture = new CultureInfo("en-US", true);

            string plantid = userManager.GetUserPlant(session_emp);
            string admin_role = en.Employees.Where(e => e.EmployeeID == session_emp.ToString() && e.Plant == plantid).Select(e => e.administrator).SingleOrDefault();
            ViewBag.IsAdmin = Convert.ToBoolean(Convert.ToInt32(admin_role));

            from_date = DateTime.ParseExact(_datetime, "yyyy-MM", culture);
            to_date = from_date.AddMonths(1).AddSeconds(-1);

            #region Remove by Mr.Sang

            //string v_mm = from_date.Month.ToString("D2");

            //switch (v_mm)
            //{
            //    case "01":
            //        to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
            //        break;
            //    case "02":
            //        to_date = DateTime.ParseExact("28/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
            //        break;
            //    case "03":
            //        to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
            //        break;
            //    case "04":
            //        to_date = DateTime.ParseExact("30/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
            //        break;
            //    case "05":
            //        to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
            //        break;
            //    case "06":
            //        to_date = DateTime.ParseExact("30/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
            //        break;
            //    case "07":
            //        to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
            //        break;
            //    case "08":
            //        to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
            //        break;
            //    case "09":
            //        to_date = DateTime.ParseExact("30/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
            //        break;
            //    case "10":
            //        to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
            //        break;
            //    case "11":
            //        to_date = DateTime.ParseExact("30/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
            //        break;
            //    case "12":
            //        to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
            //        break;
            //}
            #endregion

            string dept_id = en.Employees.Where(f => (f.EmployeeID == session_emp.ToString())).Select(f => f.DepatmentId).SingleOrDefault();

            if (session_emp != null)
            {
                IEnumerable<Incident> inc = null;
                if (ViewBag.IsAdmin == true)
                {
                    inc = en.Incidents.Where(i => i.Del != true
                        && i.datetime >= from_date
                        && i.datetime <= to_date);
                    if (plants != "All")
                    {
                        inc = inc.Where(i => i.Plant == plants).OrderByDescending(i => i.Code);
                    }
                    if (solved != "All")
                    {
                        inc = inc.Where(i => i.Solved == Convert.ToBoolean(solved));
                    }
                }
                else
                {
                    inc = en.Incidents.Where(i => i.Del != true
                    && i.User_create == session_emp
                    && i.datetime >= from_date
                    && i.datetime <= to_date
                    && i.Plant == plantid);
                    if (solved != "All")
                    {
                        inc = inc.Where(i => i.Solved == Convert.ToBoolean(solved));
                    }
                }
                return View(inc.OrderByDescending(i => i.Code));

                #region remove by Mr. Sang
                //if (solved == "False")
                //{
                //    var inc = en.Incidents.Where(i => i.Del != true
                //        //&& i.DepartmentId == dept_id 
                //        && i.User_create == session_emp
                //        && i.datetime >= from_date
                //        && i.datetime <= to_date
                //        && i.Solved == false).OrderByDescending(i => i.Code);// && i.User_create == Session["employee_id"].ToString());// || i.User_resolve == session_emp.ToString()));
                //    if (plants != "All")
                //        inc = en.Incidents.Where(i => i.Del != true
                //        //&& i.DepartmentId == dept_id 
                //        && i.User_create == session_emp
                //            && i.datetime >= from_date
                //            && i.datetime <= to_date
                //            && i.Solved == false
                //            && i.Plant == plants).OrderByDescending(i => i.Code);
                //    return View(inc);
                //}
                //else if (solved == "True")
                //{
                //    var inc = en.Incidents.Where(i => i.Del != true
                //        //&& i.DepartmentId == dept_id 
                //        && i.User_create == session_emp
                //        && i.datetime >= from_date
                //        && i.datetime <= to_date
                //        && i.Solved == true).OrderByDescending(i => i.Code);// && i.User_create == Session["employee_id"].ToString());// || i.User_resolve == session_emp.ToString()));
                //    if (plants != "All")
                //        inc = en.Incidents.Where(i => i.Del != true
                //        //&& i.DepartmentId == dept_id 
                //        && i.User_create == session_emp
                //         && i.datetime >= from_date
                //         && i.datetime <= to_date
                //         && i.Solved == true
                //         && i.Plant == plants).OrderByDescending(i => i.Code); ;// && i.User_create == Session["employee_id"].ToString());// || i.User_resolve == session_emp.ToString()));
                //    return View(inc);
                //}
                //else
                //{
                //    var inc = en.Incidents.Where(i => i.Del != true
                //        //&& i.DepartmentId == dept_id 
                //        && i.User_create == session_emp
                //        && i.datetime >= from_date
                //        && i.datetime <= to_date).OrderByDescending(i => i.Code);// && i.User_create == Session["employee_id"].ToString());// || i.User_resolve == session_emp.ToString()));
                //    if (plants != "All")
                //        inc = en.Incidents.Where(i => i.Del != true
                //        //&& i.DepartmentId == dept_id 
                //        && i.User_create == session_emp
                //             && i.datetime >= from_date
                //             && i.datetime <= to_date
                //             && i.Plant == plants).OrderByDescending(i => i.Code);
                //    return View(inc);
                //}
                #endregion
            }
            else return RedirectToAction("LogOn", "LogOn");
        }

        private string GetPlant_id()
        {
            string plant_id = en.Employees.Where(f => (f.EmployeeID == session_emp)).Select(f => f.Plant).SingleOrDefault();
            return plant_id;
        }

        //
        // GET: /Incident/Details/5

        public ActionResult Details(Guid? inc_id)
        {
            Incident inc = en.Incidents.Find(inc_id);
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", inc.LevelId);
            ViewBag.StatusId = new SelectList(en.Status, "StatusId", "StatusName", inc.StatusId);
            ViewBag.DepartmentId = new SelectList(en.Departments, "DepartmentId", "DepartmentName", inc.DepartmentId);
            return View(inc);
        }

        //
        // GET: /Incident/Create

        public ActionResult Create()
        {
            if (session_emp == "")
                return RedirectToAction("LogOn", "LogOn");
            else
            {
                Incident inc = new Incident();
                ViewBag.User_create = session_emp;
                string plantid = userManager.GetUserPlant(session_emp);
                ViewBag.Plant = plantid;
                ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", en.Levels.First().LevelId);
                ViewBag.StatusId = new SelectList(en.Status, "StatusId", "StatusName", en.Status.First().StatusId);
                ViewBag.DepartmentId = new SelectList(en.Departments.Where(o => o.Plant == plantid), "DepartmentId", "DepartmentName", en.Departments.First().DepartmentId);
                return View(inc);
            }
        }

        //
        // POST: /Incident/Create

        private string subject { get; set; }
        private string body { get; set; }
        private string address_file { get; set; }
        public UserManager userManager = new UserManager();


        [HttpPost]
        public ActionResult Create(Incident incident, HttpPostedFileBase image)
        {
            string plantid = userManager.GetUserPlant(session_emp);
            //var dept = from i in en.Departments where i.DepartmentId == incident.DepartmentId && incident.Plant == plantid select i.DepartmentName;
            string v_dept_name = en.Departments.Where(o => o.DepartmentId == incident.DepartmentId && o.Plant == plantid).Select(i => i.DepartmentName).SingleOrDefault();
            image = image ?? Request.Files["image"];
            if (ModelState.IsValid)
            {
                if (image != null && image.ContentLength > 0)
                {
                    //string filePath = Path.Combine(Server.MapPath("/Temp"), Path.GetFileName(image.FileName));
                    //image.SaveAs(filePath);

                    /*int length = image.ContentLength;
                    byte[] buffer = new byte[length];
                    image.InputStream.Read(buffer, 0, length);
                    incident.FileAttched = buffer;
                    incident.FileName = System.IO.Path.GetFileName(Request.Files["image"].FileName);

                    string a = "D:\\";
                    image.SaveAs(Path.Combine(a, incident.FileName));
                    string address_file = Path.Combine(a, incident.FileName);*/

                    foreach (string upload in Request.Files)
                    {
                        //create byte array of size equal to file input stream
                        byte[] fileData = new byte[Request.Files[upload].InputStream.Length];
                        //add file input stream into byte array
                        Request.Files[upload].InputStream.Read(fileData, 0, Convert.ToInt32(Request.Files[upload].InputStream.Length));
                        //create system.data.linq object using byte array
                        System.Data.Linq.Binary binaryFile = new System.Data.Linq.Binary(fileData);
                        //initialise object of FileDump LINQ to sql class passing values to be inserted

                        incident.FileAttched = binaryFile.ToArray();
                        incident.FileName = System.IO.Path.GetFileName(Request.Files[upload].FileName);
                    }

                    // chưa có plant
                    incident.Plant = userManager.GetUserPlant(session_emp);

                    en.Incidents.Add(incident);
                    en.SaveChanges();
                    //~~~~~~~~~~~~~~~~~~~~~
                    subject = "Yêu cầu giải quyết incident: " + incident.Code + " - tạo ngày: " + incident.datetime;
                    body = "- Incident được tạo bởi: " + incident.User_create + "\n" + incident.Note + "\n" +
                     "- Nhân viên phòng ban: " + v_dept_name + " \n- Hỗ trợ incidnet: " + incident.Code + "\n- Trân trọng!";
                    //email_send(address_file, "user_email", "pass", incident.DepartmentId, subject, body, incident.StatusId);
                    //~~~~~~~~~~~~~~~~~~~~~
                    //TryToDelete(Path.Combine(a, incident.FileName)); // delete file is create in address_file path


                    return RedirectToAction("Index", "Incident");
                }
                else
                {
                    en.Incidents.Add(incident);
                    en.SaveChanges();
                    //~~~~~~~~~~~~~~~~~~~~~
                    subject = "Yêu cầu giải quyết incident: " + incident.Code + " - tạo ngày: " + incident.datetime;
                    body = "Incident được tạo bởi: " + incident.User_create + "\n" + incident.Note + "\n" +
                    "Nhân viên phòng ban: " + v_dept_name + " \n Hỗ trợ incidnet: " + incident.Code + "\n Trân trọng!";
                    email_send(incident.FileName, "user_email", "pass", incident.DepartmentId, subject, body, incident.StatusId, incident.Plant);
                    //~~~~~~~~~~~~~~~~~~~~~
                    return RedirectToAction("Index", "Incident");
                }
            }
            //else
            //    return View();
            //ViewBag.LevelId = new SelectList(en.Level, "IdLevel", "LevelName", incident.LevelId);
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", incident.LevelId);
            ViewBag.StatusId = new SelectList(en.Status, "StatusId", "StatusName", incident.StatusId);
            ViewBag.DepartmentId = new SelectList(en.Departments, "DepartmentId", "DepartmentName", incident.DepartmentId);

            return View(incident);

        }

        //
        // GET: /Incident/Edit/5

        public ViewResult Edit(Guid? inc_id)
        {
            string plantid = userManager.GetUserPlant(session_emp);
            Incident inc = en.Incidents.Find(inc_id);
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", inc.LevelId);
            ViewBag.StatusId = new SelectList(en.Status, "StatusId", "StatusName", inc.StatusId);
            ViewBag.DepartmentId = new SelectList(en.Departments.Where(o => o.Plant == plantid), "DepartmentId", "DepartmentName", inc.DepartmentId);
            return View(inc);
        }

        //
        // POST: /Incident/Edit/5

        [HttpPost]
        public ActionResult Edit(Incident inc, HttpPostedFileBase image)
        {
            string plantid = userManager.GetUserPlant(session_emp);
            var dept = from i in en.Departments where i.DepartmentId == inc.DepartmentId && inc.Plant == plantid select i.DepartmentName;

            image = image ?? Request.Files["attachment"];
            if (ModelState.IsValid)
            {
                if (image != null && image.ContentLength > 0)
                {
                    //string filePath = Path.Combine(Server.MapPath("/Temp"), Path.GetFileName(image.FileName));
                    //image.SaveAs(filePath);

                    /*int length = image.ContentLength;
                    byte[] buffer = new byte[length];
                    image.InputStream.Read(buffer, 0, length);
                    inc.FileAttched = buffer;
                    inc.FileName = System.IO.Path.GetFileName(Request.Files["image"].FileName);

                    string a = "D:\\";
                    image.SaveAs(Path.Combine(a, inc.FileName));

                    string address_file = Path.Combine(a, inc.FileName);*/

                    foreach (string upload in Request.Files)
                    {
                        //create byte array of size equal to file input stream
                        byte[] fileData = new byte[Request.Files[upload].InputStream.Length];
                        //add file input stream into byte array
                        Request.Files[upload].InputStream.Read(fileData, 0, Convert.ToInt32(Request.Files[upload].InputStream.Length));
                        //create system.data.linq object using byte array
                        System.Data.Linq.Binary binaryFile = new System.Data.Linq.Binary(fileData);
                        //initialise object of FileDump LINQ to sql class passing values to be inserted

                        inc.FileAttched = binaryFile.ToArray();
                        inc.FileName = System.IO.Path.GetFileName(Request.Files[upload].FileName);
                    }
                }
                en.Entry(inc).State = System.Data.Entity.EntityState.Modified;
                en.SaveChanges();
                //~~~~~~~~~~~~~~~~~~~~~
                subject = "[Điều chỉnh] - Yêu cầu giải quyết incident: " + inc.Code + " - tạo ngày: " + inc.datetime;
                body = "Incident được tạo bởi: " + inc.User_create + "\n" + inc.Note + "\n" +
                 "Nhân viên phòng ban: " + dept.ToString() + " \n Hỗ trợ incidnet: " + inc.Code + "\n Trân trọng!";

                email_send("user_email", "pass", subject, body, inc);
                //~~~~~~~~~~~~~~~~~~~~~

            }
            return RedirectToAction("Index", "Incident");
        }
        //
        // GET: /Incident/Delete/5

        public ViewResult Solve(Guid? inc_id)
        {
            Incident inc = en.Incidents.Find(inc_id);
            ViewBag.User_resolve = session_emp;
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", inc.LevelId);
            ViewBag.StatusId = new SelectList(en.Status, "StatusId", "StatusName", inc.StatusId);
            ViewBag.DepartmentId = new SelectList(en.Departments, "DepartmentId", "DepartmentName", inc.DepartmentId);
            return View(inc);
        }

        [HttpPost]
        public ActionResult Solve(Incident inc)
        {
            if (inc.StatusId == "ST6")
            {
                if (ModelState.IsValid)
                    en.Entry(inc).State = System.Data.Entity.EntityState.Modified;

                string user_create = Convert.ToString(inc.User_create);
                string note = Convert.ToString(inc.Note);
                string user_resolve = Convert.ToString(inc.User_resolve);
                string reply = Convert.ToString(inc.Reply);

                //inc.Plant = userManager.GetUserPlant(session_emp);

                inc.Solved = true;
                inc.Solve_datetime = DateTime.Now;
                en.SaveChanges();

                //~~~~~~~~~~~~~~~~~~~~~
                subject = "Yêu cầu giải quyết issue: " + inc.Code + " - tạo ngày: " + inc.datetime;
                body = "Issue được tạo bởi: " + user_create + "\n" + note + "\n" +
                "Nhân viên phòng ban: " + user_resolve + " \n phản hồi issue: " + reply + "\n Trân trọng!";

                email_send("user_email", "pass", subject, body, inc);
                //~~~~~~~~~~~~~~~~~~~~~
                return RedirectToAction("Index", "Incident");
            }

            ViewBag.User_resolve = session_emp;
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", inc.LevelId);
            ViewBag.StatusId = new SelectList(en.Status, "StatusId", "StatusName", inc.StatusId);
            ViewBag.DepartmentId = new SelectList(en.Departments, "DepartmentId", "DepartmentName", inc.DepartmentId);

            ModelState.AddModelError("StatusId", "Sai tình trạng xử lý");
            return View(inc);
            //return RedirectToAction("Solve", "Incident", new { inc_id = inc.Id });
        }


        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Incident/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        DataTable dt = new DataTable();
        // attach send mail
        public void email_send(string user_email, string pass, string str_subject, string str_body, Incident inc)
        {
            //SmtpServer.Credentials = new System.Net.NetworkCredential("ithelpdesk@cjvina.com", "ithelpdesk2015");
            List<string> validStatus = new List<string>()
            {
                "ST1", "ST2", "ST3", "ST6"
            };
            if (validStatus.Contains(inc.StatusId))
            {
                switch (inc.StatusId)
                {
                    case "ST1":
                        str_subject = "Open - " + str_subject;
                        break;
                    case "ST2":
                        str_subject = "Close - " + str_subject;
                        break;
                    case "ST3":
                        str_subject = "Pending - " + str_subject;
                        break;
                    case "ST6":
                        str_subject = "Solved - " + str_subject;
                        break;
                }
                user_email = "ithelpdesk@cjvina.com";
                pass = "qwer4321!";
                string email = en.Employees.Where(e => e.EmployeeID == inc.User_create).Select(e => e.Email).SingleOrDefault();
                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("mail.cjvina.com", 587);
                    if (user_email.Contains("@cjvina.com"))
                    {
                        mail.From = new MailAddress(user_email); // user_email send
                        mail.To.Add(email);
                        mail.CC.Add("minhsang.it@cjvina.com");

                        mail.Subject = str_subject;
                        mail.Body = str_body;

                        SmtpServer.Port = 25;
                        SmtpServer.Credentials = new System.Net.NetworkCredential(user_email, pass);  // "ithelpdesk@cjvina.com", "ithelpdesk2015"
                        SmtpServer.EnableSsl = false;
                        SmtpServer.Send(mail);
                    }
                    //else { MessageBox.Show("Sai cấu trúc email", "Thông báo!"); }
                }
                catch (Exception ex) { }//MessageBox.Show("Lỗi: " + ex.ToString(), "Thông báo!"); }
            }
        }
        public void email_send(string attchedfile_name, string user_email, string pass, string department_id, string str_subject, string str_body, string inc_status, string v_plant_id)
        {
            //SmtpServer.Credentials = new System.Net.NetworkCredential("ithelpdesk@cjvina.com", "ithelpdesk2015");
            if (inc_status == "ST1" || inc_status == "ST2" || inc_status == "ST3" || inc_status == "ST6")
            {
                switch (inc_status)
                {
                    case "ST1":
                        str_subject = "Open - " + str_subject;
                        break;
                    case "ST2":
                        str_subject = "Close - " + str_subject;
                        break;
                    case "ST3":
                        str_subject = "Pending - " + str_subject;
                        break;
                    case "ST6":
                        str_subject = "Solved - " + str_subject;
                        break;
                }
                user_email = "ithelpdesk@cjvina.com";
                pass = "ithelpdesk2015";
                string email = "";
                dt = email_data(department_id, v_plant_id);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    email = dt.Rows[i][2].ToString();

                    try
                    {
                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient("mail.cjvina.com", 587);
                        if (user_email.Contains("@cjvina.com"))
                        {
                            mail.From = new MailAddress(user_email); // user_email send
                            mail.To.Add(email); // add email is sent
                            //if (str_subject == "")
                            //{ MessageBox.Show("Gởi mail không cần tiêu đề", "Thông báo!");  }
                            mail.Subject = str_subject;


                            if (!String.IsNullOrEmpty(attchedfile_name))
                            {
                                System.Net.Mail.Attachment attachment;
                                attachment = new System.Net.Mail.Attachment(attchedfile_name);
                                mail.Attachments.Add(attachment);
                            }

                            // body
                            mail.Body = str_body;

                            SmtpServer.Port = 25;
                            SmtpServer.Credentials = new System.Net.NetworkCredential(user_email, pass);  // "ithelpdesk@cjvina.com", "ithelpdesk2015"
                            SmtpServer.EnableSsl = false;
                            SmtpServer.Send(mail);
                        }
                        //else { MessageBox.Show("Sai cấu trúc email", "Thông báo!"); }
                    }
                    catch (Exception ex) { }//MessageBox.Show("Lỗi: " + ex.ToString(), "Thông báo!"); }
                }
            }
        }

        private string connString = ConfigurationManager.ConnectionStrings["Web_IT_HELPDESK_connString"].ConnectionString;
        private DataTable email_data(string department_id, string v_plant_id)
        {

            DataTable datatable = new DataTable();
            DataSet ds = new DataSet();
            string sql;

            sql = "select employeeid,EmployeeName,Email\n" +
                    "from employee\n" +
                    "Where DepatmentId='" + department_id + "'\n" +
                       "and Plant='" + v_plant_id + "'";

            using (SqlConnection connection = new SqlConnection(connString))
            {
                SqlCommand command = new SqlCommand(sql);

                command.Connection = connection;

                try
                {
                    connection.Open();
                    SqlDataAdapter sqldap = new SqlDataAdapter(sql, connection);

                    ds.Clear();
                    sqldap.Fill(ds);
                    datatable = ds.Tables[0];
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return datatable;
        }

        [HttpGet]
        public FileContentResult Get_file(Guid? inc_id)
        {
            Incident inc = en.Incidents.Find(inc_id);
            string inc_code = inc.Code;

            //declare byte array to get file content from database and string to store file name
            byte[] fileData;
            string fileName;
            //create object of LINQ to SQL class
            //using LINQ expression to get record from database for given id value
            var record = from p in en.Incidents
                         where p.Code == inc_code
                         select p;
            //only one record will be returned from database as expression uses condtion on primary field
            //so get first record from returned values and retrive file content (binary) and filename
            fileData = (byte[])record.First().FileAttched.ToArray();
            fileName = record.First().FileName;
            //return file and provide byte file content and file name
            return File(fileData, "image", fileName);
        }

        // delete file export by crystal report
        static bool TryToDelete(string file_path)
        {
            try
            {
                // A.
                // Try to delete the file.
                System.IO.File.Delete(file_path);
                return true;
            }
            catch (IOException)
            {
                // B.
                // We could not delete the file.
                return false;
            }
        }
    }
}
