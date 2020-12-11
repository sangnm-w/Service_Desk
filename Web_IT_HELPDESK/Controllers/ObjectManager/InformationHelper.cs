using System;
using System.Collections.Generic;
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

        public bool Send_Mail(int level_confirm, string subject, string body, Employee userRequest)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            List<string> toMails = new List<string>();
            List<string> ccMails = new List<string>();
            List<string> bccMails = new List<string>();

            if (level_confirm == 1) // Level 1: Department Manager
            {
                toMails.Add(en.Departments.FirstOrDefault(d => d.Plant_Id == userRequest.Plant_Id && d.Department_Id == userRequest.Department_Id).Manager_Email);
                ccMails.Add("test01.it@cjvina.com");
                bccMails.Add("it-servicedesk@cjvina.com");
            }
            else if (level_confirm == 2) // Level 2: Resend
            {
                toMails.Add(en.Departments.FirstOrDefault(d => d.Plant_Id == userRequest.Plant_Id && d.Department_Id == userRequest.Department_Id).Manager_Email);
                ccMails.Add("test01.it@cjvina.com");
                bccMails.Add("it-servicedesk@cjvina.com");
            }
            else if (level_confirm == 3) // Level 3: BOD
            {
                toMails.Add(en.Departments.FirstOrDefault(d => d.Plant_Id == userRequest.Plant_Id && d.Department_Name.Contains(")BOD")).Manager_Email);
                ccMails.Add("test01.it@cjvina.com");
                bccMails.Add("it-servicedesk@cjvina.com");
            }
            else if (level_confirm == 4) // Level 4: HR Manager
            {
                toMails.Add(en.Departments.FirstOrDefault(d => d.Plant_Id == userRequest.Plant_Id && d.Department_Name.Contains(")HR")).Manager_Email);
                ccMails.Add("test01.it@cjvina.com");
                bccMails.Add("it-servicedesk@cjvina.com");

            }
            else if (level_confirm == 5) // Level 5: HR Admin
            {
                toMails.Add(en.Departments.FirstOrDefault(d => d.Plant_Id == userRequest.Plant_Id && d.Department_Name.Contains(") HR-Admin")).Manager_Email);
                ccMails.Add("test01.it@cjvina.com");
                bccMails.Add("it-servicedesk@cjvina.com");
            }
            else if (level_confirm == 6) // Level 6: Return result
            {
                toMails.Add(userRequest.Email);
                ccMails.Add("test01.it@cjvina.com");
                bccMails.Add("it-servicedesk@cjvina.com");
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
                    msg.From = new MailAddress(Resources.SenderID); // from sender

                    msg.Subject = subject;
                    msg.Body = body;
                }
            }
            return msg;
        }
    }
}