using System.ComponentModel.DataAnnotations;

namespace Mango.Services.OrdersAPI.Models
{
    public class OrderHeader
    {
        [Key]

        public int OrderHeaderId { get; set; }
        public string UserId { get; set; }
        public string CouponCode { get; set; } = "";
        public double OrderTotal { get; set; }
        public double DiscountTotal { get; set; }
        public double OrderWithDiscountTotal { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime PickupDateTime { get; set; }
        public DateTime OrderDateTime { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public string ExpiryMonthYear { get; set; }
        public int CartTotalItems { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public bool PaymentStatus { get; set; }

    }
}
