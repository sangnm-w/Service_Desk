using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;

namespace Web_IT_HELPDESK.Controllers
{
    public class MealOrderController : Controller
    {
        //
        // GET: /MealOrder/

        ServiceDeskEntities en = new ServiceDeskEntities();
        private string session_emp = System.Web.HttpContext.Current.User.Identity.Name;
        private string v_year = "/" + DateTime.Now.ToString("yyyy");

        private DateTime to_date { get; set; }
        private DateTime from_date { get; set; }

        //GetPlant_id
        private string GetPlant_id(string v_emp)
        {
            string plant_id = en.Employees.Where(f => (f.EmployeeID == v_emp)).Select(f => f.Plant_Id).SingleOrDefault();
            return plant_id;
        }

        public ActionResult MealOrderList()
        {
            string v_plant = GetPlant_id(session_emp);
            IFormatProvider culture = new CultureInfo("en-US", true);
            //string _datetime = DateTime.Now.ToString("MM/yyyy");
            string _datetime = DateTime.Now.ToString("MM");
            string v_yymm = _datetime + v_year;
            from_date = DateTime.ParseExact("01/" + v_yymm, "dd/MM/yyyy", culture);
            switch (_datetime)
            {
                case "01":
                    to_date = DateTime.ParseExact("31/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "02":
                    to_date = DateTime.ParseExact("28/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "03":
                    to_date = DateTime.ParseExact("31/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "04":
                    to_date = DateTime.ParseExact("30/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "05":
                    to_date = DateTime.ParseExact("31/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "06":
                    to_date = DateTime.ParseExact("30/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "07":
                    to_date = DateTime.ParseExact("31/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "08":
                    to_date = DateTime.ParseExact("31/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "09":
                    to_date = DateTime.ParseExact("30/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "10":
                    to_date = DateTime.ParseExact("31/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "11":
                    to_date = DateTime.ParseExact("30/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "12":
                    to_date = DateTime.ParseExact("31/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
            }

            if (session_emp.ToLower() == "admin" || session_emp == "D83003" || session_emp == "V78157" 
                || session_emp == "MK78072" || session_emp == "H88768" || session_emp == "HN91185" || session_emp ==  "HN92244")
            {
                return View(en.ORDER_TYPE_VIEW.Where(i => i.OrderDate >= from_date
                                           && i.OrderDate <= to_date
                                           && i.Del != true
                                           && i.Plant == v_plant
                                           && i.AlbumTypeId == 2).ToList());
            }
            else return View(en.ORDER_TYPE_VIEW.Where(i => i.OrderDate >= from_date
                                             && i.OrderDate <= to_date
                                             && i.Del != true
                                             && i.EmployeeID == session_emp.ToLower()
                                             && i.Plant == v_plant
                                             && i.AlbumTypeId == 2).ToList());

        }

        [Authorize]
        [HttpPost]
        public ActionResult MealOrderList(string searchString, string _datetime)
        {
            //http://www.asp.net/mvc/overview/getting-started/getting-started-with-ef-using-mvc/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application
            string v_plant = GetPlant_id(session_emp);

            IFormatProvider culture = new CultureInfo("en-US", true);
            from_date = DateTime.ParseExact("01/" + _datetime, "dd/MM/yyyy", culture);
            string v_yymm = _datetime;
            _datetime = _datetime.Substring(0, 2);
            switch (_datetime)
            {
                case "01":
                    to_date = DateTime.ParseExact("31/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "02":
                    to_date = DateTime.ParseExact("28/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "03":
                    to_date = DateTime.ParseExact("31/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "04":
                    to_date = DateTime.ParseExact("30/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "05":
                    to_date = DateTime.ParseExact("31/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "06":
                    to_date = DateTime.ParseExact("30/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "07":
                    to_date = DateTime.ParseExact("31/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "08":
                    to_date = DateTime.ParseExact("31/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "09":
                    to_date = DateTime.ParseExact("30/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "10":
                    to_date = DateTime.ParseExact("31/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "11":
                    to_date = DateTime.ParseExact("30/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "12":
                    to_date = DateTime.ParseExact("31/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
            }

            /*
            var students = from o in en.Order_
                           join od in en.OrderDetails on o.OrderId equals od.OrderId
                           join ab in en.Albums on od.AlbumId equals ab.AlbumId
                           where ab.AlbumTypeId == 2
                             && o.OrderDate >= from_date
                             && o.OrderDate <= to_date && o.Del != true
                           orderby o.OrderId
                           select new
                           {
                               OrderId = o.OrderId,
                               OrderDate = o.OrderDate,
                               Total = o.Total,
                               Note = o.Note,
                               Confirmed = o.Confirmed,
                               EmployeeID = o.EmployeeID,
                               Employee_Name = o.Employee_Name,
                               Del = o.Del,
                               Plant = o.Plant
                           };

            if (session_emp.ToLower() != "admin")
            {
                students = students.Where(s => s.EmployeeID == session_emp.ToLower()
                                                && s.Plant == v_plant);
            }
            else if (!String.IsNullOrEmpty(searchString) &&
                    (session_emp.ToLower() == "admin" || session_emp == "D83003" || session_emp == "V78157"
                    || session_emp == "MK78072" || session_emp == "H88768" || session_emp == "HN91185" || session_emp == "HN92244"))
            {
                students = students.Where(s => (s.EmployeeID == session_emp.ToLower()
                                       || s.Note.Contains(searchString)
                                       || s.EmployeeID.Contains(searchString))
                                       && s.Plant == v_plant);
            }
            else
            {
                students = students.Where(s => (s.Note.Contains(searchString)
                                      || s.EmployeeID.Contains(searchString))
                                      && s.Plant == v_plant);
            }
            return View(students);*/

            if (session_emp.ToLower() != "admin")
            {
                return View(en.ORDER_TYPE_VIEW.Where(i => i.OrderDate >= from_date
                                                    && i.OrderDate <= to_date
                                                    && i.Del != true
                                                    && i.EmployeeID == session_emp.ToLower()
                                                    && i.Plant == v_plant
                                                    && i.AlbumTypeId == 2).Distinct().ToList());
            }
            else if (!String.IsNullOrEmpty(searchString) &&
                   (session_emp.ToLower() == "admin" || session_emp == "D83003" || session_emp == "V78157"
               || session_emp == "MK78072" || session_emp == "H88768" || session_emp == "HN91185" || session_emp == "HN92244"))
            {
                return View(en.ORDER_TYPE_VIEW.Where(i => i.OrderDate >= from_date
                                                   && i.OrderDate <= to_date
                                                   && i.Del != true
                                                   && i.Plant == v_plant
                                                   && i.AlbumTypeId == 2
                                                   && i.Note.Contains(searchString)
                                                   && i.EmployeeID.Contains(searchString)).Distinct().ToList());
            }
            else
                return View(en.ORDER_TYPE_VIEW.Where(i => i.OrderDate >= from_date
                                                   && i.OrderDate <= to_date
                                                   && i.Del != true
                                                   && i.Plant == v_plant
                                                   && i.AlbumTypeId == 2).Distinct().ToList());
        }



        //
        // GET: /Order_/Details/5

        public ActionResult Details(int? id)
        {
            OrderDetail order_detail = en.OrderDetails.Find(id);
            Session["OrderId"] = id.ToString();
            return View(order_detail);
        }

        //
        // GET: /Order_/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Order_/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Order_/Edit/5

        public ActionResult Edit(int? id)
        {
            OrderDetail seal_using = en.OrderDetails.Find(id);
            Session["OrderId"] = id.ToString();
            return View(seal_using);
        }


        //
        // POST: /Order_/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                foreach (var item in collection)
                {
                    en.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
            }
            catch
            {
                return View();
            }
            return RedirectToAction("MealOrderList", "MealOrder");
        }

        //
        // GET: /Order_/Delete/5

        public ActionResult Delete(int? id)
        {
            OrderDetail seal_using = en.OrderDetails.Find(id);
            Session["OrderId"] = id.ToString();
            return View(seal_using);
        }

        //
        // POST: /Order_/Delete/5

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var order = en.Order_.Where(i => i.OrderId == id).SingleOrDefault();
                order.Del = true;
                en.SaveChanges();
            }
            catch
            {
                return View();
            }
            return RedirectToAction("OrderList", "Order_");
        }

        private string subject { get; set; }
        private string body { get; set; }
        private string confirm_status { get; set; }

        private string GetDept_id_by_oder(int orderId)
        {
            // lay thong tin order
            string dept_id = "";
            if (orderId != 0)
            {
                var v_empno = en.Order_.Where(i => i.OrderId == orderId).SingleOrDefault().EmployeeID;
                dept_id = en.Employees.Where(f => (f.EmployeeID == v_empno)).Select(f => f.Department_Id).SingleOrDefault();
            }
            else
            {
                dept_id = en.Employees.Where(f => (f.EmployeeID == session_emp)).Select(f => f.Department_Id).SingleOrDefault();
            }
            return dept_id;
        }

        // save information
        [HttpPost]
        public string Save(List<OrderDetail> orderdetails)
        {
            string v_information = ""; int v_row = orderdetails.Count;
            try
            {
                foreach (OrderDetail orderdetail in orderdetails)
                {
                    OrderDetail Existed_orderdetail = en.OrderDetails.Find(orderdetail.OrderDetailId);
                    Existed_orderdetail.Quantity = orderdetail.Quantity;
                    Existed_orderdetail.Note = orderdetail.Note;
                    en.SaveChanges();

                    v_information = v_row.ToString() + ".  " + orderdetail.Album.Title + " | " + orderdetail.Quantity + "\n" + v_information;
                    v_row = v_row - 1;
                }

            }
            catch
            {
            }
            string result, status = "";

            //int order_id= (int)orderdetails.Distinct().FirstOrDefault().OrderId;

            var dept = from i in en.Departments where i.Department_Id == GetDept_id_by_oder(Convert.ToInt32(Session["OrderId"].ToString())).Single().ToString() select i.Department_Name;
            subject = "[Thông báo] - Phòng nhân sự điều chỉnh thông tin yêu cầu";
            result = string.Format("Thông báo! <br /> <br />" +
                                              "Đã gởi email xác nhận!  <br />" +
                                              "************** Cám ơn đã sử dụng chương trình **************");
            status = "1";
            //}
            body = "Kiểm tra thông tin đã chỉnh sửa ...\n" +
                    "   Theo đường dẫn: " + "http://52.213.3.168/servicedesk/Order_/Edit/" + Convert.ToInt32(Session["OrderId"].ToString()) + "\n" + "\n" +
                    v_information +
                    "\n \n Trân trọng!" + "\n\n\n" +
                    "Chương trình gởi mail được bởi IT TEAM: liên hệ Nguyen Thai Binh - IT Software khi cần hỗ trợ";

            Information inf = new Information();
            inf.email_send("user_email", "pass", GetDept_id_by_oder(Convert.ToInt32(Session["OrderId"].ToString())), subject, body, status, GetPlant_id(session_emp));
            //~~~~~~~~~~~~~~~~~~~~~
            //return RedirectToAction("Index", "SealUsing");


            return result;
        }

        public ActionResult Copy(int? id)
        {
            var v_getoder = en.Order_.Where(i => i.OrderId == id).SingleOrDefault();
            v_getoder.OrderDate = DateTime.Now;
            v_getoder.EmployeeID = session_emp;
            v_getoder.Confirmed = false;
            en.Order_.Add(v_getoder);
            en.SaveChanges();

            List<OrderDetail> orderdetails = en.OrderDetails.Where(i => i.OrderId == id).ToList();

            string v_information = ""; int v_row = orderdetails.Count;
            try
            {
                foreach (OrderDetail orderdetail in orderdetails)
                {
                    OrderDetail Existed_orderdetail = new OrderDetail();
                    Existed_orderdetail.OrderId = v_getoder.OrderId;
                    Existed_orderdetail.AlbumId = orderdetail.AlbumId;
                    Existed_orderdetail.Quantity = orderdetail.Quantity;
                    Existed_orderdetail.Note = orderdetail.Note;
                    en.OrderDetails.Add(Existed_orderdetail);

                    en.SaveChanges();
                    v_information = v_row.ToString() + ".  " + orderdetail.Album.Title + " | " + orderdetail.Quantity + "\n" + v_information;
                    v_row = v_row - 1;
                }

            }
            catch
            {
            }
            string result, status = "";

            var dept = from i in en.Departments where i.Department_Id == GetDept_id_by_oder(v_getoder.OrderId).Single().ToString() select i.Department_Name;
            subject = "[Thông báo] - Duyệt thông tin yêu cầu";
            result = string.Format("Thông báo! <br /> <br />" +
                                              "Đã gởi email xác nhận!  <br />" +
                                              "************** Cám ơn đã sử dụng chương trình **************");

            status = "1";
            //}
            body = "Kiểm tra thông tin đã chỉnh sửa ...\n" +
                    "   Theo đường dẫn: " + "http://52.213.3.168/servicedesk/Order_/Edit/" + v_getoder.OrderId + "\n" + "\n" +
                    v_information +
                    "\n \n Trân trọng!" + "\n \n \n" +
                    "Chương trình gởi mail được bởi IT TEAM: liên hệ Nguyen Thai Binh - IT Software khi cần hỗ trợ";

            Information inf = new Information();
            inf.email_send("user_email", "pass", GetDept_id_by_oder(v_getoder.OrderId), subject, body, status, GetPlant_id(session_emp));
            //~~~~~~~~~~~~~~~~~~~~~

            return RedirectToAction("MealOrderList", "MealOrder");
        }



        public ActionResult Confirm(int? id)
        {
            Order_ v_oder = en.Order_.Find(id);
            return View(v_oder);
        }

        //
        // POST: /SealUsing/Edit/5

        [HttpPost]
        public string Confirm(int orderid)
        {
            string result = "";
            string status = "3";
            Order_ order = en.Order_.Find(orderid);

            try
            {
                order.Confirmed = true;
                en.Entry(order).State = System.Data.Entity.EntityState.Modified;
                if (order.Confirmed == true)
                {
                    en.SaveChanges();
                    //~~~~~~~~~~~~~~~~~~~~~
                    //~~~~~~~~~~~~~~~~~~~~~
                    result = string.Format("Thông báo! <br /> <br />" +
                                                      "Đã gởi email xác nhận!  <br />" +
                                                      "************** Cám ơn đã sử dụng chương trình **************");
                }

                //string v_department_id= en.Employees.Where(i  => i.EmployeeID==order.EmployeeID).SingleOrDefault().ToString();
                //var dept = from i in en.Departments where i.DepartmentId == v_department_id select i.DepartmentName;

                Information inf = new Information();
                inf.email_send("user_email", "pass", GetDept_id_by_oder(orderid) //"0000"
                                                  , subject, body, status, GetPlant_id(session_emp));
            }
            catch
            {
                result = string.Format("Thông báo! <br /> <br />" +
                                               "THÔNG TIN CHƯA ĐƯỢC DUYỆT, HÃY KIỂM TRA LẠI!  <br />" +
                                               "************** Cám ơn đã sử dụng chương trình **************");
            }
            return result;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExportData(string _datetime)
        {
            IFormatProvider culture = new CultureInfo("en-US", true);
            //string _datetime = DateTime.Now.Month.ToString("00") + "/" + DateTime.Now.Year.ToString("0000");string _datetime
            from_date = DateTime.ParseExact("01/" + _datetime, "dd/MM/yyyy", culture);
            string v_yymm = _datetime;
            _datetime = _datetime.Substring(0, 2);
            switch (_datetime)
            {
                case "01":
                    to_date = DateTime.ParseExact("31/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "02":
                    to_date = DateTime.ParseExact("28/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "03":
                    to_date = DateTime.ParseExact("31/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "04":
                    to_date = DateTime.ParseExact("30/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "05":
                    to_date = DateTime.ParseExact("31/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "06":
                    to_date = DateTime.ParseExact("30/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "07":
                    to_date = DateTime.ParseExact("31/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "08":
                    to_date = DateTime.ParseExact("31/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "09":
                    to_date = DateTime.ParseExact("30/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "10":
                    to_date = DateTime.ParseExact("31/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "11":
                    to_date = DateTime.ParseExact("30/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "12":
                    to_date = DateTime.ParseExact("31/" + v_yymm + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
            }

            GridView gv = new GridView();
            var list =
                 from t in en.OrderDetails
                 //from j in en.Order_
                 join j in en.Order_ on t.OrderId equals j.OrderId
                 join ab in en.Albums on t.AlbumId equals ab.AlbumId //into pp
                 where (j.Del != true && t.Quantity != 0 && (j.EmployeeID != "admin" || j.EmployeeID != "D83003" || j.EmployeeID != "V78157")
                                        && j.OrderDate >= from_date && j.OrderDate <= to_date
                                        && ab.AlbumTypeId == 2)
                 //where //t.OrderId == j.OrderId && 
                 //       j.Del != true && t.Quantity != 0 && j.EmployeeID != "admin" && j.OrderDate >= from_date && j.OrderDate <= to_date 
                 group t by new
                 {
                     t.AlbumId,
                     t.Album.Title,
                     t.Album.Unit
                 } into g
                 select new
                 {
                     MaSP = g.Key.AlbumId,
                     TenSP = g.Key.Title,
                     Dvt = g.Key.Unit,
                     Tong = g.Sum(a => a.Quantity) // Sum, not Max
                 };

            gv.DataSource = list.ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Export_" + _datetime.Replace("/", "") + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View();
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExportData2(string _datetime)
        {
            IFormatProvider culture = new CultureInfo("en-US", true);
            //_datetime = DateTime.Now.Month.ToString("00") + "/" + DateTime.Now.Year.ToString("0000");string _datetime
            from_date = DateTime.ParseExact("01/" + _datetime, "dd/MM/yyyy", culture);
            string v_fromdate = from_date.ToString("s", culture);
            switch (_datetime)
            {
                case "01/2015":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "02/2015":
                    to_date = DateTime.ParseExact("28/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "03/2015":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "04/2015":
                    to_date = DateTime.ParseExact("30/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "05/2015":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "06/2015":
                    to_date = DateTime.ParseExact("30/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "07/2015":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "08/2015":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "09/2015":
                    to_date = DateTime.ParseExact("30/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "10/2015":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "11/2015":
                    to_date = DateTime.ParseExact("30/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "12/2015":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "01/2016":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "02/2016":
                    to_date = DateTime.ParseExact("28/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "03/2016":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "04/2016":
                    to_date = DateTime.ParseExact("30/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "05/2016":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "06/2016":
                    to_date = DateTime.ParseExact("30/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "07/2016":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "08/2016":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "09/2016":
                    to_date = DateTime.ParseExact("30/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "10/2016":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "11/2016":
                    to_date = DateTime.ParseExact("30/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "12/2016":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
            }
            string v_todate = to_date.ToString("s", culture);
            GridView gv = new GridView();
            var list =
                //en.func_report_detail(v_fromdate, v_todate).ToList();
                            en.func_report_detail(from_date, to_date, GetPlant_id(session_emp)).ToList();
            //.Where(t => t. >= from_date && t.ORDERDATE <= to_date).ToList();

            gv.DataSource = list.ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Export_" + _datetime.Replace("/", "") + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View();
        }

        [HttpPost, ActionName("DownloadData")]
        public ActionResult DownloadData()
        {
            int id = Convert.ToInt32(Convert.ToString(Session["OrderId"]).Trim());

            try
            {
                GridView gv = new GridView();


                var list = from t in en.OrderDetails
                           join j in en.Order_ on t.OrderId equals j.OrderId into pp
                           from j in pp.DefaultIfEmpty()
                           where j.OrderId == id
                           group t by new
                           {
                               t.Order_.EmployeeID,
                               t.AlbumId,
                               t.Album.Title,
                               t.Album.Unit,
                               t.Quantity,
                               t.Note
                           } into g
                           select new
                           {
                               PhongBan = g.Key.EmployeeID,
                               MaSP = g.Key.AlbumId,
                               TenSP = g.Key.Title,
                               Dvt = g.Key.Unit,
                               Tong = g.Key.Quantity, // Sum, not Max
                               GhiChu = g.Key.Note
                           };

                //var list = en.OrderDetails.ToList().Where(i => i.OrderId == id);

                var department_info = en.Order_.Where(i => i.OrderId == id).FirstOrDefault();
                string department_name = department_info.EmployeeID;

                gv.DataSource = list.ToList();
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=" + department_name + "_" + id.ToString().Replace("/", "") + ".xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                gv.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
            catch
            {
                return View();
            }
            return RedirectToAction("MealOrderList", "MealOrder");
        }

        [HttpPost, ActionName("MealOrderPrint")]
        public ActionResult MealOrderPrint(string reportid)
        {
            int id = Convert.ToInt32(Convert.ToString(Session["OrderId"]).Trim());

            if (reportid != "")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/Report"), "Report_Viewer.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    return View("Index");
                }

                try
                {
                    GridView gv = new GridView();


                    var list = from t in en.OrderDetails
                               join j in en.Order_ on t.OrderId equals j.OrderId
                               into pp
                               from j in pp.DefaultIfEmpty()
                               where j.OrderId == id
                               group t by new
                               {
                                   t.Order_.OrderId,
                                   t.Order_.OrderDate,
                                   t.Order_.Confirmed,
                                   t.Order_.EmployeeID,
                                   t.Order_.Employee_Name,
                                   t.Order_.Plant,
                                   t.OrderDetailId,
                                   t.AlbumId,
                                   t.Quantity,
                                   t.Note,
                                   t.Album.Title,
                                   t.Album.Unit,
                                   //(en.Departments.Where(o => o.DepartmentId == j.Employee.DepartmentId && o.Plant == j.Plant).Select(i=>i.DepartmentName).SingleOrDefault())
                                   t.Order_.Employee.Department_Id
                               } into g
                               select new
                               {
                                   OrderId = g.Key.OrderId,
                                   OrderDate = g.Key.OrderDate,
                                   Confirmed = g.Key.Confirmed,
                                   Emp_CJ = g.Key.EmployeeID,
                                   Employe_Name = g.Key.Employee_Name,
                                   Plant = g.Key.Plant,
                                   OrderDetailId = g.Key.OrderDetailId,
                                   AlbumId = g.Key.AlbumId,
                                   Quantity = g.Key.Quantity,
                                   Note = g.Key.Note,
                                   Title = g.Key.Title,
                                   Unit = g.Key.Unit,
                                   DepatmentName = g.Key.Department_Id
                               };

                    //var list = en.OrderDetails.ToList().Where(i => i.OrderId == id);


                    string v_department_name = en.Departments.Where(o => o.Department_Id == list.Select(i => i.DepatmentName).FirstOrDefault()
                                                                        && o.Plant_Id == list.Select(i => i.Plant).FirstOrDefault()).Select(e => e.Department_Name).SingleOrDefault();


                    //ReportParameter p1 = new ReportParameter("p_department_name", v_department_name);


                    ReportDataSource rd = new ReportDataSource("DataSetReport", list);
                    lr.DataSources.Add(rd);
                    string reportType = reportid;
                    string mimeType;
                    string encoding;
                    string fileNameExtension;



                    string deviceInfo =

                    "<DeviceInfo>" +
                    "  <OutputFormat>" + reportid + "</OutputFormat>" +
                    "  <PageWidth>11.7in</PageWidth>" +
                    "  <PageHeight>16.5in</PageHeight>" +
                    "  <MarginTop>0.5in</MarginTop>" +
                    "  <MarginLeft>1in</MarginLeft>" +
                    "  <MarginRight>1in</MarginRight>" +
                    "  <MarginBottom>0.5in</MarginBottom>" +
                    "</DeviceInfo>";

                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;

                    renderedBytes = lr.Render(
                        reportType,
                        deviceInfo,
                        out mimeType,
                        out encoding,
                        out fileNameExtension,
                        out streams,
                        out warnings);


                    return File(renderedBytes, mimeType);

                }
                catch
                {
                    return View();
                }
            }
            return PartialView("MealOrderPrint", en.Order_.Where(o => o.OrderId == id));
        }

    }
}
