using System.Net.Mail;

namespace YankiApi.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
   
}
