using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.Models.Extensions;

namespace Web_IT_HELPDESK.Controllers
{
    public class CheckoutController : Controller
    {
        //
        // GET: /Checkout/

        ServiceDeskEntities storeDB = new ServiceDeskEntities();
        ApplicationUser _appUser = new ApplicationUser();

        public ActionResult AddressAndPayment()
        {
            return View();
        }

        private string subject { get; set; }
        private string body { get; set; }
        private string confirm_status { get; set; }

        private string session_emp = System.Web.HttpContext.Current.User.Identity.Name;
        //  Get_department_Id
        private string GetEmp_name()
        {
            string emp_name = storeDB.Employee_New.Where(f => (f.Emp_CJ == session_emp)).Select(f => f.Employee_Name).SingleOrDefault();
            //string emp_name = storeDB.Employees.Where(f => (f.EmployeeID == session_emp)).Select(f => f.EmployeeName).SingleOrDefault();
            return emp_name;
        }
        Information inf = new Information();

        [HttpPost]
        public ActionResult AddressAndPayment(FormCollection values)
        {
            var order = new Order_();
            TryUpdateModel(order);

            try
            {
                order.EmployeeID = User.Identity.Name;
                order.Del = false;
                order.Plant = _appUser.GetPlantID();
                order.Employee_Name = GetEmp_name();
                //Save Order
                storeDB.Order_.Add(order);
                storeDB.SaveChanges();

                //Process the order
                var cart = ShoppingCart.GetCart(this.HttpContext);
                cart.CreateOrder(order);

                //gởi mail đến trưởng phòng
                string result, status = "";

                var dept = from i in storeDB.Departments where i.Department_Id == _appUser.GetDepartmentID() select i.Department_Name;
                subject = "[Duyệt] - Thông tin yêu cầu văn phòng phẩm";
                result = string.Format("Thông báo! <br /> <br />" +
                                                  "Đã gởi email xác nhận!  <br />" +
                                                  "************** Thank you!!! **************");
                status = "1";
                //}
                body = "Duyệt thông tin yêu cầu \n" +
                        "   Theo đường dẫn: " + "http://52.213.3.168/Order_/Confirm/" + order.OrderId.ToString() + "\n" + "\n" +
                        "Trân trọng!" + "\n" + "\n" + "\n" +

                        "Chương trình gởi mail được bởi IT TEAM: liên hệ Nguyen Thai Binh - IT Software khi cần hỗ trợ";


                inf.email_send("user_email", "pass", _appUser.GetDepartmentID()  //"0000"
                                                        , subject, body, status, _appUser.GetPlantID());

                return RedirectToAction("Complete",
                    new { id = order.OrderId });
            }
            catch
            {
                //Invalid - redisplay with errors
                return View(order);
            }
        }

        //
        // GET: /Checkout/Complete

        public ActionResult Complete(int id)
        {
            // Validate customer owns this order
            bool isValid = storeDB.Order_.Any(
                o => o.OrderId == id &&
                o.EmployeeID == User.Identity.Name);

            if (isValid)
            {
                return View(id);
            }
            else
            {
                return View("Error");
            }
        }

    }
}
