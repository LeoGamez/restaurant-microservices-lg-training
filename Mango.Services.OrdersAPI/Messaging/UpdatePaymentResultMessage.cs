namespace Mango.Services.OrdersAPI.Messaging
{
    public class UpdatePaymentResultMessage
    {
        public int OrderId { get; set; }
        public bool Status { get; set; }
        public string Email { get; set; }

    }
}
