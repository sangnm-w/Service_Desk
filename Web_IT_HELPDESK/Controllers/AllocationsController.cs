using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Web_IT_HELPDESK.Commons;
using Web_IT_HELPDESK.Controllers.ObjectManager;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.Models.Extensions;
using Web_IT_HELPDESK.Properties;
using Web_IT_HELPDESK.ViewModels;
using static Web_IT_HELPDESK.Models.AutoCompleteModel;
using EntityState = System.Data.Entity.EntityState;

namespace Web_IT_HELPDESK.Controllers
{

    public class AllocationsController : Controller
    {
        public ServiceDeskEntities en { get; set; }
        public ApplicationUser _appUser { get; set; }
        public string currentEmployeeID { get; set; }

        public AllocationsController()
        {
            en = new ServiceDeskEntities();
            _appUser = new ApplicationUser();
            currentEmployeeID = _appUser.EmployeeID;
        }

        // GET: Allocations/Index
        [CustomAuthorize]
        public ActionResult Index()
        {
            string currentPlantId = _appUser.GetPlantID();

            List<Rule> userRules = new List<Rule>();
            List<Role> userRoles = new List<Role>();
            List<Plant> userPlants = new List<Plant>();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            bool IsAdmin = _appUser.isAdmin;

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

                bool isITManager = userRoles.Any(ro => ro.Role_ID == 2);
            }

            ViewBag.userRules = userRules.Select(ru => ru.Rule_Name).ToList();
            ViewBag.userPlants = userPlants;
            ViewBag.currentPlantId = currentPlantId;

            IEnumerable<AllocationViewModel> avm = AllocationModel.Instance.GetLastAllocationOfDeviceByPlantId(currentPlantId);
            return View(avm);
        }


        //POST: Allocations/Index
        [HttpPost]
        [CustomAuthorize]
        public ActionResult Index(string plantId)
        {
            string currentPlantId = _appUser.GetPlantID();
            List<Rule> userRules = new List<Rule>();
            List<Role> userRoles = new List<Role>();
            List<Plant> userPlants = new List<Plant>();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            bool IsAdmin = _appUser.isAdmin;

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
            }

            ViewBag.userRules = userRules.Select(ru => ru.Rule_Name).ToList();
            ViewBag.userPlants = userPlants;
            ViewBag.currentPlantId = string.IsNullOrEmpty(plantId) ? currentPlantId : plantId;

            IEnumerable<AllocationViewModel> avm = AllocationModel.Instance.GetLastAllocationOfDeviceByPlantId(plantId);
            return View(avm);
        }

        // GET: Allocations/Details/5
        [CustomAuthorize]
        public ActionResult Details(Guid? id)
        {
            Device device = null;
            AllocationViewModel.Representation lastAllocationOfDevice = null;
            IEnumerable<AllocationViewModel.Representation> allocationsOfDevice = null;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            device = en.Devices.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }

            allocationsOfDevice = AllocationModel.Instance.get_AllocationsByDeviceId(id);
            bool hasAllocation = allocationsOfDevice.Any();
            ViewBag.hasAllocation = hasAllocation;

            if (hasAllocation)
            {
                lastAllocationOfDevice = allocationsOfDevice.OrderByDescending(a => a.Allocation.Create_Date).FirstOrDefault();
            }

            AllocationViewModel.Details vm = new AllocationViewModel.Details(device, lastAllocationOfDevice, allocationsOfDevice);

            return View(vm);
        }

        // GET: Allocations/Create
        [CustomAuthorize]
        public ActionResult Create(Guid? id)
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

            ViewBag.Device = device;

            string empPlantID = _appUser.GetPlantID();

            ViewBag.Deliver = currentEmployeeID;
            ViewBag.DeliverName = en.Employees.FirstOrDefault(e => e.Emp_CJ == currentEmployeeID).Employee_Name;

            List<EmployeeFieldModel> employeeFields = EmployeeModel.Instance.EmployeeFieldsByPlant(empPlantID);
            List<SelectListItem> SLIEmployee = new List<SelectListItem>();
            SLIEmployee.AddRange(new SelectList(employeeFields, "Emp_CJ", "EmployeeField", ""));
            SLIEmployee.Insert(0, new SelectListItem { Text = "None", Value = "" });
            ViewBag.Receiver = SLIEmployee;

            List<SelectListItem> Department_Id = new List<SelectListItem>();
            Department_Id.AddRange(new SelectList(en.Departments.Where(d => d.Plant_Id == empPlantID), "Department_Id", "Department_Name", ""));
            Department_Id.Insert(0, new SelectListItem { Text = "None", Value = "" });
            ViewBag.Department_Id = Department_Id;

            ViewBag.Allocation_Code = AllocationModel.Instance.Generate_AllocationCode(device.Device_Id, device.Device_Code);

            return View();
        }

        // POST: Allocations/Create
        [HttpPost]
        [CustomAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Allocation allocation)
        {
            string empPlantID = _appUser.GetPlantID();

            if (allocation.Return_Date != null)
            {
                DateTime deliveryD = allocation.Delivery_Date.GetValueOrDefault();
                DateTime returnD = allocation.Return_Date.GetValueOrDefault();

                if (deliveryD > returnD)
                {
                    ModelState.AddModelError("Return_Date", "Return Date cannot be less than date of purchase");
                }
            }
            if (ModelState.IsValid)
            {
                allocation.Allocation_Id = Guid.NewGuid();
                allocation.Create_Date = DateTime.Now;
                allocation.Flag_ReAllocation = true;

                string filePath = AllocationHelper.Instance.CreateQRCode(allocation);
                allocation.QRCodeFile = filePath;

                en.Allocations.Add(allocation);
                en.SaveChanges();

                return RedirectToAction("Index");
            }

            Device device = en.Devices.Find(allocation.Device_Id);
            ViewBag.Device = device;

            ViewBag.Deliver = currentEmployeeID;
            ViewBag.DeliverName = en.Employees.FirstOrDefault(e => e.Emp_CJ == currentEmployeeID).Employee_Name;

            List<EmployeeFieldModel> employeeFields = EmployeeModel.Instance.EmployeeFieldsByPlant(empPlantID);
            List<SelectListItem> SLIEmployee = new List<SelectListItem>();
            SLIEmployee.AddRange(new SelectList(employeeFields, "Emp_CJ", "EmployeeField", ""));
            SLIEmployee.Insert(0, new SelectListItem { Text = "None", Value = "" });
            ViewBag.Receiver = SLIEmployee;

            List<SelectListItem> Department_Id = new List<SelectListItem>();
            Department_Id.AddRange(new SelectList(en.Departments.Where(d => d.Plant_Id == empPlantID), "Department_Id", "Department_Name", ""));
            Department_Id.Insert(0, new SelectListItem { Text = "None", Value = "" });
            ViewBag.Department_Id = Department_Id;

            ViewBag.Allocation_Code = AllocationModel.Instance.Generate_AllocationCode(device.Device_Id, device.Device_Code);

            return View(allocation);
        }

        // GET: Allocations/Edit/5
        [CustomAuthorize]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Allocation allocation = en.Allocations.Find(id);
            if (allocation == null)
            {
                return HttpNotFound();
            }
            string empPlantID = _appUser.GetPlantID();

            ViewBag.DeliverName = en.Employees.FirstOrDefault(e => e.Emp_CJ == allocation.Deliver).Employee_Name;

            ViewBag.DeviceTypeId = en.Devices.FirstOrDefault(d => d.Device_Id == allocation.Device_Id).Device_Type_Id;

            List<EmployeeFieldModel> employeeFields = EmployeeModel.Instance.EmployeeFieldsByPlant(empPlantID);
            List<SelectListItem> Receiver = new List<SelectListItem>();
            Receiver.AddRange(new SelectList(employeeFields, "Emp_CJ", "EmployeeField", allocation.Receiver));
            Receiver.Insert(0, new SelectListItem { Text = "None", Value = "" });
            ViewBag.Receiver = Receiver;

            List<SelectListItem> Department_Id = new List<SelectListItem>();
            Department_Id.AddRange(new SelectList(en.Departments.Where(d => d.Plant_Id == empPlantID), "Department_Id", "Department_Name", allocation.Department_Id));
            Department_Id.Insert(0, new SelectListItem { Text = "None", Value = "" });
            ViewBag.Department_Id = Department_Id;

            List<AllocationViewModel.Representation> representations = AllocationModel.Instance.get_AllocationsByDeviceId(allocation.Device_Id);
            ViewBag.Representations = representations;

            return View(allocation);
        }

        // POST: Allocations/Edit/5
        [HttpPost]
        [CustomAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Allocation allocation)
        {
            if (allocation.Return_Date != null)
            {
                DateTime deliveryD = allocation.Delivery_Date.GetValueOrDefault();
                DateTime returnD = allocation.Return_Date.GetValueOrDefault();

                if (deliveryD > returnD)
                {
                    ModelState.AddModelError("Return_Date", "Return Date cannot be less than date of purchase");
                }
            }
            if (ModelState.IsValid)
            {
                string filePath = AllocationHelper.Instance.CreateQRCode(allocation);
                allocation.QRCodeFile = filePath;
                en.Entry(allocation).State = EntityState.Modified;

                Device d = en.Devices.Find(allocation.Device_Id);
                d.QRCodeFile = filePath;
                en.Entry(d).State = EntityState.Modified;

                en.SaveChanges();
                return RedirectToAction("Index");
            }

            string empPlantID = _appUser.GetPlantID();
            ViewBag.Plant_Name = _appUser.GetPlantName();

            ViewBag.DeliverName = en.Employees.FirstOrDefault(e => e.Emp_CJ == allocation.Deliver).Employee_Name;

            ViewBag.DeviceTypeId = en.Devices.FirstOrDefault(d => d.Device_Id == allocation.Device_Id).Device_Type_Id;

            List<EmployeeFieldModel> employeeFields = EmployeeModel.Instance.EmployeeFieldsByPlant(empPlantID);
            List<SelectListItem> Receiver = new List<SelectListItem>();
            Receiver.AddRange(new SelectList(employeeFields, "Emp_CJ", "EmployeeField", allocation.Receiver));
            Receiver.Insert(0, new SelectListItem { Text = "None", Value = "" });
            ViewBag.Receiver = Receiver;

            List<SelectListItem> Department_Id = new List<SelectListItem>();
            Department_Id.AddRange(new SelectList(en.Departments.Where(d => d.Plant_Id == empPlantID), "Department_Id", "Department_Name", allocation.Department_Id));
            Department_Id.Insert(0, new SelectListItem { Text = "None", Value = "" });
            ViewBag.Department_Id = Department_Id;

            List<AllocationViewModel.Representation> representations = AllocationModel.Instance.get_AllocationsByDeviceId(allocation.Device_Id);
            ViewBag.Representations= representations;

            return View(allocation);
        }

        // GET: Allocations/Revoke/5
        [CustomAuthorize]
        public ActionResult Revoke(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Allocation allocation = en.Allocations.Find(id);
            if (allocation == null)
            {
                return HttpNotFound();
            }
            string empPlantID = _appUser.GetPlantID();

            ViewBag.DeliverName = en.Employees.FirstOrDefault(e => e.Emp_CJ == allocation.Deliver).Employee_Name;

            ViewBag.ReceiverName = en.Employees.FirstOrDefault(e => e.Emp_CJ == allocation.Receiver).Employee_Name;

            ViewBag.DeviceTypeId = en.Devices.FirstOrDefault(d => d.Device_Id == allocation.Device_Id).Device_Type_Id;

            ViewBag.DepartmentName = DepartmentModel.Instance.getDeptNameByDeptId(allocation.Department_Id);

            List<AllocationViewModel.Representation> representations = AllocationModel.Instance.get_AllocationsByDeviceId(allocation.Device_Id);
            ViewBag.Representations = representations;

            return View(allocation);
        }

        // POST: Allocations/Revoke/5
        [HttpPost]
        [CustomAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult Revoke(Allocation allocation)
        {
            if (allocation.Return_Date != null)
            {
                DateTime deliveryD = allocation.Delivery_Date.GetValueOrDefault();
                DateTime returnD = allocation.Return_Date.GetValueOrDefault();

                if (deliveryD > returnD)
                {
                    ModelState.AddModelError("Return_Date", "Return Date cannot be less than date of purchase");
                }
            }
            else
            {
                ModelState.AddModelError("Return_Date", "The Revoke Day field is required.");
            }
            if (ModelState.IsValid)
            {
                string filePath = AllocationHelper.Instance.CreateQRCode(allocation);
                allocation.QRCodeFile = filePath;

                en.Entry(allocation).State = EntityState.Modified;
                en.SaveChanges();
                return RedirectToAction("Index");
            }
            string empPlantID = _appUser.GetPlantID();

            ViewBag.Plant_Name = _appUser.GetPlantName();

            ViewBag.DeliverName = en.Employees.FirstOrDefault(e => e.Emp_CJ == allocation.Deliver).Employee_Name;

            ViewBag.ReceiverName = en.Employees.FirstOrDefault(e => e.Emp_CJ == allocation.Receiver).Employee_Name;

            ViewBag.DeviceTypeId = en.Devices.FirstOrDefault(d => d.Device_Id == allocation.Device_Id).Device_Type_Id;

            ViewBag.DepartmentName = DepartmentModel.Instance.getDeptNameByDeptId(allocation.Department_Id);

            List<AllocationViewModel.Representation> representations = AllocationModel.Instance.get_AllocationsByDeviceId(allocation.Device_Id);
            ViewBag.Representations = representations;

            return View(allocation);
        }

        // GET: Allocations/ReAllocation
        [CustomAuthorize]
        public ActionResult ReAllocation(Guid? id)
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

            ViewBag.Device = device;

            string empPlantID = _appUser.GetPlantID();

            ViewBag.Deliver = currentEmployeeID;
            ViewBag.DeliverName = en.Employees.FirstOrDefault(e => e.Emp_CJ == currentEmployeeID).Employee_Name;

            List<EmployeeFieldModel> employeeFields = EmployeeModel.Instance.EmployeeFieldsByPlant(empPlantID);
            List<SelectListItem> SLIEmployee = new List<SelectListItem>();
            SLIEmployee.AddRange(new SelectList(employeeFields, "Emp_CJ", "EmployeeField", ""));
            SLIEmployee.Insert(0, new SelectListItem { Text = "None", Value = "" });
            ViewBag.Receiver = SLIEmployee;

            List<SelectListItem> Department_Id = new List<SelectListItem>();
            Department_Id.AddRange(new SelectList(en.Departments.Where(d => d.Plant_Id == empPlantID), "Department_Id", "Department_Name", ""));
            Department_Id.Insert(0, new SelectListItem { Text = "None", Value = "" });

            ViewBag.Department_Id = Department_Id;

            List<AllocationViewModel.Representation> representations = AllocationModel.Instance.get_AllocationsByDeviceId(id);
            ViewBag.Representations = representations;

            ViewBag.Allocation_Code = AllocationModel.Instance.Generate_AllocationCode(device.Device_Id, device.Device_Code);

            return View();
        }

        // POST: Allocations/ReAllocation
        [HttpPost]
        [CustomAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult ReAllocation(Allocation allocation)
        {
            if (allocation.Return_Date != null)
            {
                DateTime deliveryD = allocation.Delivery_Date.GetValueOrDefault();
                DateTime returnD = allocation.Return_Date.GetValueOrDefault();

                if (deliveryD > returnD)
                {
                    ModelState.AddModelError("Return_Date", "Return Date cannot be less than date of purchase");
                }
            }

            if (ModelState.IsValid)
            {
                allocation.Allocation_Id = Guid.NewGuid();
                allocation.Create_Date = DateTime.Now;
                allocation.Flag_ReAllocation = true;

                string filePath = AllocationHelper.Instance.CreateQRCode(allocation);
                allocation.QRCodeFile = filePath;

                en.Allocations.Add(allocation);
                en.SaveChanges();
                return RedirectToAction("Index");
            }
            string empPlantID = _appUser.GetPlantID();
            Device device = en.Devices.Find(allocation.Device_Id);
            ViewBag.Device = device;

            ViewBag.Deliver = currentEmployeeID;
            ViewBag.DeliverName = _appUser.EmployeeName;

            List<EmployeeFieldModel> employeeFields = EmployeeModel.Instance.EmployeeFieldsByPlant(empPlantID);
            List<SelectListItem> SLIEmployee = new List<SelectListItem>();
            SLIEmployee.AddRange(new SelectList(employeeFields, "Emp_CJ", "EmployeeField", ""));
            SLIEmployee.Insert(0, new SelectListItem { Text = "None", Value = "" });
            ViewBag.Receiver = SLIEmployee;

            List<SelectListItem> Department_Id = new List<SelectListItem>();
            Department_Id.AddRange(new SelectList(en.Departments.Where(d => d.Plant_Id == empPlantID), "Department_Id", "Department_Name", ""));
            Department_Id.Insert(0, new SelectListItem { Text = "None", Value = "" });

            ViewBag.Department_Id = Department_Id;

            List<AllocationViewModel.Representation> representations = AllocationModel.Instance.get_AllocationsByDeviceId(allocation.Device_Id);
            ViewBag.Representations = representations;

            ViewBag.Allocation_Code = AllocationModel.Instance.Generate_AllocationCode(device.Device_Id, device.Device_Code);

            return View(allocation);
        }

        [HttpGet]
        public JsonResult getDeptVal(string receiverId)
        {
            string deptId = null;
            deptId = en.Employees.FirstOrDefault(e => e.Emp_CJ == receiverId).Department_ID;
            return Json(deptId, JsonRequestBehavior.AllowGet);
        }

        //[CustomAuthorize]
        public FileContentResult Download(string plantId)
        {
            var devices = AllocationModel.Instance.GetLastAllocationOfDeviceByPlantId(plantId);

            // Sort by Code
            devices = devices.OrderBy(model => model.Device.Device_Code);

            List<AllocationViewModel.ExcelReport> devices_ExcelReport = new List<AllocationViewModel.ExcelReport>();
            foreach (var item in devices)
            {
                AllocationViewModel.ExcelReport entry = new AllocationViewModel.ExcelReport(item.Device, item.Allocation,
                    item.Device?.Device_Type?.Device_Type_Name, item.Deliver_Name, item.Receiver_Name, item.Deliver_Name,
                    item.Allocation?.Department?.Plant?.Plant_Name);
                devices_ExcelReport.Add(entry);
            }

            //Col need format date
            List<int> colsDate = new List<int>()
                {
                    5 , 15 ,18 ,22 ,23
                };

            var stream = ExcelHelper.Instance.CreateExcelFile(null, devices_ExcelReport, ExcelTitle.Instance.DevicesExcelReport(), colsDate);
            var buffer = stream as MemoryStream;

            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "IT Devices.xlsx";
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
