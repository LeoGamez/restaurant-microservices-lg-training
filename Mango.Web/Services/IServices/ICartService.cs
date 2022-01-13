using System.Threading.Tasks;
using System.Collections.Generic;
using Mango.Web.Models.DTOs;



namespace Mango.Web.Services.IServices
{
    public interface ICartService : IBaseService
    {
        Task<T> GetCartByUserIdAsync<T>(string userId, string token = null);
        Task<T> AddToCartAsync<T>(CartDetailDto cartDetailDto, string token = null);
        Task<T> UpdateCartAsync<T>(CartDto cartDto, string token = null);
        Task<T> RemoveFromCartAsync<T>(int cartId, string token = null);
        Task<T> CreateCartAsync<T>(CartDto cartDto, string token = null);
        Task<T> ApplyCoupon<T>(CartDto cartDto, string token = null);
        Task<T> RemoveCoupon<T>(CartDto cartDto, string token = null);
        Task<T> Checkout<T>(CartHeaderDto cartHeaderDto, string token = null);




    }
}
