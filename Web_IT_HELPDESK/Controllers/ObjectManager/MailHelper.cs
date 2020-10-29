﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Resources;
using System.Web;
using System.Web.Providers.Entities;
using Web_IT_HELPDESK.Properties;

namespace Web_IT_HELPDESK.Controllers.ObjectManager
{
    public class MailHelper
    {
        /// <summary>
        /// This method just send mail
        /// </summary>
        /// <param name="msg"></param>
        /// <returns> (bool) Result of sending </returns>
        public static bool Send_Mail(MailMessage msg)
        {
            using (SmtpClient SmtpServer = new SmtpClient("mail.cjvina.com", 587))
            {
                try
                {
                    SmtpServer.Port = 25;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(Resources.SenderID, Resources.SenderPW);
                    SmtpServer.EnableSsl = false;
                    SmtpServer.Send(msg);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}