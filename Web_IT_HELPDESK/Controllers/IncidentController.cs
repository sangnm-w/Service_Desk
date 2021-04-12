using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_IT_HELPDESK.Controllers.ObjectManager;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.Models.Extensions;
using Web_IT_HELPDESK.ViewModels;

using EntityState = System.Data.Entity.EntityState;

namespace Web_IT_HELPDESK.Controllers
{
    public class IncidentController : Controller
    {
        private ServiceDeskEntities en = new ServiceDeskEntities();
        private string curr_EmployeeID = ApplicationUser.Instance.EmployeeID;
        private string curr_DeptID = ApplicationUser.Instance.GetDepartmentID();
        private string curr_PlantID = ApplicationUser.Instance.GetPlantID();

        private DateTime from_date { get; set; }
        private DateTime to_date { get; set; }

        // GET: /Incident/Index
        [CustomAuthorize("End User", "IT Manager", "IT Staff")]
        public ActionResult Index()
        {
            List<Rule> userRules = new List<Rule>();
            List<PlantViewModel> userPlants = new List<PlantViewModel>();

            bool IsAdmin = ApplicationUser.Instance.isAdmin;
            ViewBag.IsAdmin = IsAdmin;

            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            if (IsAdmin)
            {
                //userRules = en.Rules.Join(en.Modules, ru => ru.Module_ID, mo => mo.Module_ID, (ru, mo) => ru).Where(grp => grp.ru.Deactive != true && grp.mo.Module_Name.ToUpper() == controllerName).ToList();

                userPlants = en.Plants.Select(p => new PlantViewModel
                {
                    Plant_Id = p.Plant_ID,
                    Plant_Name = p.Plant_Name
                }).ToList();
                ViewBag.Plants = userPlants;
            }
            else
            {
                // add moduleID ~ controller name as params
                userRules = ApplicationUser.Instance.GetRules(null, controllerName);
                ViewBag.userRules = userRules;

                userPlants = ApplicationUser.Instance.GetAuthorizations()
                   .Join(en.Plants, au => au.Plant_ID, p => p.Plant_ID,
                   (au, p) => new PlantViewModel
                   {
                       Plant_Id = p.Plant_ID,
                       Plant_Name = p.Plant_Name
                   })
                   .Distinct().ToList();
                ViewBag.Plants = userPlants;
            }

            from_date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            to_date = from_date.AddMonths(1).AddSeconds(-1);

            IEnumerable<IncidentViewModel> ivm = IncidentModel.Instance.get_IEnum_Inc();

            ivm = ivm.Where(item => item.Del != true
                                        && item.datetime >= from_date
                                        && item.datetime <= to_date);

            List<string> userPlantsId = userPlants.Select(p => p.Plant_Id).ToList();
            if (IsAdmin == false) // Not Admin
            {
                ivm = ivm.Where(i => i.Del != true
                               && i.User_create == curr_EmployeeID
                               && userPlantsId.Contains(i.Plant));
            }

            Dictionary<string, string> Params = new Dictionary<string, string>();

            Params.Add("IsAdmin", IsAdmin.ToString());
            Params.Add("plants", "all");
            Params.Add("solved", "all");
            Params.Add("from_date", from_date.ToString());
            Params.Add("to_date", to_date.ToString());

            Session["Params"] = Params;
            ivm = ivm.OrderByDescending(i => i.Code);
            return View(ivm);
        }

        // POST: /Incident/Index
        [Authorize]
        [HttpPost]
        public ActionResult Index(string solved, string _datetime, string plants)
        {
            List<Rule> userRules = new List<Rule>();
            List<PlantViewModel> userPlants = new List<PlantViewModel>();

            bool IsAdmin = ApplicationUser.Instance.isAdmin;
            ViewBag.IsAdmin = IsAdmin;

            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            if (IsAdmin)
            {
                //userRules = en.Rules.Join(en.Modules, ru => ru.Module_ID, mo => mo.Module_ID, (ru, mo) => ru).Where(grp => grp.ru.Deactive != true && grp.mo.Module_Name.ToUpper() == controllerName).ToList();

                userPlants = en.Plants.Select(p => new PlantViewModel
                {
                    Plant_Id = p.Plant_ID,
                    Plant_Name = p.Plant_Name
                }).ToList();
                ViewBag.Plants = userPlants;
            }
            else
            {
                // add moduleID ~ controller name as params
                userRules = ApplicationUser.Instance.GetRules(null, controllerName);
                ViewBag.userRules = userRules;

                userPlants = ApplicationUser.Instance.GetAuthorizations()
                   .Join(en.Plants, au => au.Plant_ID, p => p.Plant_ID,
                   (au, p) => new PlantViewModel
                   {
                       Plant_Id = p.Plant_ID,
                       Plant_Name = p.Plant_Name
                   })
                   .Distinct().ToList();
                ViewBag.Plants = userPlants;
            }

            IFormatProvider culture = new CultureInfo("en-US", true);
            from_date = DateTime.ParseExact(_datetime, "yyyy-MM", culture);
            to_date = from_date.AddMonths(1).AddSeconds(-1);

            IEnumerable<IncidentViewModel> inc = IncidentModel.Instance.get_IEnum_Inc();

            inc = inc.Where(item => item.Del != true
                                            && item.datetime >= from_date
                                            && item.datetime <= to_date);

            List<string> userPlantsId = userPlants.Select(p => p.Plant_Id).ToList();
            if (IsAdmin == false)
            {
                inc = inc.Where(i => i.Del != true
                               && i.User_create == curr_EmployeeID
                               && userPlantsId.Contains(i.Plant));
            }

            if (plants != "all" && plants != null)
            {
                inc = inc.Where(i => i.Plant == plants);
            }
            if (solved != "all")
            {
                inc = inc.Where(i => i.Solved == Convert.ToBoolean(solved));
            }


            Dictionary<string, string> Params = new Dictionary<string, string>();

            Params.Add("IsAdmin", IsAdmin.ToString());
            Params.Add("plants", plants);
            Params.Add("solved", solved);
            Params.Add("from_date", from_date.ToString());
            Params.Add("to_date", to_date.ToString());

            Session["Params"] = Params;

            return View(inc);
        }

        // GET: /Incident/Details
        [CustomAuthorize]
        public ActionResult Details(Guid? inc_id)
        {
            Incident inc = en.Incidents.Find(inc_id);

            ViewBag.LevelName = en.Levels.FirstOrDefault(l => l.LevelId == inc.LevelId).LevelName;
            ViewBag.StatusName = en.Status.FirstOrDefault(s => s.StatusId == inc.StatusId).StatusName;

            ViewBag.plantName = DepartmentModel.Instance.getPlantName(inc.Plant);
            ViewBag.DepartmentName = DepartmentModel.Instance.getDeptName(inc.DepartmentId);

            ViewBag.User_Create = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == inc.User_create)?.Employee_Name;
            ViewBag.User_Resolve = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == inc.User_resolve)?.Employee_Name;
            return View(inc);
        }

        // GET: /Incident/Create
        [CustomAuthorize]
        public ActionResult Create()
        {
            Incident inc = new Incident();
            inc.Code = IncidentModel.Instance.Generate_IncidentCode(curr_PlantID);
            inc.User_create = curr_EmployeeID;
            inc.Plant = curr_PlantID;
            inc.DepartmentId = curr_DeptID;
            inc.StatusId = "ST1";

            ViewBag.UserCreateName = CurrentUser.Instance.User.Employee_Name;
            ViewBag.plantName = en.Plants.FirstOrDefault(d => d.Plant_ID == curr_PlantID).Plant_Name;
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", en.Levels.First().LevelId);
            ViewBag.StatusName = en.Status.FirstOrDefault(s => s.StatusId == "ST1").StatusName;
            ViewBag.DepartmentName = DepartmentModel.Instance.getDeptName(curr_DeptID);
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

                toMails = IncidentModel.Instance.GetITMemberEmails(curr_PlantID);

                List<string> ccMails = new List<string>();

                string managerIdOfUser = en.Departments.Find(curr_DeptID).Manager_ID;
                string managerMail = en.Employee_New.Find(managerIdOfUser).Email;
                if (managerMail != null)
                    ccMails.Add(managerMail);

                if (curr_PlantID != "V2090" && curr_PlantID != "V2010")
                    ccMails.Add("itgroup@cjvina.com");

                bool resultSend = IncidentHelper.Instance.Send_IncidentEmail(incEx, "CREATE", toMails, ccMails);
                /*1==================================================================================================================*/

                return RedirectToAction("Index", "Incident");
            }

            ViewBag.UserCreateName = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == curr_EmployeeID).Employee_Name;
            ViewBag.plantName = DepartmentModel.Instance.getPlantName(curr_PlantID);
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", en.Levels.First().LevelId);
            ViewBag.StatusName = en.Status.FirstOrDefault(s => s.StatusId == "ST1").StatusName;
            ViewBag.DepartmentName = DepartmentModel.Instance.getDeptName(curr_DeptID);

            return View(incident);
        }

        // GET: /Incident/Edit
        [CustomAuthorize]
        public ActionResult Edit(Guid? inc_id)
        {
            Incident inc = en.Incidents.Find(inc_id);

            ViewBag.plantName = DepartmentModel.Instance.getPlantName(inc.Plant);
            ViewBag.DepartmentName = DepartmentModel.Instance.getDeptName(inc.DepartmentId);
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
            //bool? hasPermission = CurrentUser.Instance.hasPermission(Commons.ActionConstant.EDIT, Commons.ModuleConstant.INCIDENT);
            //if (hasPermission.Value == false)
            //    return RedirectToAction("Index", "Home");

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

                en.Entry(inc).State = EntityState.Modified;
                en.SaveChanges();

                /*1========== Sending Mail ==========================================================================================*/
                IncidentViewModel incEx = IncidentModel.Instance.get_Incident(inc.Id);
                List<string> toMails = new List<string>();

                toMails = IncidentModel.Instance.GetITMemberEmails(curr_PlantID);

                List<string> ccMails = new List<string>();
                if (curr_PlantID != "V2090" && curr_PlantID != "V2010")
                    ccMails.Add("itgroup@cjvina.com");
                bool resultSend = IncidentHelper.Instance.Send_IncidentEmail(incEx, "EDIT", toMails, ccMails);
                /*1==================================================================================================================*/

                return RedirectToAction("Index", "Incident");
            }

            ViewBag.plantName = DepartmentModel.Instance.getPlantName(inc.Plant);
            ViewBag.DepartmentName = DepartmentModel.Instance.getDeptName(inc.DepartmentId);
            ViewBag.StatusId = new SelectList(en.Status.Where(s => s.StatusId != "ST6" && s.StatusId != "ST2"), "StatusId", "StatusName", inc.StatusId);
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", inc.LevelId);
            //ViewBag.UserCreateName = en.Employees.FirstOrDefault(e => e.Emp_CJ == inc.User_create)?.EmployeeName;
            ViewBag.UserCreateName = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == inc.User_create)?.Employee_Name;

            return View(inc);
        }

        // GET: /Incident/Solve
        [CustomAuthorize]
        public ActionResult Solve(Guid? inc_id)
        {
            //bool? hasPermission = CurrentUser.Instance.hasPermission(Commons.ActionConstant.SOLVE, Commons.ModuleConstant.INCIDENT);
            //if (hasPermission.Value == false)
            //    return RedirectToAction("Index", "Home");

            Incident inc = en.Incidents.Find(inc_id);
            inc.StatusId = "ST6";

            ViewBag.plantName = DepartmentModel.Instance.getPlantName(inc.Plant);
            ViewBag.DepartmentName = DepartmentModel.Instance.getDeptName(curr_DeptID);
            ViewBag.StatusName = en.Status.FirstOrDefault(s => s.StatusId == "ST6").StatusName;
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", inc.LevelId);
            ViewBag.UserCreateName = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == inc.User_create)?.Employee_Name;
            ViewBag.UserResolveName = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == curr_EmployeeID)?.Employee_Name;

            return View(inc);
        }

        // POST: /Incident/Solve
        [HttpPost]
        [CustomAuthorize]
        public ActionResult Solve(Incident inc)
        {
            //bool? hasPermission = CurrentUser.Instance.hasPermission(Commons.ActionConstant.SOLVE, Commons.ModuleConstant.INCIDENT);
            //if (hasPermission.Value == false)
            //    return RedirectToAction("Index", "Home");

            if (inc.StatusId == "ST6" && ModelState.IsValid)
            {
                inc.Solved = true;
                inc.Solve_datetime = DateTime.Now;
                inc.User_resolve = curr_EmployeeID;

                en.Entry(inc).State = EntityState.Modified;
                en.SaveChanges();

                /*1========== Sending Mail ==========================================================================================*/
                IncidentViewModel incEx = IncidentModel.Instance.get_Incident(inc.Id);
                //List<string> toMails = new List<string>() { en.Employees.FirstOrDefault(e => e.Emp_CJ == inc.User_create).Email };
                List<string> toMails = new List<string>() { en.Employee_New.FirstOrDefault(e => e.Emp_CJ == inc.User_create).Email };

                List<string> ccMails = new List<string>();

                ccMails = IncidentModel.Instance.GetITMemberEmails(curr_PlantID);

                string managerIdOfUser = en.Departments.Find(curr_DeptID).Manager_ID;
                string managerMail = en.Employee_New.Find(managerIdOfUser).Email;
                if (managerMail != null && !ccMails.Contains(managerMail))
                    ccMails.Add(managerMail);

                if (curr_PlantID != "V2090" && curr_PlantID != "V2010")
                    ccMails.Add("itgroup@cjvina.com");

                bool resultSend = IncidentHelper.Instance.Send_IncidentEmail(incEx, "SOLVE", toMails, ccMails);
                /*1==================================================================================================================*/

                return RedirectToAction("Index", "Incident");
            }

            ViewBag.plantName = DepartmentModel.Instance.getPlantName(inc.Plant);
            ViewBag.DepartmentName = DepartmentModel.Instance.getDeptName(curr_DeptID);
            ViewBag.StatusName = en.Status.FirstOrDefault(s => s.StatusId == "ST6").StatusName;
            ViewBag.LevelId = new SelectList(en.Levels, "LevelId", "LevelName", inc.LevelId);
            ViewBag.UserCreateName = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == inc.User_create)?.Employee_Name;
            ViewBag.UserResolveName = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == curr_EmployeeID)?.Employee_Name;

            return View(inc);
        }

        // GET: /Incident/Delete
        [CustomAuthorize]
        public ActionResult Delete(int id)
        {
            //bool? hasPermission = CurrentUser.Instance.hasPermission(Commons.ActionConstant.DELETE, Commons.ModuleConstant.INCIDENT);
            //if (hasPermission.Value == false)
            //    return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: /Incident/Delete
        [HttpPost]
        [CustomAuthorize]
        public ActionResult Delete(int id, FormCollection collection)
        {
            //bool? hasPermission = CurrentUser.Instance.hasPermission(Commons.ActionConstant.DELETE, Commons.ModuleConstant.INCIDENT);
            //if (hasPermission.Value == false)
            //    return RedirectToAction("Index", "Home");

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
            //bool? hasPermission = CurrentUser.Instance.hasPermission(Commons.ActionConstant.DOWNLOAD, Commons.ModuleConstant.INCIDENT);
            //if (hasPermission.Value == false)
            //    return RedirectToAction("Index", "Home");

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
            //bool? hasPermission = CurrentUser.Instance.hasPermission(Commons.ActionConstant.DOWNLOAD, Commons.ModuleConstant.INCIDENT);
            //if (hasPermission.Value == false)
            //    return RedirectToAction("Index", "Home");

            Dictionary<string, string> Params = (Dictionary<string, string>)Session["Params"];

            bool IsAdmin = Convert.ToBoolean(Params["IsAdmin"]);
            string plants = Params["plants"];
            string solved = Params["solved"];
            DateTime from_date = Convert.ToDateTime(Params["from_date"]);
            DateTime to_date = Convert.ToDateTime(Params["to_date"]);

            if (curr_EmployeeID != null)
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
                                    && i.User_create == curr_EmployeeID
                                    && i.datetime >= from_date
                                    && i.datetime <= to_date
                                    && i.Plant == curr_PlantID);

                    if (Convert.ToBoolean(solved))
                    {
                        inc = inc.Where(i => i.Solved == true);
                    }
                    else
                    {
                        inc = inc.Where(i => i.Solved != true);
                    }
                }

                var lstInc = from i in inc
                             join d in en.Departments on i.DepartmentId equals d.Department_ID
                             join p in en.Plants on d.Plant_ID equals p.Plant_ID
                             select new
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
                             };

                //var lstInc = inc.Join(en.Departments,
                //    i => i.DepartmentId ,
                //    p =>  p.Department_ID ,
                //    (i, p) => new
                //    {
                //        i.Code,
                //        Date = i.datetime,
                //        Status = i.statusName,
                //        Level = i.levelName,
                //        Creator = i.userCreateName,
                //        Solver = i.userResolveName,
                //        i.Note,
                //        i.Reply,
                //        File = i.FileName,
                //        Department = i.departmentName,
                //        Plant = p.Plant_Name
                //    }).ToList();

                //Col need format date
                List<int> colsDate = new List<int>()
                {
                    2
                };

                if (lstInc.Count() > 0)
                {
                    var stream = ExcelHelper.Instance.CreateExcelFile(null, lstInc.ToList(), ExcelTitle.Instance.Incidents(), colsDate);
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
