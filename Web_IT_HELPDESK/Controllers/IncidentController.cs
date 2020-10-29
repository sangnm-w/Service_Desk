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
        ServiceDeskEntities en = new ServiceDeskEntities();
        private string session_emp = System.Web.HttpContext.Current.User.Identity.Name;
        [Authorize]
        public ActionResult Index()
        {
            // Kiểm tra thông tin user không được phân quyền nhưng lưu địa chỉ vào lậu, check session user và màn hình đang sử dụng
            string curr_plantId = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Plant_Id;
            string curr_deptId = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Department_Id;

            bool admin_role = (bool)en.Employees.Where(e => e.EmployeeID == session_emp.ToString() && e.Plant_Id == curr_plantId).Select(e => e.Administrator).SingleOrDefault();
            ViewBag.IsAdmin = Convert.ToBoolean(Convert.ToInt32(admin_role));

            ViewBag.Plants = en.Departments.Select(d => new PlantViewModel { Plant_Id = d.Plant_Id, Plant_Name = d.Plant_Name }).Distinct().ToList();

            IFormatProvider culture = new CultureInfo("en-US", true);
            string _datetime = DateTime.Now.ToString("MM/yyyy");
            from_date = DateTime.ParseExact("01/" + _datetime, "dd/MM/yyyy", culture);
            to_date = from_date.AddMonths(1).AddSeconds(-1);

            if (session_emp != null)
            {
                IEnumerable<IncidentViewModel> ivm = IncidentModel.Instance.get_IEnum_Inc();

                ivm = ivm.Where(item => item.Del != true
                                            && item.datetime >= from_date
                                            && item.datetime <= to_date);

                if (admin_role == false)
                {
                    ivm = ivm.Where(i => i.Del != true
                                    && i.User_create == session_emp
                                    && i.Plant == curr_plantId);
                }

                Dictionary<string, string> Params = new Dictionary<string, string>();

                Params.Add("IsAdmin", admin_role.ToString());
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

            string curr_plantId = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Plant_Id;
            string curr_deptId = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Department_Id;

            bool admin_role = (bool)en.Employees.Where(e => e.EmployeeID == session_emp.ToString() && e.Plant_Id == curr_plantId).Select(e => e.Administrator).SingleOrDefault();
            ViewBag.IsAdmin = Convert.ToBoolean(Convert.ToInt32(admin_role));

            ViewBag.Plants = en.Departments.Select(d => new PlantViewModel { Plant_Id = d.Plant_Id, Plant_Name = d.Plant_Name }).Distinct().ToList();

            from_date = DateTime.ParseExact(_datetime, "yyyy-MM", culture);
            to_date = from_date.AddMonths(1).AddSeconds(-1);

            //string dept_id = en.Employees.Where(f => (f.EmployeeID == session_emp.ToString())).Select(f => f.DepartmentId).SingleOrDefault();

            if (session_emp != null)
            {
                IEnumerable<IncidentViewModel> inc = IncidentModel.Instance.get_IEnum_Inc();
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
                                    && i.Plant == curr_plantId);
                    if (solved != "all")
                    {
                        inc = inc.Where(i => i.Solved == Convert.ToBoolean(solved));
                    }
                }
                Dictionary<string, string> Params = new Dictionary<string, string>();

                Params.Add("IsAdmin", admin_role.ToString());
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
            string plant_id = en.Employees.Where(f => (f.EmployeeID == session_emp)).Select(f => f.Plant_Id).SingleOrDefault();
            return plant_id;
        }

        // GET: /Incident/Details
        public ActionResult Details(Guid? inc_id)
        {
            Incident inc = en.Incidents.Find(inc_id);

            ViewBag.LevelName = en.Levels.FirstOrDefault(l => l.LevelId == inc.LevelId).LevelName;
            ViewBag.StatusName = en.Status.FirstOrDefault(s => s.StatusId == inc.StatusId).StatusName;

            ViewBag.plantName = DepartmentModel.Instance.getPlantName(inc.Plant);
            ViewBag.DepartmentName = DepartmentModel.Instance.getDeptName(inc.Plant, inc.DepartmentId);

            ViewBag.User_Create = en.Employees.FirstOrDefault(e => e.EmployeeID == inc.User_create)?.EmployeeName;
            ViewBag.User_Resolve = en.Employees.FirstOrDefault(e => e.EmployeeID == inc.User_resolve)?.EmployeeName;
            return View(inc);
        }

        // GET: /Incident/Create
        public ActionResult Create()
        {
            if (session_emp == "")
                return RedirectToAction("LogOn", "LogOn");
            else
            {

                string curr_plantId = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Plant_Id;
                string curr_deptId = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Department_Id;

                Incident inc = new Incident();
                inc.Code = IncidentModel.Instance.Generate_IncidentCode(curr_plantId);
                inc.User_create = session_emp;
                inc.Plant = curr_plantId;
                inc.DepartmentId = curr_deptId;
                inc.StatusId = "ST1";


                ViewBag.UserCreateName = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).EmployeeName;
                ViewBag.plantName = en.Departments.FirstOrDefault(d => d.Plant_Id == curr_plantId).Plant_Name;
                ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", en.Levels.First().LevelId);
                ViewBag.StatusName = en.Status.FirstOrDefault(s => s.StatusId == "ST1").StatusName;
                ViewBag.DepartmentName = DepartmentModel.Instance.getDeptName(curr_plantId, curr_deptId);
                return View(inc);
            }
        }

        // POST: /Incident/Create
        private string subject { get; set; }
        private string body { get; set; }
        private string address_file { get; set; }
        public UserManager userManager = new UserManager();

        [HttpPost]
        public ActionResult Create(Incident incident, HttpPostedFileBase attachment)
        {
            string curr_plantId = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Plant_Id;
            string curr_deptId = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Department_Id;

            attachment = attachment ?? Request.Files["attachment"];
            if (ModelState.IsValid)
            {
                if (attachment != null && attachment.ContentLength > 0)
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

                en.Incidents.Add(incident);
                en.SaveChanges();

                /*1========== Sending Mail ==========================================================================================*/
                IncidentViewModel incEx = IncidentModel.Instance.get_Incident(incident.Id);

                List<string> toMails = en.Employees.Where(e => e.Plant_Id == curr_plantId && e.Department_Id == "V20S000001").Select(e => e.Email).ToList();
                List<string> ccMails = new List<string>();
                if (curr_plantId != "V2090")
                {
                    ccMails.Add("itgroup@cjvina.com");
                }

                bool resultSend = IncidentHelper.Instance.Send_IncidentEmail(incEx, "CREATE", toMails, ccMails);
                /*1==================================================================================================================*/

                return RedirectToAction("Index", "Incident");
            }

            ViewBag.UserCreateName = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).EmployeeName;
            ViewBag.plantName = en.Departments.FirstOrDefault(d => d.Plant_Id == curr_plantId).Plant_Name;
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", en.Levels.First().LevelId);
            ViewBag.StatusName = en.Status.FirstOrDefault(s => s.StatusId == "ST1").StatusName;
            ViewBag.DepartmentName = DepartmentModel.Instance.getDeptName(curr_plantId, curr_deptId);

            return View(incident);
        }

        // GET: /Incident/Edit

        public ViewResult Edit(Guid? inc_id)
        {
            string curr_plantId = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Plant_Id;
            string curr_deptId = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Department_Id;

            Incident inc = en.Incidents.Find(inc_id);

            ViewBag.plantName = en.Departments.FirstOrDefault(d => d.Plant_Id == curr_plantId).Plant_Name;
            ViewBag.DepartmentName = DepartmentModel.Instance.getDeptName(inc.Plant, inc.DepartmentId);
            ViewBag.StatusId = new SelectList(en.Status.Where(s => s.StatusId != "ST6" && s.StatusId != "ST2"), "StatusId", "StatusName", inc.StatusId);
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", inc.LevelId);
            ViewBag.UserCreateName = en.Employees.FirstOrDefault(e => e.EmployeeID == inc.User_create)?.EmployeeName;

            return View(inc);
        }

        // POST: /Incident/Edit

        [HttpPost]
        public ActionResult Edit(Incident inc, HttpPostedFileBase attachment)
        {
            string curr_plantId = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Plant_Id;
            string curr_deptId = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Department_Id;

            attachment = attachment ?? Request.Files["attachment"];
            if (ModelState.IsValid)
            {
                if (attachment != null && attachment.ContentLength > 0)
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

                        inc.FileAttched = binaryFile.ToArray();
                        inc.FileName = System.IO.Path.GetFileName(Request.Files[upload].FileName);
                    }
                }

                en.Entry(inc).State = EntityState.Modified;
                en.SaveChanges();

                /*1========== Sending Mail ==========================================================================================*/
                IncidentViewModel incEx = IncidentModel.Instance.get_Incident(inc.Id);
                List<string> toMails = en.Employees.Where(e => e.Plant_Id == curr_plantId && e.Department_Id == "V20S000001").Select(e => e.Email).ToList();
                List<string> ccMails = new List<string>();
                if (curr_plantId != "V2090")
                {
                    ccMails.Add("itgroup@cjvina.com");
                }
                bool resultSend = IncidentHelper.Instance.Send_IncidentEmail(incEx, "EDIT", toMails, ccMails);
                /*1==================================================================================================================*/

                return RedirectToAction("Index", "Incident");
            }

            ViewBag.plantName = en.Departments.FirstOrDefault(d => d.Plant_Id == curr_plantId).Plant_Name;
            ViewBag.DepartmentName = DepartmentModel.Instance.getDeptName(inc.Plant, inc.DepartmentId);
            ViewBag.StatusId = new SelectList(en.Status.Where(s => s.StatusId != "ST6" && s.StatusId != "ST2"), "StatusId", "StatusName", inc.StatusId);
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", inc.LevelId);
            ViewBag.UserCreateName = en.Employees.FirstOrDefault(e => e.EmployeeID == inc.User_create)?.EmployeeName;

            return View(inc);
        }

        // GET: /Incident/Solve

        public ViewResult Solve(Guid? inc_id)
        {
            string curr_plantId = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Plant_Id;
            string curr_deptId = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Department_Id;

            Incident inc = en.Incidents.Find(inc_id);
            inc.StatusId = "ST6";

            ViewBag.plantName = en.Departments.FirstOrDefault(d => d.Plant_Id == inc.Plant).Plant_Name;
            ViewBag.DepartmentName = DepartmentModel.Instance.getDeptName(curr_plantId, curr_deptId);
            ViewBag.StatusName = en.Status.FirstOrDefault(s => s.StatusId == "ST6").StatusName;
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", inc.LevelId);
            ViewBag.UserCreateName = en.Employees.FirstOrDefault(e => e.EmployeeID == inc.User_create)?.EmployeeName;
            ViewBag.UserResolveName = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp)?.EmployeeName;

            return View(inc);
        }

        [HttpPost]
        public ActionResult Solve(Incident inc)
        {
            string curr_plantId = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Plant_Id;
            string curr_deptId = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Department_Id;

            if (inc.StatusId == "ST6" && ModelState.IsValid)
            {
                inc.Solved = true;
                inc.Solve_datetime = DateTime.Now;
                inc.User_resolve = session_emp;

                en.Entry(inc).State = EntityState.Modified;
                en.SaveChanges();

                /*1========== Sending Mail ==========================================================================================*/
                IncidentViewModel incEx = IncidentModel.Instance.get_Incident(inc.Id);
                List<string> toMails = new List<string>() { en.Employees.FirstOrDefault(e => e.EmployeeID == inc.User_create).Email };
                List<string> ccMails = en.Employees.Where(e => e.Plant_Id == curr_plantId && e.Department_Id == "V20S000001").Select(e => e.Email).ToList();
                if (curr_plantId != "V2090")
                {
                    ccMails.Add("itgroup@cjvina.com");
                }

                bool resultSend = IncidentHelper.Instance.Send_IncidentEmail(incEx, "SOLVE", toMails, ccMails);
                /*1==================================================================================================================*/

                return RedirectToAction("Index", "Incident");
            }

            ViewBag.plantName = en.Departments.FirstOrDefault(d => d.Plant_Id == inc.Plant).Plant_Name;
            ViewBag.DepartmentName = DepartmentModel.Instance.getDeptName(curr_plantId, curr_deptId);
            ViewBag.StatusName = en.Status.FirstOrDefault(s => s.StatusId == "ST6").StatusName;
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", inc.LevelId);
            ViewBag.UserCreateName = en.Employees.FirstOrDefault(e => e.EmployeeID == inc.User_create)?.EmployeeName;
            ViewBag.UserResolveName = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp)?.EmployeeName;

            return View(inc);
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: /Incident/Delete

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
            catch (Exception) { }//MessageBox.Show("Lỗi: " + ex.ToString(), "Thông báo!"); }
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
                    "Where DepartmentId='" + department_id + "'\n" +
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

            Dictionary<string, string> Params = (Dictionary<string, string>)Session["Params"];

            bool IsAdmin = Convert.ToBoolean(Params["IsAdmin"]);
            string plants = Params["plants"];
            string solved = Params["solved"];
            DateTime from_date = Convert.ToDateTime(Params["from_date"]);
            DateTime to_date = Convert.ToDateTime(Params["to_date"]);

            if (session_emp != null)
            {
                IEnumerable<IncidentViewModel> inc = IncidentModel.Instance.get_IEnum_Inc();
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

                var lstInc = inc.Join(en.Departments,
                    i => new { plantId = i.Plant, DeptId = i.DepartmentId },
                    p => new { plantId = p.Plant_Id, DeptId = p.Department_Id },
                    (i, p) => new
                    {
                        i.Code,
                        Date = i.datetime,
                        Status = i.statusName,
                        Level = i.levelName,
                        Creator = i.userCreateName,
                        Solver = i.userResolveName,
                        i.Note,
                        i.Reply,
                        File = i.FileName,
                        Department = i.departmentName,
                        Plant = p.Plant_Name
                    }).ToList();

                var stream = ExcelHelper.Instance.CreateExcelFile(null, lstInc);
                var buffer = stream as MemoryStream;
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-Disposition", "attachment; filename=IT Order Request.xlsx");
                Response.BinaryWrite(buffer.ToArray());
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
