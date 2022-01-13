using Mango.Web.Model.DTOs;
using Mango.Web.Models.DTOs;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {

        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;

        public CartController(IProductService productService, ICartService cartService, ICouponService couponService)
        {
            _productService = productService;
            _cartService = cartService;
            _couponService = couponService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await LoadCartDtoByLoggedInUser());
        }


        [Authorize]
        [HttpGet]

        public async Task<IActionResult> Checkout()
        {
            var model = await LoadCartDtoByLoggedInUser();

            return View(model);
        }

        [ActionName("Checkout")]
        [Authorize]
        [HttpPost]

        public async Task<IActionResult> CheckoutPost(CartDto cartDto)
        {
            try
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                var response = await _cartService.Checkout<ResponseDto>(cartDto.CartHeader, token);
                if (response.IsSuccess)
                {
                    return RedirectToAction("Confirmation");

                }
                else
                {
                    ViewBag.Error = response.DisplayMessage;
                    return View(await LoadCartDtoByLoggedInUser());

                }
                return View(await LoadCartDtoByLoggedInUser());


            }
            catch (Exception ex)
            {
                return View(await LoadCartDtoByLoggedInUser());

            }

        }


        [Authorize]
        [ActionName("Confirmation")]

        public async Task<IActionResult> Confirmation()
        {
            return View();
        }

        [HttpPost]
        [ActionName("ApplyCoupon")]

        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.ApplyCoupon<ResponseDto>(cartDto, token);

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }


            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("RemoveCoupon")]

        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.RemoveCoupon<ResponseDto>(cartDto, token);

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.RemoveFromCartAsync<ResponseDto>(id, token);

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }
        private async Task<CartDto> LoadCartDtoByLoggedInUser()
        {
            var userId = User.Claims.Where(t => t.Type == "sub").FirstOrDefault().Value;
            var token = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.GetCartByUserIdAsync<ResponseDto>(userId,token);


            CartDto cartDto = new CartDto();
            if (response != null && response.IsSuccess)
            {
                cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
            }

            if (cartDto.CartHeader != null)
            {
                foreach (var detail in cartDto.CartDetails)
                {
                    cartDto.CartHeader.OrderTotal += (detail.Product.Price * detail.Count);
                }
            }

            if (cartDto.CartHeader != null)
            {
                if (!String.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
                {
                    var responseCoupon = await _couponService.GetCouponByCode<ResponseDto>(cartDto.CartHeader.CouponCode.ToUpper(), token);

                    if (responseCoupon != null && responseCoupon.IsSuccess)
                    {
                        if (responseCoupon.Result != null)
                        {
                            var coupon = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(responseCoupon.Result));
                            cartDto.CartHeader.DiscountTotal = cartDto.CartHeader.OrderTotal * (coupon.DiscountAmount / 100.0);
                            cartDto.CartHeader.OrderWithDiscountTotal = cartDto.CartHeader.OrderTotal - cartDto.CartHeader.DiscountTotal;
                        }
                        else
                        {
                            cartDto.CartHeader.DiscountTotal = 0;
                            cartDto.CartHeader.OrderWithDiscountTotal = cartDto.CartHeader.OrderTotal - cartDto.CartHeader.DiscountTotal;
                        }
                    }
                    else
                    {
                        cartDto.CartHeader.DiscountTotal = 0;

                        cartDto.CartHeader.OrderWithDiscountTotal = cartDto.CartHeader.OrderTotal - cartDto.CartHeader.DiscountTotal;

                    }
                }
            }

            return cartDto;
        }
    }
}
