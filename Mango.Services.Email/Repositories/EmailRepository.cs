using Mango.Services.Email.DbContexts;
using Mango.Services.Email.Messaging;
using Mango.Services.Email.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.Email.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public EmailRepository(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public async Task SendAndLogEmail(UpdatePaymentResultMessage message)
        {
            using var db= _factory.CreateDbContext();
            //! You can implement email sender

            EmailLog log = new()
            {
                Email = message.Email,
                EmailSent = DateTime.Now,
                Log = $"Order - {message.OrderId} has been created"
            };

            db.EmailLogs.Add(log);
            await db.SaveChangesAsync();
        }
    }
}
