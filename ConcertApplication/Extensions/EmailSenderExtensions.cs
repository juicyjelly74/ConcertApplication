using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ConcertApplication.Models;
using ConcertApplication.Services;
using Microsoft.AspNetCore.Hosting;

namespace ConcertApplication.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }

        public static void SendBookingConfirmationAsync(this IEmailSender emailSender, IHostingEnvironment appEnvironment,
            string emailAddress, int amount, ConcertModel currentConcert)
        {
            MailMessage mail = new MailMessage("fromm@gmail.com", emailAddress);
            mail.Subject = "Ticket Booking";

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.Credentials = new System.Net.NetworkCredential()
            {
                UserName = "username",
                Password = "password"
            };

            mail.IsBodyHtml = true;
            var pathToFile = appEnvironment.WebRootPath
                    + Path.DirectorySeparatorChar.ToString()
                    + "templates"
                    + Path.DirectorySeparatorChar.ToString()
                    + "concertEmailPage.html";
            using (StreamReader reader = System.IO.File.OpenText(pathToFile))
            {
                mail.Body = reader.ReadToEnd();
            }

            //mail.Body = String.Format(mail.Body, emailAddress, currentConcert.Name, amount.ToString()
            //    , currentConcert.Performer, currentConcert.Type, currentConcert.Place, currentConcert.Price.ToString()
            //    , currentConcert.TicketsAmount.ToString(), currentConcert.TicketsLeft.ToString()
            //    , String.Format("{0:d/M/yyyy HH:mm:ss}", currentConcert.Date)
            //    , String.Format("{0:d/M/yyyy HH:mm:ss}", DateTime.Now)
            //    );

            mail.Body = mail.Body.Replace("{user}", emailAddress);
            mail.Body = mail.Body.Replace("{name}", currentConcert.Name);
            mail.Body = mail.Body.Replace("{amount}", amount.ToString());
            mail.Body = mail.Body.Replace("{performer}", currentConcert.Performer);
            mail.Body = mail.Body.Replace("{type}", currentConcert.Type);
            mail.Body = mail.Body.Replace("{place}", currentConcert.Place);
            mail.Body = mail.Body.Replace("{price}", currentConcert.Price.ToString());
            mail.Body = mail.Body.Replace("{ticketsAmount}", currentConcert.TicketsAmount.ToString());
            mail.Body = mail.Body.Replace("{ticketsLeft}", currentConcert.TicketsLeft.ToString());
            mail.Body = mail.Body.Replace("{date}", String.Format("{0:d/M/yyyy HH:mm:ss}", currentConcert.Date));
            mail.Body = mail.Body.Replace("{now}", String.Format("{0:d/M/yyyy HH:mm:ss}", DateTime.Now));

            smtpClient.EnableSsl = true;
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
                    System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                    System.Security.Cryptography.X509Certificates.X509Chain chain,
                    System.Net.Security.SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };
            smtpClient.Send(mail);
        }
    }
}
