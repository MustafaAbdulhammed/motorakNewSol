using motorak.dal.Entites;
using Motorak.DAL;
using Motorak.DAL.Entites;
using motorak.dal.Enums.CartEnums;

namespace motorak.dal.Repo.Abstractions
{
    public interface ICartRepo
    {
        
        Task<List<CartItem>> GetAllAsync();


        Task<List<CartItem>> GetCartItemsByUserIdAsync(string userId);
        Task<CartItem?> GetCartItemAsync(int id);
        Task<int> GetCartItemCountAsync(string userId);


        Task<CartItem> AddCartItemAsync(CartItem cartItem);
        Task<CartItem> UpdateCartItemAsync(CartItem cartItem);
        Task<bool> DeleteCartItemAsync(int id);
        Task<bool> ClearUserCartAsync(string userId);


        Task<decimal> GetCartTotalAsync(string userId);

        
        Task<bool> IsCarInUserCartAsync(string userId, int carId, CartItemType itemType);
        Task<bool> ValidateCartItemOwnershipAsync(int cartItemId, string userId);
        Task<List<CartItem>> GetExpiredRentalItemsAsync();
    }
}