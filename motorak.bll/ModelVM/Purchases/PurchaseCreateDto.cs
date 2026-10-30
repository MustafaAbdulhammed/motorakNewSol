

using Motorak.DAL.Enums.TransactionEnums;
namespace Motorak.BLL.ModelVM.Purchases
{
    public class PurchaseCreateDto
    {
        public string SellerName { get; set; } = "Motorak";
        public string PaymentMethod { get; set; }
        public decimal TotalPrice { get; set; }
        public int CustomerId { get; set; }
        public int CarId { get; set; }
        public string? CarName { get; set; }
        public TransactionStatus Status { get; set; }
    }
}
