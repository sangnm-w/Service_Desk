using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.Properties;

namespace Web_IT_HELPDESK.Controllers.ObjectManager
{
    public class InformationHelper
    {
        private static InformationHelper _Instance;
        public static InformationHelper Instance { get { if (_Instance == null) _Instance = new InformationHelper(); return _Instance; } set => _Instance = value; }
        private InformationHelper() { }

        public bool Send_Mail(int level_confirm, string subject, string body, string userRequestId)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            List<string> toMails = new List<string>();
            List<string> ccMails = new List<string>();
            List<string> bccMails = new List<string>();

            if (level_confirm == 1) // Level 1: Department Manager
            {
                string managerIdOfUserRequest = en.Employees
                    .Join(en.Departments, e => e.Department_ID, d => d.Department_Id, (e, d) => new { e, d })
                    .FirstOrDefault(grp => grp.e.Emp_CJ == userRequestId)
                    .d.Manager_Id;
                string mangerEmail = en.Employees.Find(managerIdOfUserRequest).Email;
                toMails.Add(mangerEmail);
                //bccMails.Add("it-servicedesk@cjvina.com");
            }
            else if (level_confirm == 2) // Level 2: Resend
            {
                string managerIdOfUserRequest = en.Employees
                    .Join(en.Departments, e => e.Department_ID, d => d.Department_Id, (e, d) => new { e, d })
                    .FirstOrDefault(grp => grp.e.Emp_CJ == userRequestId)
                    .d.Manager_Id;
                string mangerEmail = en.Employees.Find(managerIdOfUserRequest).Email;
                toMails.Add(mangerEmail);
                //bccMails.Add("it-servicedesk@cjvina.com");
            }
            else if (level_confirm == 3) // Level 3: BOD
            {
                string bodEmailOfUserRequest = en.Employees
                    .Join(en.Departments, e => e.Department_ID, d => d.Department_Id, (e, d) => new { e, d })
                    .FirstOrDefault(grp => grp.e.Emp_CJ == userRequestId)
                    .d.BOD_Email;
                toMails.Add(bodEmailOfUserRequest);
                //bccMails.Add("it-servicedesk@cjvina.com");
            }
            else if (level_confirm == 4) // Level 4: HR Manager
            {
                string userRequestPlantId = en.Employees
                    .Join(en.Departments, e => e.Department_ID, d => d.Department_Id, (e, d) => new { e, d })
                    .FirstOrDefault(grp => grp.e.Emp_CJ == userRequestId)
                    .d.Plant_Id;
                string hrMangerIdByPlant = en.Departments
                    .FirstOrDefault(d => (d.Plant_Id == userRequestPlantId && d.Department_Name.Contains(")HR")))
                    .Manager_Id;
                string hrMangerEmail = en.Employees.Find(hrMangerIdByPlant).Email;
                toMails.Add(hrMangerEmail);
                //bccMails.Add("it-servicedesk@cjvina.com");
            }
            else if (level_confirm == 5) // Level 5: HR Admin
            {
                string userRequestPlantId = en.Employees
                    .Join(en.Departments, e => e.Department_ID, d => d.Department_Id, (e, d) => new { e, d })
                    .FirstOrDefault(grp => grp.e.Emp_CJ == userRequestId)
                    .d.Plant_Id;
                string hrAdminIdByPlant = en.Departments
                    .FirstOrDefault(d => (d.Plant_Id == userRequestPlantId && d.Department_Name.Contains(")HR Admin")))
                    .Manager_Id;
                string hrAdminEmail = en.Employees.Find(hrAdminIdByPlant).Email;
                toMails.Add(hrAdminEmail);
                //bccMails.Add("it-servicedesk@cjvina.com");
            }
            else if (level_confirm == 6) // Level 6: HR Seal Using
            {
                string userRequestPlantId = en.Employees
                    .Join(en.Departments, e => e.Department_ID, d => d.Department_Id, (e, d) => new { e, d })
                    .FirstOrDefault(grp => grp.e.Emp_CJ == userRequestId)
                    .d.Plant_Id;
                string hrSealIdByPlant = en.Departments
                    .FirstOrDefault(d => (d.Plant_Id == userRequestPlantId && d.Department_Name.Contains(")HR Seal Manager")))
                    .Manager_Id;
                string hrSealEmail = en.Employees.Find(hrSealIdByPlant).Email;
                toMails.Add(hrSealEmail);
                //bccMails.Add("it-servicedesk@cjvina.com");
            }
            else if (level_confirm == 7) // Level 7: Return APPROVED
            {
                string userRequestEmail = en.Employees.Find(userRequestId).Email;
                toMails.Add(userRequestEmail);

                string managerIdOfUserRequest = en.Employees
                    .Join(en.Departments, e => e.Department_ID, d => d.Department_Id, (e, d) => new { e, d })
                    .FirstOrDefault(grp => grp.e.Emp_CJ == userRequestId)
                    .d.Manager_Id;
                string mangerEmail = en.Employees.Find(managerIdOfUserRequest).Email;
                ccMails.Add(mangerEmail);
                //bccMails.Add("it-servicedesk@cjvina.com");
            }
            else if (level_confirm == 8) // Level 8: Return NOT APPROVED
            {
                string userRequestEmail = en.Employees.Find(userRequestId).Email;
                toMails.Add(userRequestEmail);

                string managerIdOfUserRequest = en.Employees
                    .Join(en.Departments, e => e.Department_ID, d => d.Department_Id, (e, d) => new { e, d })
                    .FirstOrDefault(grp => grp.e.Emp_CJ == userRequestId).d.Manager_Id;
                string mangerEmail = en.Employees.Find(managerIdOfUserRequest).Email;
                ccMails.Add(mangerEmail);
                //bccMails.Add("it-servicedesk@cjvina.com");
            }

            // Check duplicate between toMails and ccMails
            if (ccMails != null)
                foreach (string m in toMails)
                    if (ccMails.Contains(m))
                        ccMails.Remove(m);


            if (bccMails != null)
            {
                // Check duplicate between toMails and bccMails
                foreach (string m in toMails)
                    if (bccMails.Contains(m))
                        bccMails.Remove(m);
                // Check duplicate between ccMails and bccMails
                foreach (string m in ccMails)
                    if (bccMails.Contains(m))
                        bccMails.Remove(m);
            }

            return MailHelper.Send_Mail(Define_Email(subject, body, toMails, ccMails, bccMails));
        }
        public MailMessage Define_Email(string subject, string body, List<string> toMails = null, List<string> ccMails = null, List<string> bccMails = null)
        {
            MailMessage msg = new MailMessage();

            //receiver TO
            if (toMails != null && toMails.Count > 0)
            {
                foreach (string m in toMails)
                    if (!string.IsNullOrWhiteSpace(m) && m.Contains(@"@cjvina.com"))
                        msg.To.Add(m);

                if (msg.To.Count > 0)
                {
                    //receiver CC
                    if (ccMails != null && ccMails.Count > 0)
                        foreach (string m in ccMails)
                            if (!string.IsNullOrWhiteSpace(m))
                                msg.CC.Add(m);

                    //receiver BCC
                    if (bccMails != null && bccMails.Count > 0)
                        foreach (string m in bccMails)
                            if (!string.IsNullOrWhiteSpace(m))
                                msg.Bcc.Add(m);

                    ///Define mail message
                    msg.From = new MailAddress(ConfigurationManager.AppSettings.Get("EmailID")); // from it-servicedesk@cjvina.com

                    msg.Subject = subject;
                    msg.Body = body;
                }
            }
            return msg;
        }
    }
}