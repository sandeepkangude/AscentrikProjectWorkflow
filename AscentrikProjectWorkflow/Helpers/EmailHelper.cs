
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace AscentrikProjectWorkflow.Helper
{
    public static class EmailHelper
    {
        public static readonly String SMTPService = GetConfigValue("SmtpServer");
        public static readonly String Port = GetConfigValue("Port");
        public static readonly String ContactEmail = GetConfigValue("ContactEmail");
        public static readonly String FromEmail = GetConfigValue("FromEmail");
        public static readonly String FromPassword = GetConfigValue("FromPassword");
        public static readonly String IsSSL = GetConfigValue("IsSSL");

        private static String GetConfigValue(String key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key];
        }

        public static void SendEmail(String emailBody, String emailTo, String subject)
        {
            try
            {
                if (String.IsNullOrEmpty(emailTo))
                    emailTo = ContactEmail;
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(SMTPService);

                mail.From = new MailAddress(FromEmail);
                mail.To.Add(emailTo);
                mail.Subject = subject;
                mail.Body = emailBody;
                mail.IsBodyHtml = true;
                SmtpServer.Port = Convert.ToInt16(Port);
                SmtpServer.Credentials = new System.Net.NetworkCredential(FromEmail, FromPassword);
                SmtpServer.EnableSsl = IsSSL == "True";
                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void SmtpServer_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                //Log message
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="emailAddress"></param>
        /// <param name="webUrl"></param>
        /// <param name="changedText"></param>
        public static void SendRegisterUserEmail(string userName, string password, string emailAddress)
        {
            String body = string.Format("<p> Dear {0},<br/> <br/>Welcome to Ascentrik Project Workflow. Please find below your credentials to login. <br/><br/><strong>UserName:</strong> {1} <br> <strong>Password:</strong> {2} <br/><br/>", userName, emailAddress, password);
            SendEmail(body, emailAddress, "Welcome to Ascentrik Project Workflow.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="emailAddress"></param>
        public static void SendChangePasswordEmail(string userName, string password, string emailAddress)
        {
            String body = string.Format("<p> Dear {0},<br/> <br/>You password has been changed successfully. Please find below your new credentials to login. <br/><br/><strong>UserName:</strong> {1} <br> <strong>Password:</strong> {2} <br/><br/>", userName, emailAddress, password);
            SendEmail(body, emailAddress, "Password change - Ascentrik Project Workflow.");
        }

    }
}