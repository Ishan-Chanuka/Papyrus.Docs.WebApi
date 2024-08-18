using Papyrus.Docs.Email.Service.Models;

namespace Papyrus.Docs.Email.Service.Services.Interfaces
{
    public interface IEmailSenderService
    {
        Task<bool> SendEmailAsync(Message message);
    }
}
