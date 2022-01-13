using Mango.Services.ShoppingCartAPI.Model.DTOs;

namespace Mango.Services.ShoppingCartAPI.Repositories
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCouponByCode(string couponCode);
    }
}
