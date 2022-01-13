using Mango.Services.OrdersAPI.Models;

namespace Mango.Services.OrdersAPI.Repositories
{
    public interface IOrderRepository
    {
        Task<bool> AddOrder(OrderHeader order);
        Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid);
    }
}
