using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Web_IT_HELPDESK;
using Web_IT_HELPDESK.Controllers.ObjectManager;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.Models.Extensions;
using Web_IT_HELPDESK.Properties;
using Web_IT_HELPDESK.ViewModels;
using Web_IT_HELPDESK.ViewModels.Mailing;
using Excel = Microsoft.Office.Interop.Excel;

namespace Web_IT_HELPDESK.Controllers
{
    public class MailingController : Controller
    {
        private ServiceDeskEntities en { get; set; }
        private ApplicationUser _appUser { get; set; }
        private string currUserId { get; set; }
        private string currUserDeptId { get; set; }
        private string currUserPlantId { get; set; }

        public MailingController()
        {
            en = new ServiceDeskEntities();
            _appUser = new ApplicationUser();
            currUserId = _appUser.EmployeeID;
            currUserDeptId = _appUser.GetDepartmentID();
            currUserPlantId = _appUser.GetPlantID();
        }

        // GET: Mailing
        public ActionResult Index()
        {
            List<Rule> userRules = new List<Rule>();
            List<Role> userRoles = new List<Role>();
            List<Plant> userPlants = new List<Plant>();
            IEnumerable<MailingIndexViewModel> model = null;
            string controllerName = ControllerContext.RouteData.Values["controller"].ToString();
            bool IsAdmin = _appUser.isAdmin;
            bool IsManager = _appUser.IsManager;

            DateTime fromDate = DateTime.Now;
            DateTime toDate = fromDate.AddMonths(1).AddSeconds(-1);

            var mails = en.Mails.Include(m => m.Employee)
                .Where(m => m.Inactive != true)
                .Where(m => m.SendingDate >= fromDate && m.SendingDate <= toDate)
                .Where(m => m.Employee.Department.Plant_Id == currUserPlantId);

            if (IsAdmin)
            {
                userRules = en.Rules
                    .Join(en.Modules, ru => ru.Module_ID, mo => mo.Module_ID, (ru, mo) => new { ru, mo })
                    .Where(grp => grp.ru.Deactive != true && grp.mo.Module_Name.ToUpper() == controllerName.ToUpper())
                    .Select(grp => grp.ru)
                    .ToList();

                userPlants = en.Plants
                    .Select(p => new Plant { Plant_Id = p.Plant_Id, Plant_Name = p.Plant_Name })
                    .ToList();
            }
            else
            {
                userRoles = _appUser.GetRolesByModuleName(controllerName);
                userRules = _appUser.GetRules(userRoles, controllerName);
                userPlants = _appUser.GetAuthoPlantsByModuleName(controllerName);

                mails = mails.Where(m => m.Employee.Department_ID == currUserDeptId);

                if (!IsManager)
                {
                    mails = mails.Where(m => m.EmployeeId == currUserId);
                }
            }

            ViewBag.userRules = userRules.Select(ru => ru.Rule_Name).ToList();
            ViewBag.userPlants = userPlants;

            model = mails.Select(m => new MailingIndexViewModel
            {
                MailID = m.MailID,
                MailTitle = m.MailTitle,
                FromAddress = m.FromAddress,
                ToAddress = m.ToAddress,
                CcAddress = m.CcAddress,
                Attachment = m.Attachment,
                EmployeeName = m.Employee.Employee_Name,
                SendingDate = m.SendingDate,
                SendingStatus = m.SendingStatus,
                DepartmentName = m.Employee.Department.Department_Name
            });

            return View(model.ToList());
        }

        // POST: Mailing
        [HttpPost]
        public ActionResult Index(DateTime? fromDate, DateTime? toDate, string plants, string keyword)
        {
            List<Rule> userRules = new List<Rule>();
            List<Role> userRoles = new List<Role>();
            List<Plant> userPlants = new List<Plant>();
            IEnumerable<MailingIndexViewModel> model = null;
            string controllerName = ControllerContext.RouteData.Values["controller"].ToString();
            bool IsAdmin = _appUser.isAdmin;
            bool IsManager = _appUser.IsManager;

            var mails = en.Mails.Include(m => m.Employee)
                .Where(m => m.Inactive != true);

            if (fromDate != null && toDate != null)
            {
                mails = mails.Where(m => m.SendingDate >= fromDate && m.SendingDate <= toDate);
            }

            if (IsAdmin)
            {
                userRules = en.Rules
                    .Join(en.Modules, ru => ru.Module_ID, mo => mo.Module_ID, (ru, mo) => new { ru, mo })
                    .Where(grp => grp.ru.Deactive != true && grp.mo.Module_Name.ToUpper() == controllerName.ToUpper())
                    .Select(grp => grp.ru)
                    .ToList();

                userPlants = en.Plants
                    .Select(p => new Plant { Plant_Id = p.Plant_Id, Plant_Name = p.Plant_Name })
                    .ToList();

                if (plants != "all" && plants != null)
                {
                    mails = mails.Where(m => m.Employee.Department.Plant_Id == plants);
                }
            }
            else
            {
                userRoles = _appUser.GetRolesByModuleName(controllerName);
                userRules = _appUser.GetRules(userRoles, controllerName);
                userPlants = _appUser.GetAuthoPlantsByModuleName(controllerName);

                mails = mails.Where(m => m.Employee.Department_ID == currUserDeptId);

                if (!IsManager)
                {
                    mails = mails.Where(m => m.EmployeeId == currUserId);
                }
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                mails = mails.Where(m => m.MailTitle.Contains(keyword));
            }

            ViewBag.userRules = userRules.Select(ru => ru.Rule_Name).ToList();
            ViewBag.userPlants = userPlants;

            model = mails.Select(m => new MailingIndexViewModel
            {
                MailID = m.MailID,
                MailTitle = m.MailTitle,
                FromAddress = m.FromAddress,
                ToAddress = m.ToAddress,
                CcAddress = m.CcAddress,
                Attachment = m.Attachment,
                EmployeeName = m.Employee.Employee_Name,
                SendingDate = m.SendingDate,
                SendingStatus = m.SendingStatus,
                DepartmentName = m.Employee.Department.Department_Name
            });

            return View(model.ToList());
        }

        // GET: Mailing/Create
        public ActionResult Create()
        {
            List<Plant> plants = new List<Plant>();
            Plant p = new Plant() { Plant_Id = "", Plant_Name = "All" };
            Plant bod = new Plant() { Plant_Id = "BOD", Plant_Name = "BOD" };
            Plant t = new Plant() { Plant_Id = "Test", Plant_Name = "Test" };
            plants.Add(p);
            plants.Add(bod);
            plants.AddRange(en.Plants);
            plants.Add(t);
            SelectList plantSL = new SelectList(plants, "Plant_ID", "Plant_Name");

            List<DepartmentViewModel> deptVMs = new List<DepartmentViewModel>();
            DepartmentViewModel dVMs = new DepartmentViewModel() { Department_Id = "", Department_Name = "All" };
            deptVMs.Add(dVMs);
            deptVMs.AddRange(GetDepartmentVMsByPlantId(currUserPlantId));
            SelectList deptSL = new SelectList(deptVMs, "Department_Id", "Department_Name");

            SelectList positions = new SelectList(new List<MailingPositionViewModel>()
            {
                new MailingPositionViewModel()
                {
                    PositionId = "",
                    PositionName = "All"
                },
                new MailingPositionViewModel()
                {
                    PositionId = "1",
                    PositionName = "Head of Department"
                },
                new MailingPositionViewModel()
                {
                    PositionId = "2",
                    PositionName = "Manager"
                },
                new MailingPositionViewModel()
                {
                    PositionId = "3",
                    PositionName = "Sales"
                }
            }, "PositionId", "PositionName");

            IEnumerable<MailingEmailsViewModel> initialEmails = GetEmail(currUserPlantId, null, null);
            //IEnumerable<MailingEmailsViewModel> initialEmails = new List<MailingEmailsViewModel>();

            MailingCreateViewModel model = new MailingCreateViewModel()
            {
                Plants = plantSL,
                Departments = deptSL,
                Positions = positions,
                PlantId = currUserPlantId,
                DepartmentId = null,
                PositionId = null,
                InitialEmployees = initialEmails,
                Receivers = new List<MailingEmailsViewModel>(),
                EmployeeName = "",
                Email = "",
                MailTitle = "",
                MailContent = "",
                FromAddress = "",
                ToAddress = "",
                Attachments = null,
                MailPicture = "",
                EmployeeId = "",
                SenderPW = ""
            };

            return View(model);
        }

        // POST: Mailing/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(MailingCreateViewModel model)
        {
            string sender = model.FromAddress;
            string senderPW = model.SenderPW;
            string receiver = "it-servicedesk@cjvina.com";
            string subject = "Async Subject for testing";
            string body = "Async Body for testing";
            List<MailingEmailsViewModel> receivers = new List<MailingEmailsViewModel>();

            if (!string.IsNullOrEmpty(model.ToAddress))
            {
                receivers = JsonConvert.DeserializeObject<List<MailingEmailsViewModel>>(model.ToAddress);
            }
            else
            {
                ModelState.AddModelError("ReceiverError", "The receiver table is empty");
            }

            try
            {
                await MailHelper.SendEmail(sender, senderPW, new List<string>() { receiver }, subject, body, null);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("SenderError", ex.Message);
            }

            if (ModelState.IsValid)
            {
                DataTable errTb = new DataTable();
                errTb.Columns.Add("errMail", typeof(string));
                errTb.Columns.Add("errMessage", typeof(string));

                List<string> receiverEmails = new List<string>();
                receiverEmails.AddRange(receivers.Select(x => x.Email).ToList());

                await MailHelper.SendEmail(sender, senderPW, receiverEmails, model.MailTitle, model.MailContent, model.Attachments);
            }

            List<Plant> plants = new List<Plant>();
            Plant p = new Plant() { Plant_Id = "", Plant_Name = "All" };
            Plant bod = new Plant() { Plant_Id = "BOD", Plant_Name = "BOD" };
            Plant t = new Plant() { Plant_Id = "Test", Plant_Name = "Test" };
            plants.Add(p);
            plants.Add(bod);
            plants.AddRange(en.Plants);
            plants.Add(t);
            SelectList plantSL = new SelectList(plants, "Plant_ID", "Plant_Name");

            List<DepartmentViewModel> deptVMs = new List<DepartmentViewModel>();
            DepartmentViewModel dVMs = new DepartmentViewModel() { Department_Id = "", Department_Name = "All" };
            deptVMs.Add(dVMs);
            deptVMs.AddRange(GetDepartmentVMsByPlantId(model.PlantId));
            SelectList deptSL = new SelectList(deptVMs, "Department_Id", "Department_Name");

            SelectList positions = new SelectList(new List<MailingPositionViewModel>()
            {
                new MailingPositionViewModel()
                {
                    PositionId = "",
                    PositionName = "All"
                },
                new MailingPositionViewModel()
                {
                    PositionId = "1",
                    PositionName = "Head of Department"
                },
                new MailingPositionViewModel()
                {
                    PositionId = "2",
                    PositionName = "Manager"
                },
                new MailingPositionViewModel()
                {
                    PositionId = "3",
                    PositionName = "Sales"
                }
            }, "PositionId", "PositionName");

            //IEnumerable<MailingEmailsViewModel> initialEmails = GetEmails(model.PlantId, null, null);
            IEnumerable<MailingEmailsViewModel> initialEmails = new List<MailingEmailsViewModel>();

            //model = new MailingCreateViewModel()
            //{
            //    Plants = plantSL,
            //    Departments = deptSL,
            //    Positions = positions,
            //    PlantId = currUserPlantId,
            //    DepartmentId = null,
            //    PositionId = null,
            //    InitialEmployees = initialEmails,
            //    Receivers = receivers,
            //    EmployeeName = "",
            //    Email = "",
            //    MailTitle = "",
            //    MailContent = "",
            //    FromAddress = "",
            //    ToAddress = "",
            //    Attachments = null,
            //    MailPicture = "",
            //    EmployeeId = "",
            //    SenderPW = ""
            //};

            model.Plants = plantSL;
            model.Departments = deptSL;
            model.Positions = positions;
            model.InitialEmployees = initialEmails;
            model.Receivers = receivers;

            //HttpPostedFileBase Attachments = null;

            //if (Session["attachments"] == null && Attachments.ContentLength > 0)
            //{
            //    Session["attachments"] = ;
            //}
            //else if (Session["attachments"] != null && (!Attachments.HasFile))
            //{
            //    Attachments = (FileUpload)Session["attachments"];

            //}

            //else if (Attachments.HasFile)
            //{
            //    Session["attachments"] = Attachments.PostedFile.FileName;

            //}

            return View(model);
        }

        // GET: Mailing/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mail mail = en.Mails.Find(id);
            if (mail == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeId = new SelectList(en.Employees, "Emp_CJ", "Emp_ID", mail.EmployeeId);
            return View(mail);
        }

        // POST: Mailing/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MailID,MailTitle,MailContent,FromAddress,ToAddress,CcAddress,BccAddress,Attachment,MailPicture,EmployeeId,SendingDate,SendingStatus")] Mail mail)
        {
            if (ModelState.IsValid)
            {
                en.Entry(mail).State = EntityState.Modified;
                en.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeId = new SelectList(en.Employees, "Emp_CJ", "Emp_ID", mail.EmployeeId);
            return View(mail);
        }

        // GET: Mailing/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mail mail = en.Mails.Find(id);
            if (mail == null)
            {
                return HttpNotFound();
            }
            return View(mail);
        }

        // POST: Mailing/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Mail mail = en.Mails.Find(id);
            en.Mails.Remove(mail);
            en.SaveChanges();
            return RedirectToAction("Index");
        }

        public PartialViewResult UpdateDepartmentDDLByPlant(string plantId)
        {
            MailingCreateViewModel model = new MailingCreateViewModel();
            List<DepartmentViewModel> deptVMs = new List<DepartmentViewModel>();
            DepartmentViewModel dVMs = new DepartmentViewModel() { Department_Id = "", Department_Name = "All" };
            deptVMs.Add(dVMs);
            deptVMs.AddRange(GetDepartmentVMsByPlantId(plantId));
            SelectList deptSL = new SelectList(deptVMs, "Department_Id", "Department_Name");
            model.Departments = deptSL;
            return PartialView("_DepartmentPartialView", model);
        }

        public JsonResult LoadEmail(string plantId, string departmentId, string positionId)
        {
            var data = JsonConvert.SerializeObject(GetEmail(plantId, departmentId, positionId));

            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult LoadEmailFromFile()
        {
            List<MailingEmailsViewModel> result = new List<MailingEmailsViewModel>();
            if (Request.Files["MailListFile"].ContentLength > 0)
            {
                HttpPostedFileBase file = Request.Files["MailListFile"];
                result = GetEmailFromFile(file);
            }

            var data = JsonConvert.SerializeObject(result);
            return Json(new { data = data });
        }

        public IEnumerable<DepartmentViewModel> GetDepartmentVMsByPlantId(string plantId)
        {
            IEnumerable<Department> departments = en.Departments.Where(d => d.Deactive != true);
            if (!string.IsNullOrWhiteSpace(plantId))
            {
                departments = departments.Where(d => d.Plant_Id == plantId);
            }

            List<DepartmentViewModel> result = departments
                   .Select(d => new DepartmentViewModel()
                   {
                       Department_Id = d.Department_Id,
                       Department_Name = d.Department_Name
                   }).ToList();

            return result;
        }

        public IEnumerable<MailingEmailsViewModel> GetEmail(string plantId, string departmentId, string positionId)
        {
            if (plantId == "BOD")
            {
                List<MailingEmailsViewModel> BODs = new List<MailingEmailsViewModel>();

                BODs = GetContactBODs();

                IEnumerable<MailingEmailsViewModel> BODsEmails = BODs;
                return BODsEmails;
            }

            if (plantId == "Test")
            {
                List<MailingEmailsViewModel> tests = new List<MailingEmailsViewModel>()
                {
                    new MailingEmailsViewModel() { EmployeeName = "Test Account By IT 01", Email = "test01.it@cjvina.com", Position = "IT Tester" },
                    new MailingEmailsViewModel() { EmployeeName = "Test Account By IT 02", Email = "test02.it@cjvina.com", Position = "IT Tester" },
                    new MailingEmailsViewModel() { EmployeeName = "Test Account By IT 03", Email = "test03.it@cjvina.com", Position = "IT Tester" },
                    new MailingEmailsViewModel() { EmployeeName = "Test Account By IT 04", Email = "test04.it@cjvina.com", Position = "IT Tester" },
                    new MailingEmailsViewModel() { EmployeeName = "Test Account By IT 05", Email = "test05.it@cjvina.com", Position = "IT Tester" }
                };

                List<MailingEmailsViewModel> testgrps = new List<MailingEmailsViewModel>();
                for (int i = 0; i < 400; i++)
                {
                    testgrps.AddRange(tests);
                }
                IEnumerable<MailingEmailsViewModel> testEmails = testgrps;
                return testEmails;
            }

            var employeeWithDept = en.Employees
            .Join(en.Departments, e => e.Department_ID, d => d.Department_Id, (e, d) => new { e, d });

            if (!string.IsNullOrEmpty(plantId))
            {
                employeeWithDept = employeeWithDept.Where(grp => grp.d.Plant_Id == plantId);
            }

            if (!string.IsNullOrEmpty(departmentId))
            {
                employeeWithDept = employeeWithDept.Where(grp => grp.d.Department_Id == departmentId);
            }

            if (!string.IsNullOrEmpty(positionId))
            {
                string positionKeyword = "";
                switch (positionId)
                {
                    case "1":
                        positionKeyword = "Head of";
                        break;
                    case "2":
                        positionKeyword = "Manager";
                        break;
                    case "3":
                        positionKeyword = "Sales";
                        break;
                    default:
                        break;
                }

                employeeWithDept = employeeWithDept.Where(grp => grp.e.Position.Contains(positionKeyword));
            }

            employeeWithDept = employeeWithDept.Where(grp => grp.e.Deactive != true);
            var emails = employeeWithDept;
            if (plantId == "V2040")
            {
                emails = employeeWithDept.Where(grp => grp.e.Deactive != true && grp.e.Grade.Contains("VG"))
                    .Union(employeeWithDept.Where(grp => grp.e.Deactive != true && !grp.e.Grade.Contains("VG") && grp.e.Email != ""));
            }

            IEnumerable<MailingEmailsViewModel> result = emails
                .Select(grp => new MailingEmailsViewModel()
                {
                    EmployeeName = grp.e.Employee_Name,
                    Email = grp.e.Email,
                    Position = grp.e.Position
                });

            return result;
        }

        public List<MailingEmailsViewModel> GetEmailFromFile(HttpPostedFileBase mailListFile)
        {
            List<MailingEmailsViewModel> emailVMs = new List<MailingEmailsViewModel>();

            if (mailListFile.ContentType == "application/vnd.ms-excel" || mailListFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                using (ExcelPackage package = new ExcelPackage(mailListFile.InputStream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var table = worksheet.Tables.First();
                    int startRow = table.Address.Start.Row;
                    int endRow = table.Address.End.Row;

                    for (int rowNo = startRow + 1; rowNo <= endRow; rowNo++)
                    {
                        MailingEmailsViewModel emailVM = new MailingEmailsViewModel()
                        {
                            EmployeeName = worksheet.Cells[rowNo, 1].Value?.ToString(),
                            Email = worksheet.Cells[rowNo, 2].Value?.ToString(),
                            Position = worksheet.Cells[rowNo, 3].Value?.ToString()
                        };
                        emailVMs.Add(emailVM);
                    }
                }
            }
            return emailVMs;
        }

        public List<MailingEmailsViewModel> GetContactBODs()
        {
            List<MailingEmailsViewModel> result = new List<MailingEmailsViewModel>()
            {
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Nam Ki Don", Email = "kidon.nam@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Lee Jea Ho", Email = "minsoo.kang@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Hwang Hyun Jo", Email = "hyunjo.hwang@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Lim Hyun Jung", Email = "david.lim@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Jeon Jae Won", Email = "jw.jeon@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Kim Yoon Ho", Email = "yoonho.kim@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Dr.Kim Chung Hyun", Email = "chunghyun.kim@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Jung Jihyeon", Email = "jiheon.jung@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Dr.Choi Hyun Soo", Email = "hyunsoo.choi@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Dr.Shin Seung Jun", Email = "seungjun.shin@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Song Ji Woong", Email = "jiwoong.song@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Kim Wan Su", Email = "wansu.kim@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Heo Seok Won", Email = "sukwon.heo@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Lee Ki Young", Email = "gheeyoung.lee@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Kwon Hyuk Do", Email = "hd.kwon@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Jeon Saebyuk", Email = "sb.jeon@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Lee Kang Chul", Email = "kangchul.lee@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Jung Hwan Chul", Email = "hc.jung@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Kil Jun Min", Email = "jm.kil@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Chon Gu Soo", Email = "gs.chon@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Eun Sejun", Email = "sejun.eun@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Park Ju Hwan", Email = "juhwan.park@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Kim Kyoung Ho", Email = "kyoungho.kim@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Jeong Jong Yul", Email = "yuri.jeong@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Park Sung Jun", Email = "sungjun.park@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Bai Su Young", Email = "sy.bai@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Choe Ji Won", Email = "jiwon.choe@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Mr.Shin Keun Sup", Email = "keunsup.shin@cj.net", Position = "BOD" },
                    new MailingEmailsViewModel() { EmployeeName = "Ms.Linh (HR)", Email = "phuonglinh.hr@cjvina.com", Position = "BOD" },
            };
            return result;
        }

        public void logging(DataTable errTb, int countFailed)
        {
            string serverPath = HostingEnvironment.MapPath(Resources.MailingLogPath);
            if (!Directory.Exists(serverPath))
            {
                Directory.CreateDirectory(serverPath);
            }

            string logPath = Path.Combine(serverPath, "Mailing_" + DateTime.Now.ToString("yyyy'-'MM'-'dd'_'HH''mm''ss") + "_log.txt");

            using (StreamWriter sw = new StreamWriter(logPath, true))
            {
                sw.WriteLine(new string('=', 30));
                sw.WriteLine(DateTime.Now.ToShortDateString());
                sw.WriteLine("EMAILS SENT FAILED");
                sw.WriteLine("QUANTITY: {0}", countFailed);
                sw.WriteLine(new string('=', 30));
                foreach (DataRow row in errTb.Rows)
                {
                    sw.WriteLine(row["errMail"].ToString());
                    sw.WriteLine(row["errMessage"].ToString());
                    sw.WriteLine(new string('-', row["errMessage"].ToString().Length));
                    sw.WriteLine();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                en.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
