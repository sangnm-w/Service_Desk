using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Web_IT_HELPDESK.Controllers
{
    public class LaborContractController : Controller
    {
        //
        // GET: /LaborContract/
        hr_contract_binhEntities en = new hr_contract_binhEntities();
        private string session_emp = System.Web.HttpContext.Current.User.Identity.Name;

        private DateTime to_date { get; set; }
        private DateTime from_date { get; set; }
        public ActionResult LaborContractIndex()
        {
            IFormatProvider culture = new CultureInfo("en-US", true);
            from_date = DateTime.ParseExact("01/" + DateTime.Now.Month.ToString("00") + "/"+DateTime.Now.Year.ToString("0000"), "dd/MM/yyyy", culture);
            string _datetime = DateTime.Now.ToString("MM/yyyy");

            string v_mm = _datetime.Substring(0, 2);

            switch (v_mm)
            {
                case "01":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "02":
                    to_date = DateTime.ParseExact("28/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "03":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "04":
                    to_date = DateTime.ParseExact("30/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "05":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "06":
                    to_date = DateTime.ParseExact("30/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "07":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "08":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "09":
                    to_date = DateTime.ParseExact("30/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "10":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "11":
                    to_date = DateTime.ParseExact("30/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
                case "12":
                    to_date = DateTime.ParseExact("31/" + _datetime + " 23:59:59", "dd/MM/yyyy HH:mm:ss", culture);
                    break;
            }
            if (session_emp == "admin")
            {
                var laborContract = en.CJ_HR_RECRUITMENT.Where(o => o.DEL == "FALSE" && o.FROM_DATE >= from_date && o.FROM_DATE <= to_date);
                return View(laborContract);
            }
            else return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult LaborContractIndex(string search_, DateTime? fromdate_, DateTime? todate_)//, DateTime fromdate_, DateTime todate_)
        {
            IFormatProvider culture = new CultureInfo("en-US", true);
            from_date = DateTime.ParseExact(fromdate_.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", culture);
            to_date = DateTime.ParseExact(todate_.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", culture);

            if (session_emp == "admin")
            {
                if ((search_ == "" || search_ == "search") && (fromdate_ == null || todate_ == null))
                {
                    var laborContract = en.CJ_HR_RECRUITMENT;
                    return View(laborContract);
                }
                else if (search_ == "" || search_ == "search")
                {
                    var laborContract = en.CJ_HR_RECRUITMENT.Where(o => (o.FROM_DATE >= from_date && o.FROM_DATE <= to_date) && o.DEL == "FALSE");
                    return View(laborContract);
                }
                else
                {
                    var laborContract = en.CJ_HR_RECRUITMENT.Where(o => (o.FROM_DATE >= from_date && o.FROM_DATE <= to_date) && o.DEL == "FALSE");
                    var list = laborContract.Where(o => o.EMP_NAME.Contains(search_) == true
                                                    || o.CONTRACT_NO.Contains(search_) == true
                                                    || o.ADDRESS.Contains(search_) == true);
                    return View(list);
                }
            }
            else return View();
        }

        /*[HttpGet]
        public FileContentResult Get_file(string id_num)
        {
            //CONTRACT con = en.CONTRACTs.Find(con_id);
            //string inc_code = con.Code;

            //declare byte array to get file content from database and string to store file name
            byte[] fileData;
            string fileName;
            //create object of LINQ to SQL class
            //using LINQ expression to get record from database for given id value
            var record = from p in en.CJ_HR_RECRUITMENT
                         where p.ID_NUM == id_num
                         select p;
            //only one record will be returned from database as expression uses condtion on primary field
            //so get first record from returned values and retrive file content (binary) and filename
            fileData = (byte[])record.First().ATTACHED_FILE.ToArray();
            fileName = record.First().NOTE;
            //return file and provide byte file content and file name
            return File(fileData, "text", fileName);
        }
        */

        // hr approve process
        public ActionResult hr_approve(string id_num)
        {
            CJ_HR_RECRUITMENT seal_using =  en.CJ_HR_RECRUITMENT.Find(id_num);
                                            //en.CJ_HR_RECRUITMENT.Include(i => i.id_num).SingleOrDefault()
            //ViewBag.Department_confirm_date = DateTime.Now;
            return View(seal_using);
        }

        [HttpPost]
        public string hr_approve(CJ_HR_RECRUITMENT recruiment, HttpPostedFileBase image)
        {
            string result = "********ALERT! ******** ;<br /> Not yet Finished";

            //if (ModelState.IsValid)
            //{
            image = image ?? Request.Files["image"];
            if (image != null && image.ContentLength > 0)
            {
                //en.Entry(recruiment).State = System.Data.Entity.EntityState.Modified;
                if (recruiment.HR_APPROVE == true)
                {
                    byte[] fileData = new byte[Request.Files["image"].InputStream.Length];
                    Request.Files["image"].InputStream.Read(fileData, 0, Convert.ToInt32(Request.Files["image"].InputStream.Length));
                    System.Data.Linq.Binary binaryFile = new System.Data.Linq.Binary(fileData);
                    recruiment.SIGNATURE_HR_IMAGE = binaryFile.ToArray();
                    recruiment.HR_APPROVE = true;
                    recruiment.HR_APPROVE_DATE = DateTime.Now;
                    recruiment.DEL = "FALSE";
                    var existingCart = en.CJ_HR_RECRUITMENT.Find(recruiment.ID_NUM);

                    string v_ID_num = recruiment.ID_NUM;
                    ///update salary reprot
                    var existingSalaryReport = en.CJ_HR_SALARY_REPORT.Where(o => o.ID_NUM == v_ID_num).SingleOrDefault();
                    existingSalaryReport.SALES_EMPLOYEE = recruiment.SALES_EMPLOYEE;

                    if (existingCart != null)
                    {
                        recruiment.SALES_EMPLOYEE = existingCart.SALES_EMPLOYEE;

                        en.Entry(existingCart).CurrentValues.SetValues(recruiment);
                        en.Entry(existingSalaryReport).CurrentValues.SetValues(existingSalaryReport);
                        en.SaveChanges();
                        result = string.Format("ALERT! <br /> HR MANAGER Finished to approve: " + recruiment.APPROVE.ToString() + " <br />" +
                                               "Employee: " + recruiment.EMP_NAME.ToString() + " <br />" +
                                                     "Thank you.");


                        if (en.CJ_HR_SALARY_REPORT.Where(i => i.ID_NUM == v_ID_num).Count() == 1)
                        {
                            if (recruiment.SALES_EMPLOYEE == true)
                                email_send(null, v_ID_num, "sales_approve");
                            else if (recruiment.SALES_EMPLOYEE == false)
                                email_send(null, v_ID_num, "Approve");
                        }
                    }
                }
                else
                {
                    result = "ALERT! <br /> Not yet loaded SIGNATURE IMAGE:";
                }
            }
            //}
            return result; ; //RedirectToAction("Index", "SealUsing");
        }

        // sales approve process
        public ActionResult sales_approve(string id_num)
        {
            CJ_HR_RECRUITMENT seal_using = en.CJ_HR_RECRUITMENT.Find(id_num);
            //ViewBag.Department_confirm_date = DateTime.Now;
            return View(seal_using);
        }

        [HttpPost]
        public string sales_approve(CJ_HR_RECRUITMENT recruiment, HttpPostedFileBase image)
        {
            string result = "********ALERT! ******** ;<br /> Not yet Finished";

            //if (ModelState.IsValid)
            //{
            image = image ?? Request.Files["image"];
            if (image != null && image.ContentLength > 0)
            {
                //en.Entry(recruiment).State = System.Data.Entity.EntityState.Modified;
                if (recruiment.SALES_APPROVE == true)
                {

                    byte[] fileData = new byte[Request.Files["image"].InputStream.Length];
                    Request.Files["image"].InputStream.Read(fileData, 0, Convert.ToInt32(Request.Files["image"].InputStream.Length));
                    System.Data.Linq.Binary binaryFile = new System.Data.Linq.Binary(fileData);
                    recruiment.SIGNATURE_SALES_IMAGE = binaryFile.ToArray();
                    recruiment.SALES_APPROVE = true;
                    recruiment.SALES_APPROVE_DATE = DateTime.Now;
                    recruiment.DEL = "FALSE";
                    var existingCart = en.CJ_HR_RECRUITMENT.Find(recruiment.ID_NUM);

                    string v_ID_num = recruiment.ID_NUM;
                    ///update salary reprot
                    var existingSalaryReport = en.CJ_HR_SALARY_REPORT.Where(o => o.ID_NUM == v_ID_num).SingleOrDefault();
                    existingSalaryReport.SALES_APPROVE = true;
                    existingSalaryReport.SALES_APPROVE_DATE = DateTime.Now;
                    existingSalaryReport.SIGNATURE_SALES_IMAGE = binaryFile.ToArray();


                    if (existingCart != null && existingSalaryReport!=null)
                    {
                        recruiment.SALES_EMPLOYEE = existingCart.SALES_EMPLOYEE; // check nhan vien sales
                        recruiment.HR_APPROVE = existingCart.HR_APPROVE;
                        recruiment.SIGNATURE_HR_IMAGE = existingCart.SIGNATURE_HR_IMAGE;
                        recruiment.HR_APPROVE_DATE = existingCart.HR_APPROVE_DATE;

                        en.Entry(existingCart).CurrentValues.SetValues(recruiment);
                        en.Entry(existingSalaryReport).CurrentValues.SetValues(existingSalaryReport);

                        en.SaveChanges();
                        result = string.Format("ALERT! <br /> SALES DIRECTOR Finished to approve: " + recruiment.APPROVE.ToString() + " <br />" +
                                               "Employee: " + recruiment.EMP_NAME.ToString() + " <br />" +
                                                     "Thank you.");

                        if (recruiment.SALES_EMPLOYEE == true)
                            email_send(null, v_ID_num, "Approve");
                    }
                }
                else
                {
                    result = "ALERT! <br /> Not yet loaded SIGNATURE IMAGE:";
                }
            }
            //}
            return result; ; //RedirectToAction("Index", "SealUsing");
        }
        public ActionResult Approve(string id_num)
        {
            CJ_HR_RECRUITMENT seal_using = en.CJ_HR_RECRUITMENT.Find(id_num);
            //ViewBag.Department_confirm_date = DateTime.Now;
            return View(seal_using);
        }

        [HttpPost]
        public string Approve(CJ_HR_RECRUITMENT recruiment, HttpPostedFileBase image)
        {
            string result = "********ALERT! ******** ;<br /> Not yet Finished";

             //if (ModelState.IsValid)
             //{
             image = image ?? Request.Files["image"];
                 if (image != null && image.ContentLength > 0)
                 {
                     //en.Entry(recruiment).State = System.Data.Entity.EntityState.Modified;
                     if (recruiment.APPROVE == true)
                     {
                         byte[] fileData = new byte[Request.Files["image"].InputStream.Length];
                         Request.Files["image"].InputStream.Read(fileData, 0, Convert.ToInt32(Request.Files["image"].InputStream.Length));
                         System.Data.Linq.Binary binaryFile = new System.Data.Linq.Binary(fileData);
                         recruiment.SIGNATURE_IMAGE = binaryFile.ToArray();
                         recruiment.APPROVE = true;
                         recruiment.DEL = "FALSE";

                         var existingCart = en.CJ_HR_RECRUITMENT.Find(recruiment.ID_NUM);

                         string v_ID_num = recruiment.ID_NUM;
                         var existingSalaryReport = en.CJ_HR_SALARY_REPORT.Where(o => o.ID_NUM == v_ID_num).SingleOrDefault();
                         if (existingCart.SALES_EMPLOYEE != true)
                         {
                             existingSalaryReport.SALES_APPROVE = true;
                             existingSalaryReport.SALES_APPROVE_DATE = DateTime.Now;
                             existingSalaryReport.SIGNATURE_SALES_IMAGE = binaryFile.ToArray();
                             existingSalaryReport.SIGNATURE_IMAGE = binaryFile.ToArray();
                             existingSalaryReport.SIGNATURE_BOD_DATE = DateTime.Now;
                         }
                         else
                         {
                             ///update salary reprot
                             existingSalaryReport.SIGNATURE_IMAGE = binaryFile.ToArray();
                             existingSalaryReport.SIGNATURE_BOD_DATE = DateTime.Now;
                         }


                         if (existingCart != null && existingSalaryReport!=null)
                         {
                             recruiment.SALES_EMPLOYEE = existingCart.SALES_EMPLOYEE;// check nhan vien sales

                             recruiment.HR_APPROVE = existingCart.HR_APPROVE;
                             recruiment.SIGNATURE_HR_IMAGE = existingCart.SIGNATURE_HR_IMAGE;
                             recruiment.HR_APPROVE_DATE = existingCart.HR_APPROVE_DATE;

                             recruiment.SALES_APPROVE = existingCart.SALES_APPROVE;
                             recruiment.SIGNATURE_SALES_IMAGE = existingCart.SIGNATURE_SALES_IMAGE;
                             recruiment.SALES_APPROVE_DATE = existingCart.SALES_APPROVE_DATE;

                             en.Entry(existingCart).CurrentValues.SetValues(recruiment);
                             en.Entry(existingSalaryReport).CurrentValues.SetValues(existingSalaryReport);
                             en.SaveChanges();
                             result = string.Format("ALERT! <br /> Finished to approve: " + recruiment.APPROVE.ToString() + " <br />" +
                                                    "Employee: " + recruiment.EMP_NAME.ToString() + " <br />" +
                                                          "Thank you.");

                             email_send(null, v_ID_num, "done");
                         }
                     }
                     else
                     {
                         result = "ALERT! <br /> Not yet loaded SIGNATURE IMAGE:";
                     }
                 }
             //}
            return result; ; //RedirectToAction("Index", "SealUsing");
        }



        //send email hr manager
        public void email_send(string attchedfile_name, string v_idnumber, string v_approve_type)
        {
            try
            {
                string user_email = "ithelpdesk@cjvina.com";
                string pass = "ithelpdesk2015";

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("mail.cjvina.com");
                if (user_email.Contains("@cjvina.com"))
                {
                  
                    var source_salary_report = en.CJ_HR_SALARY_REPORT.Where(i => i.ID_NUM == v_idnumber).SingleOrDefault();
                    var source_recruitment = en.CJ_HR_RECRUITMENT.Where(i => i.ID_NUM == v_idnumber).SingleOrDefault();

                    string v_fullname = source_salary_report.FULLNAME;
                    string v_dob = source_salary_report.DOB;
                    string v_residence = source_salary_report.RESIDENT;
                    string _join_date = source_salary_report.JOIN_DATE;
                    string v_working_time = source_salary_report.WORKING_TIME;
                    string v_grade = source_salary_report.GRADE;
                    Decimal v_basic_salary = (Decimal)source_salary_report.BASIC_SALARY;
                    Decimal v_meal = (Decimal)source_salary_report.MEAL;
                    Decimal v_position_allowance = (Decimal)source_salary_report.POSITION_ALLOWANCE;
                    Decimal v_phone_allowance = (Decimal)source_salary_report.PHONE_ALLOWANCE;
                    Decimal v_productivity_bonus = (Decimal)source_salary_report.PRODUCTIVITY_BONUS;
                    string v_period_working = source_salary_report.PERIOD_WORKING;

                    decimal v_gross_salary = Convert.ToDecimal(v_basic_salary);
                    decimal v_allowance_salary = Convert.ToDecimal(source_recruitment.ALLOWANCE.Value);/*v_basic_salary + v_meal + v_position_allowance +
                                                                v_phone_allowance + v_productivity_bonus);*/

                    mail.From = new MailAddress(user_email);
                    mail.To.Add("binh.hr@cjvina.com"); // gởi tin nhắn đến -- chính mình 
                    if (v_approve_type == "done")
                    {
                        mail.Subject = "<<HOÀN THÀNH>> Duyệt hợp đồng thử việc - Approving  Probationary contracts ";
                        mail.Body = "ĐÃ HOÀN THÀNH DUYỆT HỢP ĐỒNG NHÂN VIÊN: \n" + source_recruitment.EMP_NAME.ToString().ToUpper();
                    }
                    else
                    {
                        mail.Subject = "Duyệt báo cáo lương - SALARY REPORT APPROVAL";

                        mail.Body = "CADIDATE INFORMATION IN PROBATIONARY CONTRACT: " + "\n" + "\n " +
                                    "Confirmation link:" + " http://52.213.3.168/servicedesk/LaborContract/" + v_approve_type + "?id_num=" + v_idnumber
                                    + "\n" + "\n " +
                                    "------------------------***------------------------" + "\n " +
                                    "               NAME: " + source_recruitment.EMP_NAME.ToString().ToUpper() + "\n " +
                                    "                DOB: " + source_recruitment.DOB.ToString() + "\n " +
                                    "             CAREER: " + source_recruitment.JOB.ToString() + "\n " +
                                    "             ADRESS: " + source_recruitment.ADDRESS.ToString() + "\n " +
                                    "          ID NUMBER: " + source_recruitment.ID_NUM.ToString() + "\n " +
                                    "            ID DATE: " + source_recruitment.ID_DATE.ToString() + "\n " +
                                    "           ID PLACE: " + source_recruitment.ID_PLACE.ToString() + "\n " +
                                    "PROBATIONARY PERIOD: " + source_recruitment.PERIOD + "\n " +
                                    "          FROM DATE: " + source_recruitment.FROM_DATE.Value + "\n " +
                                    "            TO DATE: " + source_recruitment.TO_DATE.Value + "\n " +
                                    "          JOB TITLE: " + source_recruitment.PROMOTION.ToString() + "\n " +
                                    "              GRADE: " + source_recruitment.EMP_LEVEL + "\n " +
                                    "       GROSS SALARY: " + source_recruitment.SALARY.Value.ToString("n") + "\n " +
                                    "   ALLOWANCE SALARY: " + source_recruitment.ALLOWANCE.Value.ToString("n") + "\n " +
                                    "     SALARY PERCENT: " + source_recruitment.PERCENT_SALRY.Value + "%\n " +
                                    "               DATE: " + source_recruitment.START_DATE.Value + "\n " +
                                    "              PLANT: " + source_recruitment.PLANT + "\n " +
                                    "               NOTE: " + source_recruitment.NOTE.ToString() + "\n " +
                                    "        CONTRACT NO: " + source_recruitment.CONTRACT_NO.ToString() + "\n " +

                                    "\n" + "\n " +
                                    "------------------------***------------------------" + "\n " +
                                    "SALARY REPORT" + "\n " +

                                    "           FULLNAME: " + v_fullname + "\n " +
                                    "          RESIDENCE: " + v_residence + "\n " +
                                    "          JOIN DATE: " + _join_date + "\n " +
                                    "       WORKING TIME: " + v_working_time + "\n " +
                                    "       BASIC SALARY: " + v_basic_salary.ToString("n") + "\n " +
                                    "     MEAL ALLOWANCE: " + v_meal.ToString("n") + "\n " +
                                    " POSITION ALLOWANCE: " + v_position_allowance.ToString("n") + "\n " +
                                    "    PHONE ALLOWANCE: " + v_phone_allowance.ToString("n") + "\n " +
                                    " PRODUCTIVITY BONUS: " + v_productivity_bonus.ToString("n") + "\n " +
                                    "     PERIOD WORKING: " + v_period_working + "\n " +

                        "\n" + "\n " + "PERSONEL DEPARTMENT.";
                    }
                    SmtpServer.Port = 25;
                    //SmtpServer.Credentials = new System.Net.NetworkCredential("ithelpdesk@cjvina.com", "ithelpdesk2015");
                    SmtpServer.Credentials = new System.Net.NetworkCredential(user_email, pass);
                    SmtpServer.EnableSsl = false;

                    SmtpServer.Send(mail);
                }
               // else { MessageBox.Show("Kiểm tra thông tin cấu trúc email", "Thông báo!"); }
            }
            catch (Exception ex) { }//MessageBox.Show("Lỗi: " + ex.ToString(), "Thông báo!"); }
        }

    }
}
