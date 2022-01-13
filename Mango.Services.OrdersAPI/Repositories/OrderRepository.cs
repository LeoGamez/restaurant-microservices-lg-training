using Mango.Services.OrdersAPI.DbContexts;
using Mango.Services.OrdersAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.OrdersAPI.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public OrderRepository(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<bool> AddOrder(OrderHeader order)
        {
            try
            {
                using var db = _factory.CreateDbContext();
                db.OrderHeaders.Add(order);
                await db.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid)
        {
            using var db = _factory.CreateDbContext();

            var orderHeader = db.OrderHeaders.FirstOrDefault(x => x.OrderHeaderId == orderHeaderId);

            if (orderHeader != null)
            {
                orderHeader.PaymentStatus = paid;

                db.OrderHeaders.Update(orderHeader);
                await db.SaveChangesAsync();
            }
        }
    }
}
