using Motorak.DAL.Enums.TransactionEnums;


namespace Motorak.BLL.ModelVM.Purchases
{
    public class PurchaseUpdateDto
    {
        public int Id { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TotalPrice { get; set; }
        public TransactionStatus Status { get; set; }
    }
}
