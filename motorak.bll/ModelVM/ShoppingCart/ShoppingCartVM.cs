using motorak.dal.Enums.CarEnums;
using motorak.dal.Enums.CartEnums;

namespace motorak.bll.ModelVM.ShoppingCart
{
    public class ShoppingCartVM
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public decimal? DailyRentPrice { get; set; }
        public string ImagePath { get; set; }
        public CarCategory Category { get; set; }
        public int Count { get; set; }
        public string UserId { get; set; }
        public CartItemType ItemType { get; set; }
        public DateTime? RentStartDate { get; set; }
        public DateTime? RentEndDate { get; set; }
        public decimal TotalPrice => ItemType == CartItemType.Rent && DailyRentPrice.HasValue
            ? (decimal)(DailyRentPrice * RentDays * Count)
            : Price * Count;

        public int RentDays => RentStartDate.HasValue && RentEndDate.HasValue
            ? Math.Max(1, (RentEndDate.Value - RentStartDate.Value).Days)
            : 1;
    }
}