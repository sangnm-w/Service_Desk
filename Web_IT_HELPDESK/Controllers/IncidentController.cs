using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_IT_HELPDESK.Commons;
using Web_IT_HELPDESK.Controllers.ObjectManager;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.Models.Extensions;
using Web_IT_HELPDESK.ViewModels;

using EntityState = System.Data.Entity.EntityState;

namespace Web_IT_HELPDESK.Controllers
{
    public class IncidentController : Controller
    {
        public ServiceDeskEntities en { get; set; }
        public ApplicationUser _appUser { get; set; }
        public string currUserId { get; set; }
        public string currUserDeptId { get; set; }
        public string currUserPlantId { get; set; }

        private DateTime from_date { get; set; }
        private DateTime to_date { get; set; }

        public IncidentController()
        {
            if (System.Web.HttpContext.Current.Request.IsAuthenticated)
            {
                en = new ServiceDeskEntities();
                _appUser = new ApplicationUser();
                currUserId = _appUser.EmployeeID;
                currUserDeptId = _appUser.GetDepartmentID();
                currUserPlantId = _appUser.GetPlantID();
            }
        }

        // GET: /Incident/Index
        [CustomAuthorize]
        public ActionResult Index()
        {
            List<Rule> userRules = new List<Rule>();
            List<Role> userRoles = new List<Role>();
            List<PlantViewModel> userPlants = new List<PlantViewModel>();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            bool IsAdmin = _appUser.isAdmin;

            from_date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            to_date = from_date.AddMonths(1).AddSeconds(-1);

            var ivm = IncidentModel.Instance.get_IEnum_Inc();
            ivm = ivm.Where(i => i.Del != true && i.datetime >= from_date && i.datetime <= to_date);

            if (IsAdmin)
            {
                userRules = en.Rules
                    .Join(en.Modules, ru => ru.Module_ID, mo => mo.Module_ID, (ru, mo) => new { ru, mo })
                    .Where(grp => grp.ru.Deactive != true && grp.mo.Module_Name.ToUpper() == controllerName.ToUpper())
                    .Select(grp => grp.ru)
                    .ToList();

                userPlants = en.Plants
                    .Select(p => new PlantViewModel { Plant_Id = p.Plant_Id, Plant_Name = p.Plant_Name })
                    .ToList();
            }
            else
            {
                var userAuths = _appUser.GetAuthorizations();

                userRoles = userAuths.Join(en.Roles, au => au.Role_ID, ro => ro.Role_ID, (au, ro) => ro).Distinct().ToList();
                userRules = _appUser.GetRules(userRoles, controllerName);

                var uPlants = _appUser.GetAuthorizations()
               .Join(en.Plants, au => au.Plant_ID, p => p.Plant_Id, (au, p) => new { au, p }).Select(grp => new { grp.p.Plant_Id, grp.p.Plant_Name }).Distinct();

                userPlants = uPlants.Select(p => new PlantViewModel { Plant_Id = p.Plant_Id, Plant_Name = p.Plant_Name }).ToList();

                bool isITManager = userRoles.Any(ro => ro.Role_ID == 2);
                if (!isITManager)
                {
                    ivm = ivm.Where(i => userPlants.Select(p => p.Plant_Id).Contains(i.plantId));
                }

                bool isITMember = userRoles.Any(ro => ro.Role_ID == 3);
                if (!isITMember)
                {
                    ivm = ivm.Where(i => i.DepartmentId == currUserDeptId);
                }
            }

            ViewBag.userRules = userRules.Select(ru => ru.Rule_Name).ToList();
            ViewBag.userPlants = userPlants;

            Dictionary<string, string> Params = new Dictionary<string, string>();

            Params.Add("IsAdmin", IsAdmin.ToString());
            Params.Add("plants", "all");
            Params.Add("solved", "all");
            Params.Add("from_date", from_date.ToString());
            Params.Add("to_date", to_date.ToString());

            Session["Params"] = Params;
            return View(ivm);
        }

        // POST: /Incident/Index
        [HttpPost]
        [CustomAuthorize]
        public ActionResult Index(string solved, string _datetime, string plants)
        {
            List<Rule> userRules = new List<Rule>();
            List<Role> userRoles = new List<Role>();
            List<PlantViewModel> userPlants = new List<PlantViewModel>();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            bool IsAdmin = _appUser.isAdmin;

            IFormatProvider culture = new CultureInfo("en-US", true);
            from_date = DateTime.ParseExact(_datetime, "yyyy-MM", culture);
            to_date = from_date.AddMonths(1).AddSeconds(-1);

            var ivm = IncidentModel.Instance.get_IEnum_Inc();
            ivm = ivm.Where(i => i.Del != true && i.datetime >= from_date && i.datetime <= to_date);

            if (IsAdmin)
            {
                userRules = en.Rules
                    .Join(en.Modules, ru => ru.Module_ID, mo => mo.Module_ID, (ru, mo) => new { ru, mo })
                    .Where(grp => grp.ru.Deactive != true && grp.mo.Module_Name.ToUpper() == controllerName.ToUpper())
                    .Select(grp => grp.ru)
                    .ToList();

                userPlants = en.Plants
                    .Select(p => new PlantViewModel { Plant_Id = p.Plant_Id, Plant_Name = p.Plant_Name })
                    .ToList();
            }
            else
            {
                var userAuths = _appUser.GetAuthorizations();

                userRoles = userAuths.Join(en.Roles, au => au.Role_ID, ro => ro.Role_ID, (au, ro) => ro).Distinct().ToList();
                userRules = _appUser.GetRules(userRoles, controllerName);

                var uPlants = _appUser.GetAuthorizations()
               .Join(en.Plants, au => au.Plant_ID, p => p.Plant_Id, (au, p) => new { au, p }).Select(grp => new { grp.p.Plant_Id, grp.p.Plant_Name }).Distinct();

                userPlants = uPlants.Select(p => new PlantViewModel { Plant_Id = p.Plant_Id, Plant_Name = p.Plant_Name }).ToList();

                bool isITManager = userRoles.Any(ro => ro.Role_ID == 2);
                if (!isITManager)
                {
                    ivm = ivm.Where(i => userPlants.Select(p => p.Plant_Id).Contains(i.plantId));
                }

                bool isITMember = userRoles.Any(ro => ro.Role_ID == 3);
                if (!isITMember)
                {
                    ivm = ivm.Where(i => i.DepartmentId == currUserDeptId);
                }
            }

            if (plants != "all" && plants != null)
            {
                ivm = ivm.Where(i => i.plantId == plants);
            }

            if (solved != "all")
            {
                ivm = ivm.Where(i => i.Solved == Convert.ToBoolean(solved));
            }

            ViewBag.userRules = userRules.Select(ru => ru.Rule_Name).ToList();
            ViewBag.userPlants = userPlants;

            Dictionary<string, string> Params = new Dictionary<string, string>();

            Params.Add("IsAdmin", IsAdmin.ToString());
            Params.Add("plants", plants);
            Params.Add("solved", solved);
            Params.Add("from_date", from_date.ToString());
            Params.Add("to_date", to_date.ToString());

            Session["Params"] = Params;

            return View(ivm);
        }

        // GET: /Incident/Details
        [CustomAuthorize]
        public ActionResult Details(Guid? inc_id)
        {
            Incident inc = en.Incidents.Find(inc_id);

            ViewBag.LevelName = en.Levels.FirstOrDefault(l => l.LevelId == inc.LevelId).LevelName;
            ViewBag.StatusName = en.Status.FirstOrDefault(s => s.StatusId == inc.StatusId).StatusName;

            ViewBag.plantName = DepartmentModel.Instance.getPlantNameByDeptId(inc.DepartmentId);
            ViewBag.DepartmentName = DepartmentModel.Instance.getDeptNameByDeptId(inc.DepartmentId);

            ViewBag.User_Create = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == inc.User_create)?.Employee_Name;
            ViewBag.User_Resolve = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == inc.User_resolve)?.Employee_Name;
            return View(inc);
        }

        // GET: /Incident/Create
        [CustomAuthorize]
        public ActionResult Create()
        {
            Incident inc = new Incident();
            inc.Code = IncidentModel.Instance.Generate_IncidentCode(currUserPlantId);
            inc.User_create = currUserId;
            inc.DepartmentId = currUserDeptId;
            inc.StatusId = "ST1";

            ViewBag.UserCreateName = CurrentUser.Instance.User.Employee_Name;
            ViewBag.plantName = _appUser.GetPlantName();
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", en.Levels.First().LevelId);
            ViewBag.StatusName = en.Status.FirstOrDefault(s => s.StatusId == "ST1").StatusName;
            ViewBag.DepartmentName = DepartmentModel.Instance.getDeptNameByDeptId(currUserDeptId);
            return View(inc);
        }

        // POST: /Incident/Create
        [HttpPost]
        [CustomAuthorize]
        public ActionResult Create(Incident incident, HttpPostedFileBase attachment)
        {
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
                incident.Solved = false;
                en.Incidents.Add(incident);
                en.SaveChanges();

                /*1========== Sending Mail ==========================================================================================*/
                IncidentViewModel incEx = IncidentModel.Instance.get_Incident(incident.Id);

                List<string> toMails = new List<string>();

                toMails = IncidentModel.Instance.GetITMemberEmails(currUserPlantId);

                List<string> ccMails = new List<string>();

                string managerIdOfUser = en.Departments.Find(currUserDeptId).Manager_Id;
                if (!string.IsNullOrWhiteSpace(managerIdOfUser))
                {
                    string managerMail = en.Employee_New.Find(managerIdOfUser).Email;
                    if (!string.IsNullOrWhiteSpace(managerMail))
                        ccMails.Add(managerMail);
                }

                if (currUserPlantId != "V2090" && currUserPlantId != "V2010")
                    ccMails.Add("itgroup@cjvina.com");

                bool resultSend = IncidentHelper.Instance.Send_IncidentEmail(incEx, "CREATE", toMails, ccMails);
                /*1==================================================================================================================*/

                return RedirectToAction("Index", "Incident");
            }

            ViewBag.UserCreateName = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == currUserId).Employee_Name;
            ViewBag.plantName = DepartmentModel.Instance.getPlantNameByPlantId(currUserPlantId);
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", en.Levels.First().LevelId);
            ViewBag.StatusName = en.Status.FirstOrDefault(s => s.StatusId == "ST1").StatusName;
            ViewBag.DepartmentName = DepartmentModel.Instance.getDeptNameByDeptId(currUserDeptId);

            return View(incident);
        }

        // GET: /Incident/Edit
        [CustomAuthorize]
        public ActionResult Edit(Guid? inc_id)
        {
            Incident inc = en.Incidents.Find(inc_id);

            ViewBag.plantName = DepartmentModel.Instance.getPlantNameByDeptId(inc.DepartmentId);
            ViewBag.DepartmentName = DepartmentModel.Instance.getDeptNameByDeptId(inc.DepartmentId);
            ViewBag.StatusId = new SelectList(en.Status.Where(s => s.StatusId != "ST6" && s.StatusId != "ST2"), "StatusId", "StatusName", inc.StatusId);
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", inc.LevelId);
            ViewBag.UserCreateName = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == inc.User_create)?.Employee_Name;

            return View(inc);
        }

        // POST: /Incident/Edit
        [HttpPost]
        [CustomAuthorize]
        public ActionResult Edit(Incident inc, HttpPostedFileBase attachment)
        {

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
                        Binary binaryFile = new Binary(fileData);
                        //initialise object of FileDump LINQ to sql class passing values to be inserted

                        inc.FileAttched = binaryFile.ToArray();
                        inc.FileName = Path.GetFileName(Request.Files[upload].FileName);
                    }
                }

                inc.User_resolve = CurrentUser.Instance.User.Emp_CJ;

                en.Entry(inc).State = EntityState.Modified;
                en.SaveChanges();

                /*1========== Sending Mail ==========================================================================================*/
                IncidentViewModel incEx = IncidentModel.Instance.get_Incident(inc.Id);
                List<string> toMails = new List<string>();

                toMails = IncidentModel.Instance.GetITMemberEmails(currUserPlantId);

                List<string> ccMails = new List<string>();
                if (currUserPlantId != "V2090" && currUserPlantId != "V2010")
                    ccMails.Add("itgroup@cjvina.com");
                bool resultSend = IncidentHelper.Instance.Send_IncidentEmail(incEx, "EDIT", toMails, ccMails);
                /*1==================================================================================================================*/

                return RedirectToAction("Index", "Incident");
            }

            ViewBag.plantName = DepartmentModel.Instance.getPlantNameByDeptId(inc.DepartmentId);
            ViewBag.DepartmentName = DepartmentModel.Instance.getDeptNameByDeptId(inc.DepartmentId);
            ViewBag.StatusId = new SelectList(en.Status.Where(s => s.StatusId != "ST6" && s.StatusId != "ST2"), "StatusId", "StatusName", inc.StatusId);
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", inc.LevelId);
            ViewBag.UserCreateName = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == inc.User_create)?.Employee_Name;

            return View(inc);
        }

        // GET: /Incident/Solve
        [CustomAuthorize]
        public ActionResult Solve(Guid? inc_id)
        {

            Incident inc = en.Incidents.Find(inc_id);
            inc.StatusId = "ST6";

            ViewBag.plantName = DepartmentModel.Instance.getPlantNameByDeptId(inc.DepartmentId);
            ViewBag.DepartmentName = DepartmentModel.Instance.getDeptNameByDeptId(currUserDeptId);
            ViewBag.StatusName = en.Status.FirstOrDefault(s => s.StatusId == "ST6").StatusName;
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", inc.LevelId);
            ViewBag.UserCreateName = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == inc.User_create)?.Employee_Name;
            ViewBag.UserResolveName = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == currUserId)?.Employee_Name;

            return View(inc);
        }

        // POST: /Incident/Solve
        [HttpPost]
        [CustomAuthorize]
        public ActionResult Solve(Incident inc)
        {

            if (inc.StatusId == "ST6" && ModelState.IsValid)
            {
                inc.Solved = true;
                inc.Solve_datetime = DateTime.Now;
                inc.User_resolve = currUserId;

                en.Entry(inc).State = EntityState.Modified;
                en.SaveChanges();

                /*1========== Sending Mail ==========================================================================================*/
                IncidentViewModel incEx = IncidentModel.Instance.get_Incident(inc.Id);
                List<string> toMails = new List<string>() { en.Employee_New.FirstOrDefault(e => e.Emp_CJ == inc.User_create).Email };

                List<string> ccMails = new List<string>();

                ccMails = IncidentModel.Instance.GetITMemberEmails(currUserPlantId);

                string managerIdOfUser = en.Departments.Find(currUserDeptId).Manager_Id;
                if (!string.IsNullOrWhiteSpace(managerIdOfUser))
                {
                    string managerMail = en.Employee_New.Find(managerIdOfUser).Email;
                    if (!string.IsNullOrWhiteSpace(managerMail) && !ccMails.Contains(managerMail))
                        ccMails.Add(managerMail);

                }

                if (currUserPlantId != "V2090" && currUserPlantId != "V2010")
                    ccMails.Add("itgroup@cjvina.com");

                bool resultSend = IncidentHelper.Instance.Send_IncidentEmail(incEx, "SOLVE", toMails, ccMails);
                /*1==================================================================================================================*/

                return RedirectToAction("Index", "Incident");
            }

            ViewBag.plantName = DepartmentModel.Instance.getPlantNameByDeptId(inc.DepartmentId);
            ViewBag.DepartmentName = DepartmentModel.Instance.getDeptNameByDeptId(currUserDeptId);
            ViewBag.StatusName = en.Status.FirstOrDefault(s => s.StatusId == "ST6").StatusName;
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", inc.LevelId);
            ViewBag.UserCreateName = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == inc.User_create)?.Employee_Name;
            ViewBag.UserResolveName = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == currUserId)?.Employee_Name;

            return View(inc);
        }

        // GET: /Incident/Delete
        [CustomAuthorize]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: /Incident/Delete
        [HttpPost]
        [CustomAuthorize]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult Get_file(Guid? inc_id)
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

        [CustomAuthorize]
        public ActionResult Download()
        {

            Dictionary<string, string> Params = (Dictionary<string, string>)Session["Params"];

            bool IsAdmin = Convert.ToBoolean(Params["IsAdmin"]);
            string plants = Params["plants"];
            string solved = Params["solved"];
            DateTime from_date = Convert.ToDateTime(Params["from_date"]);
            DateTime to_date = Convert.ToDateTime(Params["to_date"]);

            if (currUserId != null)
            {
                IEnumerable<IncidentViewModel> inc = IncidentModel.Instance.get_IEnum_Inc();
                if (IsAdmin == true)
                {
                    inc = inc.Where(i => i.Del != true
                                    && i.datetime >= from_date
                                    && i.datetime <= to_date);

                    if (plants != "all")
                    {
                        inc = inc.Where(i => i.plantId == plants).OrderByDescending(i => i.Code);
                    }
                    if (solved != "all")
                    {
                        inc = inc.Where(i => i.Solved == Convert.ToBoolean(solved));
                    }
                }
                else
                {
                    inc = inc.Where(i => i.Del != true
                                    && i.User_create == currUserId
                                    && i.datetime >= from_date
                                    && i.datetime <= to_date
                                    && i.plantId == currUserPlantId);

                    if (Convert.ToBoolean(solved))
                    {
                        inc = inc.Where(i => i.Solved == true);
                    }
                    else
                    {
                        inc = inc.Where(i => i.Solved != true);
                    }
                }

                var incList = inc.Join(en.Departments, i => i.DepartmentId, d => d.Department_Id, (i, d) => new { i, d })
                    .Join(en.Plants, grp => grp.d.Plant_Id, p => p.Plant_Id, (grp, p) => new
                    {
                        grp.i.Code,
                        Date = grp.i.datetime,
                        Status = grp.i.statusName,
                        Level = grp.i.levelName,
                        Creator = grp.i.userCreateName,
                        Solver = grp.i.userResolveName,
                        grp.i.Note,
                        grp.i.Reply,
                        File = grp.i.FileName,
                        Department = grp.d.Department_Name,
                        Plant = p.Plant_Name
                    });

                //Col need format date
                List<int> colsDate = new List<int>()
                {
                    2
                };

                if (incList.Count() > 0)
                {
                    var stream = ExcelHelper.Instance.CreateExcelFile(null, incList.ToList(), ExcelTitle.Instance.Incidents(), colsDate);
                    var buffer = stream as MemoryStream;
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", "attachment; filename=IT Order Request.xlsx");
                    Response.BinaryWrite(buffer.ToArray());
                    Response.Flush();
                    Response.End();
                }
                return RedirectToAction("Index");
            }
            else return RedirectToAction("LogOn", "LogOn");
        }
    }
}
