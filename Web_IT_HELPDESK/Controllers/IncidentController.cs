using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Web_IT_HELPDESK.Controllers.ObjectManager;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.ViewModels;

using EntityState = System.Data.Entity.EntityState;

namespace Web_IT_HELPDESK.Controllers
{
    public class IncidentController : Controller
    {

        // GET: /Incident/
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
            to_date = from_date.AddMonths(1).AddSeconds(-1);

            if (session_emp != null)
            {
                IEnumerable<IncidentModel> ivm = IncidentViewModel.Instance.get_IEnum_Inc();

                ivm = ivm.Where(item => item.Del != true
                                            && item.datetime >= from_date
                                            && item.datetime <= to_date);

                if (admin_role == "0")
                {
                    ivm = ivm.Where(i => i.Del != true
                                    && i.User_create == session_emp
                                    && i.Plant == plantid);
                }

                Dictionary<string, string> Params = new Dictionary<string, string>();

                Params.Add("IsAdmin", admin_role);
                Params.Add("plants", "all");
                Params.Add("solved", "all");
                Params.Add("from_date", from_date.ToString());
                Params.Add("to_date", to_date.ToString());

                Session["Params"] = Params;
                ivm = ivm.OrderByDescending(i => i.Code);
                return View(ivm);
            }

            else return RedirectToAction("LogOn", "LogOn");
        }

        private DateTime from_date { get; set; }
        private DateTime to_date { get; set; }

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

            //string dept_id = en.Employees.Where(f => (f.EmployeeID == session_emp.ToString())).Select(f => f.DepatmentId).SingleOrDefault();

            if (session_emp != null)
            {
                IEnumerable<IncidentModel> inc = IncidentViewModel.Instance.get_IEnum_Inc();
                if (ViewBag.IsAdmin == true)
                {
                    inc = inc.Where(i => i.Del != true
                                    && i.datetime >= from_date
                                    && i.datetime <= to_date);
                    if (plants != "all")
                    {
                        inc = inc.Where(i => i.Plant == plants).OrderByDescending(i => i.Code);
                    }
                    if (solved != "all")
                    {
                        inc = inc.Where(i => i.Solved == Convert.ToBoolean(solved));
                    }
                }
                else
                {
                    inc = inc.Where(i => i.Del != true
                                    && i.User_create == session_emp
                                    && i.datetime >= from_date
                                    && i.datetime <= to_date
                                    && i.Plant == plantid);
                    if (solved != "all")
                    {
                        inc = inc.Where(i => i.Solved == Convert.ToBoolean(solved));
                    }
                }
                Dictionary<string, string> Params = new Dictionary<string, string>();

                Params.Add("IsAdmin", admin_role);
                Params.Add("plants", plants);
                Params.Add("solved", solved);
                Params.Add("from_date", from_date.ToString());
                Params.Add("to_date", to_date.ToString());

                Session["Params"] = Params;

                return View(inc.OrderByDescending(i => i.Code));
            }
            else return RedirectToAction("LogOn", "LogOn");
        }

        private string GetPlant_id()
        {
            string plant_id = en.Employees.Where(f => (f.EmployeeID == session_emp)).Select(f => f.Plant).SingleOrDefault();
            return plant_id;
        }


        // GET: /Incident/Details/5

        public ActionResult Details(Guid? inc_id)
        {

            Incident inc = en.Incidents.Find(inc_id);
            //ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", inc.LevelId);
            //ViewBag.StatusId = new SelectList(en.Status, "StatusId", "StatusName", inc.StatusId);
            //ViewBag.DepartmentId = new SelectList(en.Departments.Where(o => o.Plant == inc.Plant && o.DepartmentId == "0805"), "DepartmentId", "DepartmentName", inc.DepartmentId);

            ViewBag.LevelName = en.Levels.FirstOrDefault(l => l.LevelId == inc.LevelId).LevelName;
            ViewBag.StatusName = en.Status.FirstOrDefault(s => s.StatusId == inc.StatusId).StatusId;
            ViewBag.DepartmentName = en.Departments.FirstOrDefault(d => d.DepartmentId == inc.DepartmentId).DepartmentName;

            ViewBag.UserCreate = en.Employees.FirstOrDefault(e => e.EmployeeID == inc.User_create)?.EmployeeName;
            ViewBag.UserResolve = en.Employees.FirstOrDefault(e => e.EmployeeID == inc.User_resolve)?.EmployeeName;
            return View(inc);
        }

        // GET: /Incident/Create

        public ActionResult Create()
        {
            if (session_emp == "")
                return RedirectToAction("LogOn", "LogOn");
            else
            {
                Incident inc = new Incident();
                ViewBag.User_create = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).EmployeeName;

                string plantid = userManager.GetUserPlant(session_emp);
                ViewBag.plantName = Plants.Instance[plantid];
                ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", en.Levels.First().LevelId);
                ViewBag.StatusId = new SelectList(en.Status.Where(s => s.StatusId == "ST1"), "StatusId", "StatusName", en.Status.First().StatusId);
                ViewBag.DepartmentId = new SelectList(en.Departments.Where(o => o.Plant == plantid && o.DepartmentId == "0805"), "DepartmentId", "DepartmentName", en.Departments.First().DepartmentId);
                return View(inc);
            }
        }

        // POST: /Incident/Create

        private string subject { get; set; }
        private string body { get; set; }
        private string address_file { get; set; }
        public UserManager userManager = new UserManager();

        [HttpPost]
        public ActionResult Create(Incident incident, HttpPostedFileBase image)
        {
            string plantid = userManager.GetUserPlant(session_emp);
            string deptName = en.Departments.FirstOrDefault(d => d.DepartmentId == incident.DepartmentId && d.Plant == plantid).DepartmentName;

            List<string> mailCcIT = en.Employees.Where(e => e.Plant == plantid && e.DepatmentId == "0805").Select(e => e.Email).ToList();
            string mailBccManager = en.Departments.FirstOrDefault(e => e.Plant == plantid && e.DepartmentId == "0805").Manager_Email.ToString();

            image = image ?? Request.Files["image"];
            if (ModelState.IsValid)
            {
                if (image != null && image.ContentLength > 0)
                {
                    foreach (string upload in Request.Files)
                    {
                        //create byte array of size equal to file input stream
                        byte[] fileData = new byte[Request.Files[upload].InputStream.Length];
                        //add file input stream into byte array
                        Request.Files[upload].InputStream.Read(fileData, 0, Convert.ToInt32(Request.Files[upload].InputStream.Length));
                        //create system.data.linq object using byte array
                        Binary binaryFile = new System.Data.Linq.Binary(fileData);
                        //initialise object of FileDump LINQ to sql class passing values to be inserted

                        incident.FileAttched = binaryFile.ToArray();
                        incident.FileName = Path.GetFileName(Request.Files[upload].FileName);
                    }
                }

                incident.Plant = userManager.GetUserPlant(session_emp);
                incident.User_create = session_emp;

                en.Incidents.Add(incident);
                en.SaveChanges();

                /*1========== Sending Mail ==========================================================================================*/
                IncidentModel incEx = IncidentViewModel.Instance.get_single_Inc(incident.Id);
                List<string> toMails = en.Employees.Where(e => e.Plant == plantid && e.DepatmentId == "0805").Select(e => e.Email).ToList();
                List<string> ccMails = new List<string>() { en.Departments.FirstOrDefault(e => e.Plant == plantid && e.DepartmentId == "0805").Manager_Email };

                bool resultSend = IncidentHelper.Instance.Send_IncidentEmail(incEx, "CREATE", toMails, ccMails);
                /*1==================================================================================================================*/

                if (!resultSend)
                {
                    Response.Write("Sending mail method have been fail");
                    Response.End();
                }

                //TryToDelete(Path.Combine(a, incident.FileName)); // delete file is create in address_file path

                return RedirectToAction("Index", "Incident");
            }

            ViewBag.User_create = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).EmployeeName;
            ViewBag.plantName = Plants.Instance[plantid];
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", incident.LevelId);
            ViewBag.StatusId = new SelectList(en.Status, "StatusId", "StatusName", incident.StatusId);
            ViewBag.DepartmentId = new SelectList(en.Departments, "DepartmentId", "DepartmentName", incident.DepartmentId);

            return View(incident);
        }

        // GET: /Incident/Edit/5

        public ViewResult Edit(Guid? inc_id)
        {
            string plantid = userManager.GetUserPlant(session_emp);
            Incident inc = en.Incidents.Find(inc_id);
            ViewBag.plantName = Plants.Instance[plantid];

            ViewBag.UserCreate = en.Employees.FirstOrDefault(e => e.EmployeeID == inc.User_create)?.EmployeeName;
            ViewBag.UserResolve = en.Employees.FirstOrDefault(e => e.EmployeeID == inc.User_resolve)?.EmployeeName;

            ViewBag.FileName = inc.FileName;
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", inc.LevelId);
            ViewBag.StatusId = new SelectList(en.Status.Where(s => s.StatusId != "ST6" && s.StatusId != "ST2"), "StatusId", "StatusName", inc.StatusId);
            ViewBag.DepartmentId = new SelectList(en.Departments.Where(o => o.Plant == plantid && o.DepartmentId == "0805"), "DepartmentId", "DepartmentName", inc.DepartmentId);
            return View(inc);
        }

        // POST: /Incident/Edit/5

        [HttpPost]
        public ActionResult Edit(Incident new_inc, HttpPostedFileBase image)
        {
            string plantid = userManager.GetUserPlant(session_emp);
            string deptName = en.Departments.FirstOrDefault(d => d.DepartmentId == new_inc.DepartmentId && d.Plant == plantid).DepartmentName;

            List<string> mailCcIT = en.Employees.Where(e => e.Plant == plantid && e.DepatmentId == "0805").Select(e => e.Email).ToList();
            string mailBccManager = en.Departments.FirstOrDefault(e => e.Plant == plantid && e.DepartmentId == "0805").Manager_Email.ToString();

            image = image ?? Request.Files["attachment"];
            if (ModelState.IsValid)
            {
                string plantId = Plants.Instance.FirstOrDefault(p => p.Value == new_inc.Plant).Key;
                new_inc.Plant = plantId;

                var currInc = en.Incidents.FirstOrDefault(i => i.Id == new_inc.Id);
                currInc.Note = new_inc.Note;
                currInc.LevelId = new_inc.LevelId;
                currInc.StatusId = new_inc.StatusId;
                currInc.Reply = new_inc.Reply;

                if (image != null && image.ContentLength > 0)
                {
                    foreach (string upload in Request.Files)
                    {
                        //create byte array of size equal to file input stream
                        byte[] fileData = new byte[Request.Files[upload].InputStream.Length];
                        //add file input stream into byte array
                        Request.Files[upload].InputStream.Read(fileData, 0, Convert.ToInt32(Request.Files[upload].InputStream.Length));
                        //create system.data.linq object using byte array
                        System.Data.Linq.Binary binaryFile = new System.Data.Linq.Binary(fileData);
                        //initialise object of FileDump LINQ to sql class passing values to be inserted

                        currInc.FileAttched = binaryFile.ToArray();
                        currInc.FileName = System.IO.Path.GetFileName(Request.Files[upload].FileName);
                    }
                }

                try
                {
                    en.Entry(currInc).State = EntityState.Modified;
                    en.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                        }
                    }
                    Response.End();
                }

                /*1========== Sending Mail ==========================================================================================*/
                IncidentModel incEx = IncidentViewModel.Instance.get_single_Inc(currInc.Id);
                List<string> toMails = en.Employees.Where(e => e.Plant == plantid && e.DepatmentId == "0805").Select(e => e.Email).ToList();
                List<string> ccMails = new List<string>() { en.Departments.FirstOrDefault(e => e.Plant == plantid && e.DepartmentId == "0805").Manager_Email };

                bool resultSend = IncidentHelper.Instance.Send_IncidentEmail(incEx, "EDIT", toMails, ccMails);
                /*1==================================================================================================================*/

                if (!resultSend)
                {
                    Response.Write("Sending mail method have been fail");
                    Response.End();
                }

            }
            return RedirectToAction("Index", "Incident");
        }

        // GET: /Incident/Delete/5

        public ViewResult Solve(Guid? inc_id)
        {
            Incident inc = en.Incidents.Find(inc_id);

            ViewBag.UserCreate = en.Employees.FirstOrDefault(e => e.EmployeeID == inc.User_create)?.EmployeeName;
            ViewBag.User_resolve = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp)?.EmployeeName;
            ViewBag.plantName = Plants.Instance[inc.Plant];
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", inc.LevelId);
            ViewBag.StatusId = new SelectList(en.Status.Where(s => s.StatusId == "ST6"), "StatusId", "StatusName", inc.StatusId);
            ViewBag.DepartmentId = new SelectList(en.Departments.Where(o => o.Plant == inc.Plant && o.DepartmentId == "0805"), "DepartmentId", "DepartmentName", inc.DepartmentId);
            return View(inc);
        }

        [HttpPost]
        public ActionResult Solve(Incident inc)
        {
            string plantid = userManager.GetUserPlant(session_emp);
            string deptName = en.Departments.FirstOrDefault(d => d.DepartmentId == inc.DepartmentId && d.Plant == plantid).DepartmentName;

            List<string> mailCcIT = en.Employees.Where(e => e.Plant == plantid && e.DepatmentId == "0805").Select(e => e.Email).ToList();
            string mailBccManager = en.Departments.FirstOrDefault(e => e.Plant == plantid && e.DepartmentId == "0805").Manager_Email.ToString();

            if (inc.StatusId == "ST6" && ModelState.IsValid)
            {

                var currInc = en.Incidents.FirstOrDefault(i => i.Id == inc.Id);
                currInc.Note = inc.Note;
                currInc.LevelId = inc.LevelId;
                currInc.StatusId = inc.StatusId;
                currInc.Solved = true;
                currInc.Solve_datetime = DateTime.Now;
                currInc.User_resolve = session_emp;
                currInc.Reply = inc.Reply;
                string plantId = Plants.Instance.FirstOrDefault(p => p.Value == inc.Plant).Key;
                currInc.Plant = plantId;

                // Save Incident
                try
                {
                    en.Entry(currInc).State = EntityState.Modified;
                    en.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                        }
                    }
                    Response.End();
                }

                /*1========== Sending Mail ==========================================================================================*/
                IncidentModel incEx = IncidentViewModel.Instance.get_single_Inc(currInc.Id);
                List<string> toMails = new List<string>() { en.Employees.FirstOrDefault(e => e.EmployeeID == currInc.User_create).Email };
                List<string> ccMails = en.Employees.Where(e => e.Plant == plantid && e.DepatmentId == "0805").Select(e => e.Email).ToList();
                List<string> bccMails = new List<string>() { en.Departments.FirstOrDefault(e => e.Plant == plantid && e.DepartmentId == "0805").Manager_Email };

                bool resultSend = IncidentHelper.Instance.Send_IncidentEmail(incEx, "SOLVE", toMails, ccMails, bccMails);
                /*1==================================================================================================================*/

                if (!resultSend)
                {
                    Response.Write("Sending mail method have been fail");
                    Response.End();
                }

                return RedirectToAction("Index", "Incident");
            }

            ViewBag.User_resolve = session_emp;
            ViewBag.plantName = Plants.Instance[plantid];
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", inc.LevelId);
            ViewBag.StatusId = new SelectList(en.Status, "StatusId", "StatusName", inc.StatusId);
            ViewBag.DepartmentId = new SelectList(en.Departments.Where(o => o.Plant == plantid && o.DepartmentId == "0805"), "DepartmentId", "DepartmentName", inc.DepartmentId);

            ModelState.AddModelError("StatusId", "Sai tình trạng xử lý");
            return View(inc);
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

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
        public void email_send(string user_email, string pass, List<string> mailCcIT, string mailBccManager, string str_subject, string str_body, Incident inc)
        {
            #region hide
            //SmtpServer.Credentials = new System.Net.NetworkCredential("ithelpdesk@cjvina.com", "ithelpdesk2015");
            //List<string> validStatus = new List<string>()
            //{
            //    "ST1", "ST2", "ST3", "ST6"
            //};
            //if (validStatus.Contains(inc.StatusId))
            //{
            //    switch (inc.StatusId)
            //    {
            //        case "ST1":
            //            str_subject = "Open - " + str_subject;
            //            break;
            //        case "ST2":
            //            str_subject = "Close - " + str_subject;
            //            break;
            //        case "ST3":
            //            str_subject = "Pending - " + str_subject;
            //            break;
            //        case "ST6":
            //            str_subject = "Solved - " + str_subject;
            //            break;
            //    } 
            #endregion

            user_email = "ithelpdesk@cjvina.com";
            pass = "qwer4321!";
            string email = en.Employees.FirstOrDefault(e => e.EmployeeID == inc.User_create).Email.ToString();
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("mail.cjvina.com", 587);
                if (user_email.Contains("@cjvina.com"))
                {
                    mail.From = new MailAddress(user_email); // user_email send
                    mail.To.Add(email);
                    foreach (string pMail in mailCcIT)
                        if (!string.IsNullOrWhiteSpace(pMail))
                            mail.CC.Add(pMail);

                    mail.Bcc.Add(mailBccManager);

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
            //}
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

        public ActionResult Download()
        {
            UserManager userManager = new UserManager();
            string plantid = userManager.GetUserPlant(session_emp);
            string admin_role = en.Employees.Where(e => e.EmployeeID == session_emp.ToString() && e.Plant == plantid).Select(e => e.administrator).SingleOrDefault();

            Dictionary<string, string> Params = (Dictionary<string, string>)Session["Params"];

            bool IsAdmin = Convert.ToBoolean(Convert.ToInt32(Params["IsAdmin"]));
            string plants = Params["plants"];
            string solved = Params["solved"];
            DateTime from_date = Convert.ToDateTime(Params["from_date"]);
            DateTime to_date = Convert.ToDateTime(Params["to_date"]);

            if (session_emp != null)
            {
                IEnumerable<IncidentModel> inc = IncidentViewModel.Instance.get_IEnum_Inc();
                if (IsAdmin == true)
                {
                    inc = inc.Where(i => i.Del != true
                                    && i.datetime >= from_date
                                    && i.datetime <= to_date);

                    if (plants != "all")
                    {
                        inc = inc.Where(i => i.Plant == plants).OrderByDescending(i => i.Code);
                    }
                    if (solved != "all")
                    {
                        inc = inc.Where(i => i.Solved == Convert.ToBoolean(solved));
                    }
                }
                else
                {
                    inc = inc.Where(i => i.Del != true
                                    && i.User_create == session_emp
                                    && i.datetime >= from_date
                                    && i.datetime <= to_date
                                    && i.Plant == plantid);

                    if (solved != "all")
                    {
                        inc = inc.Where(i => i.Solved == Convert.ToBoolean(solved));
                    }
                }


                var listIncs = inc.ToList();
                var grid = new System.Web.UI.WebControls.GridView();

                grid.DataSource = listIncs;
                grid.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.ContentType = "application/ms-excel"; // EPP for xlsx
                Response.Charset = "";
                Response.ContentEncoding = System.Text.Encoding.Unicode;
                Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());
                Response.AddHeader("content-disposition", "attachment; filename=IT Order Requests.xls");
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                grid.RenderControl(htw);

                Response.Output.Write(sw.ToString());

                Response.Flush();
                Response.End();

                return View(inc.OrderByDescending(i => i.Code));
            }
            else return RedirectToAction("LogOn", "LogOn");
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
