using BackEnd.Data;
using BackEnd.DTOs.Cart;
using BackEnd.IServices;
using BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        private async Task<Cart> GetOrCreateCartAsync(string userId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if(cart is not null)
            {
                return cart;
            }

            cart = new Cart
            {
                UserId = userId
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            return cart;
        }

        private async Task RemoveUnavailableItemsAsync(int cartId)
        {
            var unavailableItems = await _context.CartItems.Include(i => i.Product)
                .Where(i => i.CartId == cartId && (i.Product == null || !i.Product.IsApproved || i.Product.SellerId == null ||i.Product.SellerId == ""))
                .ToListAsync();
            if(unavailableItems.Count < 0)
            {
                return;
            }

            _context.RemoveRange(unavailableItems);
            await _context.SaveChangesAsync();
        }

        private async Task<CartDto> BuildCartDtoAsync(int cartId)
        {
            var cart = await _context.Carts.AsNoTracking().Include(c => c.Items).ThenInclude(i => i.Product).FirstAsync(c => c.Id == cartId);

            var items = cart.Items
                .Where(i => (i.Product != null) && (i.Product.IsApproved) && !string.IsNullOrEmpty(i.Product.SellerId))
                .Select(i => new CartItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    ImgPath = i.Product.ImgPath,
                    Quantity = i.Quantity,
                    UnitPrice = i.Product.Price,
                    LineTotal = i.Quantity * i.Product.Price
                }).ToList();

            return new CartDto
            {
                Id = cart.Id,
                Items = items,
                TotalAmount = items.Sum(i => i.LineTotal)
            };
        }

        public async Task<CartDto> GetAsync(string userId)
        {
            var cart = await GetOrCreateCartAsync(userId);

            await RemoveUnavailableItemsAsync(cart.Id);

            return await BuildCartDtoAsync(cart.Id);
        }

        public async Task<CartDto> AddItemAsync(string userId, AddCartItemDto dto)
        {
            if (dto.Quantity < 1 || dto.Quantity > 100)
            {
                throw new ArgumentException("تعداد محصول باید بین 1 تا 100 عدد باشد");
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == dto.ProductId);
            if(product is null)
            {
                throw new KeyNotFoundException("محصول یافت نشد");
            }

            if(!product.IsApproved)
            {
                throw new InvalidOperationException("این محصول برای خرید در دسترس نیست");
            }

            if (string.IsNullOrWhiteSpace(product.SellerId))
            {
                throw new InvalidOperationException("این محصول فروشنده فعال ندارد");
            }

            var cart = await GetOrCreateCartAsync(userId);
            await RemoveUnavailableItemsAsync(cart.Id);

            var cartItem = await _context.CartItems.FirstOrDefaultAsync(i => i.CartId == cart.Id && i.ProductId == dto.ProductId);
            if(cartItem is null)
            {
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                };
            }
            else
            {
                if (cartItem.Quantity + dto.Quantity > 100)
                {
                    throw new ArgumentException("تعداد کل محصولات نمیتواند بیشتر از 100 عدد باشد.");
                }

                cartItem.Quantity += dto.Quantity;
            }

            await _context.SaveChangesAsync();

            return await BuildCartDtoAsync(cart.Id);
        }

        public async Task<CartDto?> PatchItemAsync(string userId, int productId, PatchCartItemDto dto)
        {
            if (dto.Quantity < 1 || dto.Quantity > 100)
            {
                throw new ArgumentException("تعداد محصول باید بین 1 تا 100 عدد باشد");
            }

            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if(cart is null)
            {
                return null;
            }

            await RemoveUnavailableItemsAsync (cart.Id);

            var item = await _context.CartItems.FirstOrDefaultAsync(i => i.CartId == cart.Id && i.ProductId == productId);
            if(item is null)
            {
                return null;
            }

            var productIsApproved = await _context.Products.AnyAsync(p => p.Id == productId && p.IsApproved && p.SellerId != null && p.SellerId != "");
            if(!productIsApproved)
            {
                throw new InvalidOperationException("این محصول برای خرید در دسترس نیست.");
            }

            item.Quantity = dto.Quantity;

            await _context.SaveChangesAsync();

            return await BuildCartDtoAsync(cart.Id);
        }

        public async Task<CartDto?> RemoveItemAsync(string userId, int productId)
        {
            var cart = await _context.Carts.FirstAsync(c => c.UserId == userId);
            if(cart is null)
            {
                return null;
            }

            var item = await _context.CartItems.FirstOrDefaultAsync(i => i.CartId == cart.Id && i.ProductId == productId);
            if(item is null)
            {
                return null;
            }

            _context.CartItems.Remove(item);

            await _context.SaveChangesAsync();

            await RemoveUnavailableItemsAsync(cart.Id);

            return await BuildCartDtoAsync(cart.Id);
        }
    }
}
