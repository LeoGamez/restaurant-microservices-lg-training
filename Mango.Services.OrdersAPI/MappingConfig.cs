using AutoMapper;

namespace Mango.Services.OrdersAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config => {
                //config.CreateMap<ProductDto, Product>().ReverseMap();
                //config.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
           
            });

            return mappingConfig;
        }
    }
}
