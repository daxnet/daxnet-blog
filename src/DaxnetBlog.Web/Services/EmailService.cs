using System;
using System.Threading;
using System.Threading.Tasks;
using DaxnetBlog.Common;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;

namespace DaxnetBlog.Web.Services
{
    public class EmailService : IEmailService
    {
        public EmailService()
        {
        }

        public async Task SendEmailAsync(string toName, string toAddess, string title, string bodyHtml, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!string.IsNullOrEmpty(EnvironmentVariables.WebSmtpServerName) &&
                !string.IsNullOrEmpty(EnvironmentVariables.WebSmtpUserName) &&
                !string.IsNullOrEmpty(EnvironmentVariables.WebSmtpPassword))
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("daxnet", "daxnet@outlook.com"));
                message.To.Add(new MailboxAddress(toName, toAddess));
                message.Subject = title;
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = bodyHtml
                };

                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (a, b, c, d) => true;
                    await client.ConnectAsync(EnvironmentVariables.WebSmtpServerName, cancellationToken: cancellationToken);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(EnvironmentVariables.WebSmtpUserName, EnvironmentVariables.WebSmtpPassword);

                    client.Send(message);

                    client.Disconnect(true);
                }
            }
        }
    }
}
