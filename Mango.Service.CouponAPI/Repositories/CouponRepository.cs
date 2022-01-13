using AutoMapper;
using Mango.Service.CouponAPI.DbContexts;
using Mango.Service.CouponAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Mango.Service.CouponAPI.Repositories
{
    public class CouponRepository : ICouponRepository
    {

        private readonly ApplicationDbContext _db;
        private IMapper _mapper;

        public CouponRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<CouponDto> GetCouponByCode(string couponCode)
        {
            var coupon=await _db.Coupons.FirstOrDefaultAsync(t=>t.CouponCode==couponCode);
            return _mapper.Map<CouponDto>(coupon);  
        }
    }
}
