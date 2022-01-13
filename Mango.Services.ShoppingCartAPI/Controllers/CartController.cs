using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Messages;
using Mango.Services.ShoppingCartAPI.Model.DTOs;
using Mango.Services.ShoppingCartAPI.RabbitMQSender;
using Mango.Services.ShoppingCartAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : Controller
    {
        private readonly ICartRepository cartRepository;
        private readonly ICouponRepository couponRepository;
        private readonly IMessageBus messageBus;
        private readonly ICartMessageSender cartMessageSender;

        private ResponseDto _response;

        public CartController(ICartRepository cartRepository, ICouponRepository couponRepository, IMessageBus messageBus, ICartMessageSender cartMessageSender)
        {
            this.cartRepository = cartRepository;
            this.couponRepository = couponRepository;
            this.messageBus = messageBus;
            this.cartMessageSender = cartMessageSender;
            this._response = new ResponseDto();

        }
    
        [HttpGet("GetCart/{id}")]
        public async Task<object> GetCart(string id)
        {
            try
            {
                CartDto cartDto = await cartRepository.GetCartByUserId(id);
                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpPost("CreateCart")]
        public async Task<object> CreateCart([FromBody] CartDto cartDto)
        {
            try
            {
                _response.Result = await cartRepository.CreateCart(cartDto);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpPost("AddToCart")]
        public async Task<object> AddToCart([FromBody] CartDetailDto cartDetailDto)
        {
            try
            {
                _response.Result = await cartRepository.AddToCart(cartDetailDto);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpPost("RemoveFromCart")]
        public async Task<object> RemoveFromCart([FromBody] int id)
        {
            try
            {
                var isSuccess = await cartRepository.RemoveFromCart(id);

                _response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var isSuccess = await cartRepository.ApplyCoupon(cartDto.CartHeader.UserId,
                    cartDto.CartHeader.CouponCode);

                _response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }
        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var isSuccess = await cartRepository.RemoveCoupon(cartDto.CartHeader.UserId);

                _response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpPost("Checkout")]
        public async Task<object> Checkout([FromBody] CheckoutHeaderDto checkoutHeaderDto)
        {
            try
            {
                CartDto cartDto= await cartRepository.GetCartByUserId(checkoutHeaderDto.UserId);

                if (cartDto == null)
                {
                    return BadRequest();
                }

                if (!String.IsNullOrEmpty(checkoutHeaderDto.CouponCode))
                {
                    var couponDto= await couponRepository.GetCouponByCode(checkoutHeaderDto.CouponCode);
                    if(couponDto.DiscountAmount/100*checkoutHeaderDto.OrderTotal != checkoutHeaderDto.DiscountTotal)
                    {
                        _response.IsSuccess=false;
                        _response.ErrorMessages = new List<string> { "Coupon has changed, please Confirm" };
                        _response.DisplayMessage =  "Coupon has changed, please Confirm" ;
                        return _response;
                    }
                }


                checkoutHeaderDto.CartDetails = cartDto.CartDetails;

                //! For Azure Message Bus
                //await messageBus.PublishMessage(checkoutHeaderDto, "checkoutqueue");


                //! For RabbitMQ
                cartMessageSender.SendMessage(checkoutHeaderDto, "checkoutqueue");


                await cartRepository.ClearCart(checkoutHeaderDto.UserId);   

                _response.Result = true; 
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }
    }
}
