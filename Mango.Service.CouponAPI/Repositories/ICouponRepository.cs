using Mango.Service.CouponAPI.Models.DTOs;

namespace Mango.Service.CouponAPI.Repositories
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCouponByCode(string couponCode);
    }
}
