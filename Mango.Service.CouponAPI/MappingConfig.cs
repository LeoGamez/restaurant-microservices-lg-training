using AutoMapper;
using Mango.Service.CouponAPI.Models;
using Mango.Service.CouponAPI.Models.DTOs;

namespace Mango.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config => {
                config.CreateMap<CouponDto, Coupon>().ReverseMap();
                //config.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
                //config.CreateMap<CartDetailDto, CartDetail>().ReverseMap();
                //config.CreateMap<Cart, CartDto>().ReverseMap();
            });

            return mappingConfig;
        }
    }
}
