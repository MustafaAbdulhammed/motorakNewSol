
namespace Motorak.BLL.ModelVM.Transactions
{
    public class TransactionCreateDto
    {
        public string PaymentMethod { get; set; }
        public decimal TotalPrice { get; set; }
        public int CustomerId { get; set; }
        public int CarId { get; set; }
    }
}
