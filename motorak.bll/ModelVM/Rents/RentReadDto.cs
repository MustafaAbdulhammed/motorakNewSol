using Motorak.DAL.Enums.TransactionEnums;

namespace Motorak.BLL.ModelVM.Rents
{
    public class RentReadDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TotalPrice { get; set; }
        public TransactionStatus Status { get; set; }
        public int CustomerId { get; set; }
        public int CarId { get; set; }
        public DateTime TransactionDate { get; set; } // Optional: from base class
    }
}