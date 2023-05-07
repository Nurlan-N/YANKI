using MimeKit.Text;
using MimeKit;
using YankiApi.Interfaces;
using MailKit.Security;
using MailKit.Net.Smtp;


namespace YankiApi.Services
{
    public class EmailSender : IEmailSender
    {

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("nazarov.nurlan@gmail.com"));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("nazarov.nurlan@gmail.com", "upjypdqvjugcaipp");
            smtp.Send(email);
            smtp.Disconnect(true);


        }
    }
}
