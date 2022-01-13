using Mango.Services.ProductAPI.DbContexts;
using Mango.Services.ProductAPI.Model.DTOs;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Mango.Services.ProductAPI.Model;

namespace Mango.Services.ProductAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;


        public ProductRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates the update product.
        /// </summary>
        /// <param name="productDto">The product dto.</param>
        /// <returns></returns>
        async Task<ProductDto> IProductRepository.CreateUpdateProduct(ProductDto productDto)
        {

            var product = _mapper.Map<ProductDto, Product>(productDto);
            if (product.ProductId > 0)
            {
                _db.Products.Update(product);

            }
            else
            {
                _db.Products.Add(product);
            }

            await _db.SaveChangesAsync();

            return _mapper.Map<Product, ProductDto>(product);


        }

        /// <summary>
        /// Deletes the product.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        async Task<bool> IProductRepository.DeleteProduct(int id)
        {
            try
            {
                var product = await _db.Products.Where(x => x.ProductId == id).FirstOrDefaultAsync();

                _db.Products.Remove(product);
                _db.SaveChanges();

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// Gets the product by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        async Task<ProductDto> IProductRepository.GetProductById(int id)
        {
            var product = await _db.Products.Where(x => x.ProductId == id).FirstOrDefaultAsync();
            return _mapper.Map<ProductDto>(product);
        }

        async Task<IEnumerable<ProductDto>> IProductRepository.GetProductDtos()
        {
            List<Product> products = await _db.Products.ToListAsync();
            return _mapper.Map<List<ProductDto>>(products);
        }
    }
}
