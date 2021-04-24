using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using TryMoreAPI.Models;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Net.Http;
using System.IO;
using System.Xml.Serialization;
using System.Net.Mail;
using System.Web;
//using TherapistAPI.Common;

namespace TryMoreAPI.DataAccess
{
    public class SendMail
    {
        public void SendGeneralMail(string subject, string email, string body)
        {
            // string body = this.createEmailBody(username, htmlname);

            this.SendHtmlFormattedEmail(subject, body, email);
        }

        private void SendHtmlFormattedEmail(string subject, string body, string email)

        {

            using (MailMessage mailMessage = new MailMessage())

            {

                //mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["UserName"]);
                mailMessage.From = new MailAddress("jigarthakkar0000@gmail.com");

                mailMessage.Subject = subject;

                mailMessage.Body = body;

                mailMessage.IsBodyHtml = true;

                mailMessage.To.Add(new MailAddress(email));

                SmtpClient smtp = new SmtpClient();

                smtp.Host = "smtp.gmail.com";

                smtp.EnableSsl = true;

                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();

                NetworkCred.UserName = "jigarthakkar0000@gmail.com"; //reading from web.config  

                NetworkCred.Password = "Jj@9033392381"; //reading from web.config  

                smtp.UseDefaultCredentials = false;

                smtp.Credentials = NetworkCred;

                smtp.Port = 587; //reading from web.config 

                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                smtp.Send(mailMessage);
                //                Gmail SMTP port(TLS): 587
                //Gmail SMTP port(SSL): 465

                // Allow secure app
                //https://www.google.com/settings/security/lesssecureapps 

            }

        }

        public string createEmailBody(string htmlname)

        {

            string body = string.Empty;
            //using streamreader for reading my htmltemplate   

            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/MailTemplate/" + htmlname)))

            {

                body = reader.ReadToEnd();

            }

            //body = body.Replace("{UserName}", userName); //replacing the required things  

            return body;

        }
    }
}