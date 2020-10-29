using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;

namespace Web_IT_HELPDESK
{
    public partial class Information
    {
        // Get 
        DataTable dt = new DataTable();
        public void email_send(string user_email, string pass, string department_id, string str_subject, string str_body, string level_confirm, string v_plant)
        {
            user_email = "ithelpdesk@cjvina.com";
            pass = "ithelpdesk2015";
            string email = "";
            if (level_confirm == "1")
            {
                dt = email_data(department_id, v_plant);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    email = dt.Rows[i][2].ToString();
                    try
                    {
                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient("mail.cjvina.com", 587);
                        if (user_email.Contains("@cjvina.com"))
                        {
                            mail.From = new MailAddress(user_email); // user_email send
                            mail.To.Add(email); // add email is sent
                            //if (str_subject == "")
                            //{ MessageBox.Show("Gởi mail không cần tiêu đề", "Thông báo!");  }
                            mail.Subject = str_subject;

                            // body
                            mail.Body = str_body;

                            SmtpServer.Port = 25;
                            SmtpServer.Credentials = new System.Net.NetworkCredential(user_email, pass);  // "ithelpdesk@cjvina.com", "ithelpdesk2015"
                            SmtpServer.EnableSsl = false;
                            SmtpServer.Send(mail);
                        }
                        //else { MessageBox.Show("Sai cấu trúc email", "Thông báo!"); }
                    }
                    catch (Exception) { }//MessageBox.Show("Lỗi: " + ex.ToString(), "Thông báo!"); }
                }
            }
            else if (level_confirm == "2" && v_plant == "0301")
            {
                email = "thao.hr@cjvina.com";
                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("mail.cjvina.com", 587);
                    if (user_email.Contains("@cjvina.com"))
                    {
                        mail.From = new MailAddress(user_email); // user_email send
                        mail.To.Add(email); // add email is sent
                        //if (str_subject == "")
                        //{ MessageBox.Show("Gởi mail không cần tiêu đề", "Thông báo!");  }
                        mail.Subject = str_subject;

                        // body
                        mail.Body = str_body;

                        SmtpServer.Port = 25;
                        SmtpServer.Credentials = new System.Net.NetworkCredential(user_email, pass);  // "ithelpdesk@cjvina.com", "ithelpdesk2015"
                        SmtpServer.EnableSsl = false;
                        SmtpServer.Send(mail);
                    }
                    //else { MessageBox.Show("Sai cấu trúc email", "Thông báo!"); }
                }
                catch (Exception) { }//MessageBox.Show("Lỗi: " + ex.ToString(), "Thông báo!"); }
            }
            //quản lý văn phòng phẩm
            else if (level_confirm == "3" && v_plant == "0301")
            {
                email = "hr@cjvina.com"; // chua co nguoi 20181231

                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("mail.cjvina.com", 587);
                    if (user_email.Contains("@cjvina.com"))
                    {
                        mail.From = new MailAddress(user_email); // user_email send
                        mail.To.Add(email); // add email is sent
                        mail.Subject = str_subject;
                        // body
                        mail.Body = str_body;

                        SmtpServer.Port = 25;
                        SmtpServer.Credentials = new System.Net.NetworkCredential(user_email, pass);  // "ithelpdesk@cjvina.com", "ithelpdesk2015"
                        SmtpServer.EnableSsl = false;
                        SmtpServer.Send(mail);
                    }
                }
                catch (Exception) { }//MessageBox.Show("Lỗi: " + ex.ToString(), "Thông báo!"); }
            }
            // quản lý xe
            else if (level_confirm == "4" && v_plant == "0301")
            {
                email_send_IT("0000", "0301", user_email, pass, str_subject, str_body);
                //email = "toanthien.hr@cjvina.com";
                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("mail.cjvina.com", 587);
                    if (user_email.Contains("@cjvina.com"))
                    {
                        mail.From = new MailAddress(user_email); // user_email send
                        mail.To.Add(email); // add email is sent
                        //if (str_subject == "")
                        //{ MessageBox.Show("Gởi mail không cần tiêu đề", "Thông báo!");  }
                        mail.Subject = str_subject;

                        // body
                        mail.Body = str_body;

                        SmtpServer.Port = 25;
                        SmtpServer.Credentials = new System.Net.NetworkCredential(user_email, pass);  // "ithelpdesk@cjvina.com", "ithelpdesk2015"
                        SmtpServer.EnableSsl = false;
                        SmtpServer.Send(mail);
                    }
                    //else { MessageBox.Show("Sai cấu trúc email", "Thông báo!"); }
                }
                catch (Exception) { }//MessageBox.Show("Lỗi: " + ex.ToString(), "Thông báo!"); }
            }

            //logic for Dong Nai
            else if (level_confirm == "2" && v_plant == "0304")
            {
                email = "";
                for (int i = 0; i < 2; i++)
                {
                    if (i == 0)
                        email = "dathao.hr@cjvina.com";

                    else if (i == 1)
                        email = "tamhiep.hr@cjvina.com";
                    try
                    {
                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient("mail.cjvina.com", 587);
                        if (user_email.Contains("@cjvina.com"))
                        {
                            mail.From = new MailAddress(user_email); // user_email send
                            mail.To.Add(email); // add email is sent
                            mail.Subject = str_subject;

                            // body
                            mail.Body = str_body;

                            SmtpServer.Port = 25;
                            SmtpServer.Credentials = new System.Net.NetworkCredential(user_email, pass);  // "ithelpdesk@cjvina.com", "ithelpdesk2015"
                            SmtpServer.EnableSsl = false;
                            SmtpServer.Send(mail);
                        }
                        //else { MessageBox.Show("Sai cấu trúc email", "Thông báo!"); }
                    }

                    catch { }//MessageBox.Show("Lỗi: " + ex.ToString(), "Thông báo!"); }

                }
            }
            else if (level_confirm == "3" && v_plant == "0304")
            {
                email = "dathao.hr@cjvina.com";

                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("mail.cjvina.com", 587);
                    if (user_email.Contains("@cjvina.com"))
                    {
                        mail.From = new MailAddress(user_email); // user_email send
                        mail.To.Add(email); // add email is sent
                        mail.Subject = str_subject;
                        // body
                        mail.Body = str_body;

                        SmtpServer.Port = 25;
                        SmtpServer.Credentials = new System.Net.NetworkCredential(user_email, pass);  // "ithelpdesk@cjvina.com", "ithelpdesk2015"
                        SmtpServer.EnableSsl = false;
                        SmtpServer.Send(mail);
                    }
                }
                catch (Exception) { }//MessageBox.Show("Lỗi: " + ex.ToString(), "Thông báo!"); }
            }

            //logic for VINH LONG & MEKONG
            else if (level_confirm == "2" && (v_plant == "0303" || v_plant == "0308"))
            {
                email = "tuyen.hr@cjvina.com";
                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("mail.cjvina.com", 587);
                    if (user_email.Contains("@cjvina.com"))
                    {
                        mail.From = new MailAddress(user_email); // user_email send
                        mail.To.Add(email); // add email is sent
                        //if (str_subject == "")
                        //{ MessageBox.Show("Gởi mail không cần tiêu đề", "Thông báo!");  }
                        mail.Subject = str_subject;

                        // body
                        mail.Body = str_body;

                        SmtpServer.Port = 25;
                        SmtpServer.Credentials = new System.Net.NetworkCredential(user_email, pass);  // "ithelpdesk@cjvina.com", "ithelpdesk2015"
                        SmtpServer.EnableSsl = false;
                        SmtpServer.Send(mail);
                    }
                    //else { MessageBox.Show("Sai cấu trúc email", "Thông báo!"); }
                }
                catch (Exception) { }//MessageBox.Show("Lỗi: " + ex.ToString(), "Thông báo!"); }
            }
            else if (level_confirm == "3" && (v_plant == "0303" || v_plant == "0308"))
            {
                email = "tuyen.hr@cjvina.com";
                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("mail.cjvina.com", 587);
                    if (user_email.Contains("@cjvina.com"))
                    {
                        mail.From = new MailAddress(user_email); // user_email send
                        mail.To.Add(email); // add email is sent
                        mail.Subject = str_subject;
                        // body
                        mail.Body = str_body;

                        SmtpServer.Port = 25;
                        SmtpServer.Credentials = new System.Net.NetworkCredential(user_email, pass);  // "ithelpdesk@cjvina.com", "ithelpdesk2015"
                        SmtpServer.EnableSsl = false;
                        SmtpServer.Send(mail);
                    }
                }
                catch (Exception) { }//MessageBox.Show("Lỗi: " + ex.ToString(), "Thông báo!"); }
            }

            //logic for HA NAM HUNG YEN
            else if (level_confirm == "2" && (v_plant == "0302" || v_plant == "0305"))
            {
                email_send_IT("3333", v_plant, user_email, pass, str_subject, str_body); // goi email cho admin
            }
            else if (level_confirm == "3" && (v_plant == "0302" || v_plant == "0305"))
            {
                email_send_IT("3333", v_plant, user_email, pass, str_subject, str_body);
            }

        }

        private string str_connect = ConfigurationManager.ConnectionStrings["Web_IT_HELPDESK_connString"].ConnectionString;
        private DataTable email_data(string department_id, string v_plant)
        {
            DataTable datatable = new DataTable();
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            string sql;

            if (v_plant == "0304")
                sql = "select DepartmentId,DepartmentName,Manager_Email\n" +
                           "from department\n" +
                           "Where DepartmentId='" + department_id + "'\n" +
                           "  and plant ='" + v_plant + "'";
            else
                sql = "SELECT DISTINCT A.DepartmentId, A.DepartmentName, A.Manager_Email\n" +
                         "FROM (\n" +
                            "select DepartmentId,DepartmentName,Manager_Email\n" +
                                "from department\n" +
                                "Where DepartmentId='" + department_id + "'\n" +
                                "  and plant ='" + v_plant + "'\n" +
                                "union all \n" +
                                "select DepartmentId,DepartmentName,Manager_Email\n" +
                                  "from department\n" +
                                 "Where DepartmentId='0302'\n" +
                                   "and plant ='" + v_plant + "'\n" +
                                "union all \n" +
                                "select DepartmentId,DepartmentName,Manager_Email\n" +
                                  "from department\n" +
                                 "Where DepartmentId='3333'\n" +
                                   "and plant ='" + v_plant + "') A\n";

            using (SqlConnection connection = new SqlConnection(str_connect))
            {
                SqlCommand command = new SqlCommand(sql);

                command.Connection = connection;

                try
                {
                    connection.Open();
                    SqlDataAdapter sqldap = new SqlDataAdapter(sql, connection);

                    ds.Clear();
                    sqldap.Fill(ds);
                    datatable = ds.Tables[0];
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return datatable;
        }

        //get IT email
        private DataTable email_data_IT(string department_id, string v_plant_id)
        {

            DataTable datatable = new DataTable();
            DataSet ds = new DataSet();
            string sql;

            sql = "select employeeid,EmployeeName,Email\n" +
                    "from employee\n" +
                    "Where DepatmentId='" + department_id + "'\n" +
                       "and Plant='" + v_plant_id + "'\n" +
                       "union all \n" +
                    "select DepartmentId,DepartmentName,Manager_Email\n" +
                      "from department\n" +
                     "Where DepartmentId='0302'\n" +
                       "and plant ='" + v_plant_id + "'\n";

            using (SqlConnection connection = new SqlConnection(str_connect))
            {
                SqlCommand command = new SqlCommand(sql);

                command.Connection = connection;

                try
                {
                    connection.Open();
                    SqlDataAdapter sqldap = new SqlDataAdapter(sql, connection);

                    ds.Clear();
                    sqldap.Fill(ds);
                    datatable = ds.Tables[0];
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return datatable;
        }

        private void email_send_IT(string department_id, string v_plant_id, string user_email, string pass, string str_subject, string str_body)
        {
            string email = "";
            dt = email_data_IT(department_id, v_plant_id);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                email = dt.Rows[i][2].ToString();

                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("mail.cjvina.com", 587);
                    if (user_email.Contains("@cjvina.com"))
                    {
                        mail.From = new MailAddress(user_email); // user_email send
                        mail.To.Add(email); // add email is sent
                        //if (str_subject == "")
                        //{ MessageBox.Show("Gởi mail không cần tiêu đề", "Thông báo!");  }
                        mail.Subject = str_subject;

                        // body
                        mail.Body = str_body;

                        SmtpServer.Port = 25;
                        SmtpServer.Credentials = new System.Net.NetworkCredential(user_email, pass);  // "ithelpdesk@cjvina.com", "ithelpdesk2015"
                        SmtpServer.EnableSsl = false;
                        SmtpServer.Send(mail);
                    }
                    //else { MessageBox.Show("Sai cấu trúc email", "Thông báo!"); }
                }
                catch (Exception) { }//MessageBox.Show("Lỗi: " + ex.ToString(), "Thông báo!"); }
            }
        }
    }
}