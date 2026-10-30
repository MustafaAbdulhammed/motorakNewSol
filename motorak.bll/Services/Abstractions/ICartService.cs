using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using motorak.bll.ModelVM.ShoppingCart;
using motorak.dal.Enums.CartEnums;

namespace motorak.bll.Services.Abstractions
{
    public interface ICartService
    {
        Task<List<ShoppingCartVM>> GetCartItemsAsync(string userId);
        Task<(bool success, string message)> AddToCartAsync(int carId, string userId, CartItemType itemType, DateTime? startDate = null, DateTime? endDate = null);
        Task<(bool success, string message)> UpdateCartItemAsync(int cartItemId, int quantity, DateTime? startDate = null, DateTime? endDate = null);
        Task<(bool success, string message)> RemoveFromCartAsync(int cartItemId, string userId);
        Task<(bool success, string message)> ClearCartAsync(string userId);
        Task<int> GetCartItemCountAsync(string userId);
        Task<decimal> GetCartTotalAsync(string userId);
    }
}
