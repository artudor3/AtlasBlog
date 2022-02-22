using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace AtlasBlog.Services
{
    public class BasicEmailService : IEmailSender
    {
        private readonly IConfiguration _appSettings;

        public BasicEmailService(IConfiguration appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //Compose Email based on the data supplied by the user
            MimeMessage newEmail = new();
            var emailSender = _appSettings["SmtpSettings:UserName"];


            newEmail.Sender = MailboxAddress.Parse(emailSender);
            newEmail.To.Add(MailboxAddress.Parse(email));
            newEmail.Subject = subject;

            //Email Body
            BodyBuilder emailBody = new();
            emailBody.HtmlBody = htmlMessage;
            newEmail.Body = emailBody.ToMessageBody();

            //Email is composed at this point,
            //Configure Simple Mail Transfer Protocol (SMTP) Server
            using SmtpClient smtpClient = new();

            var host = _appSettings["SmtpSettings:Host"];
            var port = Convert.ToInt32(_appSettings["SmtpSettings:Post"]);

            await smtpClient.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(emailSender, _appSettings["SmtpSettings:Password"]);
            await smtpClient.SendAsync(newEmail);
            await smtpClient.DisconnectAsync(true);

        }
    }
}
