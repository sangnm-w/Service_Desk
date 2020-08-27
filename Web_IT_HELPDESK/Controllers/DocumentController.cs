using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace Web_IT_HELPDESK.Controllers
{
    public class DocumentController : Controller
    {
        //
        // GET: /Document/

        public ActionResult PortalIndex()
        {
            return View();
        }

        Web_IT_HELPDESKEntities en = new Web_IT_HELPDESKEntities();
        public ActionResult PortalAdmin()
        {
            var document = en.Documents;
            return View(document);
        }

        public ActionResult Create()
        {
            Document document = new Document();
            ViewBag.EmployeeID = //Session["employee_id"].ToString();
                                System.Web.HttpContext.Current.User.Identity.Name;
            ViewBag.DocumentTypeId = new SelectList(en.DocumentTypes, "DocumentTypeId", "DocumentTypeName", en.DocumentTypes.First().DocumentTypeId);
            return View(document);
        }

        [HttpPost]
        public ActionResult Create(Document document, HttpPostedFileBase image)
        {
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
                        System.Data.Linq.Binary binaryFile = new System.Data.Linq.Binary(fileData);
                        //initialise object of FileDump LINQ to sql class passing values to be inserted

                        document.FileContext = binaryFile.ToArray();
                        document.FileName = System.IO.Path.GetFileName(Request.Files[upload].FileName);
                    }       
                    en.Documents.Add(document);
                    try
                    {
                        en.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }
                    return RedirectToAction("PortalAdmin", "Document");
                }
                else
                {
                }
            }

            ViewBag.Document_Type_Id = new SelectList(en.DocumentTypes, "DocumentTypeId", "DocumentTypeName", document.DocumentTypeId);
            return View(document);

        }

        public ActionResult Contactlist()
        {
            var employeeinfo = en.EmployeeInfoes.Where(i => i.DE != true);
            return View(employeeinfo);
        }

        public ActionResult Contactlist2()
        {
            if (session_emp != "")
            {
                var emp_infor = en.EmployeeInfoes.Where(i => i.DE != true && i.Plant == "0301");
                return View(emp_infor);
            }
            else return View();
        }

        private string session_emp = System.Web.HttpContext.Current.User.Identity.Name;
        [Authorize]
        [HttpPost]
        public ActionResult Contactlist2(string solved)
        {
            if (session_emp != "")
            {
                var emp_infor = en.EmployeeInfoes.Where(i => i.DE != true && i.Plant == solved);
                return View(emp_infor);
            }
            else return View();
        }

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

        public PartialViewResult partial0000001()
        {
            var _partial0000001 = en.Documents.Where(i => i.Del != true);
            return PartialView("partial0000001", _partial0000001);
        }


        private string severIP = @"\\52.213.3.75\share file\Quan IT";

        [HttpGet]
        public FileContentResult C049F25634B7BAC67362354838256(string id) // just call id is the name
        {
            IntPtr admin_token = default(IntPtr);
            //Added these 3 lines
            WindowsIdentity wid_current = WindowsIdentity.GetCurrent();
            WindowsIdentity wid_admin = null;
            WindowsImpersonationContext wic = null;


            if (LogonUser("administrator", "52.213.3.75", "cjsap@75!s", 9, 0, ref admin_token) != 0)
            {
                //Newly added lines
                wid_admin = new WindowsIdentity(admin_token);
                wic = wid_admin.Impersonate();
            }


            // for downloading file
            //severIP + textBox_Download.Text   
            string Filename = id;
            string path = severIP + "\\ShareFile\\" + Filename.ToString();// +"(20161121)Vinamilk_CA_2.pdf";

            //string path = AppDomain.CurrentDomain.BaseDirectory + "FolderName/";
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);// + "filename.extension");
            string fileName = System.IO.Path.GetFileName(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

        }

        [HttpGet]
        public FileContentResult Download_HR(string id) // just call id is the name
        {
            IntPtr admin_token = default(IntPtr);
            //Added these 3 lines
            WindowsIdentity wid_current = WindowsIdentity.GetCurrent();
            WindowsIdentity wid_admin = null;
            WindowsImpersonationContext wic = null;


            if (LogonUser("administrator", "52.213.3.75", "cjsap@75!s", 9, 0, ref admin_token) != 0)
            {
                //Newly added lines
                wid_admin = new WindowsIdentity(admin_token);
                wic = wid_admin.Impersonate();
            }


            // for downloading file
            //severIP + textBox_Download.Text   
            string Filename = id;
            string path = severIP + "\\ShareFile\\" + Filename.ToString();// +"(20161121)Vinamilk_CA_2.pdf";

            //string path = AppDomain.CurrentDomain.BaseDirectory + "FolderName/";
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);// + "filename.extension");
            string fileName = System.IO.Path.GetFileName(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

        }

        [DllImport("advapi32.DLL", SetLastError = true)]
        public static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

    }
}
