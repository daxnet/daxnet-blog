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
        private readonly string smtpServerName;//= Environment.GetEnvironmentVariable("DAXNETBLOG_SMTP_SERVERNAME");
        private readonly string smtpUserName;
        private readonly string smtpPassword;
        private readonly Crypto crypto = Crypto.Create(CryptoTypes.EncTypeTripleDes);

        public EmailService()
        {
            try
            {
                smtpServerName = Environment.GetEnvironmentVariable("DAXNETBLOG_SMTP_SERVERNAME");
                smtpUserName = crypto.Decrypt(Environment.GetEnvironmentVariable("DAXNETBLOG_SMTP_USERNAME"), Crypto.GlobalKey);
                smtpPassword = crypto.Decrypt(Environment.GetEnvironmentVariable("DAXNETBLOG_SMTP_PASSWORD"), Crypto.GlobalKey);
            }
            catch
            {
                smtpServerName = null;
                smtpUserName = null;
                smtpPassword = null;
            }
        }

        public async Task SendEmailAsync(string toName, string toAddess, string title, string bodyHtml, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!string.IsNullOrEmpty(smtpServerName) &&
                !string.IsNullOrEmpty(smtpUserName) &&
                !string.IsNullOrEmpty(smtpPassword))
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
                    await client.ConnectAsync(smtpServerName, cancellationToken: cancellationToken);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(smtpUserName, smtpPassword);

                    client.Send(message);

                    client.Disconnect(true);
                }
            }
        }
    }
}
