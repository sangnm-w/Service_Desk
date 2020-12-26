﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Web_IT_HELPDESK.Commons;
using Web_IT_HELPDESK.Controllers.ObjectManager;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.Properties;
using Web_IT_HELPDESK.ViewModels;
using static Web_IT_HELPDESK.Models.AutoCompleteModel;
using EntityState = System.Data.Entity.EntityState;

namespace Web_IT_HELPDESK.Controllers
{
    public class AllocationsController : Controller
    {
        private ServiceDeskEntities en = new ServiceDeskEntities();
        private string session_emp = System.Web.HttpContext.Current.User.Identity.Name;

        // GET: Allocations
        [Authorize]
        public ActionResult Index()
        {
            string curr_plantId = en.Employees.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_Id;
            IEnumerable<AllocationViewModel> avm = AllocationModel.Instance.GetLastAllocationOfDeviceByPlantId(curr_plantId);
            return View(avm);
        }

        // GET: Allocations/Details/5
        public ActionResult Details(Guid? id)
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
            string empPlantID = en.Employees.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_Id;

            ViewBag.DeliverName = en.Employees.FirstOrDefault(e => e.Emp_CJ == allocation.Deliver).EmployeeName;
            ViewBag.ReceiverName = en.Employees.FirstOrDefault(e => e.Emp_CJ == allocation.Receiver).EmployeeName;
            ViewBag.DeviceTypeId = en.Devices.FirstOrDefault(d => d.Device_Id == allocation.Device_Id).Device_Type_Id;

            List<AllocationViewModel> allocations = AllocationModel.Instance.get_AllocationsByDeviceId(allocation.Device_Id);
            ViewBag.Allocations = allocations;

            return View(allocation);
        }

        // GET: Allocations/Create
        [Authorize]
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

            string empPlantID = en.Employees.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_Id;

            ViewBag.Deliver = session_emp;
            ViewBag.DeliverName = en.Employees.FirstOrDefault(e => e.Emp_CJ == session_emp).EmployeeName;

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
        [ValidateAntiForgeryToken]
        public ActionResult Create(Allocation allocation)
        {
            string empPlantID = CurrentUser.Instance.User.Plant_Id;

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
                allocation.Plant_Id = empPlantID;

                string filePath = AllocationHelper.Instance.CreateQRCode(allocation);
                allocation.QRCodeFile = filePath;

                en.Allocations.Add(allocation);
                en.SaveChanges();

                return RedirectToAction("Index");
            }

            Device device = en.Devices.Find(allocation.Device_Id);
            ViewBag.Device = device;

            ViewBag.Deliver = session_emp;
            ViewBag.DeliverName = en.Employees.FirstOrDefault(e => e.Emp_CJ == session_emp).EmployeeName;

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
        [Authorize]
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
            string empPlantID = en.Employees.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_Id;

            ViewBag.DeliverName = en.Employees.FirstOrDefault(e => e.Emp_CJ == allocation.Deliver).EmployeeName;

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

            List<AllocationViewModel> allocations = AllocationModel.Instance.get_AllocationsByDeviceId(allocation.Device_Id);
            ViewBag.Allocations = allocations;

            return View(allocation);
        }

        // POST: Allocations/Edit/5
        [HttpPost]
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
                allocation.Plant_Id = en.Employees.FirstOrDefault(e => e.Emp_CJ == allocation.Receiver).Plant_Id;

                string filePath = AllocationHelper.Instance.CreateQRCode(allocation);
                allocation.QRCodeFile = filePath;

                en.Entry(allocation).State = EntityState.Modified;
                en.SaveChanges();
                return RedirectToAction("Index");
            }
            string empPlantID = en.Employees.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_Id;
            ViewBag.Plant_Name = en.Departments.FirstOrDefault(d => d.Plant_Id == empPlantID).Plant_Name;

            ViewBag.DeliverName = en.Employees.FirstOrDefault(e => e.Emp_CJ == allocation.Deliver).EmployeeName;

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

            List<AllocationViewModel> allocations = AllocationModel.Instance.get_AllocationsByDeviceId(allocation.Device_Id);
            ViewBag.Allocations = allocations;

            return View(allocation);
        }

        // GET: Allocations/Revoke/5
        [Authorize]
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
            string empPlantID = en.Employees.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_Id;

            ViewBag.DeliverName = en.Employees.FirstOrDefault(e => e.Emp_CJ == allocation.Deliver).EmployeeName;

            ViewBag.ReceiverName = en.Employees.FirstOrDefault(e => e.Emp_CJ == allocation.Receiver).EmployeeName;

            ViewBag.DeviceTypeId = en.Devices.FirstOrDefault(d => d.Device_Id == allocation.Device_Id).Device_Type_Id;

            ViewBag.DepartmentName = DepartmentModel.Instance.getDeptName(allocation.Plant_Id, allocation.Department_Id);

            List<AllocationViewModel> allocations = AllocationModel.Instance.get_AllocationsByDeviceId(allocation.Device_Id);
            ViewBag.Allocations = allocations;

            return View(allocation);
        }

        // POST: Allocations/Revoke/5
        [HttpPost]
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
                allocation.Plant_Id = en.Employees.FirstOrDefault(e => e.Emp_CJ == allocation.Receiver).Plant_Id;

                string filePath = AllocationHelper.Instance.CreateQRCode(allocation);
                allocation.QRCodeFile = filePath;

                en.Entry(allocation).State = EntityState.Modified;
                en.SaveChanges();
                return RedirectToAction("Index");
            }
            string empPlantID = en.Employees.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_Id;

            ViewBag.Plant_Name = en.Departments.FirstOrDefault(d => d.Plant_Id == empPlantID).Plant_Name;

            ViewBag.DeliverName = en.Employees.FirstOrDefault(e => e.Emp_CJ == allocation.Deliver).EmployeeName;

            ViewBag.ReceiverName = en.Employees.FirstOrDefault(e => e.Emp_CJ == allocation.Receiver).EmployeeName;

            ViewBag.DeviceTypeId = en.Devices.FirstOrDefault(d => d.Device_Id == allocation.Device_Id).Device_Type_Id;

            ViewBag.DepartmentName = DepartmentModel.Instance.getDeptName(allocation.Plant_Id, allocation.Department_Id);

            List<AllocationViewModel> allocations = AllocationModel.Instance.get_AllocationsByDeviceId(allocation.Device_Id);
            ViewBag.Allocations = allocations;

            return View(allocation);
        }

        #region ActionResult Delete
        //// GET: Allocations/Delete/5
        //public ActionResult Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Allocation allocation = en.Allocations.Find(id);
        //    if (allocation == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(allocation);
        //}

        //// POST: Allocations/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(Guid id)
        //{
        //    Allocation allocation = en.Allocations.Find(id);
        //    en.Allocations.Remove(allocation);
        //    en.SaveChanges();
        //    return RedirectToAction("Index");
        //} 
        #endregion

        // GET: Allocations/ReAllocation
        [Authorize]
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

            string empPlantID = en.Employees.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_Id;

            ViewBag.Deliver = session_emp;
            ViewBag.DeliverName = en.Employees.FirstOrDefault(e => e.Emp_CJ == session_emp).EmployeeName;

            List<EmployeeFieldModel> employeeFields = EmployeeModel.Instance.EmployeeFieldsByPlant(empPlantID);
            List<SelectListItem> SLIEmployee = new List<SelectListItem>();
            SLIEmployee.AddRange(new SelectList(employeeFields, "Emp_CJ", "EmployeeField", ""));
            SLIEmployee.Insert(0, new SelectListItem { Text = "None", Value = "" });
            ViewBag.Receiver = SLIEmployee;

            List<SelectListItem> Department_Id = new List<SelectListItem>();
            Department_Id.AddRange(new SelectList(en.Departments.Where(d => d.Plant_Id == empPlantID), "Department_Id", "Department_Name", ""));
            Department_Id.Insert(0, new SelectListItem { Text = "None", Value = "" });

            ViewBag.Department_Id = Department_Id;

            List<AllocationViewModel> allocations = AllocationModel.Instance.get_AllocationsByDeviceId(device.Device_Id);
            ViewBag.Allocations = allocations;

            ViewBag.Allocation_Code = AllocationModel.Instance.Generate_AllocationCode(device.Device_Id, device.Device_Code);

            return View();
        }

        // POST: Allocations/ReAllocation

        [HttpPost]
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
                allocation.Plant_Id = en.Employees.FirstOrDefault(e => e.Emp_CJ == allocation.Receiver).Plant_Id;

                string filePath = AllocationHelper.Instance.CreateQRCode(allocation);
                allocation.QRCodeFile = filePath;

                en.Allocations.Add(allocation);
                en.SaveChanges();
                return RedirectToAction("Index");
            }
            string empPlantID = en.Employees.FirstOrDefault(e => e.Emp_CJ == session_emp).Plant_Id;
            Device device = en.Devices.Find(allocation.Device_Id);
            ViewBag.Device = device;

            ViewBag.Deliver = session_emp;
            ViewBag.DeliverName = en.Employees.FirstOrDefault(e => e.Emp_CJ == session_emp).EmployeeName;

            List<EmployeeFieldModel> employeeFields = EmployeeModel.Instance.EmployeeFieldsByPlant(empPlantID);
            List<SelectListItem> SLIEmployee = new List<SelectListItem>();
            SLIEmployee.AddRange(new SelectList(employeeFields, "Emp_CJ", "EmployeeField", ""));
            SLIEmployee.Insert(0, new SelectListItem { Text = "None", Value = "" });
            ViewBag.Receiver = SLIEmployee;

            List<SelectListItem> Department_Id = new List<SelectListItem>();
            Department_Id.AddRange(new SelectList(en.Departments.Where(d => d.Plant_Id == empPlantID), "Department_Id", "Department_Name", ""));
            Department_Id.Insert(0, new SelectListItem { Text = "None", Value = "" });

            ViewBag.Department_Id = Department_Id;

            List<AllocationViewModel> allocations = AllocationModel.Instance.get_AllocationsByDeviceId(device.Device_Id);
            ViewBag.Allocations = allocations;

            ViewBag.Allocation_Code = AllocationModel.Instance.Generate_AllocationCode(device.Device_Id, device.Device_Code);

            return View(allocation);
        }

        [HttpGet]
        public JsonResult getDeptVal(string receiverId)
        {
            string deptId = null;
            deptId = en.Employees.FirstOrDefault(e => e.Emp_CJ == receiverId).Department_Id;
            return Json(deptId, JsonRequestBehavior.AllowGet);
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
