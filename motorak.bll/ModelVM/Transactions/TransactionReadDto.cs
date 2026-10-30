

using Motorak.DAL.Enums.TransactionEnums;

namespace Motorak.BLL.ModelVM.Transactions
{
    public class TransactionReadDto
    {
        public int Id { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TotalPrice { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
