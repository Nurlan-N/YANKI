using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Text;
using MimeKit;
using YankiApi.DTOs.AuthDTOs;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace YankiApi.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {

        [HttpPost]
        public IActionResult SendEmail(string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("nazarov.nurlan@gmail.com"));
            email.To.Add(MailboxAddress.Parse("Shabnam.nazarova00@gmail.com"));
            email.Subject = "Test";
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("nazarov.nurlan@gmail.com", "upjypdqvjugcaipp");
            smtp.Send(email);
            smtp.Disconnect(true);

            return Ok();
        }

    }
}
