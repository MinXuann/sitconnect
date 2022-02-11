using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace sitconnect.Services
{
    public class EmailSender : IEmailSender
    {
        public readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }
        
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(subject, message, email);
        }

        public Task Execute(string subject, string message, string email)
        {
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);

            smtpClient.Credentials = new NetworkCredential(_config["SmtpEmail"], _config["SmtpPassword"]);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            
            // do not use in production, disables checks for valid SSL certificate
            ServicePointManager.ServerCertificateValidationCallback =
                delegate {
                    return true;
                };

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("sitconnectsg@gmail.com", "SITConnect");
            mail.To.Add(new MailAddress(email));
            mail.Subject = subject;
            mail.Body = message;

            return smtpClient.SendMailAsync(mail);
        }
    }
}