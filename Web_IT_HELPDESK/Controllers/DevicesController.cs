using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.ViewModels;
using EntityState = System.Data.Entity.EntityState;

namespace Web_IT_HELPDESK.Controllers
{
    public class DevicesController : Controller
    {
        private ServiceDeskEntities en = new ServiceDeskEntities();
        private string session_emp = System.Web.HttpContext.Current.User.Identity.Name;

        // GET: Devices
        public ActionResult Index()
        {
            string curr_plantId = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Plant_Id;
            IEnumerable<DeviceViewModel> devices = DeviceModel.Instance.GetDevicesByPlantId(curr_plantId);

            return View(devices.ToList());
        }

        // GET: Devices/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = en.Devices.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }
            return View(device);
        }

        #region Create
        // GET: Devices/Create
        public ActionResult Create()
        {
            string empPlantID = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Plant_Id;

            ViewBag.Plant_Id = empPlantID;

            //List<SelectListItem> Contract_Id = new List<SelectListItem>();
            //Contract_Id.AddRange(new SelectList(en.CONTRACTs.Where(c => c.PLANT == empPlantID), "ID", "CONTRACTNAME", ""));
            //Contract_Id.Insert(0, new SelectListItem { Text = "None", Value = "" });

            //ViewBag.Contract_Id = Contract_Id;

            List<SelectListItem> Device_Type_Id = new List<SelectListItem>();
            Device_Type_Id.AddRange(new SelectList(en.Device_Type.Where(dt => dt.Device_Type_Id == 3 || dt.Device_Type_Id == 6), "Device_Type_Id", "Device_Type_Name"));
            Device_Type_Id.Insert(0, new SelectListItem { Text = "None", Value = "" });

            ViewBag.Device_Type_Id = Device_Type_Id;

            return View();
        }

        // POST: Devices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Device device)
        {
            string empPlantID = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Plant_Id;

            //if (device.Contract_Id == null)
            //    ModelState.AddModelError("Contract_Id", "Please select one contract!");

            //if (device.Device_Type_Id == null)
            //    ModelState.AddModelError("Device_Type_Id", "Please select one type!");
            //
            DateTime purchaseD = device.Purchase_Date.GetValueOrDefault();
            DateTime depreciationD = device.Depreciation.GetValueOrDefault();
            if (purchaseD >= depreciationD)
            {
                ModelState.AddModelError("Depreciation", "Depreciation Date cannot be less than date of purchase");
            }

            if (ModelState.IsValid)
            {
                device.Device_Id = Guid.NewGuid();
                device.Device_Code = DeviceModel.Instance.Generate_DeviceCode(empPlantID, device.Device_Type_Id);
                device.Create_Date = DateTime.Now;

                device.Plant_Id = empPlantID;

                en.Devices.Add(device);
                en.SaveChanges();
                return RedirectToAction("Index");

            }

            List<SelectListItem> Device_Type_Id = new List<SelectListItem>();
            Device_Type_Id.AddRange(new SelectList(en.Device_Type.Where(dt => dt.Device_Type_Id == 3 || dt.Device_Type_Id == 6), "Device_Type_Id", "Device_Type_Name"));
            Device_Type_Id.Insert(0, new SelectListItem { Text = "None", Value = "" });

            ViewBag.Device_Type_Id = Device_Type_Id;

            return View();
        }

        // GET: Devices/CreateOthers
        public ActionResult CreateOthers()
        {
            string empPlantID = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Plant_Id;

            ViewBag.Plant_Id = empPlantID;

            List<SelectListItem> Contract_Id = new List<SelectListItem>();
            Contract_Id.AddRange(new SelectList(en.CONTRACTs.Where(c => c.PLANT == empPlantID), "ID", "CONTRACTNAME", ""));
            Contract_Id.Insert(0, new SelectListItem { Text = "None", Value = "" });

            ViewBag.Contract_Id = Contract_Id;

            List<SelectListItem> Device_Type_Id = new List<SelectListItem>();
            Device_Type_Id.AddRange(new SelectList(en.Device_Type.Where(dt => dt.Device_Type_Id != 3 && dt.Device_Type_Id != 6), "Device_Type_Id", "Device_Type_Name"));
            Device_Type_Id.Insert(0, new SelectListItem { Text = "None", Value = "" });

            ViewBag.Device_Type_Id = Device_Type_Id;

            return View();
        }

        // POST: Devices/CreateOthers
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOthers(Device device)
        {
            string empPlantID = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Plant_Id;

            //if (device.Contract_Id == null)
            //    ModelState.AddModelError("Contract_Id", "Please select one contract!");

            //if (device.Device_Type_Id == null)
            //    ModelState.AddModelError("Device_Type_Id", "Please select one type!");
            //
            DateTime purchaseD = device.Purchase_Date.GetValueOrDefault();
            DateTime depreciationD = device.Depreciation.GetValueOrDefault();
            if (purchaseD >= depreciationD)
            {
                ModelState.AddModelError("Depreciation", "Depreciation Date cannot be less than date of purchase");
            }

            if (ModelState.IsValid)
            {
                device.Device_Id = Guid.NewGuid();
                device.Device_Code = DeviceModel.Instance.Generate_DeviceCode(empPlantID,device.Device_Type_Id);
                device.Create_Date = DateTime.Now;

                device.Plant_Id = empPlantID;

                en.Devices.Add(device);
                en.SaveChanges();
                return RedirectToAction("Index");

            }

            List<SelectListItem> Contract_Id = new List<SelectListItem>();
            Contract_Id.AddRange(new SelectList(en.CONTRACTs, "ID", "CONTRACTNAME", ""));
            Contract_Id.Insert(0, new SelectListItem { Text = "None", Value = "" });

            ViewBag.Contract_Id = Contract_Id;

            List<SelectListItem> Device_Type_Id = new List<SelectListItem>();
            Device_Type_Id.AddRange(new SelectList(en.Device_Type.Where(dt => dt.Device_Type_Id != 3 && dt.Device_Type_Id != 6), "Device_Type_Id", "Device_Type_Name"));
            Device_Type_Id.Insert(0, new SelectListItem { Text = "None", Value = "" });

            ViewBag.Device_Type_Id = Device_Type_Id;

            return View();
        }
        #endregion

        #region Edit
        // GET: Devices/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = en.Devices.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }
            string empPlantID = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Plant_Id;

            ViewBag.Device_Type_Name = en.Device_Type.FirstOrDefault(t => t.Device_Type_Id == device.Device_Type_Id).Device_Type_Name;

            return View(device);
        }

        // POST: Devices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Device device)
        {
            string empPlantID = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Plant_Id;
            DateTime purchaseD = device.Purchase_Date.GetValueOrDefault();
            DateTime depreciationD = device.Depreciation.GetValueOrDefault();
            if (purchaseD >= depreciationD)
            {
                ModelState.AddModelError("Depreciation", "Depreciation Date cannot be less than date of purchase");
            }
            if (ModelState.IsValid)
            {
                en.Entry(device).State = EntityState.Modified;
                en.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Device_Type_Name = en.Device_Type.FirstOrDefault(t => t.Device_Type_Id == device.Device_Type_Id).Device_Type_Name;

            return View(device);
        }

        // GET: Devices/EditOthers/5
        public ActionResult EditOthers(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = en.Devices.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }
            string empPlantID = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Plant_Id;

            ViewBag.Device_Type_Name = en.Device_Type.FirstOrDefault(t => t.Device_Type_Id == device.Device_Type_Id).Device_Type_Name;

            return View(device);
        }

        // POST: Devices/EditOthers/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOthers(Device device)
        {
            string empPlantID = en.Employees.FirstOrDefault(e => e.EmployeeID == session_emp).Plant_Id;
            DateTime purchaseD = device.Purchase_Date.GetValueOrDefault();
            DateTime depreciationD = device.Depreciation.GetValueOrDefault();
            if (purchaseD >= depreciationD)
            {
                ModelState.AddModelError("Depreciation", "Depreciation Date cannot be less than date of purchase");
            }
            if (ModelState.IsValid)
            {
                en.Entry(device).State = EntityState.Modified;
                en.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Device_Type_Name = en.Device_Type.FirstOrDefault(t => t.Device_Type_Id == device.Device_Type_Id).Device_Type_Name;

            return View(device);
        }
        #endregion

        // GET: Devices/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = en.Devices.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }
            return View(device);
        }

        // POST: Devices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Device device = en.Devices.Find(id);
            en.Devices.Remove(device);
            en.SaveChanges();
            return RedirectToAction("Index");
        }

        // Post: Devices/Upload
        [HttpPost, ActionName("Upload")]
        public ActionResult UploadDevices(HttpPostedFileBase FileUpload)
        {
            if (FileUpload != null)
            {
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    try
                    {
                        List<Device> deviceList = new List<Device>();
                        using (ExcelPackage package = new ExcelPackage(FileUpload.InputStream))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                            int startRow = worksheet.Dimension.Start.Row;
                            int endRow = worksheet.Dimension.End.Row;


                            for (int rowNo = startRow + 1; rowNo < endRow; rowNo++)
                            {

                                Guid Device_Id = Guid.NewGuid();
                                Guid? Contract_Id = null;
                                bool DeviceType = int.TryParse(worksheet.Cells[rowNo, 1].Value?.ToString(), out int DeviceTypeExcel);
                                int? Device_Type_Id = DeviceType ? (int?)DeviceTypeExcel : null;
                                // TODO: ~MS Missing plantId
                                string Device_Code = DeviceModel.Instance.Generate_DeviceCode("missing",Device_Type_Id);
                                string Device_Name = worksheet.Cells[rowNo, 2].Value?.ToString();
                                string Serial_No = worksheet.Cells[rowNo, 3].Value?.ToString();
                                DateTime? Purchase_Date = Convert.ToDateTime(worksheet.Cells[rowNo, 4].Value ?? null);
                                string CPU = worksheet.Cells[rowNo, 5].Value?.ToString();
                                string RAM = worksheet.Cells[rowNo, 6].Value?.ToString();
                                string DISK = worksheet.Cells[rowNo, 7].Value?.ToString();
                                string Operation_System = worksheet.Cells[rowNo, 8].Value?.ToString();
                                string Note = worksheet.Cells[rowNo, 9].Value?.ToString();
                                DateTime? Depreciation = Convert.ToDateTime(worksheet.Cells[rowNo, 10].Value ?? null);
                                string Device_Status = worksheet.Cells[rowNo, 11].Value?.ToString();
                                string Addition_Information = worksheet.Cells[rowNo, 12].Value?.ToString();
                                string PlantNameExcel = worksheet.Cells[rowNo, 13].Value?.ToString();
                                string Plant_Id = (en.Departments.Select(d => new { d.Plant_Id, d.Plant_Name }).Distinct().ToList()).FirstOrDefault(p => p.Plant_Name == PlantNameExcel).Plant_Id;
                                DateTime? Create_Date = DateTime.Today;
                                string OS_License = worksheet.Cells[rowNo, 14].Value?.ToString();
                                string Office = worksheet.Cells[rowNo, 15].Value?.ToString();
                                string Office_License = worksheet.Cells[rowNo, 16].Value?.ToString();

                                deviceList.Add(new Device
                                {
                                    Device_Id = Device_Id,
                                    Contract_Id = Contract_Id,
                                    Device_Type_Id = Device_Type_Id,
                                    Device_Code = Device_Code,
                                    Device_Name = Device_Name,
                                    Serial_No = Serial_No,
                                    Purchase_Date = Purchase_Date,
                                    CPU = CPU,
                                    RAM = RAM,
                                    DISK = DISK,
                                    Operation_System = Operation_System,
                                    Note = Note,
                                    Depreciation = Depreciation,
                                    Device_Status = Device_Status,
                                    Addition_Information = Addition_Information,
                                    Plant_Id = Plant_Id,
                                    Create_Date = null,
                                    OS_License = null,
                                    Office = null,
                                    Office_License = null
                                });

                            }
                        }
                        try
                        {
                            en.Devices.AddRange(deviceList);
                            en.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            string error = "Saving devices have been failed!!!";
                            ex.Data.Add("Error", error);
                            throw ex;
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Error = "Read file is error: " + ex.Message;
                    }
                    var devices = en.Devices.Include(d => d.CONTRACT).Include(d => d.Device_Type);
                    return View("Index", devices.ToList());
                }
                else
                {
                    ViewBag.Error = "Only Excel file format is allowed";
                    var devices = en.Devices.Include(d => d.CONTRACT).Include(d => d.Device_Type);
                    return View("Index", devices.ToList());
                }
            }
            else
            {
                ViewBag.Error = "Please choose Excel file";
                var devices = en.Devices.Include(d => d.CONTRACT).Include(d => d.Device_Type);
                return View("Index", devices.ToList());
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
