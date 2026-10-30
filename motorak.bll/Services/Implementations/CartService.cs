// motorak.bll/Services/Implementations/CartService.cs
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using motorak.bll.ModelVM.ShoppingCart;
using motorak.bll.Services.Abstractions;
using motorak.dal.Enums.CartEnums;
using motorak.DAL.DataBase;
using Motorak.BLL.Services.Abstractions;
using Motorak.DAL.Entites;
using Motorak.DAL.Enums.CarEnums;
using motorak.dal.Enums.CarEnums;

namespace motorak.bll.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly MotorakDbContext _context;
        private readonly IMapper _mapper;

        public CartService(MotorakDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ShoppingCartVM>> GetCartItemsAsync(string userId)
        {
            var cartItems = await _context.CartItems
                .Include(c => c.Car)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return _mapper.Map<List<ShoppingCartVM>>(cartItems);
        }

        public async Task<(bool success, string message)> AddToCartAsync(
            int carId, string userId, CartItemType itemType,
            DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var car = await _context.Cars.FindAsync(carId);
                if (car == null)
                    return (false, "Car not found");

                if (car.Status != CarStatus.Available)
                    return (false, "Car is not available");

                // ✅ Fixed: support Both category
                if (itemType == CartItemType.Rent)
                {
                    if (!car.DailyRentPrice.HasValue ||
                        (car.Category != CarCategory.ForRent && car.Category != CarCategory.Both))
                        return (false, "Car is not available for rent");
                }

                if (itemType == CartItemType.Buy)
                {
                    if (car.Category != CarCategory.ForSale && car.Category != CarCategory.Both)
                        return (false, "Car is not available for purchase");
                }

                var existingItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.CarId == carId && c.UserId == userId && c.ItemType == itemType);

                if (existingItem != null)
                {
                    existingItem.Count++;
                    if (itemType == CartItemType.Rent)
                    {
                        existingItem.RentStartDate = startDate ?? DateTime.Now;
                        existingItem.RentEndDate = endDate ?? DateTime.Now.AddDays(1);
                    }
                }
                else
                {
                    var cartItem = new CartItem
                    {
                        CarId = carId,
                        UserId = userId,
                        Count = 1,
                        ItemType = itemType,
                        RentStartDate = itemType == CartItemType.Rent ? (startDate ?? DateTime.Now) : null,
                        RentEndDate = itemType == CartItemType.Rent ? (endDate ?? DateTime.Now.AddDays(1)) : null,
                        DateAdded = DateTime.Now
                    };
                    await _context.CartItems.AddAsync(cartItem);
                }

                await _context.SaveChangesAsync();
                return (true, "Item added to cart successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Error adding to cart: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> UpdateCartItemAsync(
            int cartItemId, int quantity, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var cartItem = await _context.CartItems.FindAsync(cartItemId);
                if (cartItem == null)
                    return (false, "Cart item not found");

                if (quantity <= 0)
                    _context.CartItems.Remove(cartItem);
                else
                {
                    cartItem.Count = quantity;
                    if (startDate.HasValue) cartItem.RentStartDate = startDate;
                    if (endDate.HasValue) cartItem.RentEndDate = endDate;
                }

                await _context.SaveChangesAsync();
                return (true, "Cart updated successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Error updating cart: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> RemoveFromCartAsync(int cartItemId, string userId)
        {
            try
            {
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);

                if (cartItem == null)
                    return (false, "Cart item not found");

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                return (true, "Item removed from cart");
            }
            catch (Exception ex)
            {
                return (false, $"Error removing from cart: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> ClearCartAsync(string userId)
        {
            try
            {
                var cartItems = await _context.CartItems
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                _context.CartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();
                return (true, "Cart cleared successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Error clearing cart: {ex.Message}");
            }
        }

        public async Task<int> GetCartItemCountAsync(string userId)
        {
            return await _context.CartItems
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Count);
        }

        public async Task<decimal> GetCartTotalAsync(string userId)
        {
            var cartItems = await GetCartItemsAsync(userId);
            return cartItems.Sum(c => c.TotalPrice);
        }
    }
}