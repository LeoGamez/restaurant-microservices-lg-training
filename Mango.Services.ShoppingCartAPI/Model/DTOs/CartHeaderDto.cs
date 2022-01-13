using System.ComponentModel.DataAnnotations;

namespace Mango.Services.ShoppingCartAPI.Model.DTOs
{
    public class CartHeaderDto
    {
        public int CartHeaderId { get; set; }
        public string UserId { get; set; }
        public string CouponCode { get; set; }
    }
}
