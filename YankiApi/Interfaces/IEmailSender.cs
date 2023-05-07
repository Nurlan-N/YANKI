using System.Net.Mail;

namespace YankiApi.Interfaces
{
    /// <summary>
    /// Send Email Interface
    /// </summary>
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
   
}
