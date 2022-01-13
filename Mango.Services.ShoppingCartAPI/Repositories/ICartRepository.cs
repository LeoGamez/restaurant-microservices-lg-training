using Mango.Services.ShoppingCartAPI.Model.DTOs;

namespace Mango.Services.ShoppingCartAPI.Repositories
{
    public interface ICartRepository
    {
        Task<CartDto> GetCartByUserId(string id);
        Task<CartDto> CreateCart(CartDto cartDto);
        Task<bool> AddToCart(CartDetailDto cartDetailDto);
        Task<bool> ClearCart(string id);
        Task<bool> RemoveFromCart(int cartDetailsId);         
        Task<bool> ApplyCoupon(string userId,string couponCode);
        Task<bool> RemoveCoupon(string userId);

    }
}
