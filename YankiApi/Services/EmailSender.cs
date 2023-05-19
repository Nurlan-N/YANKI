using MimeKit.Text;
using MimeKit;
using YankiApi.Interfaces;
using MailKit.Security;
using MailKit.Net.Smtp;
using YankiApi.DTOs;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;

namespace YankiApi.Services
{
    /// <summary>
    /// Email Sender
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSetting _smtpSetting;

        public EmailSender(IOptions<SmtpSetting> smtpSetting)
        {
            _smtpSetting = smtpSetting.Value;
        }

        /// <summary>
        /// Send email Services
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_smtpSetting.Email));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect(_smtpSetting.Host, (int)_smtpSetting.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_smtpSetting.Email, _smtpSetting.Password);
            smtp.Send(email);
            smtp.Disconnect(true);


        }
    }
}
