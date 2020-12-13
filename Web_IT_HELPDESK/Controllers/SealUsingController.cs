﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Web_IT_HELPDESK.Controllers.ObjectManager;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.ViewModels;

namespace Web_IT_HELPDESK.Controllers
{
    public class SealUsingController : Controller
    {
        //
        // GET: /SealUsing/
        ServiceDeskEntities en = new ServiceDeskEntities();

        [Authorize]
        public ActionResult Index()
        {
            string curr_PlantID = CurrentUser.Instance.User.Plant_Id;
            string curr_DeptID = CurrentUser.Instance.User.Department_Id;

            DateTime from_date = DateTime.ParseExact("01/" + DateTime.Now.ToString("MM/yyyy"), "dd/MM/yyyy", null);
            DateTime to_date = from_date.AddMonths(1).AddSeconds(-1);

            //+++++++ Repair for Right_Management +++++++
            //if (!Admin)
            //{
            //    if (!hasPermission)
            //    {
            //        sealusings = sealusings.Where(s => s.DepartmentId == curr_DeptID && s.Plant == curr_PlantID).ToList();
            //    }
            //}
            bool currUserIsManager = en.Departments.FirstOrDefault(d => d.Plant_Id == curr_PlantID && d.Department_Id == curr_DeptID && d.Manager_Id == CurrentUser.Instance.User.Emp_CJ) != null ? true : false;

            var suVM = en.Seal_Using
                .Where(s => s.Del != true
                        && s.Plant == curr_PlantID
                        && s.DepartmentId == curr_DeptID
                        && s.Date >= from_date
                        && s.Date <= to_date
                      )
                .Join(en.Departments,
                      s => new { deptID = s.DepartmentId, plantID = s.Plant },
                      d => new { deptID = d.Department_Id, plantID = d.Plant_Id },
                      (s, d) => new SealUsingViewModel.IndexSealUsing()
                      {
                          SealUsing = s,
                          DeptName = d.Department_Name
                      }
                      )
                .ToList();

            if (currUserIsManager == false)
            {
                suVM = suVM.Where(i => i.SealUsing.Employee_ID == CurrentUser.Instance.User.Emp_CJ).ToList();
            }
            return View(suVM);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(string searchString, string _datetime, int? page)
        {
            string curr_PlantID = CurrentUser.Instance.User.Plant_Id;
            string curr_DeptID = CurrentUser.Instance.User.Department_Id;

            DateTime from_date = DateTime.ParseExact("01/" + _datetime, "dd/MM/yyyy", null);
            DateTime to_date = from_date.AddMonths(1).AddSeconds(-1);

            var sealusings = en.Seal_Using
                .Where(s => s.Del != true
                        && s.Plant == curr_PlantID
                        && s.DepartmentId == curr_DeptID
                        && s.Date >= from_date
                        && s.Date <= to_date
                ).ToList();
            var suVM = sealusings
              .Join(en.Departments,
                    s => new { deptID = s.DepartmentId, plantID = s.Plant },
                    d => new { deptID = d.Department_Id, plantID = d.Plant_Id },
                    (s, d) => new SealUsingViewModel.IndexSealUsing()
                    {
                        SealUsing = s,
                        DeptName = d.Department_Name
                    })
              .ToList();

            bool currUserIsManager = en.Departments.FirstOrDefault(d => d.Plant_Id == curr_PlantID && d.Department_Id == curr_DeptID && d.Manager_Id == CurrentUser.Instance.User.Emp_CJ) != null ? true : false;

            //+++++++ Repair for Right_Management +++++++
            //if (!Admin)
            //{
            //    if (!hasPermission)
            //    {
            //        sealusings = sealusings.Where(s => s.DepartmentId == curr_DeptID && s.Plant == curr_PlantID).ToList();
            //    }
            //}
            if (currUserIsManager == false)
            {
                suVM = suVM.Where(i => i.SealUsing.Employee_ID == CurrentUser.Instance.User.Emp_CJ).ToList();
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                suVM = suVM.Where(s => (s.DeptName.Trim().ToUpper().Contains(searchString.Trim().ToUpper())
                                         || s.SealUsing.Employee_name.Trim().ToUpper().Contains(searchString.Trim().ToUpper()))).ToList();
            }
            return View(suVM);
        }

        //
        // GET: /SealUsing/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: /SealUsing/Create

        [Authorize]
        public ActionResult Create()
        {
            SealUsingViewModel.CreateSealUsing csuVM = new SealUsingViewModel.CreateSealUsing();

            ViewBag.ModalState = "hide";
            ViewBag.Message = "";
            return View(csuVM);
        }

        // POST: /SealUsing/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SealUsingViewModel.CreateSealUsing csuVM)
        {
            string result = "";
            if (ModelState.IsValid)
            {
                Seal_Using su = new Seal_Using();
                su = csuVM.CreateSealUsing_To_SealUsing(su);

                try
                {
                    en.Seal_Using.Add(su);
                    en.SaveChanges();
                    Employee userRequest = en.Employees.FirstOrDefault(e => e.Emp_CJ == su.Employee_ID);
                    bool resultMailing = SealUsingHelper.Instance.sendSealUsingEmail(su, 1, userRequest);
                    if (resultMailing)
                    {
                        result = string.Format("Đã gửi email xác nhận đến trưởng phòng để duyệt sử dụng con dấu.");
                    }
                    else
                    {
                        result = string.Format("Không gửi được email. Vui lòng kiểm tra lại. Liên hệ hỗ trợ: minhsang.it@cjvina.com");
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);
                    var fullErrorMessage = string.Join("; ", errorMessages);
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                    result = string.Format("Có lỗi khi tạo đăng ký. Liên hệ hỗ trợ: minhsang.it@cjvina.com");
                }
            }
            if (!string.IsNullOrEmpty(result))
            {
                ViewBag.ModalState = "show";
                ViewBag.Message = result;
            }
            else
            {
                ViewBag.ModalState = "hide";
                ViewBag.Message = "";
            }
            return View(csuVM);
        }
        [Authorize]
        public ActionResult Resend(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var seal_Using = en.Seal_Using.FirstOrDefault(s => s.Id == id);

            if (seal_Using == null)
            {
                return HttpNotFound();
            }

            SealUsingViewModel.EditSealUsing esuVM = new SealUsingViewModel.EditSealUsing(seal_Using);

            string result = "";

            Employee userRequest = en.Employees.FirstOrDefault(e => e.Emp_CJ == seal_Using.Employee_ID);
            bool resultMailing = SealUsingHelper.Instance.sendSealUsingEmail(seal_Using, 2, userRequest);
            if (resultMailing)
            {
                result = string.Format("Đã gửi lại email xác nhận đến trưởng phòng để duyệt sử dụng con dấu.");
            }
            else
            {
                result = string.Format("Không gửi được email. Vui lòng kiểm tra lại. Liên hệ hỗ trợ: minhsang.it@cjvina.com");
            }

            if (!string.IsNullOrEmpty(result))
            {
                ViewBag.ModalState = "show";
                ViewBag.Message = result;
            }
            else
            {
                ViewBag.ModalState = "hide";
                ViewBag.Message = "";
            }
            return View("Index", esuVM);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var seal_Using = en.Seal_Using.FirstOrDefault(s => s.Id == id);

            if (seal_Using == null)
            {
                return HttpNotFound();
            }

            SealUsingViewModel.EditSealUsing esuVM = new SealUsingViewModel.EditSealUsing(seal_Using);

            ViewBag.ModalState = "hide";
            ViewBag.Message = "";
            return View(esuVM);
        }

        //POST: /SealUsing/Edit/5
        [HttpPost]
        public ActionResult Edit(SealUsingViewModel.EditSealUsing model, bool approved)
        {
            string result = "";
            var sealUsing = en.Seal_Using.FirstOrDefault(s => s.Id == model.Id);
            if (sealUsing != default(Seal_Using))
            {
                sealUsing = model.EditSealUsing_To_SealUsing(sealUsing);
                sealUsing.Department_confirm = approved;
            }

            string deptName = DepartmentModel.Instance.getDeptName(sealUsing.Plant, sealUsing.DepartmentId);
            if (ModelState.IsValid)
            {
                Employee userRequest = en.Employees.FirstOrDefault(e => e.Emp_CJ == sealUsing.Employee_ID);

                try
                {
                    en.Entry(sealUsing).State = System.Data.Entity.EntityState.Modified;
                    en.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);
                    var fullErrorMessage = string.Join("; ", errorMessages);
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                    result = string.Format("Có lỗi khi xác nhận đăng ký. Liên hệ hỗ trợ: minhsang.it@cjvina.com");
                    ViewBag.ModalState = "show";
                    ViewBag.Message = result;
                    return View(model);
                }

                if (sealUsing.Department_confirm == true)
                {
                    bool resultMailing = SealUsingHelper.Instance.sendSealUsingEmail(sealUsing, 5, userRequest);
                    if (resultMailing)
                    {
                        result = string.Format("Trạng thái được duyệt đã chọn: Đồng ý xác nhận. Email xác nhận đã được gởi sang bộ phận quản lý con dấu.");
                    }
                    else
                    {
                        result = string.Format("Không gửi được email.Vui lòng kiểm tra lại. Liên hệ hỗ trợ: minhsang.it@cjvina.com");
                    }
                }
                else
                {
                    bool resultMailing = SealUsingHelper.Instance.sendSealUsingEmail(sealUsing, 6, userRequest);
                    if (resultMailing)
                    {
                        result = string.Format("Trạng thái được duyệt đã chọn: KHÔNG ĐỒNG Ý XÁC NHẬN. Email xác nhận không được gởi sang bộ phận quản lý con dấu.");
                    }
                    else
                    {
                        result = string.Format("Có lỗi khi gửi mail. Liên hệ hỗ trợ: minhsang.it@cjvina.com");
                    }
                }
            }
            if (!string.IsNullOrEmpty(result))
            {
                ViewBag.ModalState = "show";
                ViewBag.Message = result;
            }
            else
            {
                ViewBag.ModalState = "hide";
                ViewBag.Message = "";
            }
            return View(model);
        }

        public ActionResult Confirm(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var seal_Using = en.Seal_Using.FirstOrDefault(s => s.Id == id);

            if (seal_Using == null)
            {
                return HttpNotFound();
            }

            SealUsingViewModel.ConfirmSealUsing esuVM = new SealUsingViewModel.ConfirmSealUsing(seal_Using);

            ViewBag.ModalState = "hide";
            ViewBag.Message = "";
            return View(esuVM);
        }

        [HttpPost]
        public ActionResult Confirm(SealUsingViewModel.ConfirmSealUsing model, bool HRapproved)
        {
            string result = "";
            var sealUsing = en.Seal_Using.FirstOrDefault(s => s.Id == model.Id);
            if (sealUsing != default(Seal_Using))
            {
                sealUsing = model.ConfirmSealUsing_To_SealUsing(sealUsing);
                sealUsing.Employee_Seal_keep_confrim = HRapproved;
            }

            string deptName = DepartmentModel.Instance.getDeptName(sealUsing.Plant, sealUsing.DepartmentId);
            if (ModelState.IsValid)
            {
                Employee userRequest = en.Employees.FirstOrDefault(e => e.Emp_CJ == sealUsing.Employee_ID);

                try
                {
                    en.Entry(sealUsing).State = System.Data.Entity.EntityState.Modified;
                    en.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);
                    var fullErrorMessage = string.Join("; ", errorMessages);
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                    result = string.Format("Có lỗi khi xác nhận đăng ký. Liên hệ hỗ trợ: minhsang.it@cjvina.com");
                    ViewBag.ModalState = "show";
                    ViewBag.Message = result;
                    return View(model);
                }

                if (sealUsing.Employee_Seal_keep_confrim == true)
                {
                    bool resultMailing = SealUsingHelper.Instance.sendSealUsingEmail(sealUsing, 6, userRequest);
                    if (resultMailing)
                    {
                        result = string.Format("Trạng thái được duyệt đã chọn: Đồng ý xác nhận.");
                    }
                    else
                    {
                        result = string.Format("Không gửi được email.Vui lòng kiểm tra lại.Liên hệ hỗ trợ: minhsang.it@cjvina.com");
                    }
                }
                else
                {
                    bool resultMailing = SealUsingHelper.Instance.sendSealUsingEmail(sealUsing, 7, userRequest);
                    if (resultMailing)
                    {
                        result = string.Format("Trạng thái được duyệt đã chọn: KHÔNG ĐỒNG Ý XÁC NHẬN. ");
                    }
                    else
                    {
                        result = string.Format("Có lỗi khi gửi mail. Liên hệ hỗ trợ: minhsang.it@cjvina.com");
                    }
                }
            }
            if (!string.IsNullOrEmpty(result))
            {
                ViewBag.ModalState = "show";
                ViewBag.Message = result;
            }
            else
            {
                ViewBag.ModalState = "hide";
                ViewBag.Message = "";
            }
            return View(model);
        }

        //
        // GET: /SealUsing/Delete/5

        public ActionResult Delete(int? id)
        {
            Seal_Using seal_using = en.Seal_Using.Find(id);
            ViewBag.Department_confirm_date = DateTime.Now;
            string plant_id = CurrentUser.Instance.User.Plant_Id;
            ViewBag.DepartmentId = CurrentUser.Instance.User.Department_Id;
            return View(seal_using);
        }

        //
        // POST: /SealUsing/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int? id)
        {
            try
            {
                Seal_Using seal_using = en.Seal_Using.Find(id);
                if (seal_using == null)
                    return HttpNotFound();
                else if (seal_using.Department_confirm == false)
                {
                    seal_using.Del = true;
                    en.SaveChanges();
                }
            }
            catch
            {
            }
            return RedirectToAction("Index", "SealUsing");
        }
    }
}