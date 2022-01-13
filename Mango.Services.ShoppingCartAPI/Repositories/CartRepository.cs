using AutoMapper;
using Mango.Services.ShoppingCartAPI.DbContexts;
using Mango.Services.ShoppingCartAPI.Model;
using Mango.Services.ShoppingCartAPI.Model.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Repositories
{
    public class CartRepository : ICartRepository
    {

        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public CartRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<bool> AddToCart(CartDetailDto cartDetailDto)
        {

            try
            {

                //! This should be done separatedly
                var prodInDb = _db.Products.FirstOrDefault(t => t.ProductId == cartDetailDto.ProductId);

                if (prodInDb == null)
                {
                    var product = _mapper.Map<Product>(cartDetailDto.Product);
                    _db.Products.Add(product);
                    await _db.SaveChangesAsync();
                }

                cartDetailDto.Product = null;
                cartDetailDto.CartHeader = null;

                var cartDetails = _mapper.Map<CartDetail>(cartDetailDto);

                var existingInCart = _db.CartDetails
                    .Where(t => t.CartHeaderId == cartDetailDto.CartHeaderId
                             && t.ProductId == cartDetailDto.ProductId).FirstOrDefault();

                if (existingInCart != null)
                {
                    existingInCart.Count += cartDetails.Count;
                    _db.CartDetails.Update(existingInCart);
                }
                else
                {
                    _db.CartDetails.Add(cartDetails);

                }

                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        /// <summary>
        /// Creates the cart, if a cart already exist, retrieves the old cart
        /// </summary>
        /// <param name="cartDto">The cart dto.</param>
        /// <returns></returns>
        public async Task<CartDto> CreateCart(CartDto cartDto)
        {

            Cart cart = _mapper.Map<Cart>(cartDto);

            var cartHeaderDb = _db.CartHeaders
                .FirstOrDefault(t => t.UserId == cart.CartHeader.UserId);

            if (cartHeaderDb == null)
            {
                _db.CartHeaders.Add(cart.CartHeader);
                await _db.SaveChangesAsync();
                cart.CartHeader = _db.CartHeaders
                   .FirstOrDefault(t => t.UserId == cart.CartHeader.UserId);
            }
            else
            {
                cart.CartHeader = cartHeaderDb;
            }



            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> GetCartByUserId(string id)
        {

            Cart cart = new()
            {
                CartHeader = _db.CartHeaders.FirstOrDefault(t => t.UserId == id)
            };

            cart.CartDetails = _db.CartDetails.Where(t => t.CartHeaderId == cart.CartHeader.CartHeaderId)
                .Include(t => t.Product);

            return _mapper.Map<CartDto>(cart);

        }

        public async Task<bool> ClearCart(string id)
        {
            var cartHeaderDb = await _db.CartHeaders
                                  .FirstOrDefaultAsync(t => t.UserId == id);

            if (cartHeaderDb != null)
            {
                try
                {
                    _db.CartDetails
                            .RemoveRange(_db.CartDetails
                                            .Where(t => t.CartHeaderId == cartHeaderDb.CartHeaderId));

                    _db.CartHeaders.Remove(cartHeaderDb);
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                    throw;
                }

                return true;
            }

            return false;

        }

        public async Task<bool> RemoveFromCart(int cartDetailsId)
        {
            try
            {
                CartDetail cartDetail = await _db.CartDetails.FirstOrDefaultAsync(t => t.CartDetailId == cartDetailsId);

                int totalCountCartItems = _db.CartDetails.Where(t => t.CartHeaderId == cartDetail.CartHeaderId).Count();

                _db.CartDetails.Remove(cartDetail);

                if (totalCountCartItems == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders.FirstOrDefaultAsync(t => t.CartHeaderId == cartDetail.CartHeaderId);


                    _db.CartHeaders.Remove(cartHeaderToRemove);
                }

                await _db.SaveChangesAsync();


                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> ApplyCoupon(string userId, string couponCode)
        {
            var cartHeader = _db.CartHeaders.FirstOrDefault(t =>
                t.UserId==userId);

            cartHeader.CouponCode = couponCode;
            _db.CartHeaders.Update(cartHeader);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveCoupon(string userId)
        {
            var cartHeader =await _db.CartHeaders.FirstOrDefaultAsync(t =>
                t.UserId == userId);

            cartHeader.CouponCode = "";
            _db.CartHeaders.Update(cartHeader);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}
