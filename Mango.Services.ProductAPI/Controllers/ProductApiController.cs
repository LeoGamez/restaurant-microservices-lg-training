using Mango.Services.ProductAPI.Model.DTOs;
using Mango.Services.ProductAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductApiController : ControllerBase
    {
        protected ResponseDto _response;
        private IProductRepository _productRepository;

        public ProductApiController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
            this._response = new ResponseDto();
        }

        [HttpGet]
        //[Authorize]
        public async Task<ResponseDto> Get()
        {
            try
            {
                IEnumerable<ProductDto> productDtos = await _productRepository.GetProductDtos();
                _response.Result = productDtos;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.ToString()
                };
            }

            return _response;
        }

        [HttpGet]
        [Route("{id}")]
        //[Authorize]
        public async Task<ResponseDto> Get(int id)
        {
            try
            {
                ProductDto productDtos = await _productRepository.GetProductById(id);
                _response.Result = productDtos;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.ToString()
                };
            }

            return _response;
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDto> Post([FromBody]ProductDto productDto)
        {
            try
            {
                ProductDto productDtos = await _productRepository.CreateUpdateProduct(productDto);
                _response.Result=productDtos;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.ToString()
                };
            }

            return _response;
        }

        [Authorize]
        [HttpPut]
        public async Task<ResponseDto> Put([FromBody] ProductDto productDto)
        {
            try
            {
                ProductDto productDtos = await _productRepository.CreateUpdateProduct(productDto);
                _response.Result = productDtos;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.ToString()
                };
            }

            return _response;
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]

        public async Task<ResponseDto> Delete(int id)
        {
            try
            {
                _response.Result = await _productRepository.DeleteProduct(id); 
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.ToString()
                };
            }

            return _response;
        }
    }
}

