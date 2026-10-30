using Motorak.DAL.Enums.TransactionEnums;


namespace Motorak.BLL.ModelVM.Rents
{
    public class RentUpdateDto
    {
        public int Id { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TotalPrice { get; set; }
        public TransactionStatus Status { get; set; }
        
    }
}
