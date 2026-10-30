using Microsoft.EntityFrameworkCore;
using motorak.dal.Entites;
using motorak.dal.Repo.Abstractions;
using motorak.DAL.DataBase;
using Motorak.DAL;
using Motorak.DAL.Entites;
using motorak.dal.Enums.CartEnums;

namespace motorak.dal.Repo.Implementations
{
    public class CartRepo : ICartRepo
    {
        private readonly MotorakDbContext _context;

        public CartRepo(MotorakDbContext context)
        {
            _context = context;
        }

        public async Task<List<CartItem>> GetAllAsync()
        {
            return await _context.CartItems
                .Include(ci => ci.Car)
                .Include(ci => ci.User)
                .ToListAsync();
        }

        public async Task<List<CartItem>> GetCartItemsByUserIdAsync(string userId)
        {
            return await _context.CartItems
                .Include(ci => ci.Car)
                .Include(ci => ci.User)
                .Where(ci => ci.UserId == userId)
                .OrderByDescending(ci => ci.DateAdded)
                .ToListAsync();
        }

        public async Task<CartItem?> GetCartItemAsync(int id)
        {
            return await _context.CartItems
                .Include(ci => ci.Car)
                .Include(ci => ci.User)
                .FirstOrDefaultAsync(ci => ci.Id == id);
        }

        public async Task<CartItem> AddCartItemAsync(CartItem cartItem)
        {

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CarId == cartItem.CarId &&
                                         ci.UserId == cartItem.UserId &&
                                         ci.ItemType == cartItem.ItemType);

            if (existingItem != null)
            {

                if (cartItem.ItemType == CartItemType.Rent)
                {
                    bool sameRentalDates = existingItem.RentStartDate == cartItem.RentStartDate &&
                                          existingItem.RentEndDate == cartItem.RentEndDate;

                    if (sameRentalDates)
                    {
                        existingItem.Count += cartItem.Count;
                        existingItem.DateAdded = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                        return existingItem;
                    }
                }
                else
                {

                    existingItem.Count += cartItem.Count;
                    existingItem.DateAdded = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return existingItem;
                }
            }


            cartItem.DateAdded = DateTime.UtcNow;
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            await _context.Entry(cartItem)
                .Reference(ci => ci.Car)
                .LoadAsync();
            await _context.Entry(cartItem)
                .Reference(ci => ci.User)
                .LoadAsync();

            return cartItem;
        }

        public async Task<CartItem> UpdateCartItemAsync(CartItem cartItem)
        {
            var existingItem = await _context.CartItems.FindAsync(cartItem.Id);
            if (existingItem == null)
            {
                throw new ArgumentException("Cart item not found");
            }
            existingItem.Count = cartItem.Count;
            existingItem.RentStartDate = cartItem.RentStartDate;
            existingItem.RentEndDate = cartItem.RentEndDate;
            existingItem.ItemType = cartItem.ItemType;

            _context.CartItems.Update(existingItem);
            await _context.SaveChangesAsync();


            await _context.Entry(existingItem)
                .Reference(ci => ci.Car)
                .LoadAsync();
            await _context.Entry(existingItem)
                .Reference(ci => ci.User)
                .LoadAsync();

            return existingItem;
        }

        public async Task<bool> DeleteCartItemAsync(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return false;
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearUserCartAsync(string userId)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return true;
            }

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetCartTotalAsync(string userId)
        {
            var total = await _context.CartItems
                .Include(ci => ci.Car)
                .Where(ci => ci.UserId == userId)
                .SumAsync(ci => ci.ItemType == CartItemType.Buy
                    ? ci.Car.Price * ci.Count
                    : CalculateRentalPrice(ci.Car.Price, ci.RentStartDate, ci.RentEndDate) * ci.Count);

            return total;
        }

        public async Task<int> GetCartItemCountAsync(string userId)
        {
            return await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .SumAsync(ci => ci.Count);
        }

        public async Task<bool> IsCarInUserCartAsync(string userId, int carId, CartItemType itemType)
        {
            return await _context.CartItems
                .AnyAsync(ci => ci.UserId == userId &&
                              ci.CarId == carId &&
                              ci.ItemType == itemType);
        }

        public async Task<List<CartItem>> GetExpiredRentalItemsAsync()
        {
            var today = DateTime.Today;
            return await _context.CartItems
                .Include(ci => ci.Car)
                .Include(ci => ci.User)
                .Where(ci => ci.ItemType == CartItemType.Rent &&
                           ci.RentStartDate.HasValue &&
                           ci.RentStartDate.Value < today)
                .ToListAsync();
        }

        public async Task<bool> ValidateCartItemOwnershipAsync(int cartItemId, string userId)
        {
            return await _context.CartItems
                .AnyAsync(ci => ci.Id == cartItemId && ci.UserId == userId);
        }

        private decimal CalculateRentalPrice(decimal dailyRate, DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                return dailyRate; 
            }

            var days = (endDate.Value - startDate.Value).Days;
            if (days <= 0)
            {
                days = 1; 
            }

            return dailyRate * days;
        }
    }
}