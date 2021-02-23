using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Web;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.Properties;
using Web_IT_HELPDESK.ViewModels;

namespace Web_IT_HELPDESK.Controllers.ObjectManager
{
    public class IncidentHelper
    {

        /// <summary>
        /// This method send mail of Incident
        /// </summary>
        /// <param name="incM"></param>
        /// <param name="actionName"></param>
        /// <param name="toMails"></param>
        /// <param name="ccMails"></param>
        /// <param name="bccMails"></param>
        /// <returns> (bool) Result of sending </returns>
        public bool Send_IncidentEmail(IncidentViewModel incM, string actionName = null, List<string> toMails = null, List<string> ccMails = null, List<string> bccMails = null)
        {
            // Check toMails is null
            if (toMails == null)
            {
                return false;
            }
            else
            {
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
            }

            return MailHelper.Send_Mail(Define_IncidentMail(incM, actionName, toMails, ccMails, bccMails));
        }

        /// <summary>
        /// This method define mail message of Incident
        /// </summary>
        /// <param name="incM"></param>
        /// <param name="actionName"></param>
        /// <param name="toMails"></param>
        /// <param name="ccMails"></param>
        /// <param name="bccMails"></param>
        /// <returns> Mail Message </returns>
        public MailMessage Define_IncidentMail(IncidentViewModel incM, string actionName, List<string> toMails = null, List<string> ccMails = null, List<string> bccMails = null)
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
                    //Create mail subject
                    string subject = string.Empty;
                    switch (actionName.ToUpper())
                    {
                        case "CREATE":
                            subject = string.Format("[ServiceDesk]New IT Order Request no.{0} has been CREATED!", incM.Code);
                            break;
                        case "EDIT":
                            subject = string.Format("[ServiceDesk]Request no.{0} has been EDITED!", incM.Code);
                            break;
                        case "SOLVE":
                            subject = string.Format("[ServiceDesk]Request no.{0} has been SOLVED!", incM.Code);
                            break;
                        case "DELETE":
                            subject = string.Format("[ServiceDesk]Request no.{0} has been DELETED!", incM.Code);
                            break;
                        default:
                            subject = string.Format("[ServiceDesk]IT Order Request no.{0}!", incM.Code);
                            break;
                    }

                    //Create mail body html
                    string bodyHtml = string.Empty;
                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath(@"~/Views/Incident/IncidentEmail.cshtml")))
                    {
                        bodyHtml = reader.ReadToEnd();
                    }

                    bodyHtml = bodyHtml.Replace("{incNo}", incM.Code);
                    bodyHtml = bodyHtml.Replace("{datetime}", incM.datetime.ToString());
                    bodyHtml = bodyHtml.Replace("{plant}", DepartmentModel.Instance.getPlantName(incM.Plant));
                    bodyHtml = bodyHtml.Replace("{User_Create}", incM.userCreateName);
                    bodyHtml = bodyHtml.Replace("{Status}", incM.statusName);
                    bodyHtml = bodyHtml.Replace("{Level}", incM.levelName);
                    bodyHtml = bodyHtml.Replace("{Note}", incM.Note);
                    bodyHtml = bodyHtml.Replace("{Reply}", incM.Reply);
                    bodyHtml = bodyHtml.Replace("{Attached_File}", incM.FileName);

                    //Create attached file if incident.FileAttached exist
                    Attachment attached_file = null;
                    if (incM.FileAttched != null && !string.IsNullOrWhiteSpace(incM.FileName))
                    {
                        byte[] dataAttFile = incM.FileAttched;
                        Stream attFileStream = new MemoryStream(dataAttFile);
                        attached_file = new Attachment(attFileStream, incM.FileName);
                    }

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
                    msg.From = new MailAddress(ConfigurationManager.AppSettings.Get("EmailID")); // from sender
                    msg.Subject = subject;
                    msg.IsBodyHtml = true;
                    msg.Body = bodyHtml.ToString();
                    if (attached_file != null)
                        msg.Attachments.Add(attached_file);
                }
            }
            return msg;
        }

        private IncidentHelper() { }

        private static IncidentHelper _instance;

        public static IncidentHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new IncidentHelper();
                }
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

    }
}