using Mango.Services.ShoppingCartAPI.Model.DTOs;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartAPI.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly HttpClient _httpClient;

        public CouponRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CouponDto> GetCouponByCode(string couponCode)
        {
            var response = await _httpClient.GetAsync($"/api/coupon/{couponCode}");
            var apiContent= await response.Content.ReadAsStringAsync();
            var apiResponse =JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (apiResponse.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(apiResponse.Result));
            }

            return new CouponDto();
        }
    }
}
