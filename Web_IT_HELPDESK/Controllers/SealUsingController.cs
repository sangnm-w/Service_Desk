using System;
using System.Data;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
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

            from_date = DateTime.ParseExact("01/" + DateTime.Now.ToString("MM/yyyy"), "dd/MM/yyyy", null);
            to_date = from_date.AddMonths(1).AddSeconds(-1);

            //+++++++ Repair for Right_Management +++++++
            //if (!Admin)
            //{
            //    if (!hasPermission)
            //    {
            //        sealusings = sealusings.Where(s => s.DepartmentId == curr_DeptID && s.Plant == curr_PlantID).ToList();
            //    }
            //}

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
                      (s, d) => new SealUsingViewModel()
                      {
                          SealUsing = s,
                          DeptName = d.Department_Name
                      }
                      )
                .ToList();

            return View(suVM);
        }

        private DateTime to_date { get; set; }
        private DateTime from_date { get; set; }
        [Authorize]
        [HttpPost]
        public ActionResult Index(string searchString, string _datetime, int? page)
        {
            string curr_PlantID = CurrentUser.Instance.User.Plant_Id;
            string curr_DeptID = CurrentUser.Instance.User.Department_Id;

            from_date = DateTime.ParseExact("01/" + _datetime, "dd/MM/yyyy", null);
            to_date = from_date.AddMonths(1).AddSeconds(-1);

            var sealusings = en.Seal_Using
                .Where(s => s.Del != true
                        && s.Date >= from_date
                        && s.Date <= to_date
                ).ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                sealusings = sealusings.Where(s => (s.DepartmentId.Contains(searchString) || s.Employee_name.Contains(searchString))).ToList();
            }

            var suVM = sealusings
              .Join(en.Departments,
                    s => new { deptID = s.DepartmentId, plantID = s.Plant },
                    d => new { deptID = d.Department_Id, plantID = d.Plant_Id },
                    (s, d) => new SealUsingViewModel()
                    {
                        SealUsing = s,
                        DeptName = d.Department_Name
                    }
                    )
              .ToList();

            //+++++++ Repair for Right_Management +++++++
            //if (!Admin)
            //{
            //    if (!hasPermission)
            //    {
            //        sealusings = sealusings.Where(s => s.DepartmentId == curr_DeptID && s.Plant == curr_PlantID).ToList();
            //    }
            //}

            return View(suVM);
        }

        //
        // GET: /SealUsing/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /SealUsing/Create

        [Authorize]
        public ActionResult Create()
        {
            SealUsingViewModel suVM = new SealUsingViewModel();

            suVM.SealUsing = new Seal_Using();
            //suVM.DeptName =
            return View(suVM);
        }

        //
        // POST: /SealUsing/Create

        private string subject { get; set; }
        private string body { get; set; }
        private string confirm_status { get; set; }

        [HttpPost]
        public string Create(Seal_Using seal_using)
        {
            string result = "";
            string plantid = CurrentUser.Instance.User.Plant_Id;
            var dept = from i in en.Departments where i.Department_Id == seal_using.DepartmentId && i.Plant_Id == plantid select i.Department_Name;
            seal_using.Employee_ID = CurrentUser.Instance.User.Emp_CJ;
            en.Seal_Using.Add(seal_using);

            try
            {
                en.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);
                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                result = string.Format("Thông báo! <br /> <br />" +
                                              "Có lỗi khi tạo yêu cầu. Liên hệ hỗ trợ: minhsang.it@cjvina.com <br />" +
                                              "************** Cám ơn đã sử dụng chương trình **************");
                return result;
            }

            Employee userRequest = en.Employees.FirstOrDefault(e => e.Emp_CJ == seal_using.Employee_ID);
            bool resultMailing = SealUsingHelper.Instance.sendSealUsingEmail(seal_using, 1, userRequest);

            if (resultMailing)
            {
                result = string.Format("Thông báo! <br /> <br />" +
                                              "Đã gởi email xác nhận đến trưởng phòng nhân sự để duyệt sử dụng con dấu <br />" +
                                              "************** Cám ơn đã sử dụng chương trình **************");
            }
            else
            {
                result = string.Format("Thông báo! <br /> <br />" +
                                              "Có lỗi khi gửi mail. Liên hệ hỗ trợ: minhsang.it@cjvina.com <br />" +
                                              "************** Cám ơn đã sử dụng chương trình **************");
            }

            return result;
        }

        public string Resend(int? id)
        {
            var seal_using = en.Seal_Using.Where(i => i.Id == id).FirstOrDefault();
            string result = "";

            Employee userRequest = en.Employees.FirstOrDefault(e => e.Emp_CJ == seal_using.Employee_ID);
            bool resultMailing = SealUsingHelper.Instance.sendSealUsingEmail(seal_using, 2, userRequest);
            if (resultMailing)
            {
                result = string.Format("Thông báo! <br /> <br />" +
                                              "Đã gởi lại email xác nhận đến trưởng phòng nhân sự để duyệt sử dụng con dấu <br />" +
                                              "************** Cám ơn đã sử dụng chương trình **************");
            }
            else
            {
                result = string.Format("Thông báo! <br /> <br />" +
                                              "Có lỗi khi gửi mail. Liên hệ hỗ trợ: minhsang.it@cjvina.com <br />" +
                                              "************** Cám ơn đã sử dụng chương trình **************");
            }
            return result;
        }

        public ActionResult Edit(int? id)
        {
            Seal_Using seal_using = en.Seal_Using.Find(id);
            ViewBag.Department_confirm_date = DateTime.Now;
            ViewBag.DepartmentId = seal_using.DepartmentId;
            ViewBag.Plant = seal_using.Plant;
            ViewBag.DepartmentName = en.Departments.Where(o => o.Department_Id == seal_using.DepartmentId
                                                         && o.Plant_Id == seal_using.Plant).Select(i => i.Department_Name).SingleOrDefault();
            /*get employee name*/
            ViewBag.User_name = en.Employees.Where(f => (f.Emp_CJ == CurrentUser.Instance.User.Emp_CJ && f.Plant_Id == seal_using.DepartmentId.ToString())).Select(f => f.EmployeeName).SingleOrDefault();
            /*get employee name*/
            return View(seal_using);
        }

        //
        // POST: /SealUsing/Edit/5

        [HttpPost]
        public string Edit(Seal_Using seal_using)
        {
            string result;
            string departmentName = DepartmentModel.Instance.getDeptName(seal_using.Plant, seal_using.DepartmentId);

            Employee userRequest = en.Employees.FirstOrDefault(e => e.Emp_CJ == seal_using.Employee_ID);
            if (seal_using.Department_confirm == true)
            {
                en.Entry(seal_using).State = System.Data.Entity.EntityState.Modified;
                en.SaveChanges();

                bool resultMailing = SealUsingHelper.Instance.sendSealUsingEmail(seal_using, 5, userRequest);
                if (resultMailing)
                {
                    result = string.Format("Thông báo! <br /> Trạng thái được duyệt đã chọn: Đồng ý xác nhận <br />" +
                                                      "Bộ phận: " + departmentName + " <br />" +
                                                      "Email xác nhận đã được gởi sang bộ phận quản lý con dấu.");
                }
                else
                {
                    result = string.Format("Thông báo! <br /> <br />" +
                                                  "Có lỗi khi gửi mail. Liên hệ hỗ trợ: minhsang.it@cjvina.com <br />" +
                                                  "************** Cám ơn đã sử dụng chương trình **************");
                }
            }
            else
            {
                bool resultMailing = SealUsingHelper.Instance.sendSealUsingEmail(seal_using, 6, userRequest);
                if (resultMailing)
                {
                    result = string.Format("Thông báo! <br /> Trạng thái được duyệt đã chọn: KHÔNG ĐỒNG Ý XÁC NHẬN <br />" +
                                                  "Bộ phận: " + departmentName + " <br />" +
                                                  "Email xác nhận không được gởi sang bộ phận quản lý con dấu.");
                }
                else
                {
                    result = string.Format("Thông báo! <br /> <br />" +
                                                  "Có lỗi khi gửi mail. Liên hệ hỗ trợ: minhsang.it@cjvina.com <br />" +
                                                  "************** Cám ơn đã sử dụng chương trình **************");
                }
            }
            return result;
        }

        public ActionResult Confirm(int? id)
        {
            Seal_Using seal_using = en.Seal_Using.Find(id);
            ViewBag.Employee_Seal_keep = DateTime.Now;
            ViewBag.DepartmentId = ViewBag.DepartmentId = seal_using.DepartmentId;
            ViewBag.Plant = seal_using.Plant;
            ViewBag.DepartmentName = en.Departments.Where(o => o.Department_Id == seal_using.DepartmentId
                                                         && o.Plant_Id == seal_using.Plant).Select(i => i.Department_Name).SingleOrDefault();
            /*get employee name*/
            ViewBag.User_name = en.Employees.Where(f => (f.Emp_CJ == CurrentUser.Instance.User.Emp_CJ && f.Plant_Id == seal_using.DepartmentId.ToString())).Select(f => f.EmployeeName).SingleOrDefault();
            /*get employee name*/
            return View(seal_using);
        }

        [HttpPost]
        public ActionResult Confirm(Seal_Using seal_using)
        {
            seal_using.Employee_Seal_keep = DateTime.Now;
            en.Entry(seal_using).State = System.Data.Entity.EntityState.Modified;
            en.SaveChanges();
            return RedirectToAction("Index", "SealUsing");
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

        // call class information to send mail
        Information inf = new Information();
    }
}