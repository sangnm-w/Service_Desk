using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Resources;
using System.Threading.Tasks;
using System.Web;
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
        //public static bool SendMail(string sender, string senderPW, string receiver, string subject, string body)
        //{
        //    // Command-line argument must be the SMTP host.
        //    SmtpClient client = new SmtpClient("mail.cjvina.com", 25);
        //    client.Credentials = new NetworkCredential(sender, senderPW);
        //    client.EnableSsl = false;
        //    // Specify the email sender.
        //    // Create a mailing address that includes a UTF8 character
        //    // in the display name.
        //    MailAddress from = new MailAddress("it-servicedesk@cjvina.com",
        //       "IT " + (char)0xD8 + " Service Desk",
        //    System.Text.Encoding.UTF8);
        //    // Set destinations for the email message.
        //    MailAddress to = new MailAddress(receiver);
        //    // Specify the message content.
        //    MailMessage message = new MailMessage(from, to);
        //    message.Body = body;
        //    // Include some non-ASCII characters in body and subject.
        //    string someArrows = new string(new char[] { '\u2190', '\u2191', '\u2192', '\u2193' });
        //    message.Body += Environment.NewLine + someArrows;
        //    message.BodyEncoding = System.Text.Encoding.UTF8;
        //    message.Subject = subject + someArrows;
        //    message.SubjectEncoding = System.Text.Encoding.UTF8;

        //    // Set the method that is called back when the send operation ends.
        //    client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
        //    // The userState can be any object that allows your callback
        //    // method to identify this send operation.
        //    // For this example, the userToken is a string constant.
        //    string userState = "test message1";
        //    client.SendAsync(message, userState);
        //    //Console.WriteLine("Sending message... press c to cancel mail. Press any other key to exit.");
        //    //string answer = Console.ReadLine();
        //    // If the user canceled the send, and mail hasn't been sent yet,
        //    // then cancel the pending operation.
        //    //if (answer.StartsWith("c") && mailSent == false)
        //    //{
        //    //    client.SendAsyncCancel();
        //    //}
        //    // Clean up.
        //    message.Dispose();
        //    return received;
        //}

        public static async Task SendEmail(string sender, string senderPW, string receiver, string subject, string body, List<HttpPostedFileBase> attachments)
        {
            MailAddress from = new MailAddress(sender);

            MailAddress to = new MailAddress(receiver);
            MailMessage message = new MailMessage(from, to);
            message.Body = body;
            message.IsBodyHtml = true;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.Subject = subject;
            message.SubjectEncoding = System.Text.Encoding.UTF8;

            for (int i = 0; i < attachments?.Count; i++)
            {
                string fileName = Path.GetFileName(attachments[i].FileName);
                message.Attachments.Add(new Attachment(attachments[i].InputStream, fileName));
            }

            try
            {
                using (var smtpClient = new SmtpClient("mail.cjvina.com", 25))
                {
                    smtpClient.Credentials = new NetworkCredential(sender, senderPW);
                    smtpClient.EnableSsl = false;
                    await smtpClient.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
    }
}