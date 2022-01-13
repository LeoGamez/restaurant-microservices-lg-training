using Mango.Services.Email.Messaging;

namespace Mango.Services.Email.Repositories
{
    public interface IEmailRepository
    {
        Task SendAndLogEmail(UpdatePaymentResultMessage message);
    }
}
