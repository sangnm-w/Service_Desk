﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using Web_IT_HELPDESK.Models;
using Web_IT_HELPDESK.Properties;
using Web_IT_HELPDESK.ViewModels;

namespace Web_IT_HELPDESK.Controllers.ObjectManager
{
    public class BizTripHelper
    {
        private BizTripHelper() { }
        private static BizTripHelper _instance;

        public static BizTripHelper Instance { get { if (_instance == null) _instance = new BizTripHelper(); return _instance; } private set => _instance = value; }

        /// <summary>
        /// This method define mail message of Biztrip
        /// </summary>
        /// <param name="incM"></param>
        /// <param name="actionName"></param>
        /// <param name="toMails"></param>
        /// <param name="ccMails"></param>
        /// <param name="bccMails"></param>
        /// <returns> Mail Message </returns>
        public MailMessage Define_BiztripMail(IncidentViewModel incM, string actionName, List<string> toMails = null, List<string> ccMails = null, List<string> bccMails = null)
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
                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath(@"~/Views/BIZ_TRIP/BiztripEmail.cshtml")))
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
                    msg.From = new MailAddress(Resources.SenderID); // from sender
                    msg.Subject = subject;
                    msg.IsBodyHtml = true;
                    msg.Body = bodyHtml.ToString();
                    if (attached_file != null)
                        msg.Attachments.Add(attached_file);
                }
            }
            return msg;
        }
    }
}