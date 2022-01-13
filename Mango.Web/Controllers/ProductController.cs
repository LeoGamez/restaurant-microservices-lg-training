using Mango.Web.Model.DTOs;
using Mango.Web.Models.DTOs;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<IActionResult> Index()
        {
            List<ProductDto> list = new();

            var token = await HttpContext.GetTokenAsync("access_token");

            var response = await _productService.GetALLProductsAsync<ResponseDto>(token);

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }

            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(ProductDto product)
        {

            var token = await HttpContext.GetTokenAsync("access_token");

            var response = await _productService.CreateProductAsync<ResponseDto>(product, token);

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction("Index");
            }


            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var token = await HttpContext.GetTokenAsync("access_token");

            ProductDto product = new();
            var response = await _productService.GetProductByIdAsync<ResponseDto>(id.Value, token);

            if (response != null && response.IsSuccess)
            {
                product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            }

            return View(product);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(ProductDto product)
        {

            var token = await HttpContext.GetTokenAsync("access_token");

            var response = await _productService.UpdateProductAsync<ResponseDto>(product, token);

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction("Index");
            }


            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductDto product = new();
            var token = await HttpContext.GetTokenAsync("access_token");

            var response = await _productService.GetProductByIdAsync<ResponseDto>(id.Value, token);

            if (response != null && response.IsSuccess)
            {
                product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            }

            return View(product);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(ProductDto product)
        {
           
                var token = await HttpContext.GetTokenAsync("access_token");

                var response = await _productService.DeleteProductAsync<ResponseDto>(product.ProductId, token);

                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
            

            return View(product);
        }
    }
}
