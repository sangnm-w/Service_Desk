using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web_IT_HELPDESK.Controllers.ObjectManager;
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
        [Authorize]
        public ActionResult Index()
        {
            //string curr_plantId = en.Employees.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_Id;
            string curr_plantId = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_ID;
            IEnumerable<DeviceViewModel> devices = DeviceModel.Instance.GetDevicesByPlantId(curr_plantId);
            return View(devices.ToList());
        }

        // GET: Devices/Details/5
        [Authorize]
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

            List<AllocationViewModel> allocations = AllocationModel.Instance.get_AllocationsByDeviceId(device.Device_Id);
            ViewBag.Allocations = allocations;

            return View(device);
        }

        #region Create
        // GET: Devices/Create
        [Authorize]
        public ActionResult Create()
        {
            //string empPlantID = en.Employees.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_Id;
            string empPlantID = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_ID;

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
            //string empPlantID = en.Employees.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_Id;
            string empPlantID = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_ID;

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
                device.Device_Status = DeviceModel.DeviceStatus.In_Stock.ToString();

                device.Plant_Id = empPlantID;

                string deviceQRPath = DeviceHelper.Instance.CreateQRCode(device);
                device.QRCodeFile = deviceQRPath;

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
        [Authorize]
        public ActionResult CreateOthers()
        {
            //string empPlantID = en.Employees.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_Id;
            string empPlantID = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_ID;

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
            //string empPlantID = en.Employees.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_Id;
            string empPlantID = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_ID;

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
                device.Device_Status = DeviceModel.DeviceStatus.In_Stock.ToString();
                device.Plant_Id = empPlantID;

                string deviceQRPath = DeviceHelper.Instance.CreateQRCode(device);
                device.QRCodeFile = deviceQRPath;

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
        [Authorize]
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
            //string empPlantID = en.Employees.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_Id;
            string empPlantID = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_ID;

            ViewBag.Device_Type_Name = en.Device_Type.FirstOrDefault(t => t.Device_Type_Id == device.Device_Type_Id).Device_Type_Name;

            return View(device);
        }

        // POST: Devices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Device device)
        {
            //string empPlantID = en.Employees.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_Id;
            string empPlantID = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_ID;
            DateTime purchaseD = device.Purchase_Date.GetValueOrDefault();
            DateTime depreciationD = device.Depreciation.GetValueOrDefault();
            if (purchaseD >= depreciationD)
            {
                ModelState.AddModelError("Depreciation", "Depreciation Date cannot be less than date of purchase");
            }
            if (ModelState.IsValid)
            {
                string deviceQRPath = DeviceHelper.Instance.CreateQRCode(device);
                device.QRCodeFile = deviceQRPath;

                en.Entry(device).State = EntityState.Modified;
                en.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Device_Type_Name = en.Device_Type.FirstOrDefault(t => t.Device_Type_Id == device.Device_Type_Id).Device_Type_Name;

            return View(device);
        }

        // GET: Devices/EditOthers/5
        [Authorize]
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
            //string empPlantID = en.Employees.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_Id;
            string empPlantID = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_ID;

            ViewBag.Device_Type_Name = en.Device_Type.FirstOrDefault(t => t.Device_Type_Id == device.Device_Type_Id).Device_Type_Name;

            return View(device);
        }

        // POST: Devices/EditOthers/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOthers(Device device)
        {
            //string empPlantID = en.Employees.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_Id;
            string empPlantID = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_ID;
            DateTime purchaseD = device.Purchase_Date.GetValueOrDefault();
            DateTime depreciationD = device.Depreciation.GetValueOrDefault();
            if (purchaseD >= depreciationD)
            {
                ModelState.AddModelError("Depreciation", "Depreciation Date cannot be less than date of purchase");
            }
            if (ModelState.IsValid)
            {
                string deviceQRPath = DeviceHelper.Instance.CreateQRCode(device);
                device.QRCodeFile = deviceQRPath;

                en.Entry(device).State = EntityState.Modified;
                en.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Device_Type_Name = en.Device_Type.FirstOrDefault(t => t.Device_Type_Id == device.Device_Type_Id).Device_Type_Name;

            return View(device);
        }
        #endregion

        // GET: Devices/Delete/5
        [Authorize]
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
        [Authorize]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Device device = en.Devices.Find(id);
            en.Devices.Remove(device);
            en.SaveChanges();
            return RedirectToAction("Index");
        }

        // Post: Devices/Upload
        [HttpPost, ActionName("Upload")]
        [Authorize]
        public ActionResult UploadDevices(HttpPostedFileBase FileUpload)
        {
            //string curr_plantId = en.Employees.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_Id;
            string curr_plantId = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_ID;
            if (FileUpload != null)
            {
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    List<Device> deviceList = new List<Device>();
                    List<DeviceViewModel.ErrDeviceExcel> errDeviceList = new List<DeviceViewModel.ErrDeviceExcel>();

                    deviceList = DeviceHelper.Instance.GetDevicesFromExcel(FileUpload.InputStream, out errDeviceList);

                    foreach (Device item in deviceList)
                    {
                        try
                        {
                            item.Device_Code = DeviceModel.Instance.Generate_DeviceCode_Upload_OnebyOne(item.Plant_Id, item.Device_Type_Id);
                            string devicePath = DeviceHelper.Instance.CreateQRCode(item);
                            item.QRCodeFile = devicePath;

                            en.Configuration.ValidateOnSaveEnabled = false;
                            en.Devices.Add(item);
                            en.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            DeviceViewModel.ErrDeviceExcel errDevice = new DeviceViewModel.ErrDeviceExcel(item);
                            errDevice.errMsg = ex.ToString();
                            errDeviceList.Add(errDevice);
                        }
                    }

                    //Column need format date
                    List<int> colsDate = new List<int>() { 4, 14, 18 };

                    if (errDeviceList.Count > 0)
                    {
                        Dictionary<int, string> errDeviceTitles = ExcelTitle.Instance.Devices();
                        errDeviceTitles.Add(19, "Error Message");

                        var stream = ExcelHelper.Instance.CreateExcelFile(null, errDeviceList, errDeviceTitles, colsDate);
                        var buffer = stream as MemoryStream;
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("Content-Disposition", "attachment; filename=Error Devices List.xlsx");
                        Response.BinaryWrite(buffer.ToArray());
                        ViewBag.Error = "An error has occurred while uploading. Please check Error_Devices_List file!";
                    }
                    else
                    {
                        ViewBag.Error = "Upload success!";
                    }

                    IEnumerable<DeviceViewModel> devices = DeviceModel.Instance.GetDevicesByPlantId(curr_plantId);
                    return View("Index", devices.ToList());
                }
                else
                {
                    ViewBag.Error = "Only Excel file format is allowed";
                    IEnumerable<DeviceViewModel> devices = DeviceModel.Instance.GetDevicesByPlantId(curr_plantId);
                    return View("Index", devices.ToList());
                }
            }
            else
            {
                ViewBag.Error = "Please choose Devices Excel file";
                IEnumerable<DeviceViewModel> devices = DeviceModel.Instance.GetDevicesByPlantId(curr_plantId);
                return View("Index", devices.ToList());
            }
        }

        public FileContentResult Download()
        {
            //bool? hasPermission = CurrentUser.Instance.hasPermission(Commons.ActionConstant.DOWNLOAD, Commons.ModuleConstant.INCIDENT);
            //if (hasPermission.Value == false)
            //    return RedirectToAction("Index", "Home");

            //string curr_plantId = en.Employees.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_Id;
            string curr_plantId = en.Employee_New.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_ID;
            var devices = DeviceModel.Instance.GetQRDevicesByPlantID(curr_plantId);

            var stream = ExcelHelper.Instance.CreateQRExcel(null, devices.ToList(), ExcelTitle.Instance.QRDevices(), null);
            var buffer = stream as MemoryStream;
            //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //Response.AddHeader("Content-Disposition", "attachment; filename=IT Order Request.xlsx");
            //Response.BinaryWrite(buffer.ToArray());
            //Response.Flush();
            //Response.End();
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "QR Code Devices.xlsx";
            return File(buffer.ToArray(), contentType, fileName);
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
