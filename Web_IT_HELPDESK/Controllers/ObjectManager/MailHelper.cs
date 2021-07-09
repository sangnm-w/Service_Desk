using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Resources;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Providers.Entities;
using Web_IT_HELPDESK.Properties;

namespace Web_IT_HELPDESK.Controllers.ObjectManager
{
    public class MailHelper
    {
        static bool mailSent = false;
        static bool received = false;
        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String token = (string)e.UserState;

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", token);
            }
            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                received = true;
                Console.WriteLine("Message sent.");
            }
            mailSent = true;
        }

        public static async Task SendEmail(string sender, string senderPW, List<string> receivers, string subject, string body, List<HttpPostedFileBase> attachments)
        {
            MailAddress from = new MailAddress(sender);
            MailMessage message = new MailMessage();
            message.From = from;
            message.Body = body;
            message.IsBodyHtml = true;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.Subject = subject;
            message.SubjectEncoding = System.Text.Encoding.UTF8;

            for (int i = 0; i < attachments?.Count; i++)
            {
                if (attachments[i]?.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(attachments[i].FileName);
                    message.Attachments.Add(new Attachment(attachments[i].InputStream, fileName));
                }
            }

            DataTable loggingTb = new DataTable();
            loggingTb.Columns.Add("receiver", typeof(string));
            loggingTb.Columns.Add("Feedback", typeof(string));
            int countSended = 0;
            int countFailed = 0;
            foreach (var receiver in receivers)
            {
                MailAddress to = new MailAddress(receiver);
                message.To.Add(to);
                try
                {
                    using (var smtpClient = new SmtpClient("mail.cjvina.com", 25))
                    {
                        smtpClient.Credentials = new NetworkCredential(sender, senderPW);
                        smtpClient.EnableSsl = false;
                        await smtpClient.SendMailAsync(message);
                        countSended++;
                        loggingTb.Rows.Add(message.To.ToString(), "Email sent successfully.");
                    }
                }
                catch (Exception ex)
                {
                    countFailed++;
                    loggingTb.Rows.Add(message.To.ToString(), ex.Message);
                    if (ex.Message.Contains("5.7.0 Authentication required. The account is disable by Spam Filter. Please contact Mr. Sang for support."))
                    {
                        break;
                    }
                }
                message.To.Clear();
            }
            logging(loggingTb, countSended, countFailed);
            loggingTb.Rows.Clear();
        }


        /// <summary>
        /// This method just send mail
        /// </summary>
        /// <param name="msg"> This is email is initialized in Define_Email method </param>
        /// <returns> (bool) Result of sending </returns>
        public static bool Send_Mail(MailMessage msg)
        {
            using (SmtpClient client = new SmtpClient("mail.cjvina.com", 25))
            {
                try
                {
                    client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings.Get("EmailID"), ConfigurationManager.AppSettings.Get("EmailPW"));
                    client.EnableSsl = false;
                    client.Send(msg);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public static void logging(DataTable loggingTb, int countSended, int countFailed)
        {
            string serverPath = HostingEnvironment.MapPath(Resources.MailingLogPath);
            if (!Directory.Exists(serverPath))
            {
                Directory.CreateDirectory(serverPath);
            }
            string logPath = Path.Combine(serverPath, DateTime.Now.ToString("yyyy'-'MM'-'dd'_'HH''mm''ss") + "_Mailing_log.txt");
            if (loggingTb.Rows[0]["receiver"].ToString().Equals("it-servicedesk@cjvina.com"))
                logPath = Path.Combine(serverPath, DateTime.Now.ToString("yyyy'-'MM'-'dd'_'HH''mm''ss") + "_CheckSender_log.txt");

            using (StreamWriter sw = new StreamWriter(logPath, true))
            {
                sw.WriteLine(new string('=', 30));
                sw.WriteLine(DateTime.Now.ToShortDateString());
                sw.WriteLine("Logging");
                sw.WriteLine($"Total: {loggingTb.Rows.Count}");
                sw.WriteLine($"Success: {countSended} and Failure: {countFailed}");
                sw.WriteLine(new string('=', 30));
                foreach (DataRow row in loggingTb.Rows)
                {
                    sw.WriteLine(row["receiver"].ToString());
                    sw.WriteLine(row["Feedback"].ToString());
                    sw.WriteLine(new string('-', row["Feedback"].ToString().Length));
                    sw.WriteLine();
                }
            }
        }
    }
}