using Mango.Web.Model.DTOs;
using Mango.Web.Models;
using Mango.Web.Models.DTOs;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto> list = new();
            var token = await HttpContext.GetTokenAsync("access_token");

            var response = await _productService.GetALLProductsAsync<ResponseDto>("");

            if(response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }

            return View(list);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            ProductDto product = new ProductDto();

            var token = await HttpContext.GetTokenAsync("access_token");

            var response = await _productService.GetProductByIdAsync<ResponseDto>(id,"");

            if (response != null && response.IsSuccess)
            {
                product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            }

            return View(product);
        }

        [HttpPost]
        [Authorize]
        [ActionName("Details")]
        public async Task<IActionResult> DetailsPost(ProductDto productDto)
        {
            var userId = User.Claims.Where(t => t.Type == "sub").FirstOrDefault().Value;
            var token = await HttpContext.GetTokenAsync("access_token");
            var responseGetCart = await _cartService.GetCartByUserIdAsync<ResponseDto>(userId,token);

            CartDto cart = new CartDto();
            if (responseGetCart == null || !responseGetCart.IsSuccess)
            {

                cart.CartHeader = new CartHeaderDto()
                {
                    UserId = User.Claims.Where(t => t.Type == "sub")?.FirstOrDefault()?.Value
                };

                cart.CartDetails = new List<CartDetailDto>();

                var responseCart = await _cartService.CreateCartAsync<ResponseDto>(cart,token);

                if (responseCart != null && responseCart.IsSuccess)
                {
                    cart = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(responseCart.Result));
                }
                else
                {
                    return View(productDto);
                }
            }
            else if (responseGetCart != null && responseGetCart.IsSuccess)
            {
                cart = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(responseGetCart.Result));
            }


            CartDetailDto cartDetailDto = new CartDetailDto()
            {
                Count = productDto.Count,
                ProductId = productDto.ProductId
            };

            var response = await _productService.GetProductByIdAsync<ResponseDto>(productDto.ProductId, token);

            if (response != null && response.IsSuccess)
            {
                cartDetailDto.Product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                cartDetailDto.CartHeader = cart.CartHeader;
                cartDetailDto.CartHeaderId = cart.CartHeader.CartHeaderId;
            }

            var addToCartResponse = await _cartService.AddToCartAsync<ResponseDto>(cartDetailDto, token);

            if (addToCartResponse != null && addToCartResponse.IsSuccess)
            {
                return RedirectToAction("Index");
            }

            return View(productDto);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> Login()
        {
            return RedirectToAction("Index");
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}