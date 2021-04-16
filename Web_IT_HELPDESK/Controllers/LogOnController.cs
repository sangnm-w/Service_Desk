using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Web_IT_HELPDESK.Controllers.ObjectManager;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.Models.Extensions;

namespace Web_IT_HELPDESK.Controllers
{
    public class LogOnController : Controller
    {
        //
        // GET: /LogOn/
        ServiceDeskEntities en = new ServiceDeskEntities();

        public ActionResult Logon()
        {
            var documents = en.Documents.OrderBy(d => d.Code);
            ViewBag.Documents = documents.ToList();
            return View();
        }

        /*public ActionResult Logon(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            string emailcookies = System.Web.HttpContext.Current.User.Identity.Name;
            if (!String.IsNullOrEmpty(emailcookies))
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new LoginPersonViewModel());
        }*/

        private string GetPlant_id(string v_emp)
        {
            string plant_id = en.Employee_New
                .Join(en.Departments, e => e.Department_ID, d => d.Department_ID, (e, d) => new { e, d })
                .Where(grp => grp.e.Emp_CJ == v_emp)
                .Select(grp => grp.d.Plant_ID).SingleOrDefault();
            return plant_id;
        }

        private string GetDept_id(string session_emp)
        {
            string dept_id = en.Employee_New.Where(f => f.Emp_CJ == session_emp).Select(f => f.Department_ID).SingleOrDefault();
            return dept_id;
        }

        [HttpPost]
        public ActionResult LogOn(Employee_New emp, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                UserManager userManager = new UserManager();
                string password = userManager.GetUserPassword(emp.Emp_CJ);

                if (emp.Password == password)
                {
                    FormsAuthentication.SetAuthCookie(emp.Emp_CJ, false);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        Session["employee_id"] = emp.Emp_CJ;
                        Session["plant_id"] = GetPlant_id(emp.Emp_CJ);
                        Session["dept_id"] = GetDept_id(emp.Emp_CJ);
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        Session["employee_id"] = emp.Emp_CJ;
                        Session["plant_id"] = GetPlant_id(emp.Emp_CJ);
                        Session["dept_id"] = GetDept_id(emp.Emp_CJ);
                        string emailcookies = System.Web.HttpContext.Current.User.Identity.Name;
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("LogonF", "The Id or Password is invalid!");
                }
            }
            var documents = en.Documents.OrderBy(d => d.Code);
            ViewBag.Documents = documents;
            return View(emp);
        }

        // log out
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            ApplicationUser.Instance = null;
            return RedirectToAction("Logon", "Logon");
        }

        //
        // GET: /LogOn/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /LogOn/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /LogOn/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
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

        //
        // GET: /LogOn/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /LogOn/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
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

        //
        // GET: /LogOn/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /LogOn/Delete/5

        [HttpPost]
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

        /*public ActionResult DownloadFile()
        {
            string filename = "DC-10_CJ_VINA_Security _Approval_format.docx";
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "/Content/" + filename;
            byte[] fileData = System.IO.File.ReadAllBytes(filepath);
            string contentType = MimeMapping.GetMimeMapping(filepath);
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = filepath,
                Inline = true,
            };
            Response.AppendHeader("Conteent-Disposition",cd.ToString());
            return File(fileData, contentType, filename);
        }

        public ActionResult DownloadFile_LockScreen()
        {
            string filename = "IS_document.pdf";
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "/Content/" + filename;
            byte[] fileData = System.IO.File.ReadAllBytes(filepath);
            string contentType = MimeMapping.GetMimeMapping(filepath);
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = filepath,
                Inline = true,
            };
            Response.AppendHeader("Conteent-Disposition", cd.ToString());
            return File(fileData, contentType, filename);
        }*/

        [HttpGet]
        public FileContentResult Get_file(Guid? id)
        {
            Document doc = en.Documents.Find(id);
            string doc_code = doc.Code;

            //declare byte array to get file content from database and string to store file name
            byte[] fileData;
            string fileName;
            //create object of LINQ to SQL class
            //using LINQ expression to get record from database for given id value
            var record = from p in en.Documents
                         where p.Code == doc_code
                         select p;
            //only one record will be returned from database as expression uses condtion on primary field
            //so get first record from returned values and retrive file content (binary) and filename
            fileData = (byte[])record.First().FileContext.ToArray();
            fileName = record.First().FileName;
            //return file and provide byte file content and file name
            return File(fileData, "text", fileName);
        }

    }
}
